using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NervousSystem
{
    public struct Connection
    {
        public Connection(int id_in_, int id_out_, bool en, int innovate)
        {
            id_in = id_in_;
            id_out = id_out_;
            weight = Random.Range(-5f, 5f);
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


        public int id_in;
        public int id_out;
        public float weight;
        public bool enabled;
        public int innovationNum;
    } 


    // The data structures for the NEAT genome
    public List<char> nodes;
    public List<Connection> connections;


    private float[,] input;


    public NervousSystem(List<char> nodes, List<Connection> connections)
    {
        this.nodes = new List<char>();
        this.connections = new List<Connection>();
        foreach (var c in nodes)
            this.nodes.Add(c);

        foreach (var c in connections)
            this.connections.Add(c);

    }



    /// <summary>
    /// Computed on each time step, take in input from the surroundings and return the output
    /// Feed forward (run) the nervous system from end to end
    /// </summary>
    /// <returns>
            // The output layer of the nervous system
            //  byte[]  {u, r, d, l, consume, null}
    /// </returns>
    public float[] perceive()
    {
        return new float[6];
    }



    /// <summary>
    /// Return the index of max in byte array
    /// </summary>
    /// <param name="todo"> length 6</param>
    /// <returns></returns>
    public int indexOfMax(float[] todo)
    {
        int maxi = 0;
        float m = todo[0];

        for (int i = 1; i < todo.Length; i++)
        {
            if (todo[i] > m)
            {
                maxi = i;
                m = todo[i];
            }
        }
        return maxi;
    }
}
