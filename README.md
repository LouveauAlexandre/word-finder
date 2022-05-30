# word-finder

The exercise here is to implement a test client that accepts a search string, perform a search in a given list of words and return every words of the list that starts with the search string and print the resulting words and the search time.

The algorithm shall make use of the multi-core CPU for distributing the search on several cores.



For demonstration purpose the word list consists of all possible combination of 4 upper cas letters (AAAA to ZZZZ) in a random order.

## C# implementation

The fist thing is to generate to list in which the search will be performed.

Generating the word list simply consists on 4 loops that loop from 'A' to 'Z' character and add the concatenation of the 4 characters to the word list:

```csharp
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
```

For comparison purpose next thing is to perform the search on a single thread.

On single thread the search basically consist of a for loop that iter over every word of the list:

```csharp
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
```

Next step is to perform the search on multiple threads.
