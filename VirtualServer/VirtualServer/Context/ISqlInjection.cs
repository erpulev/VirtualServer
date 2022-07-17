using System.Collections.Generic;
using VirtualServer.Models;

namespace VirtualServer.Context
{
    internal interface ISqlInjection
    {
        void CheckTotalTimeWorked();
        bool Delete(int[] serversID);
        List<VirtualServers> GetAll();
        long GetTotalUsageTime();
        bool Insert();
    }
}