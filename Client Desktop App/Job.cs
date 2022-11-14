using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client_Desktop_App
{
    [ServiceBehavior]
    public class Job : JobService
    {
        private JobServiceCallBack callback = null;
        private ObservableCollection<ClientClass> clients;

        public Job()
        {
            clients = new ObservableCollection<ClientClass>();
        }

        public void Connect(ClientClass client)
        {
            Console.WriteLine($"{client.ip} : {client.port} just connected");
            callback = new JobCallBack();
            clients.Add(client);
        }

        public ObservableCollection<ClientClass> GetConnnectedClients()
        {
            return clients;
        }

        public List<JobClass> GetJobs(string id)
        {
            
            return callback.returnJobs();
        }

        public void AddJob(JobClass job)
        {
            callback.GetJob(job);
        }

        public void RemoveJob(JobClass job)
        {
            callback.RemoveJob(job);
            Console.WriteLine("Job Removed");
        }
        public string GetJobResult(string result)
        {
            if(result == "executed")
            {
                return "success";
            }
            return "unsuccess";
        }

        public void Disconnect(ClientClass client)
        {
           
            clients.Remove(client);
            Console.WriteLine($"{client.ip} : {client.port} just disconnected");
        }
    }
}
