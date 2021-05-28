using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Creates and cycles through a shuffled sequence of integers.
/// </summary>
public class RandomIntSequence
{
    private List<int> sequence;
    private int sequenceIndex;

    public RandomIntSequence(int startNum, int count)
    {
        sequence = Enumerable.Range(startNum, count).ToList();
        sequence.Shuffle();
    }

    public int Next()
    {
        int toReturn = sequence[sequenceIndex];
        sequenceIndex = (sequenceIndex + 1) % sequence.Count;
        return toReturn;
    }
}