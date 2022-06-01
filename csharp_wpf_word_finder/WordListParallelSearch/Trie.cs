using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace WordListParallelSearch
{
    public class PrefixTree
    {
        private PrefixTreeNode root;
        private IEnumerable<string> matchingWords;

        private object _lock = new object();
        
        public PrefixTree()
        {
            root = new PrefixTreeNode(string.Empty);
        }

        public void Add(string word)
        {
            Add(root, word, string.Empty);
        }

        private void Add(PrefixTreeNode node, string remainingString, string currentString)
        {
            if (remainingString.Length <= 0)
            {
                return;
            }

            char prefix = remainingString[0];
            string substring = remainingString.Substring(1);
            lock (node.ChildNodes)
            {
                if (!node.ChildNodes.ContainsKey(prefix))
                {
                    node.ChildNodes.TryAdd(prefix, new PrefixTreeNode(currentString + prefix));
                }
            }

            if (substring.Length == 0)
            {
                lock(node.ChildNodes)
                {
                    node.ChildNodes[prefix].IsWord = true;
                }
                return;
            }
            else
            {
                Add(node.ChildNodes[prefix], substring, currentString + prefix);
            }
        }


        public IEnumerable<string> Search(string pattern)
        {
            PrefixTreeNode node = root;
            foreach (var search in pattern)
            {
                if (!node.ChildNodes.ContainsKey(search))
                {
                    return new string[0];
                }
                node = node.ChildNodes[search];
            }

            //FindAllWordsParallel(node);
            //return matchingWords;
            return FindAllWords(node);

        }

        private IEnumerable<string> FindAllWords(PrefixTreeNode node)
        {
            if (node.IsWord)
            {
                yield return node.Word;
            }

            foreach (var childNode in node.ChildNodes)
            {
                foreach (var result in FindAllWords(childNode.Value))
                {
                    yield return result;
                }
            }
        }

        private void FindAllWordsParallel(PrefixTreeNode node)
        {
            ConcurrentStack<PrefixTreeNode> nodesToRun;
            matchingWords = new List<string>();
            nodesToRun = new ConcurrentStack<PrefixTreeNode>();
            nodesToRun.Push(node);
            object sync = new object();

            //while (nodesToRun.Count > 0)
            //{
            //    ThreadPool.QueueUserWorkItem(_ =>
            //    {
            //        PrefixTreeNode current;
            //        bool poped;
            //        poped = nodesToRun.TryPop(out current);
            //        if (poped)
            //        {
            //            if (current.IsWord)
            //            {
            //                lock (_lock)
            //                    matchingWords = matchingWords.Append(node.Word);
            //            }
            //            else
            //            {
            //                nodesToRun.PushRange(current.ChildNodes.Values.ToArray());
            //            }
            //        }
            //    });
            //}

            while (nodesToRun.Count > 0)
            {
                PrefixTreeNode[] nodes = new PrefixTreeNode[nodesToRun.Count];
                nodesToRun.CopyTo(nodes, 0);
                nodesToRun.Clear();
                Parallel.ForEach(nodes, current =>
                {
                    if (current.IsWord)
                    {        
                        lock(_lock)
                        {
                            matchingWords = matchingWords.Append(node.Word);
                        }
                    }
                    else
                    {
                        nodesToRun.PushRange(current.ChildNodes.Values.ToArray());
                    }
                });
            }
        }

        protected class PrefixTreeNode
        {
            private readonly ConcurrentDictionary<char, PrefixTreeNode> childNode;
            private bool isWord;
            private readonly string word;

            public PrefixTreeNode(string word)
            {
                childNode = new ConcurrentDictionary<char, PrefixTreeNode>();
                isWord = false;
                this.word = word;
            }

            public ConcurrentDictionary<char, PrefixTreeNode> ChildNodes { get { return childNode; } }
            public bool IsWord { get { return isWord; } set { isWord = value; } }
            public string Word { get { return word; } }
        }
    }
}
