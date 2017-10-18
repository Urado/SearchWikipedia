using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchWebPart.Models.Searcher.SearchDatas;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class TermDocTF : IParseTermTF
    {
        private List<TfFile> Doc;
        private int pos;
        private int numWord;

        public TermDocTF(List<TfFile> doc,int NumWord)
        {
            Doc = doc;
            pos = 0;
            numWord = NumWord;
        }

        public List<Tuple<int, int>> GetInfo()
        {
            return new List<Tuple<int, int>> { new Tuple<int, int>(numWord, pos) };
        }

        public TfFile GetNext()
        {
            if (pos < Doc.Count)
            {
                TfFile ret = Doc[pos];
                pos++;
                return ret;
            }
            return null;
        }
    }
}