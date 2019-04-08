

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeInnovateInfoSys.Helper
{   
    /// <summary>
    /// 实现分页功能
    /// </summary>
    public class PageBarHelper
    {
        /// <summary>
        /// 获得分页界面
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="listCount">总记录</param>
        /// <param name="pageSize">分页数量</param>
        /// <param name="barCount">显示分页数</param>
        /// <param name="value">搜索参数</param>
        /// <returns></returns>
        public static string GetPageBar(int pageIndex, int listCount, int pageSize = 10, int barCount = 5,string value="")
        {
            //空记录
            if (listCount == 0)
            {
                return string.Empty;
            }
            //求页面数
            int pageCount = Convert.ToInt32(Math.Ceiling(listCount * 1.0 / pageSize));

            if (pageCount == 1)
            {
                return string.Empty;
            }
            int start = 0;
            int end = 0;
            if (pageCount <= barCount)//页数小于指定页数
            {
                start = 1;
                end = pageCount;
            }
            else
            {
                //剩余页数
                int count = pageCount - pageIndex;
                if (count >= 2)
                {
                    start = pageIndex - 2;
                    if (start <= 0)
                    {
                        start = 1;
                        end = start + barCount - 1;
                    }
                    else
                        end = pageIndex + 2;
                }
                else
                {
                    start = pageIndex - barCount + 1 + count;
                    end = pageIndex + count;
                }
            }
          
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("共 {0} 条数据   当前 {1}/{2} 页 &nbsp;&nbsp;&nbsp;&nbsp;", listCount, pageIndex, pageCount);
            //首页
            if (pageIndex > 1)
            {
                sb.AppendFormat("<a href='?pageIndex={0}&txtSearch={1}'    >首页</a>&nbsp;&nbsp;", 1, value);
                sb.AppendFormat("<a href='?pageIndex={0}&txtSearch={1}'   >上一页</a>&nbsp;&nbsp;", pageIndex - 1, value);
            }
            //计算中间页
            for (int i = start; i <= end; i++)
            {
                if (i == pageIndex)
                {
                    sb.Append(i + "&nbsp;&nbsp;&nbsp;&nbsp;");
                }
                else
                {
                    sb.AppendFormat("<a href='?pageIndex={0}&txtSearch={1}'  >{0}</a>&nbsp;&nbsp;&nbsp;&nbsp;", i, value);
                }
            }
            //下一页
            if (pageIndex < pageCount)
            {
                sb.AppendFormat("<a href='?pageIndex={0}&txtSearch={1}' >下一页</a>&nbsp;&nbsp;&nbsp;&nbsp;", pageIndex + 1, value);
                sb.AppendFormat("<a href='?pageIndex={0}&txtSearch={1}'  onfocus='this.blur()'>尾页</a>&nbsp;&nbsp;&nbsp;&nbsp;", pageCount, value);
            }

            return sb.ToString();
        }
    }
}
