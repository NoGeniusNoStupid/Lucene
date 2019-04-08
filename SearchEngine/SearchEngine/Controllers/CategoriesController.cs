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
    public class CategoriesController : BaseController
    {
        
        public ActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(search))
                search = "";
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<Categories> mlist = new List<Categories>();
            mlist = DB.Categories.Where(a => a.Name.Contains(search)).OrderByDescending(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.Categories.Where(a => a.Name.Contains(search)).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();

        }

        //
        // GET: /Categories/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Categories/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                //获取输入的类别名称
                string TypeName = collection["TypeName"];

                //判断是否存在
                var isExist = DB.Categories.FirstOrDefault(a => a.Name == TypeName);
                if (isExist != null)
                    return RedirectDialogToAction("该类别已经存在！");

                //添加
                Categories Categories = new Categories();
                Categories.Name = TypeName;
             
                DB.Categories.Add(Categories);
                return RedirectDialogToAction("Categories", "Index", "添加成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Categories/Edit/5

        public ActionResult Edit(int id)
        {
            var Info = DB.Categories.FirstOrDefault(a => a.Id == id);

            return View(Info);
        }

        //
        // POST: /Categories/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                //获取输入的图书类别名称
                string TypeName = collection["TypeName"];

                ////判断是否存在
                //var isExist = DB.Categories.FirstOrDefault(a => a.Name == TypeName);
                //if (isExist != null)
                //    return RedirectDialogToAction("该类别已经存在！");

                var Info = DB.Categories.FirstOrDefault(a => a.Id == id);
                Info.Name = TypeName;
                DB.Entry(Info).State = EntityState.Modified;
                return RedirectDialogToAction("Categories", "Index", "保存成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Categories/Delete/5

        public ActionResult Delete(int id)
        {
            var Info = DB.Categories.FirstOrDefault(a => a.Id == id);
            DB.Entry(Info).State = EntityState.Deleted;
            return RedirectDialogToAction("Categories", "Index", "删除成功！", DB.SaveChanges());
        }

    }
}
