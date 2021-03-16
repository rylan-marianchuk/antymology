using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Agents
{
    public class Ant : MonoBehaviour
    {
        protected int totalHealth = 0;
        protected int currhealth;

        protected bool dead = false;

        /// <summary>
        /// Integer vector of this agents position
        /// </summary>
        protected Vector3Int position;


        /// <summary>
        /// A reference to this colony the ant belongs to.
        /// </summary>
        protected Colony colony;


        /// <summary>
        /// A reference to the nervous system the ant has
        /// </summary>
        protected NervousSystem ns;

        public Ant()
        {
            this.currhealth = this.totalHealth;

            // Create the nervous system
            ns = new NervousSystem();
        }

        public void consumeBlock()
        {
            if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
                this.currhealth = totalHealth;
            Terrain.WorldManager.Instance.SetBlock(position.x, position.y-1, position.z, new Terrain.AirBlock());
            this.setPosition(new Vector3Int(position.x, position.y - 1, position.z));
            
        }

        public void updateHealth(int amount)
        {
            if (this.currhealth + amount < 0)
                this.dead = true;

            if (this.currhealth + amount > totalHealth)
                this.currhealth = totalHealth;
            else
                this.currhealth += amount;
        }

        public void setPosition(Vector3Int p) 
        { 
            this.position = p;
            this.gameObject.transform.position = new Vector3Int(p.x, p.y - 1, p.z);
        }

        public void setColony(Colony c) { this.colony = c; }


        /// <summary>
        /// A very important function. Compute the neural network within the ant to choose an action.
        /// 
        /// First go over restrictions for immediate actions to constrain the search of the neuroevolution
        /// </summary>
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
                consumeBlock();
            }
            else
            {
                // Do nothing
            }


        }



        protected void Move(Vector2Int dir)
        {

        }

    }
}

