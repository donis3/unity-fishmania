using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.Toolkit;

public class GridController : MonoBehaviour
{
    #region Inspector

    [Header("Grid Settings Object")]
    [SerializeField]
    private GridSettings settings;


    [Header("Enemy Library Object")]
    [SerializeField]
    private EnemyLibrary enemyLibrary;

    [Header("Object to track in the grid")]
    [SerializeField]
    private GameObject player;


    [Header("Enable/Disable grid runtime")]
    [SerializeField]
    private bool isActive = true;
    public bool Active => isActive;

    #endregion

    //Grid Objects real world position.
    private Vector2 gridPosition;
    //Current coordinates of the tracked object
    private Vector2 coords;
    //Library of grid blocks for each coordinate
    private Dictionary<Vector2, GridBlock> grid = new Dictionary<Vector2, GridBlock>();

    //Public enemy library access point
    public EnemyLibrary EnemyLibrary => enemyLibrary;
    //Public settings access point
    public GridSettings Settings => settings;
    //Public coords of the tracked object
    public Vector2 Coords => coords;

    #region MonoBehaviour

    private void Awake()
    {
        if(settings == null)
        {
            Debug.Log("GridController: missing grid settings.");
        }

        gridPosition = transform.position;


        EventManager.StartListening<GameObject>("PlayerSpawn", (spawnedObject) =>
        {
            if (spawnedObject != null)
                player = spawnedObject;
        });
    }

    private void Start()
    {
        
        
        
    }

    private void Update()
    {
        if (!isActive) return;
        if (player == null) return;

        //track player coords
        TrackPlayer();

        //manage grid blocks
        ExpandGrid();
    }

    #endregion


    #region Grid Engine
    /// <summary>
    /// Generate required grid blocks
    /// </summary>
    private void ExpandGrid()
    {
        if (settings == null) return;
        if (player == null) return;

        //Origin is the current player coordinates
        Vector2 origin = Coords;

        //Calculate range of expansion
        int xStart = (int)origin.x - (int)settings.ExpansionRate;
        int xEnd = (int)origin.x + (int)settings.ExpansionRate;

        int yStart = (int)origin.y - (int)settings.ExpansionRate;
        int yEnd = (int)origin.y + (int)settings.ExpansionRate;


        //Loop through the range and create grid blocks for each coordinate
        //If block already exists, run Activate()
        for (int x = xStart; x < xEnd; x++)
        {
            for(int y = yStart; y < yEnd; y++)
            {
                GridBlock thisBLock = CreateGridBlock(new Vector2(x, y));
                //Activate it
                if (thisBLock != null)
                    thisBLock.Activate();

            }
        }

    }


    /// <summary>
    /// Track the player movement in the grid and calculate current coordinates
    /// </summary>
    private void TrackPlayer()
    {
        if( player != null)
            coords = PositionToCoords(player.transform.position);
    }


    /// <summary>
    /// Instantiate a single grid block at given position
    /// </summary>
    /// <param name="_coords"></param>
    /// <returns></returns>
    private GridBlock CreateGridBlock(Vector2 _coords)
    {
        //Check dictionary if it exists
        GridBlock block;
        if( grid.TryGetValue(_coords, out block))
        {
            //this grid position already instantiated
            return block;
        }

        //Create a new grid block object and add gridblock component to it
        GameObject newBlock = new GameObject("GridBlock [" + _coords.x.ToString() + ", " + _coords.y.ToString() + "]", typeof(GridBlock));
        newBlock.transform.SetParent(transform);
        newBlock.transform.localPosition = CoordsToPosition(_coords);

        block = newBlock.GetComponent<GridBlock>();
        block.Initialize(_coords, this);

        //add newly created object
        grid.Add(_coords, block);

        return block;
    }

    #endregion


    #region Public Api
    /// <summary>
    /// Return local position of grid coords
    /// </summary>
    /// <param name="coords">Grid Coordinates</param>
    /// <returns></returns>
    public Vector2 CoordsToPosition(Vector2 coords)
    {
        if (settings == null) return Vector2.zero;

        return new Vector2(
            x: settings.CellSize * coords.x,
            y: settings.CellSize * coords.y
            );
    }

    /// <summary>
    /// Return grid coordinates of world position
    /// </summary>
    /// <param name="_position">World position</param>
    /// <returns></returns>
    public Vector2 PositionToCoords(Vector2 _position)
    {
        //objects world position converted to local grid position
        Vector2 localPos = (Vector2)_position - gridPosition;

        

        //Convert local grid position to coord
        float x = localPos.x / settings.CellSize;
        float y = localPos.y / settings.CellSize;

        /*
        if (x > 0) x = Mathf.Ceil(x);
        else if (x < 0) x = Mathf.Floor(x);

        if (y > 0) y = Mathf.Ceil(y);
        else if (y < 0) y = Mathf.Floor(y);
        */
        
        x = Mathf.Floor(x);
        y = Mathf.Floor(y);
        
        return new Vector2(x, y);
    }

    #endregion
}