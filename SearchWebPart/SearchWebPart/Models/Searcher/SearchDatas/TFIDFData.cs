using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class TFIDFData
    {
        private long[] TFPosishon;
        private Single[] IDF;
        private int[] DocWordCount;


        private FileStream TFFile;
        private BinaryReader TFBinFile;

        private FileStream IDFFile;
        private BinaryReader IDFBinFile;

        private static Single k1 = 2.0f;
        private static Single b = 0.75f;
        private static Single avgdl = 257;//1217;//347;

        public TFIDFData()
        {
            InitTf();
            InitIDF();
            InitDocWordCount();
        }

        private void InitTf()
        {
            TFFile=new FileStream(WayToWikiFiles.TFNameFlile, FileMode.Open, FileAccess.Read);
            TFBinFile = new BinaryReader(TFFile);

            FileStream Fw = new FileStream(WayToWikiFiles.TFPos, FileMode.Open, FileAccess.Read);
            BinaryReader Br = new BinaryReader(Fw);
            int count = Br.ReadInt32();
            TFPosishon= new long[count];
            for (int i = 0; i < count; i++)
            {
                TFPosishon[i] = Br.ReadInt64();
            }

            /*
            TFPosishon = new long[6000000];
            int nByte;
            Byte[] ByteMas;
            for (int i=0; TFBinFile.BaseStream.Position != TFBinFile.BaseStream.Length;i++)
            {
                TFPosishon[i]= TFBinFile.BaseStream.Position;
                nByte = TFBinFile.ReadInt32();
                ByteMas = TFBinFile.ReadBytes(nByte);
            }
            */
        }

        private void InitIDF()
        {
            IDFFile = new FileStream(WayToWikiFiles.IDFNameFile, FileMode.Open, FileAccess.Read);
            IDFBinFile = new BinaryReader(IDFFile);
            IDF = new float[6000000];
            for (int i = 0; IDFBinFile.BaseStream.Position != IDFBinFile.BaseStream.Length; i++)
            {
                IDF[i] = IDFBinFile.ReadSingle();
            }
            IDFBinFile.Close();
        }

        private void InitDocWordCount()
        {
            FileStream WordCountDocFile = new FileStream(WayToWikiFiles.WordCountDoc, FileMode.Open, FileAccess.Read);
            BinaryReader WordCountDocBinFile = new BinaryReader(WordCountDocFile);
            DocWordCount = new int[6000000];
            for (int i = 0; WordCountDocBinFile.BaseStream.Position != WordCountDocBinFile.BaseStream.Length; i++)
            {
                DocWordCount[i] = WordCountDocBinFile.ReadInt32();
            }
            WordCountDocBinFile.Close();
        }

        private List<Single> AllTFForWord(int IdWord)
        {
            List<Single> ret = new List<Single>();
            int nByte;
            Byte[] ByteMas;
            TFBinFile.BaseStream.Position = TFPosishon[IdWord];
            nByte = TFBinFile.ReadInt32();
            ByteMas = TFBinFile.ReadBytes(nByte);
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
                List<int> AllDoc = SearchData.Singelton.blockTab.AllDockForWord(WordId);
                List<Single> AllTf = SearchData.Singelton.TfIdf.AllTFForWord(WordId);
                List<TfFile> AllFileTf;
                if (AllDoc == null)
                    return null;
                if (AllDoc.Count == AllTf.Count)
                {
                    AllFileTf = new List<TfFile>(AllTf.Count);
                    for (int i = 0; i < AllDoc.Count; i++)
                    {
                        AllFileTf.Add(TfFile.Creator(AllDoc[i], BM25(AllTf[i], Idf, DocWordCount[AllDoc[i]])));
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
        public Single BM25(Single TF,Single Idf,Single d)
        {
            Single top = TF * (k1 + 1);
            Single down = (TF+k1*(1-b+b*(d/avgdl)));
            return (top/down)*Idf;
        }
    }
}