

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.Models
{
    //数据上下文
    public class DBContextFactory
    {

        public static DbContext CreateDbContext()
        {
            //线程槽
            DbContext dbContext = (SearchEngineEntities)CallContext.GetData("dbContext");
            if(dbContext==null)
            {
                dbContext = new SearchEngineEntities();
                CallContext.SetData("dbContext", dbContext);
            }
            return dbContext;
        }
    }
}
