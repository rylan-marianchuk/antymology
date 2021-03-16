using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{ 
    public class Colony : MonoBehaviour
    {
        private List<Ant> colony;

        public QueenAnt queen;

        public Colony()
        {
            colony = new List<Ant>();
            this.spawnColony(20);
        }

        private int totalNestBlocks = 0;

        public void incrementNestBlocks()
        {
            totalNestBlocks++;
        }

        public int getTotalNestBlocks()
        {
            return this.totalNestBlocks;
        }



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
                obj = Instantiate(Terrain.WorldManager.Instance.antPrefab, new Vector3(c.x + rx - 0.6f, y - 1.3f, c.y + rz + 0.1f), Quaternion.identity);
            
                
            obj.GetComponent<Ant>().setPosition(new Vector3Int(c.x + rx, y, c.y + rz));
            obj.GetComponent<Ant>().setColony(this);
            obj.GetComponent<Ant>().consumeMulch();
            this.colony.Add(obj.GetComponent<Ant>());
        }
    }
}

