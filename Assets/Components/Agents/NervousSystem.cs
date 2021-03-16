using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NervousSystem
{
    private byte[,] input;


    public NervousSystem()
    {

    }


    public byte[] perceive()
    {
        return new byte[6];
    }



    /// <summary>
    /// Return the index of max in byte array
    /// </summary>
    /// <param name="todo"> length 6</param>
    /// <returns></returns>
    public int indexOfMax(byte[] todo)
    {
        int maxi = 0;
        byte m = todo[0];

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
