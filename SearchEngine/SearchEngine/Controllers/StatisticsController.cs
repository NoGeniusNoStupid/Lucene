using CollegeInnovateInfoSys.Helper;
using SearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SearchEngine.Controllers
{
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// 热词统计
        /// </summary>
        /// <returns></returns>
        public ActionResult KeyWordsRank()
        {
           
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<KeyWordsRank> mlist = new List<KeyWordsRank>();
            mlist = DB.KeyWordsRank.Where(a => true).OrderByDescending(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.KeyWordsRank.Where(a => true).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();

        }
        /// <summary>
        /// 搜索记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchDetail()
        {
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<SearchDetail> mlist = new List<SearchDetail>();
            mlist = DB.SearchDetail.Where(a => true).OrderByDescending(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.SearchDetail.Where(a => true).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();
        }
        
    }
}
