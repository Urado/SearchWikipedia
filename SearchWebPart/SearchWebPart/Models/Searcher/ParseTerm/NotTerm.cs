using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class NotTerm : IParseTerm
    {
        private IParseTerm Ch;
        private int now;
        private int LastCh;
        private int MaxDoc;
        public List<Tuple<int, int>> GetInfo()
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>(2);
            ret.AddRange(Ch.GetInfo());
            return ret;
        }

        public NotTerm(IParseTerm NotTerm, int maxDoc)
        {
            Ch = NotTerm;
            now = 0;
            LastCh = NotTerm.NextPage(now);
            MaxDoc = maxDoc;
        }
        public int NextPage(int min)
        {
            now++;
            for (; now == LastCh;)
            {
                now++;
                LastCh = Ch.NextPage(now);
            }
            if (LastCh == -1 && now >= MaxDoc)
                return -1;
            return now;
        }
    }
}