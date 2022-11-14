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
using static IronPython.Modules._ast;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

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

        private static string curJob;
        private static int Jcount = 0;
        bool exec = false;

        public MainWindow()
        {
            InitializeComponent();

            Thread serverThread = new Thread(new ThreadStart(server));
            serverThread.Start();

            Thread networkThread = new Thread(new ThreadStart(clientDelMethod));
            networkThread.Start();

            string hostName = Dns.GetHostName();
            myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            Random random = new Random();
            port = random.Next(1000, 9999);
            //port = 3232;
            PortNum.Text = port.ToString();

        }

        public void server()
        {
            try
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

                host.AddServiceEndpoint(typeof(JobService), tcp, "net.tcp://0.0.0.0:" + port + "/ClientService");
                //And open the host for business!
                host.Opened += HostOnOpened;
                host.Open();
                Console.WriteLine("System Online");
                Console.ReadLine();
            }
            catch(Exception e)
            {
                MessageBox.Show("Data Server is not up");
            }

        }

        private void HostOnOpened(object sender, EventArgs e)
        {
            Console.WriteLine("tcp service started");

            clientsTable clientT = new clientsTable();
            //clientT.Id = num++;
            clientT.Ip = myIP;
            clientT.port = port;
            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/clientsTables1", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(clientT));
            RestResponse restResponse = client.Execute(restRequest);

        }

        public void clientDelMethod()
        {
            string mycleanIP = myIP.Replace(".", string.Empty);
            string myipandport = mycleanIP + port.ToString();

            while (true)
            {
                JobClass job = new JobClass();
                List<clientsTable> clientsLs = null;
                RestClient client = null;
                RestRequest request = null;
                RestResponse resp = null;

                try
                { 
                    string URL = "https://localhost:44381/";
                    client = new RestClient(URL);
                    request = new RestRequest("api/clientsTables1");
                    resp = client.Get(request);
                    clientsLs = JsonConvert.DeserializeObject<List<clientsTable>>(resp.Content);
                }
                catch(Exception e)
                {
                    clientsLs = null;
                }

                if(clientsLs != null)
                {
                    foreach (clientsTable cl in clientsLs)
                    {
                        string cleanIP = cl.Ip.Replace(".", string.Empty);
                        string ipandport = cleanIP + cl.port;
                        if ((myipandport != ipandport) && exec)
                        {

                            RestRequest Jobrequest = new RestRequest("api/jobsTables/" + ipandport);
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

                            foobFactory = new ChannelFactory<JobService>(tcp, foobURL);
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
                                        if (jobx.jobStatus == "false")
                                        {
                                            jobsTable newjob = new jobsTable();
                                            newjob.Id = jobx.jobId;
                                            newjob.clientId = cleanIP + port.ToString();
                                            newjob.JobName = jobx.job;

                                            Console.WriteLine("Currently Doing : " + jobx.job);

                                            curJob = jobx.job;
                                            //iron python

                                            int var1, var2;
                                            var1 = 0;
                                            var2 = 1;
                                            ScriptEngine engine = Python.CreateEngine();
                                            ScriptScope scope = engine.CreateScope();
                                            engine.Execute(jobx.job, scope);
                                            dynamic testFunction = scope.GetVariable("test_func");
                                            var result = testFunction(var1, var2);
                                            string res = "";
                                            if (result != null)
                                            {
                                                res = "executed";
                                                Jcount++;
                                            }
                                            MessageBox.Show("Current Job : " + jobx.job + " Result : " + result + ",Completed Jobs :" + Jcount);

                                            if (proxy.GetJobResult(res) == "success")
                                            {
                                                Console.WriteLine("Successfully executed : " + jobx.job);
                                                //JobsCompleted.Text = Jcount.ToString();
                                                newjob.JobStatus = "completed";
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error occured : " + jobx.job);
                                                newjob.JobStatus = "false";

                                            }
                                            RestRequest restRequest = new RestRequest("api/jobsTables/" + jobx.jobId, Method.Put);
                                            restRequest.AddJsonBody(JsonConvert.SerializeObject(newjob));
                                            RestResponse restResponse = client.Execute(restRequest);
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
                                continue;
                                //callBack.Abort();
                            }
                            catch (TimeoutException e)
                            {
                                foobFactory.Abort();
                                continue;
                                //callBack.Abort();
                            }
                            catch (Exception e)
                            {
                                foobFactory.Abort();
                                continue;
                                //callBack.Abort();
                                throw;
                            }
                        }


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


            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/jobsTables", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(newjob));
            RestResponse restResponse = client.Execute(restRequest);

            if (restResponse.IsSuccessStatusCode)
            {
                MessageBox.Show("Data Added Successfully");
            }
        }
        private void ExButton_Click(object sender, RoutedEventArgs e)
        {
            exec = true;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            JobsCompleted.Text = Jcount.ToString();
            CurrentJob.Text = curJob;
        }
    }
}
