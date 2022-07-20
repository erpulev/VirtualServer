using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using VirtualServer.Models;

namespace VirtualServer.Context
{
    public class VirtualServerContext : DbContext
    {
        public VirtualServerContext() //|DataDirectory| - path to local file mdf (if solution change directory)
            : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DatabaseVirtualServer.mdf;Integrated Security=True") //|DataDirectory| --> need create manual folder App_Data
        {}

        public DbSet<VirtualServers> VirtualServer { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<VirtualServers>();

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Model
    {
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
    }
    class SqlInjection : ISqlInjection
    {
        public bool Delete(int[] serversID)
        {
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    var res = msql.VirtualServer.Where(w => serversID.Any(a => a.Equals(w.VirtualServerId))).ToList();

                    res.ForEach(f => f.RemoveDateTime = DateTime.Now);

                    //msql.Entry(res[0]).State = EntityState.Modified;
                    var r1 = msql.SaveChanges();

                    if (serversID.Length == r1) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                //обработчик ошибок в файл/бд/почту
                return false;
            }
        }

        public List<Models.VirtualServers> GetAll()
        {
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    var model = msql.VirtualServer.OrderByDescending(o => o.VirtualServerId).ToList();
                    return model;
                }
            }
            catch (Exception e)
            {
                //Logics save error to bd or file
                return null;
            }
        }

        //Get total time worked servers
        public TimeSpan GetTotalUsageTime()
        {
            List<Model> list = new List<Model>();
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    var res = msql.VirtualServer.ToList();

                    //В зависимости от длительности выполнения кода ниже, фиксируем DateTime.Now на момент начала запроса.
                    DateTime now = DateTime.Now;  //new DateTime(2019, 01, 21, 16, 51, 00);

                    //флаги для циклов
                    bool findWhile = false; //1ый цикл
                    bool first = true; //выход из 2ого цикла

                    //Разбор все периодов, сопоставление на совпадение по 4м признакам.
                    while (!findWhile)
                    {
                        findWhile = true;
                        first = true;

                        for (int i = 0; i < res.Count(); i++)
                        {
                            if (!first)
                            {
                                break;
                            }
                            DateTime start = res[i].CreateDateTime;
                            DateTime end = res[i].RemoveDateTime == null ? now : (DateTime)res[i].RemoveDateTime;

                            for (int ii = 0; ii < res.Count(); ii++)
                            {
                                if (i == ii)
                                {
                                    continue;
                                }

                                DateTime startP = res[ii].CreateDateTime;
                                DateTime endP = res[ii].RemoveDateTime == null ? now : (DateTime)res[ii].RemoveDateTime;

                                //left period
                                if (startP <= start && endP >= start && endP <= end )
                                {
                                    if (startP < start)
                                    {
                                        res[i].CreateDateTime = startP;
                                        res.RemoveAt(ii); findWhile = false; first = false; break;
                                    }
                                    else
                                    {
                                        res.RemoveAt(ii); findWhile = false; first = false; break;
                                    }
                                }
                                //right period
                                else if (startP >= start && endP >= end && startP <= end)
                                {
                                    if (endP > end)
                                    {
                                        res[i].RemoveDateTime = endP;
                                        res.RemoveAt(ii); findWhile = false; first = false; break;
                                    }
                                    else
                                    {
                                        res.RemoveAt(ii); findWhile = false; first = false; break;
                                    }
                                }
                                //in middle period. 
                                else if (startP >= start && endP <= end)
                                {
                                    res.RemoveAt(ii); findWhile = false; first = false; break;
                                }
                                //out middle period
                                else if (startP <= start && endP >= end)
                                {
                                    res[i].CreateDateTime = startP;
                                    res[i].RemoveDateTime = endP;
                                    res.RemoveAt(ii); findWhile = false; first = false; break;
                                }
                            }

                        }
                    }

                    long total = 0;
                    foreach (var item in res)
                    {
                        total += ((item.RemoveDateTime == null ? now : item.RemoveDateTime) - item.CreateDateTime).Value.Ticks;
                    }

                    return TimeSpan.FromTicks(total);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                //обработчик ошибок в файл/бд/почту
                throw;
            }
        }

        //Add new server
        public bool Insert()
        {
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    var server = new Models.VirtualServers()
                    {
                        CreateDateTime = DateTime.Now //or UTC if regional
                    };
                    var r1 = msql.VirtualServer.Add(server);
                    var r2 = msql.SaveChanges();

                    return true;
                }
            }
            catch (Exception e)
            {
                //обработчик ошибок в файл/бд/почту
                return false;
            }
        }

    }

}