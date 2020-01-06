using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rhinotap.Grid2D
{
    /// <summary>
    /// Create a grid at the game objects position
    /// Define grid size
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        
        #region Singleton Instance
        private static GridManager _instance; //instance of this object
        public static GridManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GridManager>();//find the instance in the scene
                    if (_instance == null)
                        Debug.LogError("Grid manager Not available in the scene");
                }
                return _instance;
            }
        }
        #endregion
        public static int CellSize { get { return instance.cellSize; } }

        public static Vector2 Coords { get { return instance.coords; } }

        [Header("Grid Settings")]
        [SerializeField]
        private int cellSize = 10;
        [SerializeField]
        private int expandSize = 3;
        [SerializeField]
        private int triggerExpandDistance = 3;

        private bool isInitialized = false;
        private bool hasError = false;
        private Transform player;
        private Vector2 coords = Vector2.zero;
        private Vector2 lastCoords = Vector2.zero;
        private Vector2 realPosition = Vector2.zero;
        private Vector2 lastRealPosition = Vector2.zero;
        private Vector2 direction = Vector2.zero;

        private GameObject originalGridChild;


        private int playerLevel = 1;


        private Dictionary<Vector2, GameObject> zones = new Dictionary<Vector2, GameObject>();

        private void Awake()
        {
            originalGridChild = transform.Find("Spawner")?.gameObject;
            if( originalGridChild == null)
            {
                Debug.Log("GridManager requires a child object named Spawner");
                hasError = true;
            }else
            {
                originalGridChild.name = "Spawner-Original";
                originalGridChild.transform.position = Vector2.zero;
                originalGridChild.SetActive(false);
            }
        }



        private void Update()
        {
            //Find player object
            if( player == null)
                FindPlayerObject();

            //Calculate current coords
            CalculatePlayerGridPosition();

            ManageGrid();

            DebugCanvas.Set("Current Grid Coordinates: " + coords, 1);
            
        }




        //=======================| Tracking Current Coordinates |====================//
        private void FindPlayerObject()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player == null)
                {
                    hasError = true;
                    Debug.Log("Grid Manager. could not find player object");
                    return;
                }
            }
        }

        private void CalculatePlayerGridPosition()
        {
            if (hasError)
                return;

            lastRealPosition = realPosition;
            realPosition = player.position;

            if (realPosition != lastRealPosition)
            {
                direction = realPosition - lastRealPosition;
            }

            //Find real grid position: player world position - grid origin world position
            Vector2 realPositionDifference = realPosition - (Vector2)transform.position;

            //Convert the real vector to grid
            int currentX = Mathf.FloorToInt(realPositionDifference.x / cellSize);
            int currentY = Mathf.FloorToInt(realPositionDifference.y / cellSize);

            Vector2 playerGridCoords = new Vector2(currentX, currentY);

            if (coords == playerGridCoords)
            {
                //grid coords have not changed since last update
                return;
            }
            else
            {
                lastCoords = coords;
                coords = playerGridCoords;
                return;
            }
        }



        private void InstantiateZone(Vector2 position)
        {
            if (hasError)
                return;
            
            if( zones.ContainsKey(position))
            {
                //this coordinate has a grid zone instantiated already
                zones[position].SetActive(true);
            }else
            {
                //this coordinate needs a new grid zone
                GameObject newZone = GameObject.Instantiate(originalGridChild, transform);
                newZone.name = "Zone [" + position.x + ", " + position.y + "]";
                newZone.transform.position = CoordToPosition(position);
                newZone.SetActive(true);
                zones.Add(position, newZone);

            }
        }


        private void ManageGrid()
        {
            Vector2 origin;
            if (isInitialized == false)
            {
                origin = transform.position;
                isInitialized = true;
            }
            else
            {
                origin = coords;
            }

            int radius = expandSize;

            Vector2 spawnZoneKey;
            //Loop through grid positions. 
            for (int x = (int)origin.x - radius; x <= (int)origin.x + radius; x++)
            {
                for (int y = (int)origin.y - radius; y <= (int)origin.y + radius; y++)
                {
                    spawnZoneKey = new Vector2(x, y);
                    InstantiateZone(spawnZoneKey);
                }
            }
        }



        private Vector2 CoordToPosition(Vector2 coords)
        {
            Vector2 result = new Vector2(
                coords.x * cellSize,
                coords.y * cellSize);

            result = result + (Vector2)transform.position;

            return result;
        }


    }
}



