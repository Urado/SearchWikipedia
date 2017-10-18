using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SearchTitleBlockCreator
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
        public const string TableTitle = Directory + "\\TableTitle";
        public const string TFTitle = Directory + "\\TFTitle";
        public const string IDFTitle = Directory + "\\IDFTitle";

        static Dictionary<string, char[]> Page = new Dictionary<string, char[]>(10);

        static List<int> DocumentCount = new List<int>(6000000);

        static Dictionary<string, int> PageWord = new Dictionary<string, int>(100000);
        static List<List<int>> Table = new List<List<int>>(6000000);
        static Dictionary<string, int> Dict = new Dictionary<string, int>(10000000);

        static List<List<Single>> TF = new List<List<Single>>();

        static void Main(string[] args)
        {
            
            FileStream inFile = new FileStream(WikiNameFile, FileMode.Open, FileAccess.Read);//("./wiki2/AA/lwiki_05", ios::binary);//
            StreamReader InF = new StreamReader(inFile);

            FileStream FileIDF = new FileStream(IDFTitle, FileMode.Create, FileAccess.Write);
            BinaryWriter OutFIDF = new BinaryWriter(FileIDF);

            char[] str;
            int pages = 1;
            int i;
            char[] key;
            char[] item;
            char[] ArWord;
            int words;
            string word;
            int p;
            int wtf=1;
            int numWord;

            WordDictionaryCreator();

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
                ToOneLine(Page["title"]);
                //OutFile << Page["text"] << endl;
                //cout << "text " << endl;
                PageWord.Clear();
                for (; i < Page["title"].Count();)
                {
                    //if (words % 1000 == 0)
                    //cout << "1000 ";
                    ArWord = SplitWord(Page["title"], ref i);
                    ToOneLine(ArWord);
                    word = (new String(ArWord)).ToLower();
                    i++;
                    //cout << word <<" "<< pages<<endl;
                    if (word != "" && word != "-" && word != " -")
                    {
                        word = EraseEnding(word);
                        words++;
                        if (!Dict.TryGetValue(word, out p))
                        {
                            Console.WriteLine(wtf++.ToString()+" "+word);
                            p = Dict.Count;
                            DocumentCount.Add(0);
                            //dictionary.Add(word, 1);
                            Dict.Add(word, Dict.Count);
                            Table.Add(new List<int>());
                            TF.Add(new List<Single>());
                        }
                        if (PageWord.TryGetValue(word, out p))
                        {
                            PageWord[word]++;
                        }
                        else
                        {
                            PageWord.Add(word, 1);
                        }
                    }
                }
                foreach (KeyValuePair<string, int> setIt in PageWord)
                {
                    Dict.TryGetValue(setIt.Key, out numWord);
                    Table[numWord].Add(countStat);
                    TF[numWord].Add(TFfind(setIt.Value, words));
                    DocumentCount[numWord]++;
                }
                if (pages % 10000 == 0)
                {
                    System.Console.WriteLine(pages);
                }
            }

            foreach (KeyValuePair<string, int> KeyValPair in Dict)
            {
                OutFIDF.Write(IDFfind(DocumentCount[KeyValPair.Value], pages));
            }
           // /*
            FileStream FStrOut = new FileStream(TFTitle, FileMode.Create, FileAccess.Write);
            BinaryWriter Bw = new BinaryWriter(FStrOut);

            List<byte> chm;
            byte[] by4;
            int ByteCount;
            i = 0;
            chm = new List<byte>(1000000);

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
            Bw.Close();

            FStrOut = new FileStream(TableTitle, FileMode.Create, FileAccess.Write);
            Bw = new BinaryWriter(FStrOut);

            for (i=0; i < Table.Count; i++)
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
            //*/

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
                    str[i] == '\'' ||
                    str[i] == '[' ||
                    str[i] == ']' ||
                    str[i] == '»' ||
                    str[i] == '…' ||
                    str[i] == '«' ||
                    str[i] == '_' ||
                    str[i] == ' ' ||
                    str[i] == '—' ||
                    str[i] == ' ' ||
                    str[i] == '\t' ||
                    //str[i] == '\u0301'||
                    str[i] == '*')
                {
                    str[i] = ' ';
                }

            }

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

        static string EraseEnding(string word)
        {
            return word;//porter.Stemm(word);//
        }

        static public void WordDictionaryCreator()
        {
            FileStream inFile = new FileStream(DictNameFile, FileMode.Open, FileAccess.Read);
            StreamReader InF = new StreamReader(inFile);
            Dict = new Dictionary<string, int>(5500000);
            string Line;
            for (; !InF.EndOfStream;)
            {
                Line = InF.ReadLine();
                Dict.Add(Line.Split(' ')[0], Int32.Parse(Line.Split(' ')[1]));
                Table.Add(new List<int>());
                TF.Add(new List<float>());
                DocumentCount.Add(0);
            }
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
    }
}
