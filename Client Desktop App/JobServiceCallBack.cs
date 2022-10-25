using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client_Desktop_App
{
    public interface JobServiceCallBack
    {
        [OperationContract]
        void GetJob(JobClass job);

        [OperationContract]
        void RemoveJob(JobClass job);

        [OperationContract]
        List<JobClass> returnJobs();

        [OperationContract]
        void ClientConnected(ObservableCollection<ClientClass> clients);

    }
}
