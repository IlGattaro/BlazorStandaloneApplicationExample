namespace BlazorStandaloneApplicationExample.Forms
{
    public partial class WebLauncher : Form
    {
        public const string USER_AGENT = "WebView2/WebLauncher";

        public WebLauncher(Uri uri, string title="WebLauncher")
        {
            InitializeComponent();

            Text = title;

            // free resources: avoid an error of missed unregistration of the webView window class!
            this.FormClosing += (_,_) => webView.Dispose();

            // configure custom User-Agent header
            webView.CoreWebView2InitializationCompleted += (_,_) => {
                webView.CoreWebView2.Settings.UserAgent = USER_AGENT;

#if !DEBUG
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif
            };

            webView.Source = uri;
        }
    }
}
