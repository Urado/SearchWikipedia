using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SearchPozishonInFile
{
    public class WayToWikiFiles
    {
        public const string Directory = "D:\\AAplak";
        public const string DictNameFile = Directory + "\\Dict";
        public const string IndexNameFile = Directory + "\\Ind";
        public const string TableNameFile = Directory + "\\Table";
        public const string TFNameFlile = Directory + "\\TF";
        public const string IDFNameFile = Directory + "\\IDF";
        public const string TmpNameFile = Directory + "\\Tmp";
        public const string QuoteIndNameFile = Directory + "\\QuoteInd";
        public const string NamePagesNameFile = Directory + "\\NamePages";
        public const string WordCountDoc = Directory + "\\WordCountPerDoc";
        public const string QuoteFile = Directory + "\\QuoteDoc";
        public const string TableArch = Directory + "\\TableArch";

        public const string TablePos = Directory + "\\IndPos";
        public const string TableArchPos = Directory + "\\IndArchPos";
        public const string TFPos = Directory + "\\TFPos";
        public const string QuotePos = Directory + "\\QuotePos";

    }

    class Program
    {
        public static int count;
        static void Main(string[] args)
        {
            QuoteData();
            InitTf();
            BlockTable();
            BlockTableArch();
        }
        public static void  QuoteData()
        {
            BinaryReader BR;
            long[] Posishon;
            int countIn;
            int countDoc;

            FileStream Fr = new FileStream(WayToWikiFiles.QuoteFile, FileMode.Open, FileAccess.Read);

            BR = new BinaryReader(Fr);

            count = BR.ReadInt32();
            Posishon = new long[count];
            for (int i = 0; i < count; i++)
            {
                Posishon[i] = BR.BaseStream.Position;
                countIn = BR.ReadInt32();
                for (int j = 0; j < countIn; j++)
                {
                    /*
                    countDoc = BR.ReadInt32();
                    //BR.BaseStream.Position += countDoc * sizeof(Int32);
                    BR.ReadBytes(countDoc * sizeof(Int32));
                    */
                    countDoc = BR.ReadInt32();
                    for(int a=0;a<countDoc;a++)
                    {
                        BR.ReadInt32();
                    }
                }
            }
            BR.Close();
            Fr.Close();
            FileStream Fw = new FileStream(WayToWikiFiles.QuotePos, FileMode.Create, FileAccess.Write);
            BinaryWriter Bw = new BinaryWriter(Fw);
            Bw.Write(count);
            for(int i=0;i<count;i++)
            {
                Bw.Write(Posishon[i]);
            }
        }

        public static void InitTf()
        {
            FileStream TFFile;
            BinaryReader TFBinFile;
            List<long> TFPosishon;
            TFFile = new FileStream(WayToWikiFiles.TFNameFlile, FileMode.Open, FileAccess.Read);
            TFBinFile = new BinaryReader(TFFile);
            TFPosishon = new List<long>(6000000);
            int nByte;
            Byte[] ByteMas;
            for (int i = 0; TFBinFile.BaseStream.Position != TFBinFile.BaseStream.Length; i++)
            {
                TFPosishon.Add(TFBinFile.BaseStream.Position);
                nByte = TFBinFile.ReadInt32();
                ByteMas = TFBinFile.ReadBytes(nByte);
            }
            FileStream Fw = new FileStream(WayToWikiFiles.TFPos, FileMode.Create, FileAccess.Write);
            BinaryWriter Bw = new BinaryWriter(Fw);
            Bw.Write(TFPosishon.Count);
            for (int i = 0; i < count; i++)
            {
                Bw.Write(TFPosishon[i]);
            }
        }

        public static void BlockTable()
        {
            List<long> tableind;

            FileStream inFile;

            BinaryReader InF;
            inFile = new FileStream(WayToWikiFiles.TableNameFile, FileMode.Open, FileAccess.Read);
            InF = new BinaryReader(inFile);
            tableind = new List<long>(5500000);
            int nByte;
            Byte[] ByteMas;
            for (; InF.BaseStream.Position != InF.BaseStream.Length;)
            {
                tableind.Add(InF.BaseStream.Position);
                nByte = InF.ReadInt32();
                ByteMas = InF.ReadBytes(nByte);
            }
            FileStream Fw = new FileStream(WayToWikiFiles.TablePos, FileMode.Create, FileAccess.Write);
            BinaryWriter Bw = new BinaryWriter(Fw);
            Bw.Write(tableind.Count);
            for (int i = 0; i < count; i++)
            {
                Bw.Write(tableind[i]);
            }
        }

        public static void BlockTableArch()
        {
            List<long> tableind;

            FileStream inFile;

            BinaryReader InF;
            inFile = new FileStream(WayToWikiFiles.TableArch, FileMode.Open, FileAccess.Read);
            InF = new BinaryReader(inFile);
            tableind = new List<long>(5500000);
            int nByte;
            Byte[] ByteMas;
            for (; InF.BaseStream.Position != InF.BaseStream.Length;)
            {
                tableind.Add(InF.BaseStream.Position);
                nByte = InF.ReadInt32();
                ByteMas = InF.ReadBytes(nByte);
            }
            FileStream Fw = new FileStream(WayToWikiFiles.TableArchPos, FileMode.Create, FileAccess.Write);
            BinaryWriter Bw = new BinaryWriter(Fw);
            Bw.Write(tableind.Count);
            for (int i = 0; i < count; i++)
            {
                Bw.Write(tableind[i]);
            }
        }
    }
}
