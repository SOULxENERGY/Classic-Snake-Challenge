using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform food;
    [SerializeField] private float foodSpawningDelay = 1.2f;
    private GridSystem gridSystemSingleton;
    private bool isfoodEaten = true;
    private Vector2Int currentFoodGrid = new Vector2Int(-1, -1);
    public static FoodSpawner singleton;
   
    


    private void Awake()
    {
        singleton = this;
    }
    void Start()

    {
        StartCoroutine(SpawnTheFood(0));
        gridSystemSingleton = GridSystem.singleton;
    }



    public Vector2Int GetGridIdOfTheFood()
    {
        //it is the id of the grid the food is in
        return currentFoodGrid;
    }

    public bool EatTheFood()
    {
        if (!isfoodEaten)
        {
            
            food.gameObject.SetActive(false);
            
            isfoodEaten = true;
            StartCoroutine(SpawnTheFood(foodSpawningDelay));
            return true;
        }


        return false;
    }

    IEnumerator SpawnTheFood(float delay)
    {
       
        yield return new WaitForSeconds(delay);
         
            int randomGridIdX = Random.Range(0, (int)gridSystemSingleton.GetWidth);
            int randomGridIdY = Random.Range(0, (int)gridSystemSingleton.GetHeight);
            currentFoodGrid = new Vector2Int(randomGridIdX, randomGridIdY);
            Vector3 spawnPos = gridSystemSingleton.GetWorldPosition(currentFoodGrid);
            food.position = spawnPos;
            
            food.gameObject.SetActive(true);
        isfoodEaten = false;

    }
}
