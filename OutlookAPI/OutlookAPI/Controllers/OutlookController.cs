using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using Newtonsoft.Json;
using OutlookAPI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OutlookAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(500)]
    [ProducesResponseType(400)]
    public class OutlookController : ControllerBase
    {
        #region ----- Private -----

        private const string UrlBase = "https://graph.microsoft.com/v1.0/";
        private readonly ILogger<OutlookController> _logger;

        public OutlookController(ILogger<OutlookController> logger) => _logger = logger;

        /*

        private APIResponse GetResponse<T>(string uri, string method)
        {
            return GetResponse<T>(uri, method, new Dictionary<string, object>());
        }

        private T GetResponse<T>(string uri, string method, Dictionary<string, object> data)
        {
            string _accessToken = "";
            if (Request.Headers.Where(x => x.Key == "Authorization").Any())
            {
                var headerValues = Request.Headers.FirstOrDefault(x => x.Key == "Authorization");

                _accessToken = headerValues.Value.ToString().Replace("Bearer ", "");
            }

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Headers.Add("Authorization", string.Format("Bearer {0}", _accessToken));

            if (data.Count != 0)
            {
                string paramsStr = string.Join("&", data.Select(item => String.Format("{0}={1}", item.Key, item.Value)));
                request.ContentLength = paramsStr.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                using (var sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(paramsStr);
                    sw.Close();
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            string responseText;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(responseText);
        }


        private APIResponse GetResponse<T>(string uri, string method, Dictionary<string, object> data)
        {
            var response = new APIResponse { isError = true };

            try
            {
                string _accessToken = "";
                if (Request.Headers.Where(x => x.Key == "Authorization").Any())
                {
                    var headerValues = Request.Headers.FirstOrDefault(x => x.Key == "Authorization");

                    _accessToken = headerValues.Value.ToString().Replace("Bearer ", "");
                }

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = method;
                request.Headers.Add("Authorization", string.Format("Bearer {0}", _accessToken));

                if (data.Count != 0)
                {
                    string paramsStr = string.Join("&", data.Select(item => String.Format("{0}={1}", item.Key, item.Value)));
                    request.ContentLength = paramsStr.Length;
                    request.ContentType = "application/x-www-form-urlencoded";

                    using (var sw = new StreamWriter(request.GetRequestStream()))
                    {
                        sw.Write(paramsStr);
                        sw.Close();
                    }
                }

                string responseText;

                using (var webResponse = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                var resultObject = JsonConvert.DeserializeObject<T>(responseText);

                response.isError = false;
                response.Data = resultObject;
            }
            catch (System.Net.WebException exWeb)
            {
                string errorMessage = "";
                try
                {
                    using (var reader = new StreamReader(exWeb.Response.GetResponseStream()))
                        errorMessage = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(errorMessage)) errorMessage = exWeb.ToString();

                    _logger.LogError(errorMessage);
                }
                catch (Exception exInner)
                {
                    errorMessage = exWeb.ToString() + Environment.NewLine + exInner.ToString();
                    _logger.LogError(errorMessage);
                }

                response.isError = true;
                response.ErrorMessage = errorMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return response;
        }

        */

        private async Task<APIResponse> GetResponse<T>(string uri, string method) => await GetResponse<T>(uri, method, null);

        private async Task<APIResponse> GetResponse<T>(string uri, string method, OutlookEvent @event)
        {
            var response = new APIResponse { isError = true };

            try
            {
                string _accessToken = "";
                if (Request.Headers.Where(x => x.Key == "Authorization").Any())
                {
                    var headerValues = Request.Headers.FirstOrDefault(x => x.Key == "Authorization");

                    _accessToken = headerValues.Value.ToString().Replace("Bearer ", "");
                }


                string responseText = "";
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                    HttpResponseMessage result = null;

                    switch (method)
                    {
                        case "POST":
                            result = await client.PostAsJsonAsync(uri, @event);
                            break;
                        case "GET":
                            result = await client.GetAsync(uri);
                            break;
                        case "DELETE":
                            result = await client.DeleteAsync(uri);
                            break;
                        case "PATCH":
                            var json = JsonConvert.SerializeObject(@event);
                            var encoding = System.Text.Encoding.UTF8;
                            var content = new StringContent(json, encoding, "application/json");
                            result = await client.PatchAsync(uri, content);
                            break;
                    }

                    responseText = await result?.Content.ReadAsStringAsync();

                    if (!result.IsSuccessStatusCode)
                    {
                        response.ErrorMessage = responseText;
                        return response;
                    }
                }



                var resultObject = JsonConvert.DeserializeObject<T>(responseText);

                response.isError = false;
                response.Data = resultObject;
            }
            catch (System.Net.WebException exWeb)
            {
                string errorMessage = "";
                try
                {
                    using (var reader = new StreamReader(exWeb.Response.GetResponseStream()))
                        errorMessage = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(errorMessage)) errorMessage = exWeb.ToString();

                    _logger.LogError(errorMessage);
                }
                catch (Exception exInner)
                {
                    errorMessage = exWeb.ToString() + Environment.NewLine + exInner.ToString();
                    _logger.LogError(errorMessage);
                }

                response.isError = true;
                response.ErrorMessage = errorMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        #endregion


        //[HttpGet]
        //public IEnumerable<OutlookCalendar> GetCalendars()
        //{
        //    var data = GetResponse<ReceivedCalendarsData>(UrlBase + "me/calendars", "GET");
        //    return data.value.Where(item => item.canEdit == true);
        //}


        /// <summary>
        /// Create an Event
        /// </summary>
        /// <param name="event">Event JSON</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(OutlookEvent), 200)]
        public IActionResult CreateEvent(OutlookEvent @event)
        {
            var response = GetResponse<OutlookEvent>(UrlBase + "me/events", "POST", @event).Result;

            if (response.isError)
                return new ContentResult { StatusCode = 500, Content = response.ErrorMessage };

            return Ok(response.Data);
        }


        /// <summary>
        /// Edit an Event by ID
        /// </summary>
        /// <param name="event">Event JSON</param>
        /// <returns></returns>
        [HttpPatch]
        [ProducesResponseType(typeof(OutlookEvent), 200)]
        public IActionResult UpdateEvent(OutlookEvent @event)
        {
            var response = GetResponse<OutlookEvent>(UrlBase + "/me/events/" + @event.id, "PATCH", @event).Result;

            if (response.isError)
                return new ContentResult { StatusCode = 500, Content = response.ErrorMessage };

            return Ok(response.Data);
        }


        /// <summary>
        /// Get Events List
        /// </summary>
        /// <returns><list type="OutlookEvent"/>"</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OutlookEvent>), 200)]
        public IActionResult ListEvents() //string calendarId, DateTime from, DateTime to)
        {
            string requestStr = String.Format("{0}me/events?$select=subject,bodyPreview,start,end,isAllDay,isCancelled,isOrganizer", UrlBase);

            var response = GetResponse<ReceivedEventsData>(requestStr, "GET").Result;

            if (response.isError)
                return new ContentResult { StatusCode = 500, Content = response.ErrorMessage };

            return Ok(response.Data);
        }



        /// <summary>
        /// Delete an event based on ID
        /// </summary>
        /// <param name="id">Valid event ID</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        public IActionResult DeleteEvent(string id)
        {
            var response = GetResponse<object>(UrlBase + "me/events/" + id, "DELETE").Result;

            if (response.isError)
                return new ContentResult { StatusCode = 500, Content = response.ErrorMessage };

            return NoContent();
        }
    }
}
