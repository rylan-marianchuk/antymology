using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Connection(int id_in_, int id_out_, bool en, int innovate)
    {
        id_in = id_in_;
        id_out = id_out_;
        weight = sampleGaussian(1f, 4f);
        enabled = en;
        innovationNum = innovate;
    }

    public Connection(int id_in_, int id_out_, float weight, bool en, int innovate)
    {
        id_in = id_in_;
        id_out = id_out_;
        this.weight = weight;
        enabled = en;
        innovationNum = innovate;
    }

    /**
     * 
     * Pull a single float from the specified Gaussian Distribution which uses Box-Muller transform.
     * 
     */
    private float sampleGaussian(float mean, float sd)
    {
        // Two uniform random variables
        float u1 = Random.Range(0.00001f, 0.99999f);
        float u2 = Random.Range(0.00001f, 0.99999f);

        // Box-Muller transform
        float z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);

        // Apply paramter scale
        return z * sd + mean;
    }

    public int id_in;
    public int id_out;
    public float weight;
    public bool enabled;
    public int innovationNum;
}
