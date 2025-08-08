using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Transform gridPrefab; // The prefab to spawn 
    [SerializeField] private float width = 10;        // Number of columns
    public float GetWidth => width;
    [SerializeField] private float height = 10;       // Number of rows
    public float GetHeight => height;
    [SerializeField] private float cellSize = 1;   // Distance between grid cells
    public float GetCellSize => cellSize;
    [SerializeField] private Vector2 startingpos;
    [SerializeField] private Transform GridParent;
    public static GridSystem singleton;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        //making sure width and height are always a whole number
        width = Mathf.Round(width);
        height = Mathf.Round(height);
        //................................

        //generating grid
          GenerateGrid();
    }

 

    void GenerateGrid()
    {
        for (float x = startingpos.x; x < startingpos.x+ width*cellSize; x+= cellSize)

        {
            for (float y = startingpos.y; y < startingpos.y+ height * cellSize; y+=cellSize)
            {
                Vector2 spawnPosition = new Vector2(x +(cellSize/2),y+(cellSize/2));
               Transform newGrid=  Instantiate(gridPrefab, spawnPosition, Quaternion.identity);
                newGrid.SetParent(GridParent);
                newGrid.localScale = new Vector2(cellSize, cellSize);

            }
        }
    }

    // Converts a world position to a grid ID (bottom-left is 0,0)
    public Vector2Int GetGridId(Vector3 worldPos)
    {
        // Check bounds
        if (worldPos.x < startingpos.x || worldPos.x >= startingpos.x + width * cellSize ||
            worldPos.y < startingpos.y || worldPos.y >= startingpos.y + height * cellSize)
        {
           //Position is out of grid bounds");
            return new Vector2Int(-1, -1); // invalid ID
        }

        // Distance from bottom-left corner
        float dx = worldPos.x - startingpos.x;
        float dy = worldPos.y - startingpos.y;

        // Cell indices
        int xId = Mathf.FloorToInt(dx / cellSize);
        int yId = Mathf.FloorToInt(dy / cellSize);

        return new Vector2Int(xId, yId);
    }

    // Converts a grid ID back to the center world position
    public Vector3 GetWorldPosition(Vector2Int gridId)
    {
        // Center of the cell
        float worldX = startingpos.x + (gridId.x * cellSize) + (cellSize / 2f);
        float worldY = startingpos.y + (gridId.y * cellSize) + (cellSize / 2f);

        return new Vector3(worldX, worldY, 0f);
    }


}
