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
            public List<Connection> attached;

            // For DFS
            private bool visited;
            public Node(char _c, float _val, List<Connection> attached)
            {
                this.c = _c;
                this.val = _val;
                this.attached = new List<Connection>(attached);
                this.visited = false;
            }

            public Node(char _c, List<Connection> attached)
            {
                this.c = _c;
                this.val = 0;
                this.attached = new List<Connection>(attached);
                this.visited = false;
            }

            public Node(char _c)
            {
                this.c = _c;
                this.val = 0;
                this.attached = new List<Connection>();
                this.visited = false;
            }

            public void setVisited() { this.visited = true; }
            public void setUNvisited() { this.visited = false; }

            public bool getVisited() { return this.visited; }
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
            foreach (var n in nodes)
            {
                this.nodes.Add(new Node(n.c, n.val, new List<Connection>(n.attached)));
            }
                

            foreach (var c in connections)
                this.connections.Add(new Connection(c.id_in, c.id_out, c.enabled, c.innovationNum));

        }


        public NervousSystem()
        {
            this.inputGridLength = ConfigurationManager.Instance.inputGridSize;
            this.nodes = new List<Node>(inputGridLength * inputGridLength);
            this.connections = new List<Connection>();
            // Adding the input nodes
            for (int i = 0; i < inputGridLength * inputGridLength; i++)
            {
                Node anIn = new Node('i');
                nodes.Add(anIn);
            }
            // Health Input node
            nodes.Add(new Node('i'));
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
            // Set each visited field in the nodes to be false. Only input nodes are always visited
            foreach (Node node in nodes)
            {
                node.setUNvisited();
            }

            int off = (this.inputGridLength - 1) / 2;
            int i = 0;
            // Retrieve input values - get from environment
            for (int x = -off; x <= off; x++)
            {
                for (int z = -off; z <= off; z++)
                {
                    nodes[i].setVal(perceiveAtPixel(new Vector2Int(this.antOn.getPosition().x + x, this.antOn.getPosition().z + z)));
                    nodes[i].setVisited();
                    i++;
                }
            }
            // Health node
            nodes[i].setVal((float)antOn.getCurrentHealth() / this.antOn.getTotalHealth());
            nodes[i].setVisited();
            // Depth first search from the output nodes to grab value. Update node and weight activation along the way

            Connection nullC = new Connection(-1, -1, false, 0);
            return new float[6] {CalcNetwork(outputNodes[0], nullC), CalcNetwork(outputNodes[1], nullC), CalcNetwork(outputNodes[2], nullC),
                                 CalcNetwork(outputNodes[3], nullC), CalcNetwork(outputNodes[4], nullC), CalcNetwork(outputNodes[5], nullC)};
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
        /*
        private float DFSonNetwork(Node start)
        {

            Stack<Node> S = new Stack<Node>();
            S.Push(start);
            while(!(S.Count == 0))
            {
                Node u = S.Pop();
                if (!u.getVisited())
                {
                    u.setVisited();
                    foreach (Connection Cneighbor in u.attached)
                    {
                        if (Cneighbor.id_in != nodes.IndexOf(u))
                        {
                            // id_in it the neighbor node
                            if (!nodes[Cneighbor.id_in].getVisited()) S.Push(nodes[Cneighbor.id_in]);
                            else u.val += Cneighbor.weight * nodes[Cneighbor.id_in].val;
                        }
                        else
                        {
                            // id_out is the neighbor node
                            if (!nodes[Cneighbor.id_out].getVisited()) S.Push(nodes[Cneighbor.id_out]);
                            else u.val += Cneighbor.weight * nodes[Cneighbor.id_out].val;
                        }
                    }
                }
            }

        }
        */

        private float CalcNetwork(Node start, Connection from)
        {
            start.setVisited();
            foreach (Connection Cneighbor in start.attached)
            {
                if (Cneighbor.id_in == from.id_in && Cneighbor.id_out == from.id_out)
                    continue;

                if (Cneighbor.id_in != nodes.IndexOf(start))
                {
                    // id_in it the neighbor node
                    if (!nodes[Cneighbor.id_in].getVisited())
                    {
                        start.val += Cneighbor.weight * CalcNetwork(nodes[Cneighbor.id_in], Cneighbor);
                    }
                    else start.val += Cneighbor.weight * nodes[Cneighbor.id_in].val;
                }
                else
                {
                    // id_out is the neighbor node
                    if (!nodes[Cneighbor.id_out].getVisited())
                    {
                        start.val += Cneighbor.weight * CalcNetwork(nodes[Cneighbor.id_out], Cneighbor);
                    }
                    else start.val += Cneighbor.weight * nodes[Cneighbor.id_out].val;
                }
            }
            return start.val;
        }
    }
}

