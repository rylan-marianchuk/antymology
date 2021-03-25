using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Antymology.Agents
{ 
    public class Colony
    {
        // All these gameobjects are regular ants
        public List<GameObject> colony;


        public GameObject queen;

        public short colonyId;

        private int totalNestBlocks = 0;


        /// <summary>
        /// Spawn an ant colony in the specified world quadrant. Default split of the world into 1/4 sections
        /// </summary>
        /// <param name="worldQuadrant"></param>
        public Colony(short worldQuadrant, Colony singleParent)
        {
            colony = new List<GameObject>();
            colonyId = worldQuadrant;
            this.spawnColony(ConfigurationManager.Instance.spawnRadius, singleParent);
        }


        /// <summary>
        /// Initial best colony to begin the genetic search
        /// </summary>
        public Colony()
        {
            colony = new List<GameObject>();
            for (int i = 0; i < ConfigurationManager.Instance.antsPerColony; i++)
            {
                colony.Add(Object.Instantiate(Terrain.WorldManager.Instance.antPrefab));
            }
            queen = Object.Instantiate(Terrain.WorldManager.Instance.queenAntPrefab);
        }

        public void incrementNestBlocks()
        {
            totalNestBlocks++;
        }

        public int getTotalNestBlocks()
        {
            return this.totalNestBlocks;
        }

        public bool isAllDead() 
        {
            foreach (var ant in this.colony)
            {
                if (ant == null) continue;
                if (!ant.GetComponent<Ant>().dead) return false;
            }
            return true;
        }


        /// <summary>
        /// Create a new colony based off a single parent for now.
        /// 
        /// Mutate each nervous system by the proper probabilities before passing it on.
        /// </summary>
        /// <param name="spawnRadius"></param>
        /// <param name="singleParent"></param>
        /// <param name="randomChoice"></param>
        public void spawnColony(int spawnRadius, Colony singleParent, bool randomChoice = false)
        {

            int l = ConfigurationManager.Instance.Chunk_Diameter * ConfigurationManager.Instance.World_Diameter;
            Vector2Int c;
            // Choose a center location in this colony
            if (randomChoice)
            {
                c = new Vector2Int(Random.Range(spawnRadius / 2, l/2 - spawnRadius / 2),
                                   Random.Range(spawnRadius / 2, l/2 - spawnRadius / 2));
            }
            else
            {
                c = new Vector2Int(l/4, l/4);
                if (colonyId == 1) c.y += l/2;
                else if (colonyId == 2)
                {
                    c.y += l/2;
                    c.x += l/2;
                }
                else if (colonyId == 3) c.x += l/2;
            }

            
            // Spawn all ants
            for (int i = 0; i < ConfigurationManager.Instance.antsPerColony; i++)
            {
                if (Random.Range(0f, 1f) < ConfigurationManager.Instance.connectionMutationRate)
                {
                    NeuroEvolution.MutateByConnection(singleParent.colony[i].GetComponent<Ant>().getNervousSystem());
                }
                else if (Random.Range(0f, 1f) < ConfigurationManager.Instance.nodeMutationRate)
                {
                    NeuroEvolution.MutateByNode(singleParent.colony[i].GetComponent<Ant>().getNervousSystem());
                }

                NervousSystem newNS = new NervousSystem(singleParent.colony[i].GetComponent<Ant>().getNervousSystem().nodes,
                                                        singleParent.colony[i].GetComponent<Ant>().getNervousSystem().connections);
                spawnAnt(c, spawnRadius, newNS, false);
            }

            // Spawning the queen
            if (Random.Range(0f, 1f) < ConfigurationManager.Instance.connectionMutationRate)
            {
                NeuroEvolution.MutateByConnection(singleParent.queen.GetComponent<Ant>().getNervousSystem());
            }
            else if (Random.Range(0f, 1f) < ConfigurationManager.Instance.nodeMutationRate)
            {
                NeuroEvolution.MutateByNode(singleParent.queen.GetComponent<Ant>().getNervousSystem());
            }
            NervousSystem newNSqueen = new NervousSystem(singleParent.queen.GetComponent<Ant>().getNervousSystem().nodes, 
                                                         singleParent.queen.GetComponent<Ant>().getNervousSystem().connections);
            
            spawnAnt(c, spawnRadius, newNSqueen, true);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">colonyCenter</param>
        /// <param name="isQueen"></param>
        private void spawnAnt(Vector2Int c, int spawnRadius, NervousSystem toHave, bool isQueen)
        {
            int rx = Random.Range(-spawnRadius, spawnRadius);
            int rz = Random.Range(-spawnRadius, spawnRadius);

            // Finding the highest airblock here
            int y = 0;

            while (Terrain.WorldManager.Instance.GetBlock(c.x + rx, y, c.y + rz).GetType() != typeof(Terrain.AirBlock)) { y++; }
            // The offsets here for mere display of gameobject position

            GameObject obj;
            if (isQueen)
            {
                obj = UnityEngine.Object.Instantiate(Terrain.WorldManager.Instance.queenAntPrefab, new Vector3(c.x + rx, y, c.y + rz), Quaternion.identity);
                this.queen = obj;
            }
            else
                obj = UnityEngine.Object.Instantiate(Terrain.WorldManager.Instance.antPrefab, new Vector3(c.x + rx, y, c.y + rz), Quaternion.identity);
            
                
            obj.GetComponent<Ant>().setPosition(new Vector3Int(c.x + rx, y, c.y + rz));
            obj.GetComponent<Ant>().setColony(this);
            obj.GetComponent<Ant>().setNervousSystem(toHave);
            obj.GetComponent<Ant>().colonyId = this.colonyId;
            toHave.antOn = obj.GetComponent<Ant>();
            this.colony.Add(obj);
        }



        /// <param name="p">World Position</param>
        /// <returns>Return true if more than one ant is at this position</returns>
        public bool isOtherAntHere(Vector3Int p)
        {
            int count = 0;
            foreach (var ant in colony)
            {
                if (ant == null) continue;
                if (ant.GetComponent<Ant>().getPosition() == p) count++;
            }
            return count >= 2;
        }


        public void MoveColony()
        {
            foreach (GameObject ant in this.colony)
            {
                if (ant == null) continue;
                if (ant == queen)
                {
                    queen.GetComponent<QueenAnt>().Act();
                    continue;
                }

                Ant s = ant.GetComponent<Ant>();
                if (!s.dead)
                    s.Act();

                if (s.timeSinceLastAction >= 5)
                {
                    s.die();
                }
                    
            }
        }
    }

}

