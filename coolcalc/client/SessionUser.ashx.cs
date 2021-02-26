﻿using System.Web;
using Newtonsoft.Json;

namespace asp.coolcalc.client
{
    /// <summary>
    /// Descripción breve de SessionUser
    /// </summary>
    public class SessionUser : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            /* Implement your own code here, sample output is provided below.
		     * If user is logged in, respond with known account nr and individual user id.
		     * If user is not logged in, generate a random John-or-Jane-Doe id for the duration of the session.
		     * When a not logged in user becomes known, add a direct API call at the end of your login flow
		     * to update the identifying information for any work that the not logged in user may have done.
		     */

            // JSON response to client.
            context.Response.ContentType = "application/json";
            context.Response.Write(
                JsonConvert.SerializeObject(
                    new
                    {
                        userReference = "111",
                        dealerReference = "1626443386",
                        isAdmin = true
                    })
            );
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