using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using SearchWebPart.Models.Searcher;

namespace SearchWebPart.Models.Searcher.SearchDatas
{
    public class BlockTable
    {
        //List<List<int>> table=new List<List<int>>();

        long[] tableind;

        long[] arhTableInd;

        FileStream inFile;

        BinaryReader InF;

        FileStream ArhInFile;

        BinaryReader ArhInR;

        public BlockTable()
        {
            inFile = new FileStream(WayToWikiFiles.TableNameFile, FileMode.Open, FileAccess.Read);
            InF = new BinaryReader(inFile);

            ArhInFile = new FileStream(WayToWikiFiles.TableArch, FileMode.Open, FileAccess.Read);
            ArhInR = new BinaryReader(ArhInFile);
            /*
            tableind = new List<long>(5500000);
            //List<int> st;
            int nByte;
            Byte[] ByteMas;
            for (; InF.BaseStream.Position != InF.BaseStream.Length;)
            {
                //st = new List<int>();
                tableind.Add(InF.BaseStream.Position);
                nByte = InF.ReadInt32();
                ByteMas = InF.ReadBytes(nByte);
                /*
                for(int i=0;i<nByte;i+=4)
                {
                    st.Add(BitConverter.ToInt32(ByteMas, i));
                }
                //table.Add(st);
                //*//*
            }//*/
            FileStream Fw = new FileStream(WayToWikiFiles.TablePos, FileMode.Open, FileAccess.Read);
            BinaryReader Br = new BinaryReader(Fw);
            int count = Br.ReadInt32();
            tableind = new long[count];
            for (int i = 0; i < count; i++)
            {
                tableind[i] = Br.ReadInt64();
            }
            Fw.Close();
            Br.Close();

            ///*
            Fw = new FileStream(WayToWikiFiles.TableArchPos, FileMode.Open, FileAccess.Read);
            Br = new BinaryReader(Fw);
            count = Br.ReadInt32();
            arhTableInd = new long[count];
            for (int i = 0; i < count; i++)
            {
                arhTableInd[i] = Br.ReadInt64();
            }
            Fw.Close();
            Br.Close();
            //*/
            /*
            int nByte;
            Byte[] ByteMas;
            arhTableInd = new long[count];
            for (int i = 0; i < count; i++)
            {
                arhTableInd[i]=(ArhInR.BaseStream.Position);
                nByte = ArhInR.ReadInt32();
                ByteMas = ArhInR.ReadBytes(nByte);
            }
            */
        }

        public List<int> AllDockForWord(int IdWord)
        {
            List<int> ret = new List<int>();
            int nByte;
            Byte[] ByteMas;
            InF.BaseStream.Position = tableind[IdWord];
            nByte = InF.ReadInt32();
            ByteMas = InF.ReadBytes(nByte);
            for (int i = 0; i < nByte; i += 4)
            {
                ret.Add(BitConverter.ToInt32(ByteMas, i));
            }
            return ret;
            //return table[IdWord];
        }

        public byte[] StringTableForWord(int IdWord)
        {
            byte[] ret;
            int count;
            ArhInR.BaseStream.Position = arhTableInd[IdWord];
            count = ArhInR.ReadInt32();
            ret = ArhInR.ReadBytes(count);
            return ret;
        }
    }
}