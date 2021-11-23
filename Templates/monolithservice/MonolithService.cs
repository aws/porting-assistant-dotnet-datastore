/*

using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Web.Routing;

namespace #NAMESPACEPLACEHOLDER#
{
    public static class MonolithService
    {
        private static string ServiceHost = WebConfigurationManager.AppSettings["ServiceHost"];
        private static string ServicePort = WebConfigurationManager.AppSettings["ServicePort"];
        private static string ServiceUrl = $"https://{ServiceHost}";
        private static HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private static HttpClient client = new HttpClient(handler);


        public static async Task<string> CreateRequestAsync(HttpRequestMessage httpRequest, System.Web.Http.Routing.IHttpRouteData routeData)
        {
            string actionName = routeData.Values["action"].ToString();
            string controllerName = routeData.Values["controller"].ToString();
            var httpMessage = GetRequestMessage(httpRequest, controllerName, actionName);
            return await InvokeRequestAsync(httpMessage);
        }

        public static string CreateRequest(HttpRequestMessage httpRequest, System.Web.Http.Routing.IHttpRouteData routeData)
        {
            string actionName = routeData.Values["action"].ToString();
            string controllerName = routeData.Values["controller"].ToString();
            var httpMessage = GetRequestMessage(httpRequest, controllerName, actionName);
            return InvokeRequest(httpMessage);
        }


        public static async Task<string> CreateRequestAsync(HttpRequestBase httpRequest, System.Web.Routing.RouteData routeData)
        {
            string actionName = routeData.Values["action"].ToString();
            string controllerName = routeData.Values["controller"].ToString();
            var httpMessage = GetRequestMessage(httpRequest, controllerName, actionName);
            return await InvokeRequestAsync(httpMessage);
        }

        public static string CreateRequest(HttpRequestBase httpRequest, System.Web.Routing.RouteData routeData)
        {
            string actionName = routeData.Values["action"].ToString();
            string controllerName = routeData.Values["controller"].ToString();
            var httpMessage = GetRequestMessage(httpRequest, controllerName, actionName);
            return InvokeRequest(httpMessage);
        }

        private static async Task<string> InvokeRequestAsync(HttpRequestMessage requestMessage)
        {
            var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
            var stringResult = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return stringResult;
        }

        private static string InvokeRequest(HttpRequestMessage requestMessage)
        {
            var result = client.SendAsync(requestMessage).Result;
            var stringResult = result.Content.ReadAsStringAsync().Result;
            return stringResult;
        }

        private static HttpRequestMessage GetRequestMessage(HttpRequestMessage httpRequestMessage, string controllerName, string actionName)
        {
            var currentUri = httpRequestMessage.RequestUri;
            var uriBuilder = new UriBuilder($"{ServiceUrl}/{controllerName}/{actionName}")
            {
                Host = ServiceHost,
                Port = int.Parse(ServicePort),
                Fragment = currentUri.Fragment,
                Scheme = currentUri.Scheme,
                Path = currentUri.PathAndQuery
            };

            var newRequest = httpRequestMessage;
            newRequest.RequestUri = uriBuilder.Uri;
            return newRequest;
        }
        private static HttpRequestMessage GetRequestMessage(HttpRequestBase httpRequest, string controllerName, string actionName)
        {
            var httpMethod = new HttpMethod(httpRequest.HttpMethod);
            var currentUri = httpRequest.Url;
            var uriBuilder = new UriBuilder($"{ServiceUrl}/{controllerName}/{actionName}")
            {
                Host = ServiceHost,
                Port = int.Parse(ServicePort),
                Fragment = currentUri.Fragment,
                Scheme = currentUri.Scheme,
                Path = currentUri.PathAndQuery
            };
            var requestMessage = new HttpRequestMessage(httpMethod, uriBuilder.Uri);
            var currentHeaders = httpRequest.Headers.Keys;

            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                var streamReader = new StreamReader(httpRequest.InputStream).ReadToEnd();
                requestMessage.Content = new StringContent(streamReader);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(httpRequest.ContentType);
                requestMessage.Content.Headers.ContentEncoding.Add(httpRequest.ContentEncoding.HeaderName);
                requestMessage.Content.Headers.ContentLength = httpRequest.ContentLength;
            }
            for (int i = 0; i < currentHeaders.Count; i++)
            {
                try
                {
                    if (currentHeaders[i] == "Host")
                    {
                        requestMessage.Headers.Add(currentHeaders[i], ServiceHost);
                    }
                    else if (currentHeaders[i].StartsWith("Content-"))
                    {
                        requestMessage.Content.Headers.TryAddWithoutValidation(currentHeaders[i], httpRequest.Headers[currentHeaders[i]]);
                    }
                    else
                    {
                        requestMessage.Headers.TryAddWithoutValidation(currentHeaders[i], httpRequest.Headers[currentHeaders[i]]);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return requestMessage;
        }
    }
}


*/
