using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordListParallelSearch
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

    internal class WordList
    {
        public List<string> List { get; private set; }

        public WordList()
        {
            List = new List<string>();
            Parallel.For('A', 'Z' + 1, c =>
            {
                for (char c2 = 'A'; c2 <= 'Z'; c2++)
                {
                    for (char c3 = 'A'; c3 <= 'Z'; c3++)
                    {
                        for (char c4 = 'A'; c4 <= 'Z'; c4++)
                        {
                            lock(List)
                            {
                                List.Add("" + (char)c + c2 + c3 + c4);
                            }
                        }
                    }
                }
            });
            List.Shuffle();
        }
    }
}
