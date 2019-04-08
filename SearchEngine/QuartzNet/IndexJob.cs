using Quartz;
using SearchEngine.QuartzNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzNet
{
    public class IndexJob : IJob
    {
        KeyWordsRankService bll = new KeyWordsRankService();
        public void Execute(JobExecutionContext context)
        {
            bll.DeleteAllKeyWordsRank();
            bll.InsertKeyWordsRank();
        }
    }
}
