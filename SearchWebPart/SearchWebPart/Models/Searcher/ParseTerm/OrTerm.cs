using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class OrTerm : IParseTerm
    {
        private List<IParseTerm> Ch;
        private List<int> chLast;
        public OrTerm(IParseTerm first, IParseTerm second)
        {
            Ch = new List<IParseTerm> { first, second };
            chLast = new List<int> { first.NextPage(0), second.NextPage(0) };
        }

        public List<Tuple<int, int>> GetInfo()
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>(2);
            ret.AddRange(Ch[0].GetInfo());
            ret.AddRange(Ch[1].GetInfo());
            return ret;
        }


        public int NextPage(int min)
        {
            int[] p= new Int32[2] {0,0};
            int ret = 0;
            if (chLast[0] == chLast[1])
            {
                ret = Ch[0].NextPage(chLast[1]);
                p[1] = 1;
                p[0] = 1;
            }
            if (chLast[0] > chLast[1] || chLast[0] == -1)
            {
                ret = chLast[1];
                chLast[1] = Ch[1].NextPage(chLast[1]);
            }
            if (chLast[0] < chLast[1] || chLast[1] == -1)
            {
                ret = chLast[0];
                chLast[0] = Ch[0].NextPage(chLast[0]);
            }
            if (chLast[1] == -1 || chLast[1] == -1)
                ret = -1;
            else
            {
                for(int i=0;i<2; i++)
                {
                    if(p[i]==1)
                        chLast[i] = Ch[i].NextPage(chLast[i]);
                }
            }
            return ret;
        }
    }
}