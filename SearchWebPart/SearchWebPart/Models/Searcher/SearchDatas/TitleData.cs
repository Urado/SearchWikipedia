using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class TitleData
    {
        private long[] TfPos;
        private long[] TablePos;
        private List<float> IDF;


        private BinaryReader TfF;
        private BinaryReader TableF;

        private static Single k1 = 2.0f;
        private static Single b = 0.75f;
        private static Single avgdl = 257;//1217;//347;


        public TitleData()
        {
            FileStream inTfF = new FileStream(WayToWikiFiles.TFTitle, FileMode.Open, FileAccess.Read);
            TfF = new BinaryReader(inTfF);

            TfPos = new long[6000000];
            int nByte;
            Byte[] ByteMas;
            for (int i = 0; TfF.BaseStream.Position != TfF.BaseStream.Length; i++)
            {
                TfPos[i] = TfF.BaseStream.Position;
                nByte = TfF.ReadInt32();
                ByteMas = TfF.ReadBytes(nByte);
            }


            FileStream inTableF = new FileStream(WayToWikiFiles.TFTitle, FileMode.Open, FileAccess.Read);
            TableF = new BinaryReader(inTableF);

            TfPos = new long[6000000];
            for (int i = 0; TableF.BaseStream.Position != TableF.BaseStream.Length; i++)
            {
                TablePos[i] = TableF.BaseStream.Position;
                nByte = TableF.ReadInt32();
                ByteMas = TableF.ReadBytes(nByte);
            }
        }

        private List<Single> AllTFForWord(int IdWord)
        {
            List<Single> ret = new List<Single>();
            int nByte;
            Byte[] ByteMas;
            TfF.BaseStream.Position = TfPos[IdWord];
            nByte = TfF.ReadInt32();
            ByteMas = TfF.ReadBytes(nByte*4);
            for (int i = 0; i < nByte; i += 4)
            {
                ret.Add(BitConverter.ToSingle(ByteMas, i));
            }
            return ret;
            //return table[IdWord];
        }

        public List<TfFile> TfFilesForWord(string word)
        {

            int WordId = SearchData.Singelton.wordDic.NumbWord(word);
            if (WordId >= 0)
            {
                Single Idf = SearchData.Singelton.TfIdf.IDFToWord(WordId);
                if (WordId == -1)
                    return null;
                List<int> AllDoc = AllDockForWord(WordId);
                List<Single> AllTf = AllTFForWord(WordId);
                List<TfFile> AllFileTf;
                if (AllDoc == null)
                    return null;
                if (AllDoc.Count == AllTf.Count)
                {
                    AllFileTf = new List<TfFile>(AllTf.Count);
                    for (int i = 0; i < AllDoc.Count; i++)
                    {
                        AllFileTf.Add(TfFile.Creator(AllDoc[i], BM25(AllTf[i], Idf, 4)));
                    }
                }
                else
                {
                    AllFileTf = new List<TfFile>(0);
                    Console.WriteLine("ErrorRead");
                }
                //PTerm.Add(new TermDockTF(AllDoc));
                return AllFileTf;
            }
            return null;
        }

        public Single IDFToWord(int IdWord)
        {
            return IDF[IdWord];
        }

        public Single BM25(Single TF, Single Idf, Single d)
        {
            Single top = TF * (k1 + 1);
            Single down = (TF + k1 * (1 - b + b * (d / avgdl)));
            return (top / down) * Idf;
        }

        public List<int> AllDockForWord(int IdWord)
        {
            List<int> ret = new List<int>();
            int nByte;
            Byte[] ByteMas;
            TableF.BaseStream.Position = TablePos[IdWord];
            nByte = TableF.ReadInt32();
            ByteMas = TableF.ReadBytes(nByte);
            for (int i = 0; i < nByte; i += 4)
            {
                ret.Add(BitConverter.ToInt32(ByteMas, i));
            }
            return ret;
            //return table[IdWord];
        }
    }
}