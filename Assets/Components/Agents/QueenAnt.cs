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
            if (Terrain.WorldManager.Instance.GetBlock((int)position.x, (int)position.y - 1, (int)position.z).GetType() == typeof(Terrain.NestBlock))
                return;
            // Decrement health by one third
            this.updateHealth(-this.currhealth / 3);
            Terrain.WorldManager.Instance.SetBlock((int)position.x, (int)position.y-1, (int)position.z, new Terrain.NestBlock());
            colony.incrementNestBlocks();
        }



        public new void Act()
        {
            // Exchange health


            // Consume mulch

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
                // Only difference here the queen ant will make nest block
                MakeNestBlock();
            }
            else
            {
                // Consume the below block
                consumeBlock();
            }

            // Reduce health by frame amount
            this.updateHealth(-ConfigurationManager.Instance.healthReduction);
        }
    }

}
