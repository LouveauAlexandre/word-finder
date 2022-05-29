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

            Words words = new Words();
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
                long searchSingleThreadedSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchSingleThreaded(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchSingleThreadedResultCount = searchResult.Count;
                searchSingleThreadedSearchTime = stopwatch.ElapsedMilliseconds;

                searchResult.Clear();
                Thread.Sleep(100);
                // Multi Threaded
                int searchMultiThreadedResultCount;
                long searchMultiThreadedSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchMultiThreaded(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchMultiThreadedResultCount = searchResult.Count;
                searchMultiThreadedSearchTime = stopwatch.ElapsedMilliseconds;

                searchResult.Clear();
                Thread.Sleep(100);

                // ThreadPool
                int searchThreadPoolResultCount;
                long searchThreadPoolSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchThreadPool(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchThreadPoolResultCount = searchResult.Count;
                searchThreadPoolSearchTime = stopwatch.ElapsedMilliseconds;

                searchResult.Clear();
                Thread.Sleep(100);

                // Parallel Tasks
                int searchParallelTasksResultCount;
                long searchParallelTaskSearchTime;

                stopwatch.Start();
                searchResult = wordSearch.SearchParallelTasks(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchParallelTasksResultCount = searchResult.Count;
                searchParallelTaskSearchTime = stopwatch.ElapsedMilliseconds;

                searchResult.Clear();
                Thread.Sleep(100);
                // Parallel.For
                int searchTaskParallelForResultCount;
                long searchTaskParallelForSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchTaskParallelFor(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchTaskParallelForResultCount = searchResult.Count;
                searchTaskParallelForSearchTime = stopwatch.ElapsedMilliseconds;

                searchResult.Clear();
                Thread.Sleep(100);
                // Parallel.ForEach
                int searchTaskParallelForEachResultCount;
                long searchTaskParallelForEachSearchTime;

                stopwatch.Reset();
                stopwatch.Start();
                searchResult = wordSearch.SearchTaskParallelForEach(words.WordList, inputPattern.ToUpper());
                stopwatch.Stop();

                searchTaskParallelForEachResultCount = searchResult.Count;
                searchTaskParallelForEachSearchTime = stopwatch.ElapsedMilliseconds;

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
                Console.WriteLine($"\tSearch time: {searchSingleThreadedSearchTime}");

                // Print Multi Threaded Search Result
                Console.WriteLine("\nMulti threaded search Results:");
                Console.WriteLine($"\tResult count: {searchMultiThreadedResultCount}");
                Console.WriteLine($"\tSearch time: {searchMultiThreadedSearchTime}");

                // Print Thread Pool Search Result
                Console.WriteLine("\nThread pool search Results:");
                Console.WriteLine($"\tResult count: {searchThreadPoolResultCount}");
                Console.WriteLine($"\tSearch time: {searchThreadPoolSearchTime}");

                // PrintParallel Tasks Search Result
                Console.WriteLine("\nParallel tasks search Results:");
                Console.WriteLine($"\tResult count: {searchParallelTasksResultCount}");
                Console.WriteLine($"\tSearch time: {searchParallelTaskSearchTime}");

                // Print Task Parallel For Result
                Console.WriteLine("\nTask parallel for search Results:");
                Console.WriteLine($"\tResult count: {searchTaskParallelForResultCount}");
                Console.WriteLine($"\tSearch time: {searchTaskParallelForSearchTime}");

                // Print TaskParallel ForEach Result
                Console.WriteLine("\nTask parallel foreach search Results:");
                Console.WriteLine($"\tResult count: {searchTaskParallelForEachResultCount}");
                Console.WriteLine($"\tSearch time: {searchTaskParallelForEachSearchTime}\n");
            } while (inputPattern != ":q");
        }
    }
}
