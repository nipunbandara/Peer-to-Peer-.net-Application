using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Desktop_App
{
    public class JobCallBack : JobServiceCallBack
    {
        List<JobClass> joblist = new List<JobClass>();

        public void ClientConnected(ObservableCollection<ClientClass> clients)
        {
            Console.WriteLine(clients.Count);
        }

        public void GetJob(JobClass job)
        {
            joblist.Add(job);
            Console.WriteLine(joblist);
        }

        public void RemoveJob(JobClass job)
        {
            joblist.Remove(job);
            Console.WriteLine(joblist);
        }

        public List<JobClass> returnJobs()
        {
            return joblist;
        }
    }
}
