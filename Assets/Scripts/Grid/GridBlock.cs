using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    #region Member Vars
    //========================| MEMBER VARS |==============================//
    //GridController parent
    private GridController controller;

    //if false, nothing will work
    private bool isInitialized = false;

    //if not active, wont spawn 
    private bool active = false;

    

    //current distance to controller player
    private float distanceToPlayer = 0f;

    //save spawned objects in this
    private List<GameObject> spawnedObjects = new List<GameObject>();

    #endregion



    #region MonoBehaviour
    //========================| MONO BEHAVIOUR |==============================//
    private void Update()
    {
        //Deactivate itself if too far away. Activation will be done via controller
        if( active && isInitialized)
        {
            TrackDistance();
            if( distanceToPlayer > controller.Settings.ExpansionRate)
            {
                //Deactivate
                Deactivate();
            }
        }
    }
    #endregion


    #region Engine
    //========================| ENGINE |==============================//
    /// <summary>
    /// Tracks distance between this grid block and GridController current coordinates
    /// </summary>
    private void TrackDistance()
    {
        distanceToPlayer = Vector2.Distance(Coords, controller.Coords);
    }

    #endregion


    #region Public Api
    //========================| API |==============================//

    //Coords of this grid block in the grid
    public Vector2 Coords { get; private set; }

    /// <summary>
    /// Must initialize the object with this method for the grid block to work
    /// </summary>
    /// <param name="_coords">Grid coordinates of this game object</param>
    /// <param name="_controller">Parent grid controller</param>
    public void Initialize(Vector2 _coords, GridController _controller)
    {
        if (_controller == null) return;

        Coords = _coords;
        isInitialized = true;
        controller = _controller;
        
        Activate();
    }

    /// <summary>
    /// Set this block to active
    /// </summary>
    public void Activate()
    {
        if (!isInitialized) return;
        active = true;

        //Enable objects
        if (spawnedObjects.Count > 0)
        {
            foreach (GameObject obj in spawnedObjects)
            {
                if( obj != null)
                    obj.SetActive(true);
            }
        }else
        {
            SpawnEnemy();
        }

    }

    /// <summary>
    /// Set this b lock to inactive
    /// </summary>
    public void Deactivate()
    {
        active = false;

        //Disable objects
        if( spawnedObjects.Count > 0)
        {
            foreach(GameObject obj in spawnedObjects)
            {
                Destroy(obj);
            }

            spawnedObjects.Clear();
        }

    }

    public void SpawnEnemy()
    {
        if (!active) return;
        if (controller == null || controller.EnemyLibrary == null) return;

        //Center position of each grid block
        Vector2 gridBlockOffset = new Vector2(controller.Settings.CellSize / 2, controller.Settings.CellSize / 2);

        //Get a new fish
        Fish newFish = controller.EnemyLibrary.Spawn(
            GameManager.PlayerLevel, 
            (Vector2)transform.position + gridBlockOffset, 
            controller.Settings.RandomX,
            controller.Settings.RandomY);

        if( newFish == null)
        {
            //Debug.Log("Failed to spawn fish for player level " + GameManager.PlayerLevel.ToString());
        }else
        {
            //Debug.Log("Spawning a new enemy for player level: " + GameManager.PlayerLevel.ToString());
            //Save it
            SaveSpawnedObject(newFish.gameObject);
        }

        

    }


    /// <summary>
    /// Add an object to this grid blocks library
    /// </summary>
    /// <param name="SpawnedObject"></param>
    public void SaveSpawnedObject(GameObject SpawnedObject)
    {
        if (SpawnedObject == null) return;

        SpawnedObject.name = "GridBlock child";
        SpawnedObject.transform.SetParent(transform, true);

        spawnedObjects.Add(SpawnedObject);



    }

    #endregion



    #region Debug & Gizmos
    //========================| DEBUG ==============================//

    /// <summary>
    /// GIZMOS
    /// </summary>
    private void OnDrawGizmos()
    {
        if (controller == null || controller.Settings == null) return;

        Gizmos.color = Color.green;
        if (!active) Gizmos.color = Color.red;
        Vector3 size = new Vector3(controller.Settings.CellSize, controller.Settings.CellSize, 0f);
        Vector3 pos = transform.position;
        pos.x += controller.Settings.CellSize / 2f;
        pos.y += controller.Settings.CellSize / 2f;

        Gizmos.DrawWireCube(pos, size);

    }

    #endregion
}
