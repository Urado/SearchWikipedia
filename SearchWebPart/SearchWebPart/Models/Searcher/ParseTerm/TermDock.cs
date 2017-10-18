using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class TermDock : IParseTerm
    {
        private List<int> Doc;
        private int now;
        private int sqrtCount;
        private int jumpPos;
        private int jumpN;
        private int numWord;

        public TermDock(List<int> doc,int NumWord)
        {
            jumpN = 0;
            jumpPos = 0;
            Doc = doc;
            now = 0;
            sqrtCount = (int)Math.Sqrt(Doc.Count);
            numWord = NumWord;
        }

        public List<Tuple<int, int>> GetInfo()
        {
            return new List<Tuple<int, int>> { new Tuple<int, int>(numWord, now) };
        }

        public int NextPage(int min)
        {

            for (; ; now++)
            {
                if (now >= Doc.Count)
                    return -1;
                if (Doc[now] >= min)
                    break;
                ///*
                if(jumpN<min&&jumpPos>now)
                {
                    now = jumpPos;
                }
                if (now%sqrtCount==0)
                {
                    if (now + sqrtCount < Doc.Count)
                    {
                        jumpN = Doc[now + sqrtCount];
                        jumpPos = now + sqrtCount;
                    }
                }
                //*/
            }
            now++;
            return Doc[now-1];
        }
    }
}