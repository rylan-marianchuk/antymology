using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{ 
    public class Colony : MonoBehaviour
    {
        public List<Ant> colony;

        public QueenAnt queen;

        private bool allDead = false;

        private int totalNestBlocks = 0;

        public Colony(int size)
        {
            colony = new List<Ant>();
            this.spawnColony(size);
        }


        public void incrementNestBlocks()
        {
            totalNestBlocks++;
        }

        public int getTotalNestBlocks()
        {
            return this.totalNestBlocks;
        }

        public bool isAllDead() { return this.allDead;  }

        public void spawnColony(int spawnRadius)
        {
            // Choose a center location in this colony
            Vector2Int c = new Vector2Int(Random.Range(spawnRadius / 2, ConfigurationManager.Instance.Chunk_Diameter * ConfigurationManager.Instance.World_Diameter - spawnRadius / 2),
                Random.Range(spawnRadius / 2, ConfigurationManager.Instance.Chunk_Diameter * ConfigurationManager.Instance.World_Diameter - spawnRadius / 2));

            
            // Spawn all ants
            for (int i = 0; i < ConfigurationManager.Instance.antsPerColony; i++)
            {
                spawnAnt(c, spawnRadius, false);
            }
            // Spawning the queen
            spawnAnt(c, spawnRadius, true);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">colonyCenter</param>
        /// <param name="isQueen"></param>
        private void spawnAnt(Vector2Int c, int spawnRadius, bool isQueen)
        {
            int rx = Random.Range(-spawnRadius / 2, spawnRadius / 2);
            int rz = Random.Range(-spawnRadius / 2, spawnRadius / 2);

            // Finding the highest airblock here
            int y = 0;

            while (Terrain.WorldManager.Instance.GetBlock(c.x + rx, y, c.y + rz).GetType() != typeof(Terrain.AirBlock)) { y++; }
            // The offsets here for mere display of gameobject position

            GameObject obj;
            if (isQueen)
            {
                obj = Instantiate(Terrain.WorldManager.Instance.queenAntPrefab, new Vector3(c.x + rx - 1.6f, y - 2.33f, c.y + rz + 0.295f), Quaternion.identity);
                this.queen = obj.GetComponent<QueenAnt>();
            }
            else
                obj = Instantiate(Terrain.WorldManager.Instance.antPrefab, new Vector3(c.x + rx - 0.0f, y - 0.0f, c.y + rz + 0.0f), Quaternion.identity);
            
                
            obj.GetComponent<Ant>().setPosition(new Vector3Int(c.x + rx, y, c.y + rz));
            obj.GetComponent<Ant>().setColony(this);
            //obj.GetComponent<Ant>().MakeNestBlock();
            this.colony.Add(obj.GetComponent<Ant>());
        }



        /// <param name="p">World Position</param>
        /// <returns>Return true if more than one ant is at this position</returns>
        public bool isOtherAntHere(Vector3Int p)
        {
            int count = 0;
            foreach (var ant in colony)
            {
                if (ant.getPosition() == p) count++;
            }
            return count >= 2;
        }


        public void MoveColony()
        {
            foreach (Agents.Ant ant in this.colony)
            {
                if (!ant.dead)
                    ant.Act();
                if (ant.timeSinceLastAction >= 5)
                    ant.dead = true;
            }
        }
    }

}

