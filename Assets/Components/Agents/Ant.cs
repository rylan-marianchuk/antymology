using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Agents
{
    public class Ant : MonoBehaviour
    {
        protected int health = 0;


        /// <summary>
        /// Integer vector of this agents position
        /// </summary>
        protected Vector3 position;


        /// <summary>
        /// A reference to this colony the ant belongs to.
        /// </summary>
        protected Colony colony;


        public void consumeMulch()
        {
            this.health += ConfigurationManager.Instance.mulchBoost;
            Terrain.WorldManager.Instance.SetBlock((int)position.x, (int)position.y, (int)position.z, new Terrain.AirBlock());
        }

        public void updateHealth(int amount)
        {
            this.health += amount;
        }
    }
}

