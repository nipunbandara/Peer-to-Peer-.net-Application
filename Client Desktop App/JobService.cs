using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client_Desktop_App
{
    [ServiceContract]
    public interface JobService
    {
        [OperationContract]
        void Connect(ClientClass client);

        [OperationContract]
        List<JobClass> GetJobs(string id);

        [OperationContract]
        void AddJob(JobClass job);

        [OperationContract]
        void RemoveJob(JobClass job);

        [OperationContract]
        string GetJobResult(string result);

        [OperationContract]
        ObservableCollection<ClientClass> GetConnnectedClients();

        [OperationContract]
        void Disconnect(ClientClass client);

    }
}
