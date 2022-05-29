using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c_sharp_word_finder
{
    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    internal class Words
    {
        public List<string> WordList { get; private set; } = new List<string>();

        public Words()
        {
            GenerateWordList();
        }

        private void GenerateWordList()
        {
            WordList = new List<string>();
            for (char c1 = 'A'; c1 <= 'Z'; c1++)
            {
                for (char c2 = 'A'; c2 <= 'Z'; c2++)
                {
                    for (char c3 = 'A'; c3 <= 'Z'; c3++)
                    {
                        for (char c4 = 'A'; c4 <= 'Z'; c4++)
                        {
                            WordList.Add("" + c1 + c2 + c3 + c4);
                        }
                    }
                }
            }
            WordList.Shuffle();
        }
    }
}
