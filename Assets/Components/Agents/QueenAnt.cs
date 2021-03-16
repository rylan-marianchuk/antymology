using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{
    public class QueenAnt : Ant
    {
        public GameObject nestBlock;
        public void MakeNestBlock()
        {
            // Decrement health by one third
            this.updateHealth(-this.currhealth / 3);
            Terrain.WorldManager.Instance.SetBlock((int)position.x, (int)position.y-1, (int)position.z, new Terrain.NestBlock());
            colony.incrementNestBlocks();
        }
    }

}
