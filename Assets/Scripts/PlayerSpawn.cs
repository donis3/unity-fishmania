using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhinotap.Toolkit;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Current Player Prefab")]
    [SerializeField]
    private GameObject PlayerPrefab;


    private GameObject player;

    private void Start()
    {
        EventManager.StartListening("GameStart", SpawnPlayer);
        
    }

    public void SpawnPlayer()
    {
        if (player == null)
            player = Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
        else
        {
            player.SetActive(true);
            player.transform.position = transform.position;
        }

        EventManager.Trigger<GameObject>("PlayerSpawn", player);
    }

}
