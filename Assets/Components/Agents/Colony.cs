using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{ 
    public class Colony : MonoBehaviour
    {
        private int totalNestBlocks = 0;

        public void incrementNestBlocks()
        {
            totalNestBlocks++;
        }

        public int getTotalNestBlocks()
        {
            return this.totalNestBlocks;
        }


        public void spawn()
        {

        }
    }
}

