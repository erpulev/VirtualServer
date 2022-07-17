using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VirtualServer.Models;

namespace VirtualServer.Context
{
    public class VirtualServerInitializer : DropCreateDatabaseIfModelChanges<VirtualServerContext>
    {
        //Значения по умолчанию при создании таблицы
        protected override void Seed(VirtualServerContext context)
        {
            var model = new IsWorkeds() { Id = 1, Worked = false };
            context.IsWorked.Add(model);

            context.SaveChanges();
            base.Seed(context);
        }
    }
    public class VirtualServerContext : DbContext
    {
        public VirtualServerContext() //|DataDirectory| - path to local file mdf (if solution change directory)
            : base(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DatabaseVirtualServer.mdf;Integrated Security=True") //|DataDirectory| --> need create manual folder App_Data
        {
            //новый инициализатор
            Database.SetInitializer<VirtualServerContext>(new VirtualServerInitializer());
        }

        public DbSet<VirtualServers> VirtualServer { get; set; }
        public DbSet<IsWorkeds> IsWorked { get; set; }
        public DbSet<Counters> Counter { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<VirtualServers>();
            modelBuilder.Entity<IsWorkeds>();
            modelBuilder.Entity<Counters>();

            base.OnModelCreating(modelBuilder);
        }
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

                    CheckTotalTimeWorked();

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
        public long GetTotalUsageTime()
        {
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    var model = msql.Counter.ToList();
                    var res = model.Sum(s => ((s.end == null ? DateTime.Now : s.end) - s.start).Value.Ticks);
                    return res;
                }
            }
            catch (Exception e)
            {
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

                    CheckTotalTimeWorked();

                    return true;
                }
            }
            catch (Exception e)
            {
                //обработчик ошибок в файл/бд/почту
                return false;
            }
        }

        //Try Check worked any server
        public void CheckTotalTimeWorked()
        {
            //Вариант: Возможно ли использовать планировщик в SQL, "~монитор", "Хранимая процедура" ?
            //Вариант: Возможно выделить один поток на ежесекундную проверку? (Риски: ресурсо/затратно)
            //Риски: рассинхронизация статуса и работы серверов 
            try
            {
                using (var msql = new VirtualServerContext())
                {
                    //Работало ли по статусу?
                    var workWas = msql.IsWorked.Where(w => w.Id == 1).FirstOrDefault().Worked;

                    //Работает ли хоть один сервер сейчас?
                    var workNow = msql.VirtualServer.Where(w => w.RemoveDateTime == null).FirstOrDefault();

                    if (!workWas && workNow == null) //Не работала и не работает
                    {
                        return;
                    }
                    else if (!workWas && workNow != null) //Не работала, но теперь работает
                    {
                        var res = msql.IsWorked.Where(w => w.Id == 1).FirstOrDefault();
                        res.Worked = true;
                        var ri1 = msql.SaveChanges();

                        Counters counter = new Counters() { start = DateTime.Now }; //or// start = msql.VirtualServer.Where(w => w.RemoveDateTime == null).Min(m => m.CreateDateTime);
                        var rc1 = msql.Counter.Add(counter);
                        var rc2 = msql.SaveChanges();
                    }
                    else if (workWas && workNow == null) //Работала, но теперь не работает
                    {
                        var res = msql.IsWorked.Where(w => w.Id == 1).FirstOrDefault();
                        res.Worked = false;
                        var ri1 = msql.SaveChanges();

                        var req = msql.Counter.Where(w => w.end == null).FirstOrDefault(); 

                        if (req == null)// if req == null - ошибка синхронизации --
                        {
                            //Логика устранения ошибки синхронизации
                        }
                        else
                        {
                            req.end = DateTime.Now;
                            var r2 = msql.SaveChanges();
                        }
                        
                    }
                    else if (workWas && workNow != null) //Работала и ещё работает
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                //обработчик ошибок в файл/бд/почту
                throw;
            }
        }
    }

}