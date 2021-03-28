using System.Collections;
using System.Collections.Generic;


public class Node
{
        // Node: 
        // char c \in {'i', 'h', 'o'}
        // float current value
        // 'i': input, 'h': hidden, 'o': output
        public char c;
        public float val;
        public List<Connection> attached;
        public int id;

        // For DFS
        public bool visited;
        public Node(char _c, float _val, List<Connection> attached, int id)
        {
            this.c = _c;
            this.val = _val;
            this.attached = new List<Connection>(attached);
            this.visited = false;
            this.id = id;
        }

        public Node(char _c, List<Connection> attached, int id)
        {
            this.c = _c;
            this.val = 0;
            this.attached = new List<Connection>(attached);
            this.visited = false;
            this.id = id;
        }

        public Node(char _c, int id)
        {
            this.c = _c;
            this.val = 0;
            this.attached = new List<Connection>();
            this.visited = false;
            this.id = id;
        }

        public void setVisited() { this.visited = true; }
        public void setUNvisited() { this.visited = false; }

        public bool getVisited() { return this.visited; }
        public void setVal(float v) { this.val = v; }

 }
