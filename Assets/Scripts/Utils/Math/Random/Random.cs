using System;

namespace UnityUtils
{
    public class Random
    {
        public static Random Instance = new Random();

        uint position;
        uint seed;

        public uint Position { get { return position; } set { position = value; } }
        public uint Seed { get { return seed; } }

        public Random(uint seed = 0)
        {
            if (seed == 0)
            {
                // TickCount cycles between Int32.MinValue, which is a negative
                // number, and Int32.MaxValue once every 49.8 days. This removes
                // the sign bit to yield a nonnegative number that cycles
                // between zero and Int32.MaxValue once every 24.9 days.
                seed = (uint)(Environment.TickCount & int.MaxValue);
            }

            this.seed = seed;
            this.position = 0;
        }

        public uint NextUInt()
        {
            uint toReturn = RandUtils.QuickRand(position, seed);
            position += 1;
            return toReturn;
        }

        public double NextDouble()
        {
            return NextUInt() / (double)uint.MaxValue;
        }

        public int NextInt()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(NextUInt()), 0);
        }

        public float Next()
        {
            return NextUInt() / (float)uint.MaxValue;
        }

        public bool NextBool(float oddsOfTrue = 0.5f)
        {
            return Next() < oddsOfTrue;
        }

        //Inclusive
        public float Range(float min, float max)
        {
            return Next() * (max - min) + min;
        }

        //Exclusive
        public int Range(int min, int max)
        {
            return (int) (Next() * (max - min) + min);
        }

        //Exclusive
        public uint Range(uint min, uint max)
        {
            return (uint) (Next() * (max - min) + min);
        }
    }
}
