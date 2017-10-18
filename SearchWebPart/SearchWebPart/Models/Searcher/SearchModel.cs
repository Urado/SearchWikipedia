using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using SearchWebPart.Models.Searcher.SearchDatas;
using SearchWebPart.Models.Searcher.ParseTerm;
using SearchWebPart.Models.Searcher.ParserMetod;
using SearchWebPart.Models.Searcher.SearchTypes;

namespace SearchWebPart.Models.Searcher
{

    public class SearchModel
    {
        Dictionary<int, int> NewToOld;
        List<String> NamePages;
        BooleanSearcher SearB;
        TFIDFSearcher SearchTF;
        public static SearchModel Sing=null;

        public static void InitSt()
        {
            if (Sing == null)
            {
                Sing = new SearchModel();
                Sing.Init();
            }
        }

        private SearchModel()
        {
        }
        private void Init()
        {
            NewToOld = new Dictionary<int, int>(1500000);
            FileStream inFile = new FileStream(WayToWikiFiles.IndexNameFile, FileMode.Open, FileAccess.Read);
            FileStream FileNamePages = new FileStream(WayToWikiFiles.NamePagesNameFile, FileMode.Open, FileAccess.Read);
            BinaryReader InF = new BinaryReader(inFile);
            StreamReader NamePagesF = new StreamReader(FileNamePages);
            NamePages = new List<string>(20000000);
            int w1, w2;
            for (; InF.BaseStream.Position != InF.BaseStream.Length;)
            {
                /*
                Line = InF.ReadLine();
                if(Line.Split(' ').Length>=2)
                    NewToOld.Add(Int32.Parse(Line.Split(' ')[0]), Int32.Parse(Line.Split(' ')[1]));
                */
                w1 = InF.ReadInt32();
                w2 = InF.ReadInt32();
                NewToOld.Add(w1, w2);
            }
            for(;!NamePagesF.EndOfStream;)
            {
                NamePages.Add(NamePagesF.ReadLine());
            }
            SearchData.Init();
            SearB = new BooleanSearcher();
            SearB.Init(SearchData.Singelton);
            SearchTF = new TFIDFSearcher();
            SearchTF.Init(SearchData.Singelton);
        }
        public List<ResultInformation> Search(SearchRequest Sr)
        {
            List<int> result;
            List<List<Tuple<int, int>>> ToCreateSniped;
            if (Sr.TypeRequest==0)
                result = SearchTF.Search(Sr.TextRequest,out ToCreateSniped);
            else
                result = SearB.Search(Sr.TextRequest,out ToCreateSniped);
            List<ResultInformation> ret=new List<ResultInformation>();
            int old;
            foreach (int p in result)
            {
                if (NewToOld.TryGetValue(p, out old))
                    ret.Add(new ResultInformation { IdPage = old, NamePage = NamePages[p-1] });
            }
            for(int i=0;i<50&&i<ToCreateSniped.Count;i++)
            {
                ret[i].Sniped = new string(SearchData.Singelton.SniData.GetSnippetForDoc(ToCreateSniped[i],result[i]).ToArray());
            }
            return ret;
        }
    }
}