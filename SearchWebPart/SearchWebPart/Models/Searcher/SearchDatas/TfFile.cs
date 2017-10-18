using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class TfFile
    {
        public int IdDoc { get; private set; }
        public Single Tf { get; private set; }
        private TfFile(){}
        public static TfFile Creator(int idDoc, Single tf)
        {
            TfFile ret = new TfFile();
            ret.IdDoc = idDoc;
            ret.Tf = tf;
            return ret;
        }
        public void TfPluse(Single tf)
        {
            Tf += tf;
        }
    }
}