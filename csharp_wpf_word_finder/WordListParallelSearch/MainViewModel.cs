using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace WordListParallelSearch
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string filterText = "";
        private readonly List<string> words;
        private CollectionViewSource wordsCollection;
        public event PropertyChangedEventHandler PropertyChanged;
        public double elapseTime = 0;
        public int matchingCount = 0;
        private Stopwatch stopwatch;
        
        // usefull property for threading
        private int threadCount = 0;
        private int chunckSize = 0;
        private readonly List<int> immutableList;
        private PrefixTree prefixTree;

        private int method = 2;

        public MainViewModel(List<string> wordList = null)
        {
            stopwatch = new Stopwatch();
            threadCount = Environment.ProcessorCount - 1;
            immutableList = new List<int>();
            for (int i = 0; i < threadCount; i++) immutableList.Add(i);

            wordsCollection = new CollectionViewSource();

            if (wordList != null)
            {
                if (method == 0)
                {
                    words = wordList;
                    chunckSize = words.Count / Environment.ProcessorCount;
                }
                else if (method == 1)
                {
                    words = wordList;
                }
                else
                {
                    prefixTree = new PrefixTree();
                    ThreadPool.QueueUserWorkItem(AddTrieWords, wordList);
                }
                wordsCollection.Source = wordList;
                matchingCount = wordList.Count;
            }
        }

        public ICollectionView SourceCollection
        {
            get
            {
                return this.wordsCollection.View;
            }
            private set
            {
                wordsCollection.Source = value;
                RaisePropertyChanged("SourceCollection");
            }
        }

        public string FilterText
        {
            get
            {
                return filterText;
            }
            set
            {
                filterText = value.ToUpper();
                RaisePropertyChanged("FilterText");
            }
        }

        public double ElapseTime
        {
            get
            {
                return elapseTime;
            }
            private set
            {
                elapseTime = value;
                RaisePropertyChanged("ElapseTime");
            }
        }

        public int MatchingCount
        {
            get
            {
                return matchingCount;
            }
            private set
            {
                matchingCount = value;
                RaisePropertyChanged("MatchingCount");
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SearchWordList()
        {
            stopwatch.Reset();
            stopwatch.Start();
            if (method == 2)
            {
                ParallelSearchTrie();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    wordsCollection.Source = words;
                    RaisePropertyChanged("SourceCollection");
                }
                else
                {
                    if (method == 0)
                        ParallelSearchWordList();
                    else
                        ParallelForEachWordList();
                }
            }
            stopwatch.Stop();
            ElapseTime = stopwatch.Elapsed.TotalMilliseconds;
            MatchingCount = wordsCollection.View.Cast<object>().Count();
        }

        private void ParallelSearchWordList()
        {
            List<string> matchingWords = new List<string>();
            using (var countdownEvent = new CountdownEvent(threadCount))
            {
                for (int i = 0; i < threadCount; i++)
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        int index = (int)x;
                        for (int j = index * chunckSize; j < (index + 1) * chunckSize; j++)
                        {
                            if (StartsWith(words[j], FilterText))
                            {
                                lock (matchingWords)
                                {
                                    matchingWords.Add(words[j]);
                                }
                            }
                        }
                        countdownEvent.Signal();
                    }, immutableList[i]);
                }
                countdownEvent.Wait();
                wordsCollection.Source = matchingWords;
                RaisePropertyChanged("SourceCollection");
            }
        }

        private void ParallelForEachWordList()
        {
            List<string> matchingWords = new List<string>();
            Parallel.ForEach(words, word =>
            {
                if (StartsWith(word, FilterText))
                {
                    lock (matchingWords)
                    {
                        matchingWords.Add(word);
                    }
                }
            });
            wordsCollection.Source = matchingWords;
            RaisePropertyChanged("SourceCollection");
        }

        private void ParallelSearchTrie()
        {
            Task<IEnumerable<string>> matchingWords = Task.Run(() => prefixTree.Search(FilterText));
            wordsCollection.Source = matchingWords.Result;
            RaisePropertyChanged("SourceCollection");
        }

        private bool StartsWith(string s, string pattern)
        {
            if (pattern.Length > s.Length) return false;
            for (int i = 0; i < pattern.Length; i++)
            {
                if (s[i] != pattern[i]) return false;
            }
            return true;
        }

        private void AddTrieWords(object obj)
        {
            List<string> wordlList = obj as List<string>;
            Parallel.ForEach(wordlList, word =>
            {
                prefixTree.Add(word);
            });
        }
    }
}