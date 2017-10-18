using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchWebPart.Models.Searcher.SearchDatas;

namespace SearchWebPart.Models.Searcher.SearchTypes
{
    public interface ISearch
    {
        void Init(SearchData Data);
        List<int> Search(string Zapros,out List<List<Tuple<int, int>>> ToCreateSniped);
    }
}
