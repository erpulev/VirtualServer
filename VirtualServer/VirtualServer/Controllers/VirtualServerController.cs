using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualServer.Context;
using VirtualServer.Models;

namespace VirtualServer.Controllers
{
    public class VirtualServerController : Controller, IVirtualServerController
    {
        // GET: VirtualController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData()
        {
            return PartialView("~/Views/VirtualServer/partialVirtualServer.cshtml", GetDataServer());
        }

        [HttpGet]
        public ActionResult AddData()
        {
            SqlInjection psql = new SqlInjection();
            var result = psql.Insert();
            if (result)
            {
                return PartialView("~/Views/VirtualServer/partialVirtualServer.cshtml", GetDataServer());
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Во время создания сервера произошла ошибка",
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult DeleteData(int[] serversID)
        {
            SqlInjection msql = new SqlInjection();
            var result = msql.Delete(serversID); //Добавить обратку, что сервера точно удалены
            if (result)
            {
                return PartialView("~/Views/VirtualServer/partialVirtualServer.cshtml", GetDataServer());
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "При удалении серверов произошли ошибка",
                    data = PartialView("~/Views/VirtualServer/partialVirtualServer.cshtml", GetDataServer())
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GettotalUsageTime()
        {
            SqlInjection msql = new SqlInjection();
            var result = msql.GetTotalUsageTime();
            TimeSpan tm = TimeSpan.FromTicks(result);

            return Json(new { success = true, message = "mes", data = $"{tm.Days} д. {tm.Hours} ч. {tm.Minutes} м. {tm.Seconds} с." }, JsonRequestBehavior.AllowGet);
        }
        
        //Сравнение хеш кода двух моделей, для запроса на обновление страницы, если данные были изменены извне
        [HttpPost]
        public ActionResult GetEqualsHashCode(int hash)
        {
            //Логика получения и сравнения хешей двух моделей и возврат результат на страницу пользователю ввиде true false
            return Content("заглушка");
        }

        //local method - Get DATA model
        private List<Models.VirtualServers> GetDataServer()
        {
            SqlInjection psql = new SqlInjection();
            var model = psql.GetAll();
            return model;
        }
    }
}