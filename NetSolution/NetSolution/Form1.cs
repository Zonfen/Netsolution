using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

/**
 * @Author: Tony Pepic
 * @Date: 2018-03-23
 * 
 * Netsolution test - TMDB 
 * 
 * The following application is made to showcase how to communicate with third party API's.
 * The API will show a number of elements over HTTP. The number of API calls cannot be so few that the 
 * application can do it during startup.
 *
 * 
 * @ Copyright - Netsolution && Tony Pepic - All rights reserved 2018
 *
 * 
 */

namespace NetSolution
{
    public partial class Form1 : Form
    {
        private string searchText = "";
        private string searchURL = "";
        private static string apiKey = "988653aa72c493ee7cae5dc19416ecbf";        

        private dynamic jsonObject = null;
        bool isThreadfree = true;

        // Initiation 
        public Form1()
        {
            InitializeComponent();
            // Creates our directory for movie pics if it doesn't exist 
            DirectoryInfo di = Directory.CreateDirectory("C:/temp");
            // Get our "Picture not found" picture
            using (WebClient client = new WebClient())
                client.DownloadFileAsync(new Uri("http://www.wellesleysocietyofartists.org/wp-content/uploads/2015/11/image-not-found.jpg"), @"c:\temp\image404.jpg");
        }

        private async void MakeApiCall()
        {
            if (searchText == "")
                TextBlockFeedBack.Text = "You need to enter a valid query!";

            else if (searchText.Length > 0)
            {
                searchURL = "https://api.themoviedb.org/3/search/movie?api_key=" + apiKey + "&query=" + searchText;
                RestClient rClient = new RestClient();
                rClient.endPoint = searchURL;               

                string strResponse = await rClient.MakeRequestAsync();
                jsonObject = JsonConvert.DeserializeObject<dynamic>(strResponse);

                // Make a separate thread so we won't slow the UI thread down
                Thread workerThread = new Thread(() =>
                {
                    isThreadfree = false;
                    if (strResponse != string.Empty)
                    {
                        // Make the UI thread handle UI changes
                        Action action = () =>
                        {
                            OutputTextBox(strResponse);
                            treeView1.Nodes.Clear();
                            listView1.Clear();
                            TextBlockFeedBack.Text = "Results found!";
                        };
                        this.BeginInvoke(action);

                        GUIinteractions();
                    }
                    isThreadfree = true;
                });
                if(isThreadfree)
                    workerThread.Start();
            }
            else                         
                TextBlockFeedBack.Text = "An unknown error occured!"; // Not sure how this would happen, but just in case       
        }

        private void GUIinteractions()
        {
            if (jsonObject.total_results > 0)
            {
                int maxResults = 20;
                if (jsonObject.total_results < maxResults)
                    maxResults = jsonObject.total_results;

                ImageList imgs = new ImageList();
                imgs.ImageSize = new Size(75, 75);

                for (int i = 0; i < maxResults; i++)
                {
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile(new Uri("https://image.tmdb.org/t/p/original" + jsonObject.results[i].poster_path.ToString()), @"c:\temp\image" + i + ".jpg");
                        }
                        catch { /* dont want to catch anything really, non found pics will be replaced with 404 pic anyway };*/}
                    }
                    // Job for the UI thread
                    Action action = () =>
                    {
                        listView1.Columns.Add("Title");
                        listView1.Columns[0].Width = this.listView1.Width - 4;
                        try
                        {
                            imgs.Images.Add(Image.FromFile("c:/temp/image" + i.ToString() + ".jpg"));
                        }
                        catch
                        {
                            imgs.Images.Add(Image.FromFile("c:/temp/image404.jpg"));
                        }
                        // Add items to the view 
                        listView1.SmallImageList = imgs;
                        listView1.Items.Add(jsonObject.results[i].title.ToString(), i);

                        // TreeList for debugging json results
                        treeView1.Nodes.Add(jsonObject.results[i].title.ToString(), jsonObject.results[i].title.ToString());
                        foreach (var obj in jsonObject.results[i])
                            treeView1.Nodes[jsonObject.results[i].title.ToString()].Nodes.Add(obj.ToString());
                    };
                    // Invoke to make the workerthread wait for the UI thread
                    this.Invoke(action);
                }
            }
            else
            {
                Action action = () => TextBlockFeedBack.Text = "No Results found";
                this.BeginInvoke(action);
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            MakeApiCall();        
        }

        // When movies are clicked show details
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            int listIndex = listView1.SelectedIndices[0];
            overview.MaximumSize = new Size(220, 0);

            if (jsonObject != null)
            {
                title.Text = "Title: " + jsonObject.results[listIndex].title.ToString();
                overview.Text = jsonObject.results[listIndex].overview.ToString();
                rating.Text = "Rating: " + jsonObject.results[listIndex].vote_average.ToString();
                relDate.Text = "Released : " + jsonObject.results[listIndex].release_date.ToString();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            searchText = textBox2.Text;
        }

        public void OutputTextBox(string message)
        {
            textBox1.Text += message + Environment.NewLine;
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }
    }
}
