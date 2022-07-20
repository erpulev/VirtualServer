using System.Collections.Generic;
using VirtualServer.Models;

namespace VirtualServer.Context
{
    internal interface ISqlInjection
    {
        bool Delete(int[] serversID);
        List<VirtualServers> GetAll();
        System.TimeSpan GetTotalUsageTime();
        bool Insert();
    }
}