﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : Singleton<ConfigurationManager>
{

    [Header("World Generation")]

    /// <summary>
    /// The seed for world generation.
    /// </summary>
    public int Seed = 1337;

    /// <summary>
    /// The number of chunks in the x and z dimension of the world.
    /// </summary>
    public int World_Diameter = 16;

    /// <summary>
    /// The number of chunks in the y dimension of the world.
    /// </summary>
    public int World_Height = 4;

    /// <summary>
    /// The number of blocks in any dimension of a chunk.
    /// </summary>
    public int Chunk_Diameter = 8;

    /// <summary>
    /// How much of the tile map does each tile take up.
    /// </summary>
    public float Tile_Map_Unit_Ratio = 0.25f;

    /// <summary>
    /// The number of acidic regions on the map.
    /// </summary>
    public int Number_Of_Acidic_Regions = 10;

    /// <summary>
    /// The radius of each acidic region
    /// </summary>
    public int Acidic_Region_Radius = 5;

    /// <summary>
    /// The number of acidic regions on the map.
    /// </summary>
    public int Number_Of_Conatiner_Spheres = 5;

    /// <summary>
    /// The radius of each acidic region
    /// </summary>
    public int Conatiner_Sphere_Radius = 20;

    [Header("Neuro evolution parameters")]

    /// <summary>
    /// Ants per colony
    /// </summary>
    public int antsPerColony = 20;


    /// <summary>
    /// Max distance the ants may spawn from its center
    /// </summary>
    public int spawnRadius = 20;


    /// <summary>
    /// Radius of blocks around the ant it will take as input into its nervous system
    /// MUST be an odd number
    /// </summary>
    public int inputGridSize = 11;

    /// <summary>
    /// Number of evolving simultaneous colonies in the world
    /// </summary>
    public int coloniesPerWorld = 2;


    /// <summary>
    /// Initial total health
    /// </summary>
    public int initialHealth = 1000;

    /// <summary>
    /// Reduction of Health per frame
    /// </summary>
    public int healthReduction = 1;

    public float connectionMutationRate = 0.3f;

    public float nodeMutationRate = 0.2f;

    public float AntPheromone = 1.25f;
    public float QueenAntPheromone = 3f;

    [Header("If checked, no evolutionary search is run, instead the topColony previously saved is loaded to view in reduced speed.")]
    public bool loadTopColony = false;


}
