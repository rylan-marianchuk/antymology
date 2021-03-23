using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Antymology.Agents
{
    /// <summary>
    /// Class that works on a population of nervous systems to evolve copmlexity and fitness
    /// 
    /// Implementation from NEAT paper found here
    /// </summary>
    public class NeuroEvolution
    {

        /// <summary>
        /// First type of NEAT structural mutation
        /// 
        /// An existing connection is split and the new node placed where the old connection was 
        /// </summary>
        /// <returns></returns>
        public void MutateByNode(NervousSystem toMutate)
        {
            // Randomly choose a connection
            int randomI = Random.Range(0, toMutate.connections.Count);
            // Disable this connection
            NervousSystem.Connection gone = toMutate.connections[randomI];
            gone.enabled = false;

            // Add the new node
            toMutate.nodes.Add(('h', 0));

            // Add two new connections
            toMutate.connections.Add(new NervousSystem.Connection(gone.id_in, toMutate.nodes.Count - 1, 1, true, 0));
            toMutate.connections.Add(new NervousSystem.Connection(toMutate.nodes.Count - 1, gone.id_out, gone.weight, true, 0));
        }


        /// <summary>
        /// Second type of NEAT structural mutation
        /// 
        /// a single new connection gene with a random weight is added connecting two previously unconnected nodes
        /// </summary>
        /// <returns></returns>
        public void MutateByConnection(NervousSystem toMutate)
        {
            // Randomly find two unconnected nodes

            List<int> L1 = new List<int>();
            for (int i = 0; i < toMutate.nodes.Count; i++) { L1.Add(i); }
            var rnd = new System.Random();
            List<int> result = (List<int>)L1.OrderBy(item => rnd.Next());


            for (int x = 0; x < toMutate.nodes.Count; x++)
            {
                int i = result[x];
                for (int y = 0; y < toMutate.nodes.Count; y++)
                {
                    int j = result[y];
                    if (i == j) continue;
                    if ((toMutate.nodes[i].Item1 == 'i' && toMutate.nodes[j].Item1 == 'i') || (toMutate.nodes[i].Item1 == 'o' && toMutate.nodes[j].Item1 == 'o'))
                        continue;
                    if (connectionExists(i, j, toMutate)) continue;

                    // May add new connection here
                    toMutate.connections.Add(new NervousSystem.Connection(i, j, true, 0));
                    return;
                }
            }

            Debug.Log("Tried to add a connection but the nervous system was fully connected");
        }




        private bool connectionExists(int i, int j, NervousSystem ns)
        {
            foreach (var c in ns.connections)
            {

                if ((c.id_in == i && c.id_out == j) || (c.id_in == j && c.id_out == i))
                    return true;
            }
            return false;
        }
    }
}
