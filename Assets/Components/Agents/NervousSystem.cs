using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NervousSystem
{
    public struct Connection
    {
        public int id_in;
        public int id_out;
        public float weight;
        public bool enabled;
        public int innovationNum;
    } 

    public struct Genome
    {
        public List<char> nodes;
        public List<Connection> connections;


    }


    private float[,] input;


    public NervousSystem()
    {

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
