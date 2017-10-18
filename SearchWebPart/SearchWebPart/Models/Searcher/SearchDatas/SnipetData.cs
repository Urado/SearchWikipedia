using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class SnipetData
    {
        long[] WordSent;
        long[] SenenseDoc;

        BinaryReader WordReader;
        BinaryReader SentenseReader;

        public SnipetData()
        {
            FileStream FS = new FileStream(WayToWikiFiles.WordSentences, FileMode.Open, FileAccess.Read);
            WordReader = new BinaryReader(FS);
            int count = WordReader.ReadInt32();
            int count1;
            int count2;
            WordSent = new long[count];
            for (int i = 0; WordReader.BaseStream.Position != WordReader.BaseStream.Length; i++)
            {
                WordSent[i] = WordReader.BaseStream.Position;
                count1 = WordReader.ReadInt32();
                for (int j = 0; j < count1; j++)
                {
                    count2 = WordReader.ReadInt32();
                    WordReader.ReadBytes(count2 * 4);
                }
            }
            FS = new FileStream(WayToWikiFiles.AllSentenceInCorpus, FileMode.Open, FileAccess.Read);
            SentenseReader = new BinaryReader(FS);
            SenenseDoc = new long[1400000];
            for (int i = 0; SentenseReader.BaseStream.Position != SentenseReader.BaseStream.Length; i++)
            {
                SenenseDoc[i] = SentenseReader.BaseStream.Position;
                count1 = SentenseReader.ReadInt32();
                for (int j = 0; j < count1; j++)
                {
                    count2 = SentenseReader.ReadInt32();
                    SentenseReader.ReadBytes(count2);
                }
            }
        }

        public List<char> GetSnippetForDoc(List<Tuple<int, int>> NumWordAndPos,int NumDoc)
        {
            NumDoc--;
            List<char> ret= new List<char>();
            List<int> SentenseWord;
            int[] SentRang;
            int SentCount = 0;

            SentenseReader.BaseStream.Position = SenenseDoc[NumDoc];
            SentCount=SentenseReader.ReadInt32();
            SentRang = new int[SentCount];
            for(int i=0;i<SentCount; i++)
            {
                SentRang[i] = 0;
            }

            for(int i=0;i< NumWordAndPos.Count;i++)
            {
                SentenseWord = AllSenenseForWord(NumWordAndPos[i].Item1, NumWordAndPos[i].Item2-1);
                foreach(int n in SentenseWord)
                {
                    if(n<SentRang.Length)
                        SentRang[n]++;
                }
            }
            int BestSent=0;
            for (int i = 0; i < SentCount; i++)
            {
                if (SentRang[i] > SentRang[BestSent])
                    BestSent = i;
            }

            int count1;
            int count2;

            for (int i = 0; i < BestSent; i++)
            {
                count2 = SentenseReader.ReadInt32();
                SentenseReader.ReadBytes(count2);
            }


            byte[] BMas;

            count2 = SentenseReader.ReadInt32();
            BMas = SentenseReader.ReadBytes(count2);

            for(int i=0;i<count2/2;i++)
            {
                ret.Add(BitConverter.ToChar(BMas, i * 2));
            }

            return ret;
        }

        List<int> AllSenenseForWord(int NumWord,int PosDoc)
        {
            List<int> ret = new List<int>();

            WordReader.BaseStream.Position = WordSent[NumWord];
            int count1;
            int count2;
            count1 = WordReader.ReadInt32();

            for (int i = 0; i < PosDoc; i++)
            {
                count2 = WordReader.ReadInt32();
                WordReader.ReadBytes(count2 * 4);
            }

            count2 = WordReader.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                ret.Add(WordReader.ReadInt32());
            }

            return ret;
        }

    }
}