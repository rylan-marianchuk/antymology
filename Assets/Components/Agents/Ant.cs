using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Agents
{
    public class Ant : MonoBehaviour
    {
        protected int totalHealth;
        public int currhealth;
        // For debuggin purposes
        public string lastMove = "";
        public string lastPerception = "";

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
        public int timesTouchedQueen = 0;
        public int mulchBlocksConsumed = 0;
        public short colonyId;
        private bool consumedLastTurn = false;

        //public Ant()
        void Awake()
        {
            this.totalHealth = ConfigurationManager.Instance.initialHealth;
            this.currhealth = this.totalHealth;

            // Create the nervous system
            ns = new NervousSystem();
            ns.antOn = this;
        }

        public void consumeBlock()
        {
            // Cannot consume block if another ant is here
            if (consumedLastTurn || colony.isOtherAntHere(this.position) || position.y <= 5)
            {
                timeSinceLastAction++;
                consumedLastTurn = false;
                return;
            }

            if (!SeeMove(new Vector2Int(0, 1)) && !SeeMove(new Vector2Int(0, -1)) && !SeeMove(new Vector2Int(1, 0)) && !SeeMove(new Vector2Int(-1, 0)))
            {
                // Set this block now to be a mulch block
                Terrain.WorldManager.Instance.SetBlock(position.x + 0, position.y, position.z + 0, new Terrain.MulchBlock());
                // Must die
                die();
                               
                return;
            }

            this.timeSinceLastAction = 0;
            consumedLastTurn = true;
            // If this consumption is mulch, refill the health
            if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
            {
                mulchBlocksConsumed++;
                this.currhealth = totalHealth;
            }
                

            Terrain.WorldManager.Instance.SetBlock(position.x, position.y - 1, position.z, new Terrain.AirBlock());
            this.setPosition(new Vector3Int(position.x, position.y - 1, position.z));

        }

        public void die()
        {
            this.dead = true;
            this.currhealth = 0;
            gameObject.SetActive(false);
        }

        public void updateHealth(int amount)
        {
            if (this.currhealth + amount <= 0)
            {
                die();
                return;
            }
                

            if (this.currhealth + amount > totalHealth)
                this.currhealth = totalHealth;
            else
                this.currhealth += amount;
        }

        public Vector3Int getPosition(){ return this.position; } 
        public void setPosition(Vector3Int p) 
        {
            if (p.x > 127 || p.x < 0 || p.z > 127 || p.z < 0)
                return;

            this.position = p;
            this.gameObject.transform.position = new Vector3(p.x, p.y, p.z);
            if (this.colony.queen == null || gameObject != this.colony.queen)
                colony.pheromoneDeposit[p.x, p.z] = ConfigurationManager.Instance.AntPheromone;
            else
                colony.pheromoneDeposit[p.x, p.z] = ConfigurationManager.Instance.QueenAntPheromone;
        }

        public void setColony(Colony c) { this.colony = c; }
        public void setNervousSystem(NervousSystem n) { this.ns = n; }
        public NervousSystem getNervousSystem() { return this.ns; }
        public int getCurrentHealth() { return this.currhealth; }
        public int getTotalHealth() { return this.totalHealth; }

        /// <summary>
        /// A very important function. Compute the neural network within the ant to choose an action.
        /// 
        /// First go over restrictions for immediate actions to constrain the search of the neuroevolution
        /// </summary>
        public void Act()
        {
            // Exchange health according to function
            // Give three quarters of health to the queen (if room)
            if (colony.queen != null)
            {
                Ant queenScript = colony.queen.GetComponent<Ant>();
                if (!queenScript.dead && queenScript.position == this.position)
                {
                    timesTouchedQueen++;
                    if (queenScript.currhealth + (int)(0.75f * this.currhealth) < queenScript.totalHealth)
                    {
                        queenScript.updateHealth((int)(0.75f * this.currhealth));
                        this.updateHealth((int)(-0.75f * this.currhealth));

                        // No other choice but to exchange health
                        return;
                    }
                }
            }




            // Consume mulch
            //if (Terrain.WorldManager.Instance.GetBlock(position.x, position.y - 1, position.z).GetType() == typeof(Terrain.MulchBlock))
            //this.consumeBlock();


            // The output layer of the nervous system
            //  byte[]  {u, r, d, l, consume, null}
            float[] perception = ns.perceive();
            lastPerception = "";
            lastPerception += " u: " + perception[0].ToString();
            lastPerception += " r: " + perception[1].ToString();
            lastPerception += " d: " + perception[2].ToString();
            lastPerception += " l: " + perception[3].ToString();
            lastPerception += " consume: " + perception[4].ToString();
            lastPerception += " nothing: " + perception[5].ToString();
            if (perception[0] == 0 && perception[0] == perception[1] && perception[0] == perception[2] && perception[0] == perception[3] && perception[0] == perception[4] && perception[0] == perception[5])
            {
                // All outputs are 0, do nothing
                timeSinceLastAction++;
                return;
            }


            int todo = ns.indexOfMax(perception);



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

            // Reduce health by frame amount
            this.updateHealth(-ConfigurationManager.Instance.healthReduction);

        }

        protected void Move(Vector2Int dir)
        {
            
            // If Air block in desired direction is greater than 2, don't move
            int airHeight = 0;

            while (Terrain.WorldManager.Instance.GetBlock(position.x + dir.x, airHeight, position.z + dir.y).GetType() != typeof(Terrain.AirBlock)) { airHeight++; }

            if (airHeight - position.y > 2)
            {
                //Debug.Log("Ant attempted to move but couldn't, next air spot is greater than 2.");
                timeSinceLastAction++;
                return;
            }
            this.timeSinceLastAction = 0;
            
            this.setPosition(new Vector3Int(position.x + dir.x, airHeight, position.z + dir.y));
        }

        protected bool SeeMove(Vector2Int dir)
        {

            // If Air block in desired direction is greater than 2, don't move
            int airHeight = 0;

            while (Terrain.WorldManager.Instance.GetBlock(position.x + dir.x, airHeight, position.z + dir.y).GetType() != typeof(Terrain.AirBlock)) { airHeight++; }

            if (airHeight - position.y > 2)
            {
                //Debug.Log("Ant attempted to move but couldn't, next air spot is greater than 2.");
                timeSinceLastAction++;
                return false;
            }
            return true;
        }

        // Visualize this nervous System when selected
        void OnDrawGizmosSelected()
        {
            Vector3 offset = new Vector3(-40, 0, 0);
            for (int i = 0; i < ns.inputGridLength; i++)
            {
                for (int j = 0; j < ns.inputGridLength; j++)
                {
                    float intensity = ns.nodes[ns.inputGridLength * i + j].val;
                    Gizmos.color = new Color(1, 1, 0, 0.85F);
                    Gizmos.DrawCube(new Vector3(i, 0, j) + offset, Vector3.one);
                }
            }
            
            
        }
    }
}

