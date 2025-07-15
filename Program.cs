using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;

using ApiResponse;

namespace WebSocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BSWebSocket socket = new BSWebSocket();
            socket.Method();
        }
    }

    class BSWebSocket 
    {
        static string API_HOST = "https://192.168.103.3:443";
        static string WS_HOST = "wss://192.168.103.3:443";
        static string BIOSTAR2_LOGIN_API_URI = API_HOST + "/api/login";
        static string BIOSTAR2_WS_URI = WS_HOST + "/wsapi";
        static string BIOSTAR2_WS_EVENT_START_URI = API_HOST + "/api/events/start";
        static string bsSessionID = "";

        public virtual async void Method()
        {
            Task.Run(async () =>
            {
                await ConnectWebSocket();
            }).Wait();
        }

        async Task ConnectWebSocket()
        {
            try
            {
                //Set login data
                var loginData = new
                {
                    User = new
                    {
                        login_id = "admin",
                        password = "Suprema!1"
                    }
                };

                //Set SSH certificates
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (HttpClient httpClient = new HttpClient())
                {
                    //Set up the HttpClient to send the login request to the BioStar 2 server
                    httpClient.BaseAddress = new Uri(API_HOST);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.Timeout = new TimeSpan(1,1,1);
                    var httpContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                    //Login and get BioStar2 Session ID
                    HttpResponseMessage httpResponse = await httpClient.PostAsync("api/login", httpContent);
                    if (httpResponse.IsSuccessStatusCode == true)
                    {
                        bsSessionID = httpResponse.Headers.GetValues("bs-session-id").FirstOrDefault().ToString();
                        Console.WriteLine("bs-session-id: " + bsSessionID);
                    }
                    else
                    {
                        Console.WriteLine("Failed to Login");
                        Console.WriteLine(httpResponse.ToString());
                    }

                    //Connect to WebSocket endpoint
                    var clientWebSocket = new ClientWebSocket();
                    await clientWebSocket.ConnectAsync(new Uri(BIOSTAR2_WS_URI), CancellationToken.None);

                    //Add the session ID to the websocket connection URI
                    byte[] buffer = Encoding.UTF8.GetBytes("bs-session-id="+ bsSessionID);
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    //Start and Receive Realtime Event Logs
                    await StartMonitoringEvents();
                    await ReceiveMonitoringEvents(clientWebSocket);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }

        }

        private async Task StartMonitoringEvents()
        {
            var client = new RestClient(BIOSTAR2_WS_EVENT_START_URI);
            var request = new RestRequest("",RestSharp.Method.Post);
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("bs-session-id", bsSessionID);
            request.AddHeader("content-language", "en");
            request.AddParameter("application/json", "{\"Query\":{}}", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        private static async Task ReceiveMonitoringEvents(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    string jsonData = Encoding.UTF8.GetString(buffer);
                    try
                    {
                        var bsEvent = BsEvent.FromJson(jsonData);

                        //Filter by Device ID
                        if (bsEvent.Event.DeviceId.Id == 538151189)
                        {
                            Console.WriteLine(jsonData);
                        }

                        //Filter by Event ID
                        if(bsEvent.Event.EventTypeId.Code == 4102) // 4102 = VERIFY_SUCCESS_CARD, replace with target events
                        {
                            Console.WriteLine(jsonData);
                        }
                    }
                    catch(Exception x)
                    {
                        Console.WriteLine(x.Message);
                    }
                }
            }
        }
    }
}