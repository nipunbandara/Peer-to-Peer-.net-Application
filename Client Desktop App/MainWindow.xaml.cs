using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using The_Web_Server.Models;

namespace Client_Desktop_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public delegate void listClients();

        int num = 0;
        int port = 0;
        string myIP = "";

        private static JobClass staticjob;

        public MainWindow()
        {
            InitializeComponent();

            Thread serverThread = new Thread(new ThreadStart(server));
            serverThread.Start();

            Thread networkThread = new Thread(new ThreadStart(clientDelMethod));
            networkThread.Start();

            string hostName = Dns.GetHostName();
            myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            Random random = new Random(1);
            port = 3321;

        }

        public void server()
        {

            JobService service = new Job();
            num = 0;
            Console.WriteLine("hey so like welcome to my server");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding(SecurityMode.None);
            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(Job));
            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to
            //accept on any interface. :8100 means this will use port 8100. DataService is a name for the
            //actual service, this can be any string.
            
            host.AddServiceEndpoint(typeof(JobService), tcp, "net.tcp://0.0.0.0:"+port+"/ClientService");
            //And open the host for business!
            host.Opened += HostOnOpened;
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();
            
        }

        private void HostOnOpened(object sender, EventArgs e)
        {
            Console.WriteLine("tcp service started");
           
            clientsTable clientT = new clientsTable();
            clientT.Id = num++;
            clientT.Ip = myIP;
            clientT.port = port;
            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/clientsTables", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(clientT));
            RestResponse restResponse = client.Execute(restRequest);

        }

        public void clientDelMethod()
        {
           while (true)
            {
                JobClass job = new JobClass();

                string URL = "https://localhost:44381/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest("api/clientsTables");
                RestResponse resp = client.Get(request);
                List<clientsTable> clientsLs = JsonConvert.DeserializeObject<List<clientsTable>>(resp.Content);

                foreach (clientsTable cl in clientsLs)
                {
                    string cleanIP = cl.Ip.Replace(".", string.Empty);
                    RestRequest Jobrequest = new RestRequest("api/jobsTables/" + cleanIP +cl.port);
                    RestResponse Jobresp = client.Get(Jobrequest);
                    List<jobsTable> jobsLs = JsonConvert.DeserializeObject<List<jobsTable>>(Jobresp.Content);

                    string ip = cl.Ip;
                    int port = (int)cl.port;

                    ClientClass clientt = new ClientClass();
                    clientt.ip = cl.Ip;
                    clientt.port = (int)cl.port;

                    ChannelFactory<JobService> foobFactory;
                    NetTcpBinding tcp = new NetTcpBinding(SecurityMode.None);
                    string foobURL = "net.tcp://" + ip + ":" + port + "/ClientService";

                    //var callBack = new InstanceContext(new JobCallBack());
                    
                    foobFactory = new ChannelFactory<JobService> (tcp, foobURL);
                    //var endpoint = new EndpointAddress(foobURL);
                    try
                    {
                        
                        var proxy = foobFactory.CreateChannel();


                        if (proxy != null)
                        {
                            proxy.Connect(clientt);
                            /*if(staticjob != null)
                            {
                                proxy.AddJob(staticjob);
                            }*/

                            foreach (jobsTable jt in jobsLs)
                            {
                                if (jt.JobStatus == "false")
                                {
                                    job.jobId = jt.Id;
                                    job.clientId = myIP;
                                    job.job = jt.JobName;
                                    job.jobStatus = jt.JobStatus;
                                    job.jobResult = "";

                                    proxy.AddJob(job);
                                }

                            }

                            List<JobClass> joblist = proxy.GetJobs(myIP);
                            foreach (JobClass jobx in joblist)
                            {
                                if(jobx.jobStatus == "false")
                                {
                                    jobsTable newjob = new jobsTable();
                                    newjob.Id = jobx.jobId;
                                    newjob.clientId = cleanIP + port.ToString();
                                    newjob.JobName = jobx.job;
                                    newjob.JobStatus = "completed";

                                    RestRequest restRequest = new RestRequest("api/jobsTables/" + jobx.jobId, Method.Put);
                                    restRequest.AddJsonBody(JsonConvert.SerializeObject(newjob));
                                    RestResponse restResponse = client.Execute(restRequest);
                                   
                                    Console.WriteLine("Currently Doing : " + jobx.job);
                                    //iron python
                                    if(proxy.GetJobResult(jobx) == "success")
                                    {
                                        Console.WriteLine("Successfully executed : " + jobx.job);
                                    }
                                    Thread.Sleep(5000);
                                    proxy.RemoveJob(jobx);
                                }

                            }


                        }

                        proxy.Disconnect(clientt);
                        
                        
                        //callBack.Close();
                        
                    }
                    catch (CommunicationException e)
                    {
                        foobFactory.Abort();
                        //callBack.Abort();
                    }
                    catch (TimeoutException e)
                    {
                        foobFactory.Abort();
                        //callBack.Abort();
                    }
                    catch (Exception e)
                    {
                        foobFactory.Abort();
                        //callBack.Abort();
                        throw;
                    }

                }
            }
           

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            jobsTable newjob = new jobsTable();
            newjob.Id = num++;
            string cleanIP = myIP.Replace(".", string.Empty);
            newjob.clientId = cleanIP + port.ToString();
            newjob.JobName = JobBox.Text;
            newjob.JobStatus = "false";
            
           /* staticjob.job = JobBox.Text;
            staticjob.clientId = cleanIP + port.ToString();
            staticjob.jobStatus = "false";*/

            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/jobsTables", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(newjob));
            RestResponse restResponse = client.Execute(restRequest);
        }
        private void ServicesChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (clientListView.SelectedItem != null)
            {
                string serviceSelected = clientListView.SelectedItem.ToString();

                string URL = "https://localhost:44381/";
                RestClient client = new RestClient(URL);

                RestRequest request = new RestRequest("api/jobsTables/" + serviceSelected);
                RestResponse resp = client.Get(request);

                List<jobsTable> jobs = JsonConvert.DeserializeObject<List<jobsTable>>(resp.Content);
                

                List<string> regName = new List<string>();
                foreach (jobsTable reg in jobs)
                {
                    regName.Add(reg.Id.ToString());
                }


                jobsListView.ItemsSource = regName;

            }
            */
        }

        private void OnSearchCompletion(IAsyncResult asyncResult)
        {

        }
    }
}
