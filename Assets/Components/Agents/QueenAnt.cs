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
            this.updateHealth(-this.health / 3);
            Terrain.WorldManager.Instance.SetBlock((int)position.x, (int)position.y, (int)position.z, new Terrain.NestBlock());
            colony.incrementNestBlocks();
        }
    }

}
