using BlazorStandaloneApplicationExample.Forms;
using System.Reflection;

namespace BlazorStandaloneApplicationExample
{
    public static class WindowsApplicationServer
    {
        private static Mutex globalMutex = new Mutex(true, Assembly.GetEntryAssembly().FullName);


        public static bool CheckForAnotherInstance()
        {
            if (!globalMutex.WaitOne(TimeSpan.Zero, true))
                return true;

            return false;
        }

        public static void ConfigureAsStandaloneApplication(this WebApplication app, string title = "WebLauncher")
        {
            // register a middleware to handle User-Agent request
            app.Use(async (context, next) =>
            {
                var agent = context.Request.Headers.UserAgent.ToString();
                if (agent == null || agent != WebLauncher.USER_AGENT)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Application is running as stand-alone!");
                    return;
                }

                await next.Invoke();

                // need to run code after?
            });

            // register a listener when the application starts
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("Starting UI...");

                try
                {
                    // get first useful url used by this app
                    var url = app.Urls.Where(x => x.ToLower().StartsWith("https")).FirstOrDefault();
                    url = url ?? app.Urls.FirstOrDefault();
                    if (url == null)
                    {
                        Console.WriteLine("Fail to detect app URL!");
                        Environment.Exit(1);
                    }

                    UriBuilder uri = new UriBuilder(url);

                    Console.WriteLine($"App URL: {uri}");

                    // preparing the STA thread like seen in standard WinForm [STAThread] attribute
                    Thread thread = new Thread(() =>
                    {
                        // preparing Windows Forms
                        global::System.Windows.Forms.Application.EnableVisualStyles();
                        global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                        global::System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);

                        // instancing and run form launcher setting the url of this application
                        var form = new WebLauncher(uri.Uri, title);
                        form.FormClosed += (s, e) => app.Lifetime.StopApplication(); /*Environment.Exit(0);*/ // quit environment when

                        Application.Run(form);
                    });

                    // STA is mandatory!
                    thread.SetApartmentState(ApartmentState.STA);

                    // launch integrated browser
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    Console.WriteLine("\n\nPress any key to exit.");
                    Console.ReadKey();

                    Environment.Exit(2);
                }
            });
        }

        public static void RunApplication(this WebApplication app)
        {
            app.Start();

            #if !DEBUG
                WindowsInterops.HideConsoleWindow();
            #endif

            app.WaitForShutdown();

            globalMutex.ReleaseMutex();

            Console.WriteLine("Quit now.");
        }
    }
}
