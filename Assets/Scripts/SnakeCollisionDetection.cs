using System.Collections.Generic;
using UnityEngine;

public class SnakeCollisionDetection 
{
    private FoodSpawner foodSpawnerSingleton;
    private GridSystem gridSystemSingleton;

    public SnakeCollisionDetection(FoodSpawner foodSpawnerSingleton,GridSystem gridSystemSingleton)
    {
        this.foodSpawnerSingleton = foodSpawnerSingleton;
        this.gridSystemSingleton = gridSystemSingleton;
    }

  

    public bool DetectCollisionWithFood(Vector3 targetPos)
    {
        Vector2Int targetPosGridId = gridSystemSingleton.GetGridId(targetPos);
        Vector2Int foodGridId = foodSpawnerSingleton.GetGridIdOfTheFood();

        if (targetPosGridId == foodGridId)
        {
            return true;
        }
        return false;
    }

    private bool CheckIfSnakeGoingOutOfTheGrid(Vector3 targetPos)
    {
        return gridSystemSingleton.GetGridId(targetPos) == new Vector2Int(-1, -1);
    }

   public bool CheckIfDead(Vector3 targetPos, List<Transform> snakeBodyParts)
    {
      if (CheckIfSnakeGoingOutOfTheGrid( targetPos) || DetectSelfCollision(snakeBodyParts, targetPos))
        {
            return true;

        }
        return false;
    }

    private bool DetectSelfCollision(List<Transform> snakeBodyParts,Vector3 targetPos)
    {
        Vector2Int targetPosGridId = gridSystemSingleton.GetGridId(targetPos);
        if (snakeBodyParts.Count > 0)
        {


            for (int i = 0; i < snakeBodyParts.Count; i++)
            {
                Vector2Int bodyPartGridId = gridSystemSingleton.GetGridId(snakeBodyParts[i].position);
                if (targetPosGridId == bodyPartGridId)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
