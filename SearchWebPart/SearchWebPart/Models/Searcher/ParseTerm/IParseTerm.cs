using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public interface IParseTerm
    {
        int NextPage(int min);
        List<Tuple<int, int>> GetInfo();
    }
}
