using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHashmap
{
    public float CellSize { get; private set; }
    public Dictionary<int, GrowList<string>> Hashmap { get; private set; }

    public SpatialHashmap(float cellSize)
    {
        CellSize = cellSize;
        Hashmap = new Dictionary<int, GrowList<string>>();
    }

    private static List<Vector3> extentsOffsets = new List<Vector3>()
    {
        Vector3.zero,
        Vector3.forward, 
        Vector3.back, 
        Vector3.left, 
        Vector3.right
    };

    public void TryAddEntity(string entityId, Vector3 position, Vector3 extents)
    {
        HashSet<int> uniqueHashValues = new HashSet<int>();
        Vector3 extentPosition = Vector3.zero;
        for(int i = 0; i < extentsOffsets.Count; i++) 
        {
            extentPosition = position + Vector3.Scale(extentsOffsets[i], extents);
            var hashValue = GetKey(extentPosition);
            uniqueHashValues.Add(hashValue);
        }

        foreach(var hashValue in uniqueHashValues)
        {
            if(!Hashmap.ContainsKey(hashValue))
            {
                Hashmap.Add(hashValue, new GrowList<string>());
            }

            Hashmap[hashValue].Add(entityId);

            Debug.Log($"Add {entityId} to {hashValue}. Count: {Hashmap[hashValue].Length}");
        }
    }

    public void Clear()
    {
        foreach(var cellList in Hashmap.Values) 
        {
            cellList.Clear();
        }
    }

    public int GetKey(Vector3 position)
    {
        //Primes Picked from this white paper 
        //Optimized Spatial Hashing for Collision Detection of Deformable Objects
        //http://www.beosil.com/download/CollisionDetectionHashing_VMV03.pdf
        //---------------------------------------------------------------------
        //Matthias Teschner Bruno Heidelberger Matthias M¨uller Danat Pomeranets Markus Gross
        //Computer Graphics Laboratory
        //ETH Zurich


        const int PRIME_1 = 73856093;
       //const int PRIME_2 = 19349663;
       const int PRIME_3 = 83492791;

        return (Mathf.FloorToInt(position.x / CellSize) * PRIME_1) ^ (Mathf.FloorToInt(position.z / CellSize) * PRIME_3);
    }
}


public class GrowList<T>
{
    private List<T> _list;
    public int Length { get; private set; }

    public GrowList() 
    {
        _list = new List<T>();
    }

    public GrowList(int size)
    {
        _list = new List<T>(size);
    }

    public void Add(T item) 
    {
        if(Length < _list.Count) 
        {
            _list[Length] = item;
        }
        else
        {
            _list.Add(item);
        }
        
        Length++;
    }

    public T Pop()
    {
        var toReturn = _list[Length - 1];
        Length--;

        return toReturn;
    }

    public void RemoveAt(int index) 
    {
        _list[index] = Pop();
    }

    public void Clear()
    {
        Length = 0;
    }
}
