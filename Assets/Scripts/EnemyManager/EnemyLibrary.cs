using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName ="EnemyLibrary/new library")]
public class EnemyLibrary : ScriptableObject
{
    [Header("Spawn Pool for each level")]
    [SerializeField]
    private SpawnPool[] pools;


    private int maxLevel = 0;

    private void OnEnable()
    {
        maxLevel = pools.Max(x => x.Level);
        Debug.Log("Max available spawn pool level: " + maxLevel.ToString());
    }


    /// <summary>
    /// Spawn a fish and return it
    /// </summary>
    /// <param name="level">Current player level</param>
    /// <param name="position">Position to spawn</param>
    /// <returns></returns>
    public Fish Spawn(int level, Vector2 position, float RandomizeX = 3f, float RandomizeY = 3f)
    {
        if( level > maxLevel)
        {
            level = maxLevel;
        }
        SpawnPool result =  pools.FirstOrDefault(x => x.Level == level);

        if( result == null)
        {
            Debug.Log("Cant find spawnpool for level " + level);
            return null;
        }

        if( RandomizeX > 0f)
        {
            RandomizeX = Random.Range(RandomizeX * -10f, RandomizeX * 10f) / 10f;
            position.x += RandomizeX;
        }
        if (RandomizeY > 0f)
        {
            RandomizeY = Random.Range(RandomizeY * -10f, RandomizeY * 10f) / 10f;
            position.y += RandomizeY;
        }

        GameObject newFish = GameObject.Instantiate(result.Get().gameObject, position, Quaternion.identity);

        return newFish.GetComponent<Fish>();

    }
    
    
}

[System.Serializable]
public class SpawnPool
{
    [Header("Spawnable Enemies Of Player Level")]
    [SerializeField]
    private int playerLevel;

    public int Level => playerLevel;

    [Header("Add same prefab multiple times to increase spawn chance")]
    [SerializeField]
    private Fish[] fishPrefabs;

    public Fish Get()
    {
        if (fishPrefabs.Length == 0 || playerLevel <= 0) return null;

        int index = Random.Range(0, fishPrefabs.Length);

        return fishPrefabs[index];
    }
}
