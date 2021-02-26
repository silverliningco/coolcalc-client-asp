using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

namespace asp.coolcalc.client
{
    /// <summary>
    /// Descripción breve de DealerCollectionHandler
    /// </summary>
    public class DealerCollectionHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
     
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    Get(context);
                    break;
                case "POST":
                    Post(context);
                    break;
                case "PUT":
                    Put(context);
                    break;
                case "DELETE":
                    Delete(context);
                    break;
            }
        }

        private string CoolcalcApiURL(HttpRequest request)
        {
            // To-do:
            // Implement your own code here to check that the accountNr in the URL /dealers/accountNr/.... 
            // corresponds to the current user/session info.
            // This is to prevent some dishonest user from accessing someone else's project list.
            // ...
            // If the account nr in the REST URL does not correspond to the session user, respond with a 401 "are you trying to hack me" code.
            // ...

            // Remove URL segments specific to our local entry point.
            string apiPath = request.ServerVariables["REQUEST_URI"].Replace(ConfigurationManager.AppSettings["local_path_segments"], "");

            // Construct the CoolCalc API URL.
            // apiPath includes query params.
            string uri = ConfigurationManager.AppSettings["api_domain"] + apiPath;

            return uri;
        }

        // coolcalcAuthentication returns a Base64 encoded HTTP Basic authentication string.
        private void CoolcalcAuthentication(HttpWebRequest request, String myURL)
        {
            NetworkCredential networkCredential = new NetworkCredential(ConfigurationManager.AppSettings["client_id"], ConfigurationManager.AppSettings["client_key"]);
            CredentialCache myCredentialCache = new CredentialCache { { new Uri(myURL), "Basic", networkCredential } };
            request.PreAuthenticate = true;
            request.Credentials = myCredentialCache;
        }

        public void Get(HttpContext context)  
        {
            // Get data from CoolCalc API.
            string myURL = CoolcalcApiURL(context.Request);

            // Create a request using a URL that can receive a post.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myURL);
            request.Method = "GET";

            // HTTP basic authentication.
            CoolcalcAuthentication(request, myURL);

            try
            {
                // Get data from CoolCalc API.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    // Output to the browser.
                    context.Response.StatusCode = (int)response.StatusCode;
                    context.Response.AddHeader("Content-Type", response.ContentType);
                    context.Response.AddHeader("Allow",response.Headers["Allow"]);
                    context.Response.Write(responseFromServer);
                }

                // Close the response.
                response.Close();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var pageContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Output to the browser.
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.AddHeader("Content-Type", response.ContentType);
                context.Response.Write(pageContent);

                // Close the response.
                response.Close();
            }
        }

        public void Post(HttpContext context)
        {
            // Get data from CoolCalc API.
            string myURL = CoolcalcApiURL(context.Request);

            // Create a request using a URL that can receive a post.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myURL);
            request.Method = "POST";

            // HTTP basic authentication.
            CoolcalcAuthentication(request, myURL);

            // Payload from the browser.
            var bodyStream = new StreamReader(context.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var myPayload = bodyStream.ReadToEnd();

            // Add payload.
            request.SendChunked = true;
            request.ContentLength = myPayload.Length;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(myPayload);
                streamWriter.Flush();
            }

            try
            {
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    // Output to the browser.
                    context.Response.StatusCode = (int)response.StatusCode;
                    context.Response.AddHeader("Content-Type", response.ContentType);// "application/json");
                    context.Response.AddHeader("Allow", response.Headers["Allow"]);
                    context.Response.AddHeader("Location", response.Headers["Location"]);
                    context.Response.Write(responseFromServer);
                }

                // Close the response.
                response.Close();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var pageContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Output to the browser.
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.AddHeader("Content-Type", response.ContentType);
                context.Response.Write(pageContent);

                // Close the response.
                response.Close();
            }
        }

        public void Put(HttpContext context)
        {
            // Get data from CoolCalc API.
            string myURL = CoolcalcApiURL(context.Request);

            // Create a request using a URL that can receive a post.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myURL);
            request.Method = "PUT";

            // HTTP basic authentication.
            CoolcalcAuthentication(request, myURL);

            // Payload from the browser.
            var bodyStream = new StreamReader(context.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var myPayload = bodyStream.ReadToEnd();

            // Add payload.
            request.SendChunked = true;
            request.ContentLength = myPayload.Length;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(myPayload);
                streamWriter.Flush();
            }

            try
            {
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    // Output to the browser.
                    context.Response.StatusCode = (int)response.StatusCode;
                    context.Response.AddHeader("Content-Type", response.ContentType);
                    context.Response.AddHeader("Allow", response.Headers["Allow"]);
                    context.Response.Write(responseFromServer);
                }

                // Close the response.
                response.Close();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var pageContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Output to the browser.
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.AddHeader("Content-Type", response.ContentType);
                context.Response.Write(pageContent);

                // Close the response.
                response.Close();
            }
        }

        public void Delete(HttpContext context)
        {
            // Get data from CoolCalc API.
            string myURL = CoolcalcApiURL(context.Request);

            // Create a request using a URL that can receive a post.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myURL);
            request.Method = "DELETE";

            // HTTP basic authentication.
            CoolcalcAuthentication(request, myURL);

            try
            {
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    // Output to the browser.
                    context.Response.StatusCode = (int)response.StatusCode;
                }

                // Close the response.
                response.Close();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var pageContent = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Output to the browser.
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.AddHeader("Content-Type", response.ContentType);
                context.Response.Write(pageContent);

                // Close the response.
                response.Close();
            }
        }
       
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}