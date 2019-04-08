
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace SearchEngine.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public SearchEngineEntities DB
        {
            get
            {
                return (SearchEngineEntities)CreateDbContext();
            }
        }
        public static DbContext CreateDbContext()
        {
            //线程槽
            DbContext dbContext = (DbContext)CallContext.GetData("dbContext");
            if (dbContext == null)
            {
                dbContext = new SearchEngineEntities();
                CallContext.SetData("dbContext", dbContext);
            }
            return dbContext;
        }
        /// <summary>
        /// 操作提示
        /// </summary>
        /// <param name="actionName">跳转方法</param>
        /// <param name="msg">提示</param>
        /// <param name="ros">操作结果</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="roteValues">参数</param>
        /// <returns></returns>
        protected ActionResult RedirectDialogToAction(string controllerName, string actionName, string msg = null, int ros = -1, object roteValues = null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                if (ros > 0)
                    msg = "操作成功";
                else
                    msg = "操作失败,请检查数据输入或对应代码！";
            }
            string url = "/" + controllerName + "/" + actionName; 
            if(roteValues!=null)
                url = "/" + controllerName + "/" + actionName + "/" + roteValues; 
            
            string strTip = string.Empty;

            strTip = string.Format(@"<script languge='javascript'>alert('{0}');window.location='{1}'</script>", msg, url);
            Response.Write(strTip);
            return null;
        }
        /// <summary>
        /// 仅提示
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isHistory">返回上一页</param>
        /// <returns></returns>
        protected ActionResult RedirectDialogToAction(string msg, bool isHistory=true)
        {
            string tip = string.Empty;
            if (isHistory)
                tip = "self.location=document.referrer;";
            string strTip = string.Format(@"<script languge='javascript'>alert('{0}');{1}</script>", msg, tip);
            Response.Write(strTip);
            return null;

        }

    }
}
