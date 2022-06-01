using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace c_sharp_word_finder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            List<string> wordList = Words.GenerateWordList();
            WordSearch wordSearch = new WordSearch();
            List<string> searchResult;

            string inputPattern;

            do
            {
                Console.WriteLine("Enter the pattern to search (enter \":q\" to quit)");
                inputPattern = Console.ReadLine();

                if (inputPattern == null) inputPattern = "";
                else if (inputPattern == ":q") return;

                // Single Threaded
                int searchSingleThreadedResultCount;
                double searchSingleThreadedSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchSingleThreaded(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchSingleThreadedResultCount = searchResult.Count;
                searchSingleThreadedSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                searchResult.Clear();

                // Multi Threaded
                int searchMultiThreadedResultCount;
                double searchMultiThreadedSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchMultiThreaded(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchMultiThreadedResultCount = searchResult.Count;
                searchMultiThreadedSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                searchResult.Clear();

                // ThreadPool
                int searchThreadPoolResultCount;
                double searchThreadPoolSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchThreadPool(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchThreadPoolResultCount = searchResult.Count;
                searchThreadPoolSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                searchResult.Clear();

                // Parallel Tasks
                int searchParallelTasksResultCount;
                double searchParallelTaskSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchParallelTasks(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchParallelTasksResultCount = searchResult.Count;
                searchParallelTaskSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                searchResult.Clear();

                // Parallel.For
                int searchTaskParallelForResultCount;
                double searchTaskParallelForSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchTaskParallelFor(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchTaskParallelForResultCount = searchResult.Count;
                searchTaskParallelForSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                searchResult.Clear();

                // Parallel.ForEach
                int searchTaskParallelForEachResultCount;
                double searchTaskParallelForEachSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchTaskParallelForEach(wordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchTaskParallelForEachResultCount = searchResult.Count;
                searchTaskParallelForEachSearchTime = stopwatch.Elapsed.TotalMilliseconds;

                //Print result matching words
                Console.WriteLine("\nMatching words:");
                if (searchResult.Count > 0)
                    Console.Write("[");
                for (int i = 0; i < searchResult.Count - 1; i++)
                {
                    Console.Write($"{searchResult[i]}, ");
                }
                if (searchResult.Count > 0)
                    Console.Write($"{searchResult[searchResult.Count - 1]}]\n");
                else
                    Console.WriteLine("No match");

                // Print Single Threaded Search Result
                Console.WriteLine("\nSingle threaded search Results:");
                Console.WriteLine($"\tResult count: {searchSingleThreadedResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchSingleThreadedSearchTime}");
                
                // Print Multi Threaded Search Result
                Console.WriteLine("\nMulti threaded search Results:");
                Console.WriteLine($"\tResult count: {searchMultiThreadedResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchMultiThreadedSearchTime}");

                // Print Thread Pool Search Result
                Console.WriteLine("\nThread pool search Results:");
                Console.WriteLine($"\tResult count: {searchThreadPoolResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchThreadPoolSearchTime}");

                // PrintParallel Tasks Search Result
                Console.WriteLine("\nParallel tasks search Results:");
                Console.WriteLine($"\tResult count: {searchParallelTasksResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchParallelTaskSearchTime}");

                // Print Task Parallel For Result
                Console.WriteLine("\nTask parallel for search Results:");
                Console.WriteLine($"\tResult count: {searchTaskParallelForResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchTaskParallelForSearchTime}");

                // Print TaskParallel ForEach Result
                Console.WriteLine("\nTask parallel foreach search Results:");
                Console.WriteLine($"\tResult count: {searchTaskParallelForEachResultCount}");
                Console.WriteLine($"\tSearch time (in ms): {searchTaskParallelForEachSearchTime}\n");
            } while (inputPattern != ":q");
        }
    }
}
