using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

namespace asp.coolcalc.client
{
    /// <summary>
    /// Descripción breve de MJ8Reports
    /// </summary>
    public class MJ8Reports : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            // Get data from CoolCalc API.
            String myURL = ConfigurationManager.AppSettings["mj8_report_url"] + context.Request.QueryString["reportId"] + "&rev=latest";

            // Create a request using a URL that can receive a post.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(myURL);
            request.AllowAutoRedirect = true;
            request.Method = "GET";

            // HTTP basic authentication.
            coolcalcAuthentication(request, myURL);

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

        // coolcalcAuthentication returns a Base64 encoded HTTP Basic authentication string.
        private void coolcalcAuthentication(HttpWebRequest request, String myURL)
        {
            NetworkCredential networkCredential = new NetworkCredential(ConfigurationManager.AppSettings["client_id"], ConfigurationManager.AppSettings["client_key"]);
            CredentialCache myCredentialCache = new CredentialCache { { new Uri(myURL), "Basic", networkCredential } };
            request.PreAuthenticate = true;
            request.Credentials = myCredentialCache;
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