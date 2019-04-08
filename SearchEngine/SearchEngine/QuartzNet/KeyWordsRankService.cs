

using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine.QuartzNet
{
    public class KeyWordsRankService
    {
        //获取数据库操作对象
        SearchEngineEntities Db = (SearchEngineEntities)DBContextFactory.CreateDbContext();

        /// <summary>
        /// 将统计的明细表的数据插入。
        /// </summary>
        /// <returns></returns>
        public bool InsertKeyWordsRank()
        {
            string sql = "  insert into KeyWordsRank(KeyWords,SearchCount) select KeyWords,count(*)  from SearchDetail where DateDiff(day,SearchDetail.SearchDateTime,getdate())<=7 group by SearchDetail.KeyWords";
            return Db.Database.ExecuteSqlCommand(sql) > 0;
        }
        /// <summary>
        /// 删除汇总中的数据。
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllKeyWordsRank()
        {
            string sql = "truncate table KeyWordsRank";
            return Db.Database.ExecuteSqlCommand(sql) > 0;
        }

        public List<string> GetSearchMsg(string term)
        {
            //KeyWords like term%
            string sql = "select KeyWords from KeyWordsRank where KeyWords like @term";
            return Db.Database.SqlQuery<string>(sql, new SqlParameter("@term", term + "%")).ToList();
        }
    }
}
