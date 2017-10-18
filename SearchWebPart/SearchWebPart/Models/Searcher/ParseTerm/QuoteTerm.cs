using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchWebPart.Models.Searcher.SearchDatas;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class QuoteTerm : IParseTerm
    {
        int[] WordsId;
        List<int>[] WordsDoc;
        List<List<int>>[] WordsPos;
        int[] LastDoc;
        int countWord;
        int window;

        public List<Tuple<int, int>> GetInfo()
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>(countWord);
            for(int i=0;i<countWord;i++)
            {
                ret.Add(new Tuple<int, int>(LastDoc[i], WordsId[i]));
            }
            return ret;
        }


        public QuoteTerm(int[] wordsId,int Window)
        {
            WordsId = wordsId;
            window = Window;
            WordsDoc = new List<int>[WordsId.Length];
            WordsPos = new List<List<int>>[WordsId.Length];
            LastDoc= new int[WordsId.Length];
            countWord = wordsId.Length;
            for (int i=0;i<WordsId.Length;i++)
            {
                WordsDoc[i] = SearchData.Singelton.blockTab.AllDockForWord(WordsId[i]);
                WordsPos[i] = SearchData.Singelton.QData.AllPosishonInDoc(WordsId[i]);
                LastDoc[i] = 0;
            }
        }
        public int NextPage(int min)
        {
            int ret = -1;
            int imin;
            int equal;
            int i = 0;
            for(;;)
            {
                i = 0;
                imin = 0;
                equal = 0;
                for(i=0;i<countWord;i++)
                {
                    if (WordsDoc[i].Count <= LastDoc[i])
                        break;
                    if (WordsDoc[i][LastDoc[i]] < WordsDoc[imin][LastDoc[imin]])
                        imin = i;
                    if (WordsDoc[i][LastDoc[i]] == WordsDoc[0][LastDoc[0]])
                        equal++;
                }
                if (i < countWord)
                    break;
                if(equal==countWord)
                {
                    if (QuoteSearch(WordsDoc[0][LastDoc[0]]))
                        ret = WordsDoc[0][LastDoc[0]];
                    for (i = 0; i < countWord; i++)
                    {
                        LastDoc[i]++;
                    }
                    if (ret != -1)
                        break;
                }
                else
                {
                    LastDoc[imin]++;
                }
            }

            return ret;
        }
        public bool QuoteSearch(int doc)
        {
            int[] LastPos = new int[countWord];
            int imax;
            int imin;
            int i;
            for (i = 0; i < countWord; i++)
            {
                LastPos[i] = 0;
            }
            for (;;)
            {
                imax = 0;
                imin = 0;
                i = 0;
                for(i=0;i<countWord;i++)
                {
                    if (LastPos[i] >= WordsPos[i][LastDoc[i]].Count)
                        break;
                    if(WordsPos[i][LastDoc[i]][LastPos[i]]< WordsPos[imin][LastDoc[imin]][LastPos[imin]])
                        imin = i;
                    if (WordsPos[i][LastDoc[i]][LastPos[i]] > WordsPos[imax][LastDoc[imax]][LastPos[imax]])
                        imax = i;
                }
                if (i < countWord)
                    break;
                if (WordsPos[imax][LastDoc[imax]][LastPos[imax]] - WordsPos[imin][LastDoc[imin]][LastPos[imin]] < window)
                    return true;
                LastPos[imin]++;
    }
            return false;
        }
    }
}