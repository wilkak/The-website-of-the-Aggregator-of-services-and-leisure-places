using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebStoreMap.Models.Data;

namespace WebStoreMap.Models.Work
{
    public class DeleteLink : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Удаляем ccылку из базы данных
            using (DataBase DataBase = new DataBase())
            {
                DateTime DateTime = DateTime.UtcNow;
                List<LinkDataTransferObject> ListLinkDataTransferObject = await DataBase.Links.Where(x => x.Date.Month < DateTime.Date.Month).ToListAsync();

                foreach (LinkDataTransferObject Link in ListLinkDataTransferObject)
                {
                    _ = DataBase.Links.Remove(Link);
                    _ = DataBase.SaveChanges();
                }
            }
        }
    }
}