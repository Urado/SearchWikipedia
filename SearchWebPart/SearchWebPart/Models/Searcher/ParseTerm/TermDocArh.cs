using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models.Searcher.ParseTerm
{
    public class TermDocArh : IParseTerm
    {
        private byte[] str;
        private int pos;
        private int now;
        private int count;
        private int jumpPos;
        private int jumpN;
        private int numWord;

        public TermDocArh(byte[] Str, int NumWord)
        {
            count = Str.Length;
            jumpN =-1;
            str = Str;
            pos = -1;
            now = 0;
            ReadNext();
            numWord = NumWord;
        }

        public List<Tuple<int, int>> GetInfo()
        {
            return new List<Tuple<int, int>> { new Tuple<int, int>(numWord, now) };
        }

        public int NextPage(int min)
        {
            int ret;
            if (now != -1)
            {
                if (jumpN < min && jumpN != -1&&jumpN>now)
                {
                    pos = jumpPos;
                    now = jumpN;
                }
                for (;;)
                {
                    ReadNext();
                    if (now >= min || now == -1)
                        break;
                    if (jumpN < min && jumpN != -1 && jumpN > now)
                    {
                        pos = jumpPos;
                        now = jumpN;
                    }
                }
                ret = now;
                ReadNext();
            }
            else
                ret = -1;
            return ret;
        }

        private void ReadNext()
        {
            int n = 0;
            pos++;
            if (count > pos)
            {
                if (str[pos] == 0)
                {
                    pos++;
                    jumpPos = pos + BitConverter.ToInt32(str, pos) + 7;
                    pos += 4;
                    jumpN = BitConverter.ToInt32(str, pos);
                    pos += 4;
                }
                int i = 1;
                for (; str[pos] > 127; pos++)
                {
                    n = n + (str[pos] % 128) * i;
                    i *= 128;
                }
                n = n + str[pos] * i;
                now += n;
            }
            else
                now = -1;
        }

    }
}