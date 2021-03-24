using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Agents
{
    public class Ant : MonoBehaviour
    {
        protected int totalHealth;
        protected int currhealth;

        public bool dead = false;

        /// <summary>
        /// Integer vector of this agents position
        /// </summary>
        protected Vector3Int position;


        /// <summary>
        /// A reference to this colony the ant belongs to.
        /// </summary>
        public Colony colony;


        /// <summary>
        /// A reference to the nervous system the ant has
        /// </summary>
        protected NervousSystem ns;

        public int timeSinceLastAction = 0;

        public Ant()
        {
            this.totalHealth = ConfigurationManager.Instance.initialHealth;
            this.currhealth = this.totalHealth;

            // Create the nervous system
            ns = new NervousSystem();
        }

        public void consumeBlock()
        {
            // Cannot consume block if another ant is here
            if (colony.isOtherAntHere(this.position))
                return;

            this.timeSinceLastAction = 0;

            // If this consumption is mulch, refill the health
            if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
                this.currhealth = totalHealth;

            Terrain.WorldManager.Instance.SetBlock(position.x, position.y - 1, position.z, new Terrain.AirBlock());
            this.setPosition(new Vector3Int(position.x, position.y - 1, position.z));

        }

        public void updateHealth(int amount)
        {
            if (this.currhealth + amount < 0)
            {
                this.dead = true;
                this.currhealth = 0;
            }
                

            if (this.currhealth + amount > totalHealth)
                this.currhealth = totalHealth;
            else
                this.currhealth += amount;
        }

        public Vector3Int getPosition(){ return this.position; } 
        public void setPosition(Vector3Int p) 
        { 
            this.position = p;
            this.gameObject.transform.position = new Vector3(p.x - 0.0f, p.y - 0.0f, p.z + 0.0f);
        }

        public void setColony(Colony c) { this.colony = c; }


        /// <summary>
        /// A very important function. Compute the neural network within the ant to choose an action.
        /// 
        /// First go over restrictions for immediate actions to constrain the search of the neuroevolution
        /// </summary>
        public void Act(int todo)
        {
            // Exchange health according to function
            // Give three quarters of health to the queen (if room)
            if (colony.queen.position == this.position)
            {
                if (colony.queen.currhealth + (int)0.75f * this.currhealth < colony.queen.totalHealth)
                {
                    colony.queen.updateHealth((int)0.75f * this.currhealth);
                    this.updateHealth(-(int)0.75f * this.currhealth);
                }
            }



            // Consume mulch
            //if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
                //this.consumeBlock();


            // The output layer of the nervous system
            //  byte[]  {u, r, d, l, consume, null}
            //int todo = ns.indexOfMax(ns.perceive());


            if (todo == 0)
            {
                Move(new Vector2Int(0, 1));
                transform.eulerAngles = new Vector3(0, 90f, 0);
            }
            else if (todo == 1)
            {
                Move(new Vector2Int(1, 0));
                transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else if (todo == 2)
            {
                Move(new Vector2Int(0, -1));
                transform.eulerAngles = new Vector3(0, -90f, 0);
            }
            else if (todo == 3)
            {
                Move(new Vector2Int(-1, 0));
                transform.eulerAngles = new Vector3(0, 0, 0);
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

        /// <summary>
        /// A very important function. Compute the neural network within the ant to choose an action.
        /// 
        /// First go over restrictions for immediate actions to constrain the search of the neuroevolution
        /// </summary>
        public void Act()
        {
            // Exchange health according to function
            // Give three quarters of health to the queen (if room)
            if (colony.queen.position == this.position)
            {
                if (colony.queen.currhealth + (int)0.75f * this.currhealth < colony.queen.totalHealth)
                {
                    colony.queen.updateHealth((int)0.75f * this.currhealth);
                    this.updateHealth(-(int)0.75f * this.currhealth);

                    // No other choice but to do this 
                    return;
                }
            }



            // Consume mulch
            //if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
            //this.consumeBlock();


            // The output layer of the nervous system
            //  byte[]  {u, r, d, l, consume, null}
            int todo = ns.indexOfMax(ns.perceive());


            if (todo == 0)
            {
                Move(new Vector2Int(0, 1));
                transform.eulerAngles = new Vector3(0, 90f, 0);
            }
            else if (todo == 1)
            {
                Move(new Vector2Int(1, 0));
                transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else if (todo == 2)
            {
                Move(new Vector2Int(0, -1));
                transform.eulerAngles = new Vector3(0, -90f, 0);
            }
            else if (todo == 3)
            {
                Move(new Vector2Int(-1, 0));
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (todo == 4)
            {
                consumeBlock();
            }
            else
            {
                // Do nothing
                timeSinceLastAction++;
            }


        }

        protected void Move(Vector2Int dir)
        {
            this.timeSinceLastAction = 0;
            // If Air block in desired direction is greater than 2, don't move
            int airHeight = 0;

            while (Terrain.WorldManager.Instance.GetBlock(position.x + dir.x, airHeight, position.z + dir.y).GetType() != typeof(Terrain.AirBlock)) { airHeight++; }

            if (airHeight - position.y > 2)
            {
                Debug.Log("Ant attempted to move but couldn't, next air spot is greater than 2.");
                return;
            }

            // TODO ensure this bound is within the confines of the arena
            this.setPosition(new Vector3Int(position.x + dir.x, airHeight, position.z + dir.y));
        }

    }
}

