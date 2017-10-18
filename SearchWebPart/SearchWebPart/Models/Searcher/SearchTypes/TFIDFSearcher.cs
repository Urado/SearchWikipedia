using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchWebPart.Models.Searcher.SearchDatas;
using SearchWebPart.Models.Searcher.ParseTerm;
using SearchWebPart.Models.Searcher.ParserMetod;

namespace SearchWebPart.Models.Searcher.SearchTypes
{
    public class TFIDFSearcher : ISearch
    {
        public const int TopZap = 100;
        public void Init(SearchData Data)
        {

        }

        public List<int> Search(string Zapros,out List<List<Tuple<int, int>>> ToCreateSniped)
        {
            IParseTermTF Term = Parser.parseTF(Zapros);
            TfFile[] TopBuf = new TfFile[TopZap];
            TfFile Tmp;
            ToCreateSniped = new List<List<Tuple<int, int>>>();
            List<Tuple<int, int>>[] PInfo = new List<Tuple<int, int>>[TopZap];
            List<Tuple<int, int>> PInfoTMP;
            for (int i = 0; i < TopZap; i++) { TopBuf[i] = null; }
            for (;;)
            {
                Tmp = Term.GetNext();
                PInfoTMP = Term.GetInfo();
                if (Tmp == null)
                    break;
                if (TopBuf[TopZap-1] == null || TopBuf[TopZap - 1].Tf < Tmp.Tf)
                {
                    int i = 0;
                    for (i = TopZap - 2; i >= 0; i--)
                    {
                        if (TopBuf[i] != null && Tmp.Tf < TopBuf[i].Tf)
                        {
                            PInfo[i + 1] = PInfoTMP;
                            TopBuf[i + 1] = Tmp;
                            break;
                        }
                        TopBuf[i + 1] = TopBuf[i];
                        PInfo[i + 1] = PInfoTMP;
                    }
                    if (i < 0)
                    {
                        TopBuf[0] = Tmp;
                        PInfo[0] = PInfoTMP;
                    }
                }
            }
            List<int> ret = new List<int>(TopZap);
            for (int i = 0; i < TopZap; i++)
            {
                if (TopBuf[i] == null)
                    break;
                ret.Add(TopBuf[i].IdDoc);
            }
            return ret;
        }
    }
}