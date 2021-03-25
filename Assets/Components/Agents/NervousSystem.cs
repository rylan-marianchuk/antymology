using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Agents
{
    public class NervousSystem
    {
        public struct Connection
        {
            public Connection(int id_in_, int id_out_, bool en, int innovate)
            {
                id_in = id_in_;
                id_out = id_out_;
                weight = Random.Range(-3f, 3f);
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

        public struct Node
        {
            // Node: 
            // char c \in {'i', 'h', 'o'}
            // float current value
            // 'i': input, 'h': hidden, 'o': output
            public char c;
            public float val;
            public Node(char _c, float _val)
            {
                this.c = _c;
                this.val = _val;
            }

            public Node(char _c)
            {
                this.c = _c;
                this.val = 0;
            }

            public void setVal(float v) { this.val = v; }
        }

        // The data structures for the NEAT genome

        public List<Node> nodes;
        private List<Node> outputNodes;

        public List<Connection> connections;

        public Ant antOn;

        public int inputGridLength;

        public NervousSystem(List<Node> nodes, List<Connection> connections)
        {
            this.nodes = new List<Node>();
            this.connections = new List<Connection>();
            foreach (var c in nodes)
                this.nodes.Add(c);

            foreach (var c in connections)
                this.connections.Add(c);

        }


        public NervousSystem()
        {
            this.inputGridLength = ConfigurationManager.Instance.inputGridSize;
            this.nodes = new List<Node>(inputGridLength * inputGridLength);
            this.connections = new List<Connection>();
            // Adding the input nodes
            for (int i = 0; i < inputGridLength * inputGridLength; i++)
            {
                nodes.Add(new Node('i'));
            }
            // Adding the output nodes
            for (int i = 0; i < 6; i++)
            {
                Node anOut = new Node('o');
                nodes.Add(anOut);
                outputNodes.Add(anOut);
            }
            // Add a single random connection.
            NeuroEvolution.MutateByConnection(this);
        }


        /// <summary>
        /// Computed on each time step, take in input from the surroundings and return the output
        /// Feed forward (run) the nervous system from end to end
        /// 
        /// Activation:  1 / 1 + exp(-4.5x)
        /// </summary>
        /// <returns>
        // The output layer of the nervous system
        //  byte[]  {u, r, d, l, consume, null}
        /// </returns>
        public float[] perceive()
        {
            int off = (this.inputGridLength - 1) / 2;
            int i = 0;
            // Retrieve input values - get from environment
            for (int x = -off; x <= off; x++)
            {
                for (int z = -off; z <= off; z++)
                {
                    nodes[i].setVal(perceiveAtPixel(new Vector2Int(this.antOn.getPosition().x + x, this.antOn.getPosition().z + z)));
                    i++;
                }
            }

            // Depth first search from the output nodes to grab value. Update node and weight activation along the way

            return new float[6] {DFSonNetwork(outputNodes[0]), DFSonNetwork(outputNodes[1]), DFSonNetwork(outputNodes[2]), 
                                 DFSonNetwork(outputNodes[3]), DFSonNetwork(outputNodes[4]), DFSonNetwork(outputNodes[5])};
        }


        private float perceiveAtPixel(Vector2Int topDownWorldPos)
        {
            float queenIntensity = 1;
            float mulchIntensity = 0.5f;
            float elseIntensity = -0.5f;
            // Check if Queen is at this location
            if (antOn.colony.queen.GetComponent<Ant>().getPosition().x == topDownWorldPos.x && antOn.colony.queen.GetComponent<Ant>().getPosition().z == topDownWorldPos.y)
                return queenIntensity;

            // Get the top most block
            int airHeight = 0;

            while (Terrain.WorldManager.Instance.GetBlock(topDownWorldPos.x, airHeight, topDownWorldPos.y).GetType() != typeof(Terrain.AirBlock)) { airHeight++; }

            // Now check the block right below this air block
            if (Terrain.WorldManager.Instance.GetBlock(topDownWorldPos.x, airHeight-1, topDownWorldPos.y).GetType() == typeof(Terrain.MulchBlock))
                return mulchIntensity;

            return elseIntensity;
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



        /// <summary>
        /// Allow for permanent storage of this genome by writing to a file
        /// </summary>
        public void writeGenome2File()
        {

        }


        /// <summary>
        /// Activation for neural network
        /// 
        /// Activation:  1 / 1 + exp(-4.5x)
        /// </summary>
        /// <returns></returns>
        private float activation(float weightedSum)
        {
            return 1f / (1 + Mathf.Exp(-4.5f * weightedSum));
        }


        /// <summary>
        /// Called on each output node to compute the network.
        /// 
        /// Must do it this way, rather than feed forward, because NEAT does not create predefined layers of neurons
        /// </summary>
        /// <param name="start">the output node to compute</param>
        /// <returns>the float value of the output node</returns>
        private float DFSonNetwork(Node start)
        {
            /*
              Initialize an empty stack for storage of nodes, S.
                For each vertex u, define u.visited to be false.
                Push the root (first node to be visited) onto S.
                While S is not empty:
                    Pop the first element in S, u.
                    If u.visited = false, then:
                        U.visited = true
                        for each unvisited neighbor w of u:
                            Push w into S.
                End process when all nodes have been visited.
            */
            Stack<NervousSystem.Node> S = new Stack<Node>();
        }
    }
}

