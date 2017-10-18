using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchWebPart.Models;
using SearchWebPart.Models.Searcher;
using System.Diagnostics;

namespace SearchWebPart.Controllers
{
    public class HomeController : Controller
    {
        ///SearchModel Searcher=new SearchModel();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Zapros = "Введите запрос";
            SearchModel.InitSt();
            SearchModel.Sing.Search(new SearchRequest {TextRequest="", TypeRequest=1 });
            return View();
        }
        [HttpGet]
        public ActionResult Search()
        {
            ViewBag.Wiki = new List<ResultInformation>();
            return View();
        }
        [HttpPost]
        public ActionResult Search(SearchRequest sr)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            SearchModel.InitSt();
            var ListArch = SearchModel.Sing.Search(sr);
            ViewBag.Time=sw.ElapsedMilliseconds;
            ViewBag.Wiki = ListArch;
            ViewBag.Zapros=(sr.TextRequest);
            sw.Stop();

            return View();
        }
    }
}