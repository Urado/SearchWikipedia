using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchWebPart.Models.Searcher.SearchDatas;
using SearchWebPart.Models.Searcher.ParseTerm;
using SearchWebPart.Models.Searcher.ParserMetod;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchTypes
{
    public class BooleanSearcher : ISearch
    {
        WordDictionary wordDic;
        BlockTable blockTab;
        public void Init(SearchData Data)
        {
            wordDic = SearchData.Singelton.wordDic;
            blockTab = SearchData.Singelton.blockTab;
        }
        public List<int> Search(string Zapros, out List<List<Tuple<int, int>>> ToCreateSniped)
        {
            IParseTerm Term = Parser.parse(Zapros);//Parser.InitQuote(Zapros.Split(), 10);//
            ToCreateSniped = new List<List<Tuple<int, int>>>();
            List<int> SearchList = new List<int>();
            int n = 0;
            if (Term != null)
            {
                for (;;)
                {
                    n = Term.NextPage(n);
                    if (n == -1)
                        break;
                    SearchList.Add(n);
                    ToCreateSniped.Add(Term.GetInfo());
                }
            }
            return SearchList;
        }
    }
}