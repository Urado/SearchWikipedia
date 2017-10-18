using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class QuoteData
    {
        BinaryReader BR;
        long[] Posishon;

        public QuoteData()
        {
            int count;
            int countIn;
            int countDoc;

            FileStream Fr = new FileStream(WayToWikiFiles.QuoteFile, FileMode.Open, FileAccess.Read);

            BR = new BinaryReader(Fr);

            /*
            count = BR.ReadInt32();
            Posishon = new long[count];
            for (int i=0;i<count;i++)
            {
                Posishon[i] = BR.BaseStream.Position;
                countIn = BR.ReadInt32();
                for (int j=0;j<countIn;j++)
                {
                    countDoc = BR.ReadInt32();
                    for (int a = 0; a < countDoc; a++)
                    {
                        BR.ReadInt32();
                    }
                }
            }
            //*/
            /*
            FileStream Fw = new FileStream(WayToWikiFiles.QuotePos, FileMode.Open, FileAccess.Read);
            BinaryReader Br = new BinaryReader(Fw);
            count = Br.ReadInt32();
            Posishon = new long[count];
            for (int i = 0; i < count; i++)
            {
                Posishon[i] = Br.ReadInt64();
            }
            //*/
        }

        public List<int> PosishonsInDoc(int WordId,int DocIdForWord)
        {
            List<int> ret = null;
            int count;
            int countIn;
            BR.BaseStream.Position = Posishon[WordId];
            count = BR.ReadInt32();
            for(int i=0;i<DocIdForWord;i++)
            {
                countIn = BR.ReadInt32();
                BR.BaseStream.Position += countIn * sizeof(Int32);
            }
            countIn = BR.ReadInt32();
            ret = new List<int>(countIn);
            for(int i=0;i<countIn;i++)
            {
                ret.Add(BR.ReadInt32());
            }
            return ret;
        }

        public List<List<int>> AllPosishonInDoc(int WordId)
        {
            List<List<int>> ret;
            int count;
            int countIn;
            BR.BaseStream.Position = Posishon[WordId];
            count = BR.ReadInt32();
            ret = new List<List<int>>(count);
            for (int i = 0; i < count; i++)
            {
                countIn = BR.ReadInt32();
                ret.Add(new List<int>(countIn));
                for(int j=0; j<countIn; j++)
                {
                    ret[i].Add(BR.ReadInt32());
                }
            }
            return ret;
        }
    }
}