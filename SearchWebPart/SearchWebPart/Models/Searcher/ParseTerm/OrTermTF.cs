using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchWebPart.Models.Searcher.SearchDatas;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class OrTermTF : IParseTermTF
    {
        List<IParseTermTF> Children;
        List<TfFile> Last;
        float tf;
        int n;
        int min;

        public List<Tuple<int, int>> GetInfo()
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>(2);
            ret.AddRange(Children[0].GetInfo());
            ret.AddRange(Children[1].GetInfo());
            return ret;
        }


        public OrTermTF(IParseTermTF child1, IParseTermTF child2)
        {
            Children = new List<IParseTermTF> { child1, child2 };
            Last = new List<TfFile> { child1.GetNext(), child2.GetNext() };
        }
        public TfFile GetNext()
        {
            tf = 0;
            if (Last[0] != null && Last[1] != null)
            {
                min = Last[0].IdDoc < Last[1].IdDoc ? Last[0].IdDoc : Last[1].IdDoc;
            }
            else if(Last[0] != null || Last[1] != null)
            {
                min = Last[0] != null ? Last[0].IdDoc : Last[1].IdDoc;
            }
            else
            {
                return null;
            }
            for(int i=0;i<2;i++)
            {
                if(Last[i]!=null&&Last[i].IdDoc==min)
                {
                    tf += Last[i].Tf;
                    Last[i] = Children[i].GetNext();
                }
            }
            return TfFile.Creator(min, tf);
        }
    }
}
