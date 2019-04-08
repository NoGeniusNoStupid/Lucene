using CollegeInnovateInfoSys.Helper;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SearchEngine.Models;
using SearchEngine.QuartzNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SearchEngine.Controllers
{
    //搜索类
    public class SearchController : Controller
    {
        //获取数据库操作对象
        SearchEngineEntities Db = (SearchEngineEntities)DBContextFactory.CreateDbContext();

        KeyWordsRankService keyWordsRankService = new KeyWordsRankService();
        /// <summary>
        /// 展示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchContent()
        {

            List<ViewModelContent> list = ShowSearchContent();


            //实现分页功能
            int pageIndex = Request.QueryString["pageIndex"] != null ? int.Parse(Request.QueryString["pageIndex"]) : 1;
            int pageSize = 10;//页面记录数
            int listCount = list.Count;
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//分页
            //生成导航条
            string search = Request["txtSearch"].Trim();
            string strBar = PageBarHelper.GetPageBar(pageIndex, listCount, pageSize, 5, search);
            ViewData["mPageBar"] = strBar;
            ViewData["list"] = list;
            return View("Index");
            //if (!string.IsNullOrEmpty(Request["btnSearch"]))
            //{
            //    List<ViewModelContent> list = ShowSearchContent();
            //    ViewData["list"] = list;
            //    return View("Index");
            //}
            //else
            //{
            //    CreateSerachContent();
            //}
            //return Content("ok");
        }
        /// <summary>
        /// 获得搜索内
        /// </summary>
        /// <returns></returns>
        private List<ViewModelContent> ShowSearchContent()
        {
            //string indexPath = @"C:\lucenedir";
            string indexPath = Server.MapPath("~/lucenedir");
            List<string> list = Common.WebCommon.PanGuSplitWord(Request["txtSearch"].Trim());//对用户输入的搜索条件进行拆分。
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件-简单
            //PhraseQuery query = new PhraseQuery();
            //foreach (string word in list)//先用空格，让用户去分词，空格分隔的就是词“计算机   专业”
            //{
            //    query.Add(new Term("Content", word));//根据内容
            //}
            //query.SetSlop(100);//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）
            //改造搜索条件
            PhraseQuery query = new PhraseQuery();
            foreach (string word in list)//先用空格，让用户去分词，空格分隔的就是词“计算机   专业”
            {
                query.Add(new Term("Content", word));//根据内容
            }
            query.SetSlop(100);//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）

            PhraseQuery queryTitle = new PhraseQuery();
            foreach (string word in list)//先用空格，让用户去分词，空格分隔的就是词“计算机   专业”
            {
                queryTitle.Add(new Term("Title", word));//根据内容
            }
            queryTitle.SetSlop(100);
            //组合搜索条件
            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(query, BooleanClause.Occur.SHOULD);
            booleanQuery.Add(queryTitle, BooleanClause.Occur.SHOULD);

          
            //query.Add(new Term("body","语言"));--可以添加查询条件，两者是add关系.顺序没有关系.
            // query.Add(new Term("body", "大学生"));
            // query.Add(new Term("body", kw));//body中含有kw的文章
        
            //TopScoreDocCollector是盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(booleanQuery, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            ScoreDoc[] docs = collector.TopDocs(0, collector.GetTotalHits()).scoreDocs;//得到所有查询结果中的文档,GetTotalHits():表示总条数   TopDocs(300, 20);//表示得到300（从300开始），到320（结束）的文档内容.
            //可以用来实现分页功能
            List<ViewModelContent> viewModelList = new List<ViewModelContent>();
            for (int i = 0; i < docs.Length; i++)
            {
                //
                //搜索ScoreDoc[]只能获得文档的id,这样不会把查询结果的Document一次性加载到内存中。降低了内存压力，需要获得文档的详细内容的时候通过searcher.Doc来根据文档id来获得文档的详细内容对象Document.
                ViewModelContent viewModel = new ViewModelContent();
                int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//找到文档id对应的文档详细信息
                viewModel.Id = Convert.ToInt32(doc.Get("Id"));// 取出放进字段的值
                //viewModel.Title = doc.Get("Title");
               
                viewModel.Title = Common.WebCommon.CreateHightLight(Request["txtSearch"], doc.Get("Title"));//将搜索的关键字高亮显示。
                viewModel.Content = Common.WebCommon.CreateHightLight(Request["txtSearch"], doc.Get("Content"));//将搜索的关键字高亮显示。
                viewModelList.Add(viewModel);
            }
            //先将搜索的词插入到明细表。
            SearchDetail searchDetail = new SearchDetail();
            //searchDetail.Id = Guid.NewGuid();
            searchDetail.KeyWords = Request["txtSearch"].Trim() ;
            searchDetail.SearchDateTime = DateTime.Now;
            Db.SearchDetail.Add(searchDetail);
            Db.SaveChanges();
          
            return viewModelList;
        }
        /// <summary>
        /// 自动统计功能
        /// </summary>
        /// <returns></returns>
        public ActionResult AutoComplete()
        {
            //Thread.Sleep(5000);
            string term = Request["term"];
            List<string> list = keyWordsRankService.GetSearchMsg(term);
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }

        private void CreateSerachContent()
        {
            //string indexPath = @"C:\lucenedir";//注意和磁盘上文件夹的大小写一致，否则会报错。将创建的分词内容放在该目录下。//将路径写到配置文件中。
            string indexPath = Server.MapPath("~/lucenedir");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());//指定索引文件(打开索引目录) FS指的是就是FileSystem
            bool isUpdate = IndexReader.IndexExists(directory);//IndexReader:对索引进行读取的类。该语句的作用：判断索引库文件夹是否存在以及索引特征文件是否存在。
            if (isUpdate)
            {
                //同时只能有一段代码对索引库进行写操作。当使用IndexWriter打开directory时会自动对索引库文件上锁。
                //如果索引目录被锁定（比如索引过程中程序异常退出），则首先解锁（提示一下：如果我现在正在写着已经加锁了，但是还没有写完，这时候又来一个请求，那么不就解锁了吗？这个问题后面会解决）
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isUpdate, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);//向索引库中写索引。这时在这里加锁。
            //List<Books> list = BooksService.LoadEntities(b => true).ToList();
            List<Books> list = Db.Books.Where(a => true).ToList();
            foreach (var bookModel in list)
            {
                Document document = new Document();//表示一篇文档。
                //Field.Store.YES:表示是否存储原值。只有当Field.Store.YES在后面才能用doc.Get("number")取出值来.Field.Index. NOT_ANALYZED:不进行分词保存
                document.Add(new Field("Id", bookModel.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                //Field.Index. ANALYZED:进行分词保存:也就是要进行全文的字段要设置分词 保存（因为要进行模糊查询）

                //Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS:不仅保存分词还保存分词的距离。
                document.Add(new Field("Title", bookModel.Title, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));
                document.Add(new Field("Content", bookModel.ContentDescription, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));
                writer.AddDocument(document);

            }
            writer.Close();//会自动解锁。
            directory.Close();//不要忘了C
        }
    }
}
