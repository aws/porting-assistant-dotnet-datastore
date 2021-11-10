/*

using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web.Mvc;
using System.Web.Configuration;

namespace #NAMESPACEPLACEHOLDER#
{
    public class MonolithService : Controller
    {
        private static string ServiceHost = WebConfigurationManager.AppSettings["ServiceHost"];
        private static string ServicePort = WebConfigurationManager.AppSettings["ServicePort"];
        private static string ServiceUrl = $"https://{ServiceHost}";
        private static MonolithService _service = new MonolithService();

        public static async Task<string> CreateRequest()
        {
            return await _service.CreateRequestWork();
        }

        private async Task<string> CreateRequestWork()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var httpMethod = new HttpMethod(HttpContext.Request.HttpMethod);
            var currentUri = HttpContext.Request.Url;
            var uriBuilder = new UriBuilder($"{ServiceUrl}/{controllerName}/{actionName}")
            {
                Host = ServiceHost,
                Port = int.Parse(ServicePort),
                Fragment = currentUri.Fragment,
                Scheme = currentUri.Scheme,
                Path = currentUri.PathAndQuery
            };
            var requestMessage = new HttpRequestMessage(httpMethod, uriBuilder.Uri);
            var currentHeaders = HttpContext.Request.Headers.Keys;
            //ViewBag
            //ViewData
            //TempData
            // Pass as key/value pairs and initialize?
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                var streamReader = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();
                requestMessage.Content = new StringContent(streamReader);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Request.ContentType);
                requestMessage.Content.Headers.ContentEncoding.Add(Request.ContentEncoding.HeaderName);
                requestMessage.Content.Headers.ContentLength = Request.ContentLength;
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
                        requestMessage.Content.Headers.TryAddWithoutValidation(currentHeaders[i], this.ControllerContext.HttpContext.Request.Headers[currentHeaders[i]]);
                    }
                    else
                    {
                        requestMessage.Headers.TryAddWithoutValidation(currentHeaders[i], this.ControllerContext.HttpContext.Request.Headers[currentHeaders[i]]);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            var result = await client.SendAsync(requestMessage);
            var stringResult = await result.Content.ReadAsStringAsync();
            return stringResult;
        }
    }
}

*/
