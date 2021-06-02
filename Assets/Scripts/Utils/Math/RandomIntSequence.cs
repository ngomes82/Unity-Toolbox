using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityUtils.Random;

/// <summary>
/// Creates and cycles through a shuffled sequence of integers.
/// </summary>
public class RandomIntSequence
{
    public enum EndType
    {
        Cycle,
        Reshuffle,
        Reshuffle_Not_Same_Twice
    }

    private List<int> sequence;
    private int currentIndex;
    private EndType endType;

    private Random rand;

    public RandomIntSequence(int startNum, int count, EndType endType = EndType.Cycle, Random rng = null)
    {
        if (rng == null)
            rng = Random.Instance;

        this.rand = rng;
        this.currentIndex = 0;
        this.endType = endType;
        this.sequence = Enumerable.Range(startNum, count).ToList();
        this.sequence.Shuffle( rand );
    }

    public int Next()
    {
        int toReturn = sequence[currentIndex];

        currentIndex = (currentIndex + 1) % sequence.Count;

        if (endType != EndType.Cycle)
        {
            if (currentIndex == 0)
            {
                sequence.Shuffle( rand );

                if (endType == EndType.Reshuffle_Not_Same_Twice)
                {
                    if (sequence[currentIndex] == toReturn)
                    {
                        int randomIndex = rand.Range(currentIndex + 1, sequence.Count);
                        sequence.Swap(currentIndex, randomIndex);
                    }
                }
            }
        }


        return toReturn;
    }
}
