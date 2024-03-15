using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Security.Policy;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PatreonAPIWPF
{

    public partial class MainWindow : Window
    {
        // enter your info here
        private string pClientId = "";
        private string pClientSecret = "";
        private string pRedirectUri = "";


        private string pAccessToken = string.Empty;
        private string pOAuthToken = string.Empty;
        public string OAuthToken
        {
            get { return pOAuthToken; }
            set { pOAuthToken = value; OAuthLabel.Content = value; }
        }

        public string AccessToken
        {
            get { return pAccessToken; }
            set { pAccessToken = value; TokenLabel.Content = value; }
        }

        public string ClientID
        {
            get { return pClientId; }
        }

        public string ClientSecret
        {
            get { return pClientSecret; }
        }

        public string RedirectURI
        {
            get { return pRedirectUri; }
        }

        public string OAuthCodeURL
        {
            get { return $"https://www.patreon.com/oauth2/authorize?response_type=code&client_id={ClientID}&redirect_uri={RedirectURI}&scope=identity%20identity%5Bemail%5D"; ; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartListener()
        {
            // Start a local HTTP server
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            // Listen for incoming requests
            Task.Run(async () =>
            {
                while (true)
                {
                    var context = await listener.GetContextAsync();
                    await HandleRequest(context);
                }
            });

            Console.WriteLine("Server started. Listening at http://localhost:8080/");
            Console.ReadLine();
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            response.AppendHeader("Access-Control-Allow-Origin", "*");
            response.AppendHeader("Access-Control-Allow-Methods", "POST");
            response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 200;
                response.Close();
                return;
            }

            if (request.HttpMethod == "POST" && request.Url.LocalPath == "/data")
            {
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestData = await reader.ReadToEndAsync();

                    string responseString = "Data received successfully! You can close this page now.";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OAuthToken = requestData.Replace("\"", "");
                    });
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
            }
        }

        private async Task<string> GetAccessToken(string authorizationCode)
        {
            string tokenUrl = "https://www.patreon.com/api/oauth2/token";

            string postData = $"code={authorizationCode}&grant_type=authorization_code&client_id={ClientID}&client_secret={ClientSecret}&redirect_uri={RedirectURI}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(tokenUrl, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic tokenJson = JsonConvert.DeserializeObject(content);
                    return tokenJson.access_token;
                }
                else
                {
                    // Handle error
                    return null;
                }
            }
        }

        private async Task<string> GetPatreonEmails(string accessToken)
        {
            string apiUrl = "https://www.patreon.com/api/oauth2/v2/identity?fields%5Buser%5D=email";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Parse the response JSON to extract emails or other user information
                    return content;
                }
                else
                {
                    // Handle error
                    return null;
                }
            }
        }

        private void RequestOAuthCode()
        {
            StartListener();
            Process.Start(new ProcessStartInfo(OAuthCodeURL) { UseShellExecute = true });
        }

        private async void Button_Click(object sender, RoutedEventArgs e) /// get auth code
        {
            RequestOAuthCode();
        }

        private async void Button_Click2(object sender, RoutedEventArgs e)
        {
            AccessToken = await GetAccessToken(OAuthToken);
        }

        private async void Button_Click3(object sender, RoutedEventArgs e)
        {
            // Retrieve access token using the authorization code obtained earlier
            string accessToken = AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Use the access token to fetch user emails or other information
                string userEmails = await GetPatreonEmails(accessToken); ;

                int lastIndex = userEmails.LastIndexOf('}');
                string cleanedJsonString = userEmails.Substring(0, lastIndex + 1);
                JObject json = JObject.Parse(cleanedJsonString);
                ResultLabel.Content = (string)json["data"]["attributes"]["email"];
            }
            else
            {
                MessageBox.Show("Failed to retrieve access token", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}