using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{

    public class NervousSystem
    {

        // The data structures for the NEAT genome

        public List<Node> nodes;
        private List<Node> outputNodes;

        public List<Connection> connections;

        public Ant antOn;

        public int inputGridLength;

        public NervousSystem(SerializableNS serializableNS)
        {
            this.nodes = new List<Node>();
            this.outputNodes = new List<Node>();
            this.connections = new List<Connection>();

            foreach (var n in serializableNS.nodes)
            {
                Node newN = new Node(n.c, n.val, new List<Connection>(n.attached), n.id);
                this.nodes.Add(newN);
                if (n.c == 'o')
                    outputNodes.Add(newN);

            }



            foreach (var c in serializableNS.connections)
                this.connections.Add(new Connection(c.id_in, c.id_out, c.enabled, c.innovationNum));

        }

        public NervousSystem(List<Node> nodes, List<Connection> connections)
        {
            this.nodes = new List<Node>();
            this.outputNodes = new List<Node>();
            this.connections = new List<Connection>();
            
            foreach (var n in nodes)
            {
                Node newN = new Node(n.c, n.val, new List<Connection>(n.attached), n.id);
                this.nodes.Add(newN);
                if (n.c == 'o')
                    outputNodes.Add(newN);
               
            }
                


            foreach (var c in connections)
                this.connections.Add(new Connection(c.id_in, c.id_out, c.enabled, c.innovationNum));

        }


        public NervousSystem()
        {
            
            this.inputGridLength = ConfigurationManager.Instance.inputGridSize;
            this.nodes = new List<Node>(inputGridLength * inputGridLength);
            this.outputNodes = new List<Node>();
            this.connections = new List<Connection>();
            // Adding the input nodes
            int count = 0;
            for (int i = 0; i < inputGridLength * inputGridLength; i++)
            {
                Node anIn = new Node('i', count);
                nodes.Add(anIn);
                count++;
            }
            // Health Input node
            nodes.Add(new Node('i', count));
            count++;
            // Adding the output nodes
            for (int i = 0; i < 6; i++)
            {
                Node anOut = new Node('o', count);
                count++;
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
            for (int j = 0; j < nodes.Count; j++)
            {
                nodes[j].setUNvisited();
                nodes[j].val = 0;
            }

            int off = (ConfigurationManager.Instance.inputGridSize - 1) / 2;
            int i = 0;
            // Retrieve input values - get from environment
            for (int x = -off; x <= off; x++)
            {
                for (int z = -off; z <= off; z++)
                {
                    Node thisN = new Node('i', nodes[i].attached, i);
                    thisN.val = perceiveAtPixel(new Vector2Int(this.antOn.getPosition().x + x, this.antOn.getPosition().z + z));
                    thisN.setVisited();
                    nodes[i] = thisN;
                    i++;
                }
            }
            // Health node
            Node thisNHealth = new Node('i', nodes[i].attached, i);
            thisNHealth.val = (float)antOn.getCurrentHealth() / this.antOn.getTotalHealth();
            thisNHealth.setVisited();
            nodes[i] = thisNHealth;
            // Depth first search from the output nodes to grab value. Update node and weight activation along the way

            Connection nullC = new Connection(-1, -1, false, 0);
            return new float[6] {CalcNetwork(outputNodes[0], nullC, 0), CalcNetwork(outputNodes[1], nullC, 0), CalcNetwork(outputNodes[2], nullC, 0),
                                 CalcNetwork(outputNodes[3], nullC, 0), CalcNetwork(outputNodes[4], nullC, 0), CalcNetwork(outputNodes[5], nullC, 0)};
        }


        private float perceiveAtPixel(Vector2Int topDownWorldPos)
        {
            float queenIntensity = 3.5f;
            float otherAntIntensity = 2.25f;
            float mulchIntensity = 1f;
            float elseIntensity = 0f;
            float nestIntensity = -0.25f;
            float tooHighIntensity = -0.75f;
            // Check if Queen is at this location
            if (antOn.colony.queen != null && antOn.colony.queen.GetComponent<Ant>().getPosition().x == topDownWorldPos.x && antOn.colony.queen.GetComponent<Ant>().getPosition().z == topDownWorldPos.y)
                return queenIntensity;

            // Check if other ant is at this location
            foreach (var ant in this.antOn.colony.colony)
            {
                if (ant.GetComponent<Ant>().dead) continue;
                if (ant.GetComponent<Ant>().getPosition().x == topDownWorldPos.x && ant.GetComponent<Ant>().getPosition().z == topDownWorldPos.y)
                    return otherAntIntensity;
            }

            
            // Get the top most block
            int airHeight = 0;

            while (Terrain.WorldManager.Instance.GetBlock(topDownWorldPos.x, airHeight, topDownWorldPos.y).GetType() != typeof(Terrain.AirBlock)) { airHeight++; }

            /*
            int dx = Mathf.Abs(topDownWorldPos.x - antOn.getPosition().x);
            int dz = Mathf.Abs(topDownWorldPos.y - antOn.getPosition().z);
            if ((dx == 1 && dz == 0) || (dx == 0 && dz == 1) || (dx == 1 && dz == 1))
            {
                // Adjcent block
                if (airHeight - antOn.getPosition().y > 2)
                    return tooHighIntensity;
            }
            */

            // Now check the block right below this air block
            if (Terrain.WorldManager.Instance.GetBlock(topDownWorldPos.x, airHeight-1, topDownWorldPos.y).GetType() == typeof(Terrain.MulchBlock))
                return mulchIntensity;

            // Now check the block right below this air block for NEST
            if (Terrain.WorldManager.Instance.GetBlock(topDownWorldPos.x, airHeight - 1, topDownWorldPos.y).GetType() == typeof(Terrain.NestBlock))
                return nestIntensity;

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
        /// Activation for neural network
        /// 
        /// Activation:  1 / 1 + exp(-4.5x)
        /// </summary>
        /// <returns></returns>
        private float activation(float weightedSum)
        {
            return 1f / (1 + Mathf.Exp(-4.5f * weightedSum)) - 0.5f;
        }

        /// <summary>
        /// Called on each output node to compute the network.
        /// 
        /// Must do it this way, rather than feed forward, because NEAT does not create predefined layers of neurons
        /// </summary>
        /// <param name="start">the output node to compute</param>
        /// <returns>the float value of the output node</returns>
        private float CalcNetwork(Node start, Connection from, int recDepth)
        {
            start.setVisited();
            float inp = 0;
            foreach (Connection Cneighbor in start.attached)
            {   
                if (Cneighbor.id_in == from.id_in && Cneighbor.id_out == from.id_out)
                    continue;

                
                if (Cneighbor.id_in != start.id)
                {
                    // id_in it the neighbor node
                    if (!nodes[Cneighbor.id_in].getVisited())
                    {
                        inp += Cneighbor.weight * CalcNetwork(nodes[Cneighbor.id_in], Cneighbor, recDepth + 1);
                    }
                    else inp += Cneighbor.weight * nodes[Cneighbor.id_in].val;
                }
                else
                {
                    // id_out is the neighbor node
                    if (!nodes[Cneighbor.id_out].getVisited())
                    {
                        inp += Cneighbor.weight * CalcNetwork(nodes[Cneighbor.id_out], Cneighbor, recDepth + 1);
                    }
                    else inp += Cneighbor.weight * nodes[Cneighbor.id_out].val;
                }
            }
            start.val = activation(inp);
            return start.val;
        }
    }
}

