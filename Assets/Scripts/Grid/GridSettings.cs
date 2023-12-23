using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid/Grid Settings")]
public class GridSettings : ScriptableObject
{
    [Header("Size of each cell in units")]
    [SerializeField]
    private float cellSize;
    public float CellSize => cellSize;



    
    [Header("GridBlocks Expansion & Deactivation range")]
    [SerializeField]
    private int expansionRate;
    public int ExpansionRate => expansionRate;



    [Header("Enemy Spawn Position Settings")]

    [Range(0f,100f)]
    [SerializeField]
    private float ObjectSpawnRandomnessX = 0f;
    public float RandomX => ObjectSpawnRandomnessX;

    [Range(0f, 100f)]
    [SerializeField]
    private float ObjectSpawnRandomnessY = 0f;
    public float RandomY => ObjectSpawnRandomnessY;

}
