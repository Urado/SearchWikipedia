using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchWebPart.Models.Searcher.SearchDatas;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public interface IParseTermTF
    {
        TfFile GetNext();
        List<Tuple<int, int>> GetInfo();
    }
}
