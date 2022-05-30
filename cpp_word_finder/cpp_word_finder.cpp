// cpp_word_finder.cpp : Ce fichier contient la fonction 'main'. L'exécution du programme commence et se termine à cet endroit.
//
#include <omp.h>
#include <iostream>
#include <thread>
#include <vector>
#include <algorithm>
#include <random>
#include <mutex>
#include <sstream>
#include <chrono>

using namespace std;

mutex matchingWordsMutex;
unsigned int maxThreads;
vector<int> numberVector;
vector<string> generateWordList()
{
    vector<string> wordList;
    for (char c1 = 'A'; c1 <= 'Z'; c1++)
    {
        for (char c2 = 'A'; c2 <= 'Z'; c2++)
        {
            for (char c3 = 'A'; c3 <= 'Z'; c3++)
            {
                for (char c4 = 'A'; c4 <= 'Z'; c4++)
                {
                    wordList.push_back(string({ c1, c2, c3, c4 }));
                }
            }
        }
    }
    auto rng = default_random_engine{};
    std::shuffle(wordList.begin(), wordList.end(), rng);
    return wordList;
}

bool startsWith(const std::string& str, const std::string& prefix) {
    return str.size() >= prefix.size() && str.compare(0, prefix.size(), prefix) == 0;
}

vector<string> processSearchSingleThreaded(const vector<string>& wordList, const string& pattern)
{
    vector<string> matchingWords;
    for (string word : wordList)
    {
        if (startsWith(word, pattern))
        {
            matchingWords.push_back(word);
        }
    }
    return matchingWords;
}

vector<string> processSearchMultiThreaded(const vector<string>& wordList, const string& pattern)
{
    int chunckSize = wordList.size() / maxThreads;
    vector<string> matchingWords;
    vector<thread> threads;

    auto process = [&chunckSize, &wordList, &pattern, &matchingWords](int index) {
        for (int i = index * chunckSize; i < (index + 1) * chunckSize; i++)
        {
            if (startsWith(wordList[i], pattern))
            {
                lock_guard<mutex> guard(matchingWordsMutex);
                matchingWords.push_back(wordList[i]);
            }
        }
    };

    for (int i = 0; i < maxThreads; i++)
    {
        threads.push_back(thread(process, numberVector[i]));
    }

    for (int i = maxThreads * chunckSize; i < wordList.size(); i++)
    {
        if (startsWith(wordList[i], pattern))
        {
            lock_guard<mutex> guard(matchingWordsMutex);
            matchingWords.push_back(wordList[i]);
        }
    }

    for (int i = 0; i < threads.size(); i++) 
    {
        threads[i].join();
    }

    return matchingWords;
}

vector<string> processSearchOpenMP(const vector<string>& wordList, const string& pattern)
{
    vector<string> matchingWords;
#pragma omp parallel for num_threads(maxThreads)
    for (int i = 0; i < wordList.size(); i ++)
    {
        if (startsWith(wordList[i], pattern))
        {
#pragma omp critical
            matchingWords.push_back(wordList[i]);
        }
    }
    return matchingWords;
}

int main()
{
    maxThreads = thread::hardware_concurrency();
    for (int i = 0; i < maxThreads; i++)
        numberVector.push_back(i);
    ios_base::sync_with_stdio(false);

    vector<string> wordList = generateWordList();
    string inputPattern = "";

    do {
        cout << "Enter the pattern to search(enter \":q\" to quit)" << endl;
        //cin >> inputPattern;
        getline(cin, inputPattern);

        if (inputPattern.compare(":q") == 0) return 0;
        for (auto& c : inputPattern) c = toupper(c);
        vector<string> searchResult;
        
        // Single threaded
        auto start = chrono::high_resolution_clock::now();
        searchResult = processSearchSingleThreaded(wordList, inputPattern);
        auto end = chrono::high_resolution_clock::now();
        int searchSingleThreadedResultCount = searchResult.size();
        double searchSingleThreadedSearchTime = chrono::duration_cast<chrono::milliseconds>(end - start).count();


        // Multi threaded
        start = chrono::high_resolution_clock::now();
        searchResult = processSearchMultiThreaded(wordList, inputPattern);
        end = chrono::high_resolution_clock::now();
        int searchMultiThreadedResultCount = searchResult.size();
        double searchMultiThreadedSearchTime = chrono::duration_cast<chrono::milliseconds>(end - start).count();

        // OpenMP
        start = chrono::high_resolution_clock::now();
        searchResult = processSearchMultiThreaded(wordList, inputPattern);
        end = chrono::high_resolution_clock::now();
        int searchOpenMPResultCount = searchResult.size();
        double searchOpenMPSearchTime = chrono::duration_cast<chrono::milliseconds>(end - start).count();

        cout << "\nMatching words:\n[";
        if (!searchResult.empty())
        {

        for (int i = 0; i < searchResult.size() - 1; i++)
            cout << searchResult[i] << ", ";
            cout << searchResult[searchResult.size() - 1];
        }
        else
            cout << "No match";
        cout << "]" << endl;

        cout << "\n Single threaded search result:" << endl;
        cout << "Result count: " << searchSingleThreadedResultCount << endl;
        cout << "search time: " << searchSingleThreadedSearchTime << endl;

        cout << "\n Multi threaded search result:" << endl;
        cout << "Result count: " << searchMultiThreadedResultCount << endl;
        cout << "search time: " << searchMultiThreadedSearchTime << endl << endl;

        cout << "\n OpenMP search result:" << endl;
        cout << "Result count: " << searchOpenMPResultCount << endl;
        cout << "search time: " << searchOpenMPSearchTime << endl << endl;

    } while (inputPattern.compare(":q") != 0);
}