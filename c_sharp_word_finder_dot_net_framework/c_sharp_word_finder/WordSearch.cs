using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace c_sharp_word_finder
{
    internal class WordSearch
    {
        private int maxThreads = 0;
        private readonly List<int> numberList = new List<int>();

        public WordSearch()
        {
            maxThreads = Environment.ProcessorCount;
            for (int i = 0; i < maxThreads; i++) numberList.Add(i);
        }

        private bool StartsWith(string word, string pattern)
        {
            if (pattern.Length > word.Length) return false;
            for (int i = 0; i < pattern.Length; i++)
            {
                if (word[i] != pattern[i]) return false;
            }
            return true;
        }

        public List<string> SearchSingleThreaded(List<string> wordList, string pattern)
        {
            List<string> matchingWords = new List<string>();
            
            foreach (string word in wordList)
            {
                if (StartsWith(word, pattern))
                {
                    matchingWords.Add(word);
                }
            }

            return matchingWords;
        }
        
        public List<string> SearchMultiThreaded(List<string> wordList, string pattern)
        {
            int chunckSize = wordList.Count / maxThreads;
            List<Thread> threads = new List<Thread>();
            ConcurrentBag<string> matchingWords = new ConcurrentBag<string>();

            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(x =>
            {
                int index = Convert.ToInt32(x);
                if (index == -1) return;

                for (int i = index * chunckSize; i < (index + 1) * chunckSize; i++)
                {
                    if (StartsWith(wordList[i], pattern))
                    {
                        matchingWords.Add(wordList[i]);
                    }
                }
            });


            for (int i = 0; i < maxThreads; i++)
            {
                threads.Add(new Thread(parameterizedThreadStart));
                threads[i].Start(numberList[i]);
            }

            for (int i = chunckSize * maxThreads; i < wordList.Count; i++)
            {
                //if (wordList[i].StartsWith(pattern))
                if (StartsWith(wordList[i], pattern))
                {
                    matchingWords.Add(wordList[i]);
                }
            }

            foreach (Thread thread in threads)
            {
                //if (!thread.Join(3000))
                //    throw new TimeoutException();
                thread.Join();
            }

            return matchingWords.ToList();
        }

        public List<string> SearchMultiThreadedEachList(List<string> wordList, string pattern)
        {
            int chunckSize = wordList.Count / maxThreads;
            List<Thread> threads = new List<Thread>();
            List<string> matchingWords = new List<string>();
            ConcurrentBag<List<string>> results = new ConcurrentBag<List<string>>();

            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(x =>
            {
                int index = Convert.ToInt32(x);
                if (index == -1) return;
                List<string> result = new List<string>();

                for (int i = index * chunckSize; i < (index + 1) * chunckSize; i++)
                {
                    if (StartsWith(wordList[i], pattern))
                    {
                        result.Add(wordList[i]);
                    }
                }

                results.Add(result);
            });


            for (int i = 0; i < maxThreads; i++)
            {
                threads.Add(new Thread(parameterizedThreadStart));
                threads[i].Start(numberList[i]);
            }

            for (int i = chunckSize * maxThreads; i < wordList.Count; i++)
            {
                if (StartsWith(wordList[i], pattern))
                {
                    matchingWords.Add(wordList[i]);
                }
            }

            for (int i = 0; i < threads.Count; i++)
            {
                //if (!thread.Join(3000))
                //    throw new TimeoutException();
                threads[i].Join();
                matchingWords.AddRange(results.ElementAt(i));
            }

            return matchingWords.ToList();
        }

        public List<string> SearchThreadPool(List<string> wordList, string pattern)
        {
            int chunckSize = wordList.Count / maxThreads;
            ConcurrentBag<string> matchingWords = new ConcurrentBag<string>();

            using (var countdownEvent = new CountdownEvent(maxThreads))
            {
                for (int i = 0; i < maxThreads; i++)
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        int index = Convert.ToInt32(x);
                        for (int j = index * chunckSize; j < (index + 1) * chunckSize; j++)
                        {
                            if (StartsWith(wordList[j], pattern))
                            {
                                matchingWords.Add(wordList[j]);
                            }
                        }
                        countdownEvent.Signal();
                    }, numberList[i]);
                }

                for (int i = chunckSize * maxThreads; i < wordList.Count; i++)
                {
                    if (StartsWith(wordList[i], pattern))
                    {
                        matchingWords.Add(wordList[i]);
                    }
                }

                countdownEvent.Wait();
            }

            return matchingWords.ToList();
        }

        public List<string> SearchParallelTasks(List<string> wordList, string pattern)
        {
            int chunckSize = wordList.Count / maxThreads;
            Task<List<string>>[] tasks = new Task<List<string>>[maxThreads];
            List<string> matchingWords = new List<string>();

            for (int i = 0; i < maxThreads; i++)
            {
                tasks[i] = Task<List<string>>.Factory.StartNew(x =>
                {
                    int index = Convert.ToInt32(x);
                    List<string> result = new List<string>();

                    for (int j = index * chunckSize; j < (index + 1) * chunckSize; j++)
                    {
                        if (StartsWith(wordList[j], pattern))
                        {
                            result.Add(wordList[j]);
                        }
                    }
                    return result;
                }, numberList[i]);
            }

            for (int i = chunckSize * maxThreads; i < wordList.Count; i++)
            {
                if (StartsWith(wordList[i], pattern))
                {
                    matchingWords.Add(wordList[i]);
                }
            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<List<string>> task in tasks)
            {
                matchingWords.AddRange(task.Result);
            }

            return matchingWords.ToList();
        }

        public List<string> SearchTaskParallelFor(List<string> wordList, string pattern)
        {
            ConcurrentBag<string> matchingWords = new ConcurrentBag<string>();

            Parallel.For(0, wordList.Count, index =>
            {
                if (StartsWith(wordList[index], pattern))

                {
                    matchingWords.Add(wordList[index]);
                }
            });

            return matchingWords.ToList();
        }

        public List<string> SearchTaskParallelForEach(List<string> wordList, string pattern)
        {
            ConcurrentBag<string> matchingWords = new ConcurrentBag<string>();

            Parallel.ForEach(wordList, word =>
            {
                if (StartsWith(word, pattern))
                {
                    matchingWords.Add(word);
                }
            });

            return matchingWords.ToList();
        }
    }
}
