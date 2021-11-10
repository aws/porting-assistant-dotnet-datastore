/*

using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace Modernize.Web.Mvc.Controllers
{
    public static class MonolithService
    {
        private static string ServiceHost = WebConfigurationManager.AppSettings["ServiceHost"];
        private static string ServicePort = WebConfigurationManager.AppSettings["ServicePort"];
        private static string ServiceUrl = $"https://{ServiceHost}";

        public static async Task<string> CreateRequest(ControllerContext controllerContext, HttpContextBase httpContext, HttpRequestBase httpRequest)
        {
            string actionName = controllerContext.RouteData.Values["action"].ToString();
            string controllerName = controllerContext.RouteData.Values["controller"].ToString();
            var httpMethod = new HttpMethod(httpContext.Request.HttpMethod);
            var currentUri = httpContext.Request.Url;
            var uriBuilder = new UriBuilder($"{ServiceUrl}/{controllerName}/{actionName}")
            {
                Host = ServiceHost,
                Port = int.Parse(ServicePort),
                Fragment = currentUri.Fragment,
                Scheme = currentUri.Scheme,
                Path = currentUri.PathAndQuery
            };
            var requestMessage = new HttpRequestMessage(httpMethod, uriBuilder.Uri);
            var currentHeaders = httpContext.Request.Headers.Keys;
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
                var streamReader = new StreamReader(httpContext.Request.InputStream).ReadToEnd();
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
                        requestMessage.Content.Headers.TryAddWithoutValidation(currentHeaders[i], controllerContext.HttpContext.Request.Headers[currentHeaders[i]]);
                    }
                    else
                    {
                        requestMessage.Headers.TryAddWithoutValidation(currentHeaders[i], controllerContext.HttpContext.Request.Headers[currentHeaders[i]]);
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
