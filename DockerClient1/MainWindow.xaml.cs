using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Threading;

namespace DockerClient1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        container[] items;
        string content;
        JToken objT;
        docker d2 = new docker();
        
        public MainWindow()
        {
            InitializeComponent();
            
        }



        public void getPage() {
            
            var client = new RestClient("https://cloud.docker.com/");
            client.Authenticator = new HttpBasicAuthenticator("glover279", "a8a01098-fb46-47fa-8da6-2339c5b8748a");

            var request = new RestRequest("/api/app/v1/service/", Method.GET);
            var cancellationTokenSource = new CancellationTokenSource();
            // request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            // request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            //   request.AddHeader("header", "value");

            // execute the request
            IRestResponse response = client.Execute(request);
            content = response.Content; // raw content as string
            
            JsonConvert.PopulateObject(content.ToString(), d2);

            JObject jObject = JObject.Parse(content.ToString());
            JToken meta = jObject["meta"];
            objT = jObject["objects"];
            int limit1 = (int)meta["limit"];
            int total = (int)meta["total_count"];
            // string tmp = (string)objT;
            //  tmp = tmp.Substring(1);
            //tmp=tmp.Remove(tmp.Length - 1);
            //JObject json = JObject.Parse(tmp);
            Dispatcher.Invoke(new Action(populate));




        }

        private void populate() {
            items = JsonConvert.DeserializeObject<container[]>(objT.ToString());
            var dataSource = new List<container>();
            int i = 0;
            foreach (container element in items)
            {
                dataSource.Add(items[i]);
                txtContainer.Items.Add(element.nickname);
                i++;
            }

            ////   teamname = (string)jUser["teamname"];
            //  email = (string)jUser["email"];
            //  players = jUser["players"].ToArray();

            docker d1 = JsonConvert.DeserializeObject<docker>(content);
            Console.WriteLine(d1);
            //txtAccount.Text = string.Format("Limit: {0} \t Containers: {1} , AutoDestroy: {2} ", limit1, total, items[0].auto_redeploy);
            textBox.Text = d2.ToString();
            textBox.Text += "\n" + content.ToString();
        }

        
        private void button_Click(object sender, RoutedEventArgs e)
        {   
            
            Thread thread1 = new Thread(new ThreadStart(getPage));
            thread1.Start();


        }

        private void txtContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = txtContainer.SelectedIndex;
            string container = string.Format("Nickname: {0} \n Autorestart {1} \n UUID: {2} \n Network Mode: {3}", items[index].nickname, items[index].autorestart, items[index].uuid, items[index].net);
            chkPriv.IsEnabled = true;
            chkPriv.IsChecked = items[index].privileged;
            txtContainer1.Text = container;
        }

        private void btnRaw_Click(object sender, RoutedEventArgs e)
        {
            JObject json = JObject.Parse(content);
            string formatted = json.ToString();

            MessageBox.Show(formatted,
    "JSON Retrieved");
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void chkPriv_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

    class docker {
        public meta meta { get; set; }
        public object obje { get; set; }
        public override string ToString()
        {
            return string.Format("Limit: {0}", meta.limit);
        }

    }

    class meta {

        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total_count { get; set; }
    }

    class objects1 {
        public container[] cont { get; set; }

    }

    class container1 {
        public string autodestroy { get; set; }
        public bool autoredeploy { get; set; }
    }

    public class objects
    {
        [JsonProperty("objects")]
        public container container { get; set; }
    }

    public class container
    {
        [JsonProperty("auto_redeploy")]
        public bool auto_redeploy { get; set; }
        [JsonProperty("nickname")]
        public string nickname { get; set; }
        [JsonProperty("autorestart")]
        public string autorestart { get; set; }
        [JsonProperty("net")]
        public string net { get; set; }
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonProperty("privileged")]
        public bool privileged { get; set; }




    }


}
