using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/**
 * @Author: Tony Pepic
 * @Date: 2018-03-22
 * 
 * Netsolution test - TMDB 
 * 
 * This Rest Client was made to make for Async API calls *
 * 
 * @ Copyright - Netsolution && Tony Pepic - All rights reserved 2018
 *
 * 
 */


namespace NetSolution
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class RestClient
    {
        public string endPoint { get; set; }
        public httpVerb httpMethod { get; set; }

        // Constructor
        public RestClient()
        {
            endPoint = string.Empty;
            httpMethod = httpVerb.GET;
        }

        public string MakeRequest()
        {
            string strResponse = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new ApplicationException("Error Code " + response.StatusCode.ToString());

                    using (Stream responseStream = response.GetResponseStream())
                        if (responseStream != null)
                            using (StreamReader reader = new StreamReader(responseStream))
                                strResponse = reader.ReadToEnd();

                }
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return strResponse;
        }

        public Task<string> MakeRequestAsync()
        {
            return Task.Factory.StartNew(() => MakeRequest());
        }
    }
}
