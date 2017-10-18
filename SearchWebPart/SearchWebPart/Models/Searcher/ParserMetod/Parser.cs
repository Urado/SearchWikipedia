using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SearchWebPart.Models.Searcher.SearchDatas;
using SearchWebPart.Models.Searcher.ParseTerm;

namespace SearchWebPart.Models.Searcher.ParserMetod
{
    public class Parser
    {
        static Porter porter = new Porter();
        public static IParseTerm parse(string s)
        {
            s = s.ToLower();
            string[] sMas = s.Split();
            return parseBool(sMas);
            /*
            List<IParseTerm> PTerm = new List<IParseTerm>();
            string smp;
            foreach (string sm in sMas)
            {
                smp = porter.Stemm(sm);
                int WordId = SearchData.Singelton.wordDic.NumbWord(sm);
                //int WordId = SearchData.Singelton.wordDic.NumbWord(smp);
                if (WordId == -1)
                    continue;
                List<int> AllDoc = SearchData.Singelton.blockTab.AllDockForWord(WordId);
                //byte[] AllDocArch = SearchData.Singelton.blockTab.StringTableForWord(WordId);
                if (AllDoc == null)
                    continue;
                //PTerm.Add(new TermDocArh(AllDocArch));
                PTerm.Add(new TermDock(AllDoc));
            }
            IParseTerm ret = null;
            if (PTerm.Count > 0)
            {
                ret = PTerm[0];
                for (int i = 1; i < PTerm.Count; i++)
                {
                    ret = new AndTerm(ret, PTerm[i]);
                }
            }
            return ret;
            */
        }

        public static IParseTermTF parseTF(string s)
        {
            s = s.ToLower();
            string[] sMas = s.Split();
            List<IParseTermTF> PTerm = new List<IParseTermTF>();
            List<TfFile> p;
            string smp;
            foreach (string sm in sMas)
            {
                //smp = porter.Stemm(sm);
                p = SearchData.Singelton.TfIdf.TfFilesForWord(sm);
                //p = SearchData.Singelton.TfIdf.TfFilesForWord(smp);
                if(p!=null)
                    PTerm.Add(new TermDocTF(p, SearchData.Singelton.wordDic.NumbWord(sm)));
            }
            IParseTermTF ret = null;
            if (PTerm.Count > 0)
            {
                ret = PTerm[0];
                for (int i = 1; i < PTerm.Count; i++)
                {
                    ret = new OrTermTF(ret, PTerm[i]);
                }
            }
            return ret;
        }

        private static void eraseOther(char[] str)
        {

            for (int i = 0; i < str.Count(); i++)
            {
                if (str[i] == '.' ||
                    str[i] == ',' ||
                    str[i] == '\"' ||
                    str[i] == '\\' ||
                    str[i] == '/' ||
                    str[i] == '?' ||
                    str[i] == '!' ||
                    str[i] == ':' ||
                    str[i] == ';' ||
                    str[i] == '@' ||
                    str[i] == '~' ||
                    str[i] == '(' ||
                    str[i] == ')' ||
                    str[i] == '<' ||
                    str[i] == '>' ||
                    str[i] == '+' ||
                    str[i] == '\'' ||
                    str[i] == '[' ||
                    str[i] == ']' ||
                    str[i] == '»' ||
                    str[i] == '…' ||
                    str[i] == '«' ||
                    str[i] == '_' ||
                    str[i] == ' ' ||
                    str[i] == '—' ||
                    str[i] == ' ' ||
                    str[i] == '\t' ||
                    //str[i] == '\u0301'||
                    str[i] == '*')
                {
                    str[i] = ' ';
                }

            }

        }

        public static IParseTerm NewParse(string[] TermBase, int Beg,int End)
        {
            IParseTerm ret = null;
            int WordId;
            List<int> AllDoc;
            for (; Beg+1<End;Beg++)
            {

            }
            return ret;
        }

        public static QuoteTerm InitQuote(string[] Quote,int Window)
        {
            int[] WordId= new int[Quote.Length];
            for(int i=0;i<Quote.Length;i++)
            {
                WordId[i] = SearchData.Singelton.wordDic.NumbWord(Quote[i]);
            }
            return new QuoteTerm(WordId, Window);
        }

        public static IParseTerm parseBool(string[] TermBase)
        {
            IParseTerm ret = null;

            IParseTerm and = null;
            IParseTerm not = null;
            IParseTerm or = null;
            for(int i=0;i<TermBase.Length;i++)
            {
                if(TermBase[i]=="\"")
                {
                    i++;
                    List<string> Ls = new List<string>();
                    for (; i < TermBase.Length&& TermBase[i] != "\""; i++)
                    {
                        Ls.Add(TermBase[i]);
                    }
                    and = mergeAnd(and, InitQuote(Ls.ToArray(), (int)(Ls.Count * 1.2f)));
                }
                else if(TermBase[i] == "!")
                {
                    i++;
                    IParseTerm tmp=BildParse(TermBase[i]);
                    if(tmp!=null)
                        not = mergeAnd(not, new NotTerm(tmp,1350000));
                }
                else if(TermBase[i] == "|")
                {
                    i++; ;
                    or= mergeOr(or, BildParse(TermBase[i]));
                }
                else
                {
                    and = mergeAnd(and, BildParse(TermBase[i]));
                }
            }
            ret = mergeAnd(mergeAnd(or, and), not);
            return ret;
        }

        private static IParseTerm mergeAnd(IParseTerm a,IParseTerm b)
        {
            if (a != null && b != null)
            {
                return new AndTerm(a, b);
            }
            return a == null ? b : a; 
        }

        private static IParseTerm mergeOr(IParseTerm a, IParseTerm b)
        {
            if (a != null && b != null)
            {
                return new OrTerm(a, b);
            }
            return a == null ? b : a;
        }

        private static IParseTerm BildParse(string sm)
        {
            IParseTerm ret = null;
            //string smp = porter.Stemm(sm);
            int WordId= SearchData.Singelton.wordDic.NumbWord(sm);
            if (WordId >= 0)
            {

                List<int> AllDoc = SearchData.Singelton.blockTab.AllDockForWord(WordId);
                //byte[] AllDoc = SearchData.Singelton.blockTab.StringTableForWord(WordId);
                if (AllDoc != null)
                {
                    //ret = new TermDocArh(AllDoc, WordId);
                    ret = new TermDock(AllDoc, WordId);
                }
            }
            return ret;
        }
    }
}