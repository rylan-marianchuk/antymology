using System.Collections;
using System.Collections.Generic;
using System;


namespace Antymology.Agents
{
    [Serializable]
    public class SerializableNS
    {
        public List<Node> nodes;

        public List<Connection> connections;

        public SerializableNS(NervousSystem toSerialize)
        {
            this.nodes = new List<Node>();
            this.connections = new List<Connection>();

            foreach (var n in toSerialize.nodes)
            {
                Node newN = new Node(n.c, n.val, new List<Connection>(n.attached), n.id);
                this.nodes.Add(newN);
            }



            foreach (var c in toSerialize.connections)
                this.connections.Add(new Connection(c.id_in, c.id_out, c.enabled, c.innovationNum));

        }


    }
}

