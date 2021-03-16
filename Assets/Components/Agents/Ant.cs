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



        public Ant()
        {
            this.currhealth = this.totalHealth;
        }

        public void consumeMulch()
        {
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

        public void setPosition(Vector3Int p) { this.position = p; }
        public void setColony(Colony c) { this.colony = c; }
    }
}

