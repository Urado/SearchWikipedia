using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class SearchData
    {
        public static SearchData Singelton = null;
        private SearchData()
        {
            blockTab = new BlockTable();
            wordDic = new WordDictionary();
            TfIdf = new TFIDFData();
            QData = new QuoteData();
            SniData = new SnipetData();
        }
        public static void Init()
        {
            Singelton = new SearchData();
        }
        public WordDictionary wordDic;
        public BlockTable blockTab;
        public TFIDFData TfIdf;
        public QuoteData QData;
        public SnipetData SniData;
    }
}