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
        public static void MutateByNode(NervousSystem toMutate)
        {
            // Randomly choose a connection
            int randomI = Random.Range(0, toMutate.connections.Count);
            // Disable this connection
            Connection gone = toMutate.connections[randomI];
            toMutate.connections.Remove(gone);


            // Add two new connections
            Connection c1 = new Connection(gone.id_in, toMutate.nodes.Count, 1, true, 0);
            Connection c2 = new Connection(toMutate.nodes.Count, gone.id_out, gone.weight, true, 0);
            toMutate.connections.Add(c1);
            toMutate.connections.Add(c2);

            List<Connection> newNodeC = new List<Connection>();
            newNodeC.Add(c1);
            newNodeC.Add(c2);
            // Add the new node
            toMutate.nodes.Add(new Node('h', 0, newNodeC, toMutate.nodes.Count));
        }


        /// <summary>
        /// Second type of NEAT structural mutation
        /// 
        /// a single new connection gene with a random weight is added connecting two previously unconnected nodes
        /// </summary>
        /// <returns></returns>
        public static void MutateByConnection(NervousSystem toMutate)
        {
            // Randomly find two unconnected nodes

            List<int> L1 = new List<int>();
            for (int i = 0; i < toMutate.nodes.Count; i++) { L1.Add(i); }
            var rnd = new System.Random();
            var result = L1.OrderBy(item => rnd.Next());


            foreach (var i in result)
            {
                foreach (var j in result)
                {
                    if (i == j) continue;
                    if ((toMutate.nodes[i].c == 'i' && toMutate.nodes[j].c == 'i') || (toMutate.nodes[i].c == 'o' && toMutate.nodes[j].c == 'o'))
                        continue;
                    if (connectionExists(i, j, toMutate)) continue;

                    // May add new connection here on nodes i and j
                    Connection newC = new Connection(i, j, true, 0);
                    toMutate.connections.Add(newC);
                    toMutate.nodes[i].attached.Add(newC);
                    toMutate.nodes[j].attached.Add(newC);

                    return;
                }
            }

            Debug.Log("Tried to add a connection but the nervous system was fully connected");
        }




        private static bool connectionExists(int i, int j, NervousSystem ns)
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
