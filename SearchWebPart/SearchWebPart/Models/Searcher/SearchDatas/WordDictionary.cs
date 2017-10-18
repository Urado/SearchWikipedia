using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class WordDictionary
    {
        private Dictionary<string, int> Words;
        public WordDictionary()
        {
            FileStream inFile = new FileStream(WayToWikiFiles.DictNameFile, FileMode.Open, FileAccess.Read);
            StreamReader InF = new StreamReader(inFile);
            Words = new Dictionary<string, int>(5500000);
            string Line;
            for (; !InF.EndOfStream;)
            {
                Line = InF.ReadLine();
                Words.Add(Line.Split(' ')[0], Int32.Parse(Line.Split(' ')[1]));
            }
        }
        public int NumbWord(string s)
        {
            int ret;
            if (Words.TryGetValue(s, out ret))
                return ret;
            return -1;
        }
    }
}