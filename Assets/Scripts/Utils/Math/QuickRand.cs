using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on Math for Game Programmers: Noise-Based RNG 
/// -----------
/// The 2017 GDC talk by SMU Guildhall's Squirrel Eiserloh
/// https://www.youtube.com/watch?v=LWFzPP8ZbdU
/// </summary>
public static class QuickRand 
{
    public static uint Random(uint position, uint seed = 0)
    {
        const uint PRIME_1 = 0xB5297A4D;
        const uint PRIME_2 = 0x68E31DA4;
        const uint PRIME_3 = 0x1B56C4E9;

        uint output = position + 1;
        output *= PRIME_1;
        output += seed;
        output ^= (output >> 8);
        output *= PRIME_2;
        output ^= (output << 8);
        output *= PRIME_3;
        output ^= (output >> 8);

        return output;
    }
}
