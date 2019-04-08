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
    public class PublishersController : BaseController
    {

        public ActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(search))
                search = "";
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<Publishers> mlist = new List<Publishers>();
            mlist = DB.Publishers.Where(a => a.Name.Contains(search)).OrderByDescending(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.Publishers.Where(a => a.Name.Contains(search)).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();

        }

        //
        // GET: /Publishers/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Publishers/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                //获取输入的类别名称
                string TypeName = collection["TypeName"];

                //判断是否存在
                var isExist = DB.Publishers.FirstOrDefault(a => a.Name == TypeName);
                if (isExist != null)
                    return RedirectDialogToAction("该出版社已经存在！");
                //添加
                Publishers Publishers = new Publishers();
                Publishers.Name = TypeName;

                DB.Publishers.Add(Publishers);
                return RedirectDialogToAction("Publishers", "Index", "添加成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Publishers/Edit/5

        public ActionResult Edit(int id)
        {
            var Info = DB.Publishers.FirstOrDefault(a => a.Id == id);

            return View(Info);
        }

        //
        // POST: /Publishers/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                //获取输入的图书类别名称
                string TypeName = collection["TypeName"];

                ////判断是否存在
                //var isExist = DB.Publishers.FirstOrDefault(a => a.Name == TypeName);
                //if (isExist != null)
                //    return RedirectDialogToAction("该类别已经存在！");

                var Info = DB.Publishers.FirstOrDefault(a => a.Id == id);
                Info.Name = TypeName;
                DB.Entry(Info).State = EntityState.Modified;
                return RedirectDialogToAction("Publishers", "Index", "保存成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Publishers/Delete/5

        public ActionResult Delete(int id)
        {
            var Info = DB.Publishers.FirstOrDefault(a => a.Id == id);
            DB.Entry(Info).State = EntityState.Deleted;
            return RedirectDialogToAction("Publishers", "Index", "删除成功！", DB.SaveChanges());
        }

    }
}
