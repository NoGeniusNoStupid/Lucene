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
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            //判断一下是否登录
            if (Session["Id"] == null)
                return RedirectDialogToAction("Admin", "Login", "请先登录后进行操作！");
            return View();
          
        }
        //后台登录
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Tel, string Pwd)
        {
            var admin = DB.Admin.FirstOrDefault(a => a.Tel == Tel && a.Pwd == Pwd);
            if (admin == null)
            {
                return RedirectDialogToAction("用户名或密码输入错误");
            }
            Session["Id"] = admin.Id;//记录登陆状态
            return RedirectDialogToAction("Admin", "Index", "登录成功");
        }


        /// <summary>
        /// 管理员管理
        /// </summary>
        /// <param name="search">搜索条件</param>
        /// <returns></returns>
        public ActionResult Manage(string search)
        {
            if (string.IsNullOrEmpty(search))
                search = "";
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<Admin> mlist = new List<Admin>();
            mlist = DB.Admin.Where(a => a.Tel.Contains(search)).OrderByDescending(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.Admin.Where(a => a.Tel.Contains(search)).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();
        }

        //添加管理员页面
        public ActionResult Create()
        {
            return View();
        }
        //添加管理员代码
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string Tel = collection["Tel"];
                string Pwd = collection["Pwd"];
                string surepwd = collection["surepwd"];
                if (Pwd != surepwd)
                    return RedirectDialogToAction("两次输入密码不一致！");


                //判断是否存在
                var isExist = DB.Admin.FirstOrDefault(a => a.Tel == Tel);
                if (isExist != null)
                    return RedirectDialogToAction("该手机号已经存在！");
                //添加
                Admin admin = new Admin();
                admin.Tel = Tel;
                admin.Pwd = Pwd;
                admin.AddTime = DateTime.Now;
                DB.Admin.Add(admin);
                return RedirectDialogToAction("Admin", "Manage", "添加成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //删除管理员
        public ActionResult Delete(int id)
        {
            var admin = DB.Admin.FirstOrDefault(a => a.Id == id);
            DB.Entry(admin).State = EntityState.Deleted;
            return RedirectDialogToAction("Admin", "Manage", "删除成功！", DB.SaveChanges());
        }

    }
}
