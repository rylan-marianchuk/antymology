using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{
    public class QueenAnt : Ant
    {
        public GameObject nestBlock;

        void Awake()
        {
            this.totalHealth = ConfigurationManager.Instance.initialHealth;
            this.currhealth = this.totalHealth;

            // Create the nervous system
            ns = new NervousSystem();
            ns.antOn = this;
        }


        public void MakeNestBlock()
        {
            // Decrement health by one third
            this.updateHealth(-this.currhealth / 3);
            Terrain.WorldManager.Instance.SetBlock((int)position.x, (int)position.y-1, (int)position.z, new Terrain.NestBlock());
            colony.incrementNestBlocks();
        }



        public void Act()
        {
            // Exchange health


            // Consume mulch

            // The output layer of the nervous system
            //  byte[]  {u, r, d, l, consume, null}
            int todo = ns.indexOfMax(ns.perceive());

            if (todo == 0)
            {
                Move(new Vector2Int(0, 0));
            }
            else if (todo == 1)
            {
                Move(new Vector2Int(0, 0));
            }
            else if (todo == 2)
            {
                Move(new Vector2Int(0, 0));
            }
            else if (todo == 3)
            {
                Move(new Vector2Int(0, 0));
            }
            else if (todo == 4)
            {
                // Only difference here the queen ant will make nest block
                MakeNestBlock();
            }
            else
            {
                // Do nothing
            }


        }
    }

}
