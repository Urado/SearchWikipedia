using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SearchBlochCreator
{

    class Program
    {
        public const string Directory = "D:\\AAplak";
        public const string DirectoryIn = "F:";
        public const string WikiNameFile = DirectoryIn + "\\Wiki\\Wiki\\wiki2\\AAWiki";
        public const string DictNameFile = Directory + "\\Dict";
        public const string IndexNameFile = Directory + "\\Ind";
        public const string TableNameFile = Directory + "\\Table";
        public const string TFNameFlile = Directory + "\\TF";
        public const string IDFNameFile = Directory + "\\IDF";
        public const string TmpNameFile = Directory + "\\Tmp";
        public const string QuoteIndNameFile = Directory + "\\QuoteInd";
        public const string NamePagesNameFile = Directory + "\\NamePages";
        public const string Tmp1NameFile = Directory + "\\Tmp1";
        public const string Tmp2NameFile = Directory + "\\Tmp2";
        public const string TmpTF1NameFile = Directory + "\\TmpTF1";
        public const string TmpTF2NameFile = Directory + "\\TmpTF2";
        public const string TablePosInFile = Directory + "\\TablePos";
        public const string TfPosInFile = Directory + "\\TfPos";
        public const string WordCountDoc = Directory + "\\WordCountPerDoc";
        public const string TmpQuote1NameFile = Directory + "\\TmpQuote1";
        public const string TmpQuote2NameFile = Directory + "\\TmpQuote2";
        public const string QuoteDoc = Directory + "\\QuoteDoc";
        public const string TableArch = Directory + "\\TableArch";

        public static Porter porter;


        //static Dictionary<string, int> dictionary = new Dictionary<string, int>(10000000);
        static Dictionary<string, char[]> Page = new Dictionary<string, char[]>(10);

        static List<int> DocumentCount = new List<int>(6000000);

        static Dictionary<string, int> PageWord = new Dictionary<string, int>(100000);
        static List<List<int>> Table = new List<List<int>>(6000000);
        static Dictionary<string, int> Dict = new Dictionary<string, int>(10000000);

        static Dictionary<int, int> NewToOldId = new Dictionary<int, int>();
        static List<List<Single>> TF = new List<List<Single>>();

        static List<List<List<int>>> DataQuote = new List<List<List<int>>>(6000000);

        static long wordCountInCorpus = 0;

        static void Main(string[] args)
        {
            porter = new Porter();
            FileStream inFile = new FileStream(WikiNameFile, FileMode.Open, FileAccess.Read);//("./wiki2/AA/lwiki_05", ios::binary);//
            //FileStream OutFile = new FileStream(TableNameFile, FileMode.Create, FileAccess.Write);
            FileStream FileDict = new FileStream(DictNameFile, FileMode.Create, FileAccess.Write);
            FileStream FileInd = new FileStream(IndexNameFile, FileMode.Create, FileAccess.Write);
            //FileStream FileTF = new FileStream(TFNameFlile, FileMode.Create, FileAccess.Write);
            FileStream FileIDF = new FileStream(IDFNameFile, FileMode.Create, FileAccess.Write);
            FileStream FileIndForQuote = new FileStream(QuoteIndNameFile, FileMode.Create, FileAccess.Write);
            FileStream FileNamePages = new FileStream(NamePagesNameFile, FileMode.Create, FileAccess.Write);
            FileStream WordPerDocStream = new FileStream(WordCountDoc, FileMode.Create, FileAccess.Write);

            List<char[]> AllTitles = new List<char[]>(2000000);

            List<string> NameTmp = new List<string> { Tmp1NameFile, Tmp2NameFile };
            int pTmp = 0;
            FileStream Tmp = new FileStream(NameTmp[0], FileMode.Create, FileAccess.Write);
            Tmp.Close();
            Tmp = new FileStream(NameTmp[1], FileMode.Create, FileAccess.Write);
            Tmp.Close();

            List<string> NameTmpTF = new List<string> { TmpTF1NameFile, TmpTF2NameFile };
            FileStream TmpTF = new FileStream(NameTmpTF[0], FileMode.Create, FileAccess.Write);
            TmpTF.Close();
            TmpTF = new FileStream(NameTmpTF[1], FileMode.Create, FileAccess.Write);
            TmpTF.Close();

            List<string> NameTmpQuote = new List<string> { TmpQuote1NameFile, TmpQuote2NameFile };
            FileStream TmpQuote = new FileStream(NameTmpQuote[0], FileMode.Create, FileAccess.Write);
            TmpQuote.Close();
            TmpQuote = new FileStream(NameTmpQuote[1], FileMode.Create, FileAccess.Write);
            TmpQuote.Close();

            StreamReader InF = new StreamReader(inFile);
            //BinaryWriter OutF = new BinaryWriter(OutFile);
            StreamWriter OutFDict = new StreamWriter(FileDict);
            BinaryWriter OutFInd = new BinaryWriter(FileInd);
            //BinaryWriter OutFTF = new BinaryWriter(FileTF);
            BinaryWriter OutFIDF = new BinaryWriter(FileIDF);
            StreamWriter NamePages = new StreamWriter(FileNamePages);
            BinaryWriter WordPerDocWriter = new BinaryWriter(WordPerDocStream);

            BinaryWriter QuoteInd = new BinaryWriter(FileIndForQuote);
            List<Byte> IndSt = new List<byte>();

            char[] str;
            char[] key;
            char[] item;
            string word;
            char[] Title;
            char[] ArWord;
            int pages = 1;
            int i;
            int words;
            int p = 0;
            int p1 = 0;
            System.Console.WriteLine("Begin");
            int countStat;
            for (countStat = 1; !InF.EndOfStream; countStat++)
            {
                str = InF.ReadLine().ToCharArray();
                pages++;
                i = 0;
                //cout << str << endl;
                if (str == null)
                    break;
                for (; str[i] != '{'; i++) ;
                for (; i < str.Count() && str[i] != '}';)
                {
                    for (; str[i] != '\"' && str[i] != '}'; i++) ;
                    if (str[i] == '}')
                        break;
                    i++;
                    key = ReadItem(str, ref i, '\"');
                    i++;

                    for (; str[i] != '\"'; i++) ;
                    i++;
                    item = ReadItem(str, ref i, '\"');
                    i++;
                    //cout <<key<<"\n"<< item<<endl;
                    Page[new String(key)] = item;
                    if (str.Count() >= i && str[i] == '}')
                        break;
                }
                i = 0;
                words = 0;
                ToOneLine(Page["text"]);
                //OutFile << Page["text"] << endl;
                //cout << "text " << endl;
                PageWord.Clear();
                p = 0;
                p1 = 0;
                IndSt.Clear();
                for (; i < Page["text"].Count();)
                {
                    //if (words % 1000 == 0)
                    //cout << "1000 ";
                    ArWord = SplitWord(Page["text"], ref i);
                    ToOneLine(ArWord);
                    word = (new String(ArWord)).ToLower();
                    i++;
                    //cout << word <<" "<< pages<<endl;
                    if (word != "" && word != "-" && word != " -")
                    {
                        word = EraseEnding(word);
                        words++;
                        if (Dict.TryGetValue(word, out p1))
                        {
                            //dictionary[word]++;
                        }
                        else
                        {
                            p1 = Dict.Count;
                            DocumentCount.Add(0);
                            //dictionary.Add(word, 1);
                            Dict.Add(word, Dict.Count);
                            Table.Add(new List<int>());
                            TF.Add(new List<Single>());
                            DataQuote.Add(new List<List<int>>());
                        }
                        if (PageWord.TryGetValue(word, out p))
                        {
                            PageWord[word]++;
                            DataQuote[p1][DataQuote[p1].Count - 1].Add(words);
                        }
                        else
                        {
                            PageWord.Add(word, 1);
                            DataQuote[p1].Add(new List<int> { words });
                        }
                        //IndSt.AddRange(BitConverter.GetBytes(Dict[word]));
                    }
                }
                int id = countStat;
                NewToOldId.Add(countStat, Int32.Parse(new string(Page["id"])));
                int numWord;
                wordCountInCorpus += words;
                WordPerDocWriter.Write(words);
                foreach (KeyValuePair<string, int> setIt in PageWord)
                {
                    Dict.TryGetValue(setIt.Key, out numWord);
                    Table[numWord].Add(id);
                    TF[numWord].Add(TFfind(setIt.Value, words));
                    DocumentCount[numWord]++;
                }
                Title = Page["title"];
                //NamePages.WriteLine(Title);
                AllTitles.Add(Title);
                /*CInd.Write(IndSt.Count);
                foreach (byte b in IndSt)
                {
                    CInd.Write(b);
                }*/
                if (pages % 100 == 0)
                {
                    System.Console.WriteLine(pages);
                }
                if (pages % 150000 == 0)
                {
                    System.Console.WriteLine("ReWriting");
                    TableW(NameTmp[pTmp], NameTmp[1 - pTmp]);
                    TableWTF(NameTmpTF[pTmp], NameTmpTF[1 - pTmp]);
                    QuoteIndWriting(NameTmpQuote[pTmp], NameTmpQuote[1 - pTmp]);
                    pTmp = 1 - pTmp;
                }
                //if (pages > 100000)
                //break;
            }
            System.Console.WriteLine("WritingBegin");
            System.Console.WriteLine(wordCountInCorpus / pages);
            foreach (char[] c in AllTitles)
            {
                NamePages.WriteLine(c);
            }
            //Запись на диск
            TableW(NameTmp[pTmp], TableNameFile);
            System.Console.WriteLine("WritingTF");
            TableWTF(NameTmpTF[pTmp], TFNameFlile);
            System.Console.WriteLine("WritingQuote");
            QuoteIndWriting(NameTmpQuote[pTmp], QuoteDoc);
            System.Console.WriteLine("WritingOther");

            foreach (KeyValuePair<int, int> KeyValPair in NewToOldId)
            {
                //OutFInd.WriteLine(p.Key.ToString() + " " + p.Value.ToString());
                OutFInd.Write(KeyValPair.Key);
                OutFInd.Write(KeyValPair.Value);
            }
            foreach (KeyValuePair<string, int> KeyValPair in Dict)
            {
                OutFDict.WriteLine(KeyValPair.Key + " " + KeyValPair.Value.ToString());
                OutFIDF.Write(IDFfind(DocumentCount[KeyValPair.Value], pages));
            }

            InF.Close();
            //BinaryWriter OutF = new BinaryWriter(OutFile);
            OutFDict.Close();
            OutFInd.Close();
            //BinaryWriter OutFTF = new BinaryWriter(FileTF);
            OutFIDF.Close();
            NamePages.Close();

            ArchTable();
            QuoteInd.Close();
        }

        static void MarkerCreator()
        {

        }

        static Single TFfind(int word, int AllWord)
        {
            //return word / (Single)AllWord;
            Single l = (Single)Math.Log((Single)word);
            return 1f + l;
        }

        static Single IDFfind(int DocWhiz, int Doc)
        {
            return (Single)Math.Log((Single)Doc / DocWhiz);
        }

        static string EraseEnding(string word)
        {
            return word;//porter.Stemm(word);//
        }

        static void ToOneLine(char[] str)
        {
            for (int i = 1; i < str.Count(); i++)
            {
                if ((((str)[i] == 'n') && ((str)[i - 1] == '\\')))
                    (str)[i] = (str)[i - 1] = ' ';
            }
            eraseOther(str);
        }

        static void eraseOther(char[] str)
        {

            for (int i = 0; i < str.Count(); i++)
            {
                if (str[i] == '.' ||
                    str[i] == ',' ||
                    str[i] == '\"' ||
                    str[i] == '\\' ||
                    str[i] == '/' ||
                    str[i] == '?' ||
                    str[i] == '!' ||
                    str[i] == ':' ||
                    str[i] == ';' ||
                    str[i] == '@' ||
                    str[i] == '~' ||
                    str[i] == '(' ||
                    str[i] == ')' ||
                    str[i] == '<' ||
                    str[i] == '>' ||
                    str[i] == '+' ||
                    str[i] == '\''||
                    str[i] == '[' ||
                    str[i] == ']' ||
                    str[i] == '»' ||
                    str[i] == '…' ||
                    str[i] == '«' ||
                    str[i] == '_' ||
                    str[i] == ' ' ||
                    str[i] == '—' ||
                    str[i] == ' ' ||
                    str[i] == '\t'||
                    //str[i] == '\u0301'||
                    str[i] == '*')
                {
                    str[i] = ' ';
                }

            }

        }

        static char[] SplitWord(char[] str, ref int start)
        {
            List<char> ret = new List<char>();
            for (; (str).Count() > (start) && !(((str)[(start)] == 'n') && ((str)[(start) - 1] == '\\')) && ((str)[(start)] != ' '); (start)++)
            {
                ret.Add(str[start]);
            }
            //ret.push_back('\0');
            return ret.ToArray();
        }

        static char[] ReadItem(char[] str, ref int start, char StopSim)
        {
            List<char> ret = new List<char>();
            for (; (start) < str.Count() && !((str)[(start)] == StopSim && (str)[(start) - 1] != '\\'); (start)++)
            {
                ret.Add(str[start]);
            }
            //ret.push_back('\0');
            return ret.ToArray();
        }

        static void TableW(string Name1,string Name2)
        {
            FileStream FStrIn = new FileStream(Name1, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(Name2, FileMode.Create, FileAccess.Write);
            BinaryReader Br=new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);
            ReWriteTable(Br,Bw);
            FStrIn.Close();
            FStrOut.Close();
            foreach(List<int> Li in Table)
            {
                Li.Clear();
            }
        }

        static void ReWriteTable(BinaryReader Br,BinaryWriter Bw)
        {
            List<byte> chm;
            byte[] by4;
            int ByteCount;
            int i = 0;
            for (; i < Table.Count && Br.BaseStream.Position != Br.BaseStream.Length; i++)
            {
                ByteCount = Br.ReadInt32();
                chm = new List<byte>(Br.ReadBytes(ByteCount));
                foreach (int n in Table[i])
                {
                    by4 = BitConverter.GetBytes(n);
                    chm.AddRange(BitConverter.GetBytes(n));
                }
                Bw.Write(chm.Count);
                Bw.Write(chm.ToArray());
            }
            for (; i < Table.Count; i++)
            {
                chm = new List<byte>();
                foreach (int n in Table[i])
                {
                    by4 = BitConverter.GetBytes(n);
                    chm.AddRange(BitConverter.GetBytes(n));
                }
                Bw.Write(chm.Count);
                Bw.Write(chm.ToArray());
            }
        }

        static void TableWTF(string Name1, string Name2)
        {
            FileStream FStrIn = new FileStream(Name1, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(Name2, FileMode.Create, FileAccess.Write);
            BinaryReader Br = new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);
            ReWriteTableTF(Br, Bw);
            FStrIn.Close();
            FStrOut.Close();
            foreach (List<Single> Li in TF)
            {
                Li.Clear();
            }
        }

        static void ReWriteTableTF(BinaryReader Br, BinaryWriter Bw)
        {
            List<byte> chm;
            byte[] by4;
            int ByteCount;
            int i = 0;
            chm = new List<byte>(1000000);
            for (; i < TF.Count && Br.BaseStream.Position != Br.BaseStream.Length; i++)
            {
                ByteCount = Br.ReadInt32();
                chm.Clear();
                chm.AddRange(Br.ReadBytes(ByteCount));
                for (int j=0;j<TF[i].Count;j++)
                {
                    //by4 = BitConverter.GetBytes(n);
                    chm.AddRange(BitConverter.GetBytes(TF[i][j]));
                }
                Bw.Write(chm.Count);
                Bw.Write(chm.ToArray());
            }
            for (; i < TF.Count; i++)
            {
                chm.Clear();
                for (int j = 0; j < TF[i].Count; j++)
                {
                    //by4 = BitConverter.GetBytes(n);
                    chm.AddRange(BitConverter.GetBytes(TF[i][j]));
                }
                Bw.Write(chm.Count);
                Bw.Write(chm.ToArray());
            }
        }

        static void TableJump(string Name1,string Name2)
        {
            FileStream FStrIn = new FileStream(Name1, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(Name2, FileMode.Create, FileAccess.Write);
            BinaryReader Br = new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);

            List<int> ret = new List<int>();
            int nByte;
            Byte[] ByteMas;

            List<byte> chm = new List<byte>();
            byte[] by4;

            for (; Br.BaseStream.Position != Br.BaseStream.Length;)
            {
                ret.Clear();
                nByte = Br.ReadInt32();
                ByteMas = Br.ReadBytes(nByte);
                for (int i = 0; i < nByte; i += 4)
                {
                    ret.Add(BitConverter.ToInt32(ByteMas, i));
                }

                for(int i=0;i*10<ret.Capacity;i++)
                {
                    chm.AddRange(BitConverter.GetBytes(0));
                    chm.AddRange(BitConverter.GetBytes(ret[10]));
                    for (int j=0;j<10;j++)
                    {
                        by4 = BitConverter.GetBytes(ret[i*10+j]);
                        chm.AddRange(BitConverter.GetBytes(ret[i * 10 + j]));
                    }
                }
            }
            FStrIn.Close();
            FStrOut.Close();
        }

        static void TableArh(string Name1, string Name2)
        {
            FileStream FStrIn = new FileStream(Name1, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(Name2, FileMode.Create, FileAccess.Write);
            BinaryReader Br = new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);

            List<int> ret = new List<int>();
            int nByte;
            Byte[] ByteMas;

            List<byte> chm = new List<byte>();
            byte[] by4;

            for (; Br.BaseStream.Position != Br.BaseStream.Length;)
            {
                ret.Clear();
                nByte = Br.ReadInt32();
                ByteMas = Br.ReadBytes(nByte);
                for (int i = 0; i < nByte; i += 4)
                {
                    ret.Add(BitConverter.ToInt32(ByteMas, i));
                }

                foreach(int ni in ret)
                {
                    chm.AddRange(ArhInt(ni));
                }
            }
            FStrIn.Close();
            FStrOut.Close();
        }

        static List<byte> ArhInt(int n)
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(BitConverter.GetBytes(n));
            return ret;
        }

        static void QuoteIndWriting(string Name1, string Name2)
        {
            FileStream FStrIn = new FileStream(Name1, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(Name2, FileMode.Create, FileAccess.Write);
            BinaryReader Br = new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);
            ReWriterQuoteInd(Br, Bw);
            FStrIn.Close();
            FStrOut.Close();
        }

        static void ReWriterQuoteInd(BinaryReader Br, BinaryWriter Bw)
        {
            int ByteCount;
            int IntCount2;
            int p;
            int i = 0;
            Bw.Write(DataQuote.Count);
            if (Br.BaseStream.Position != Br.BaseStream.Length)
                ByteCount = Br.ReadInt32();
            for (; i < DataQuote.Count && Br.BaseStream.Position != Br.BaseStream.Length; i++)
            {
                ByteCount=Br.ReadInt32();
                Bw.Write(DataQuote[i].Count+ByteCount);
                for (int j = 0; j < ByteCount; j++)
                {
                    IntCount2= Br.ReadInt32();
                    Bw.Write(IntCount2);
                    for (int q = 0; q < IntCount2; q++)
                    {
                        p = Br.ReadInt32();
                        Bw.Write(p);
                    }
                }
                for (int j = 0; j < DataQuote[i].Count; j++)
                {
                    Bw.Write(DataQuote[i][j].Count);
                    for (int q = 0; q < DataQuote[i][j].Count; q++)
                    {
                        Bw.Write(DataQuote[i][j][q]);
                    }
                    DataQuote[i][j].Clear();
                }
                DataQuote[i].Clear();
            }
            for (; i < DataQuote.Count; i++)
            {
                Bw.Write(DataQuote[i].Count);
                for (int j = 0; j < DataQuote[i].Count; j++)
                {
                    Bw.Write(DataQuote[i][j].Count);
                    for (int q = 0; q < DataQuote[i][j].Count; q++)
                    {
                        Bw.Write(DataQuote[i][j][q]);
                    }
                    DataQuote[i][j].Clear();
                }
                DataQuote[i].Clear();
            }
        }

        static void ArchTable()
        {
            FileStream FStrIn = new FileStream(TableNameFile, FileMode.Open, FileAccess.Read);
            FileStream FStrOut = new FileStream(TableArch, FileMode.Create, FileAccess.Write);
            BinaryReader Br = new BinaryReader(FStrIn);
            BinaryWriter Bw = new BinaryWriter(FStrOut);
            List<int> str = new List<int>(6000000);
            List<int> str2 = new List<int>(6000000);
            List<byte> strbyte = new List<byte>(6000000);
            List<byte> buf = new List<byte>(5);
            List<byte> BufStr = new List<byte>();
            int JumpDoc;
            int p;
            int count;
            int i;
            int CountSqrt;
            //count = Br.ReadInt32();
            for (; Br.BaseStream.Position != Br.BaseStream.Length;)
            {
                str.Clear();
                str2.Clear();
                count = Br.ReadInt32()/4;
                for(i=0;i<count;i++)
                {
                    p = Br.ReadInt32();
                    str.Add(p);
                    str2.Add(p);
                }
                for (i = count-1; i > 0; i--)
                {
                    str[i] -= str[i - 1];
                }
                BufStr.Clear();
                buf.Clear();
                strbyte.Clear();
                CountSqrt = (int)Math.Sqrt(count);
                if (CountSqrt < 10)
                    CountSqrt = count;
                for (i = 0; i < count; i++)
                {
                    if(i%CountSqrt==0)
                    {
                        if(i-CountSqrt>0)
                        {
                            strbyte.Add(0);
                            strbyte.AddRange(BitConverter.GetBytes(BufStr.Count));
                            strbyte.AddRange(BitConverter.GetBytes(str2[i-1]));
                            strbyte.AddRange(BufStr);
                            BufStr.Clear();
                        }
                    }
                    buf.Clear();
                    for (;str[i]>127;)
                    {
                        buf.Add((byte)(str[i]%128+128));
                        str[i] /= 128;
                    }
                    buf.Add((byte)(str[i] ));
                    BufStr.AddRange(buf);
                }
                strbyte.AddRange(BufStr);
                Bw.Write(strbyte.Count);
                Bw.Write(strbyte.ToArray());
            }
        }

    }
}
