using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class AndTerm : IParseTerm
    {
        private List<IParseTerm> Ch;
        private List<int> chLast;
        public AndTerm(IParseTerm first, IParseTerm second)
        {
            Ch = new List<IParseTerm> { first, second };
            chLast = new List<int> { first.NextPage(0), second.NextPage(0) };
        }

        public List<Tuple<int, int>> GetInfo()
        {
            List<Tuple<int, int>> ret= new List<Tuple<int, int>>(2);
            ret.AddRange(Ch[0].GetInfo());
            ret.AddRange(Ch[1].GetInfo());
            return ret;
        }

        public int NextPage(int min)
        {
            for (; chLast[0] != chLast[1];)
            {
                if (chLast[0] == -1 || chLast[1] == -1)
                    break;
                if (chLast[0] > chLast[1])
                {
                    chLast[1] = Ch[1].NextPage(Max(chLast[0],min));
                }
                else
                {
                    chLast[0] = Ch[0].NextPage(Max(chLast[1],min));
                }
            }
            if (chLast[0] == -1 || chLast[1] == -1)
                return -1;
            int ret = chLast[0];

            chLast[1] = Ch[1].NextPage(Max(chLast[0],min));
            chLast[0] = Ch[0].NextPage(Max(chLast[1],min));

            return ret;
        }

        private int Max(int a,int b)
        {
            return a > b ? a : b;
        }
    }
}