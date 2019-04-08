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
    public class BookController : BaseController
    {

        public ActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(search))
                search = "";
            //分页设置
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 5;//页面记录数
            List<Books> mlist = new List<Books>();
            mlist = DB.Books.Where(a => a.Title.Contains(search)).OrderBy(a => a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            int listCount = DB.Books.Where(a => a.Title.Contains(search)).Count();
            //生成导航条
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize);
            ViewData["List"] = mlist;
            ViewData["PageBar"] = strBar;

            return View();

        }

        //
        // GET: /Books/Create

        public ActionResult Create()
        {
            //获取出版社
            List<Publishers> Publishers = new List<Publishers>();
            Publishers = DB.Publishers.Where(a => true).ToList();

            ViewData["Publishers"] = Publishers;

            //获取图书类别
            List<Categories> Categories = new List<Categories>();
            Categories = DB.Categories.Where(a => true).ToList();

            ViewData["Categories"] = Categories;

            return View();
        }

        //
        // POST: /Books/Create

        [HttpPost]
        public ActionResult Create(Books Book)
        {
            try
            {
                Book.Clicks = 0;
                DB.Books.Add(Book);
                return RedirectDialogToAction("Book", "Index", "添加成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Books/Edit/5

        public ActionResult Edit(int id)
        {
            //获取出版社
            List<Publishers> Publishers = new List<Publishers>();
            Publishers = DB.Publishers.Where(a => true).ToList();

            ViewData["Publishers"] = Publishers;

            //获取图书类别
            List<Categories> Categories = new List<Categories>();
            Categories = DB.Categories.Where(a => true).ToList();

            ViewData["Categories"] = Categories;

            var Info = DB.Books.FirstOrDefault(a => a.Id == id);

            return View(Info);
        }

        //
        // POST: /Books/Edit/5

        [HttpPost]
        public ActionResult Edit(Books Book)
        {
            try
            {
                DB.Entry(Book).State = EntityState.Modified;
                return RedirectDialogToAction("Book", "Index", "保存成功！", DB.SaveChanges());
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Books/Delete/5

        public ActionResult Delete(int id)
        {
            var Info = DB.Books.FirstOrDefault(a => a.Id == id);
            DB.Entry(Info).State = EntityState.Deleted;
            return RedirectDialogToAction("Book", "Index", "删除成功！", DB.SaveChanges());
        }

    }
}
