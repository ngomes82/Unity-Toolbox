using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float x;
    public float mean;
    public float std;

    public float pdf;
    public float normalized;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pdf = MathUtils.NormalDist(x, mean, std);
        normalized = MathUtils.NormalDistNormalized(x, mean, std);
    }
}
