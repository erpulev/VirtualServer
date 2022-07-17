using System.Web.Mvc;

namespace VirtualServer.Controllers
{
    public interface IVirtualServerController
    {
        ActionResult AddData();
        ActionResult DeleteData(int[] serversID);
        ActionResult GetData();
        ActionResult GetEqualsHashCode(int hash);
        ActionResult GettotalUsageTime();
        ActionResult Index();
    }
}