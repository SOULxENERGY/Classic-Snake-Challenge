using System.Collections.Generic;
using System;
using UnityEngine;

using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    public static Snake singleton;
    // Handles swipe gesture detection
    private SwipeDetection swipeDetection;

    // Handles checking for collisions (self, walls, food)
    private SnakeCollisionDetection snakeCollisionDetection;

    // Converts raw player input into movement directions
    private SnakeInputUtility snakeInputUtility;

    // Unity's new Input System component for player input
    [SerializeField] private PlayerInput playerInput;

    // Prefab for snake body segments
    [SerializeField] private Transform snakeBodyPartPrefab;

    // Singleton references to other systems
    private FoodSpawner foodSpawnerSingleton;
    private GridSystem gridSystemSingleton;

    // Movement direction (grid-based)
    private Vector3 moveDir = new Vector3(1,0,0);

    // Speed of snake movement (grid cells per second)
    [SerializeField] private float moveSpeed = 1;

    // How smoothly snake moves toward its target position
    [SerializeField] private float lerpSpeed = 12;

    // Accumulated movement distance since last grid step
    private float deltaPos = 0;

    // The exact position snake is moving toward
    private Vector3 targetPos;

    // Flag to mark if snake has died
    private bool isDead = false;

    // List storing all body segments (head not included)
    private List<Transform> snakeBodyParts = new List<Transform>();

    [SerializeField] private ScoreData playerData;
    private float score = 0;
    public delegate void EatDelegate(float score);
    public event EatDelegate eatEvent;
    public event Action deathEvent;

    //audio related
    [SerializeField] private AudioClip eatingSfx;
    [SerializeField] private AudioClip dyingSfx;
    [SerializeField] private AudioSource audiosrc;

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        // Cache singleton references
        foodSpawnerSingleton = FoodSpawner.singleton;
        gridSystemSingleton = GridSystem.singleton;

        // Start by targeting the current head position
        targetPos = transform.position;

        // Create helper classes for collision detection and input
        snakeCollisionDetection = new SnakeCollisionDetection(foodSpawnerSingleton, gridSystemSingleton);
        swipeDetection = new SwipeDetection(true, new List<int>() { 2 }); // "true" for vertical/horizontal restriction, "2" = min swipe length
        snakeInputUtility = new SnakeInputUtility(playerInput, swipeDetection);
    }


    void Update()
    {
        if (!isDead)
        {
            // Process input
            snakeInputUtility.Update();
            AssignMoveDir();

            // Increase distance traveled based on speed and frame time
            float snakeSizeBasedSpeed = moveSpeed + (snakeBodyParts.Count == 0 ? 1 : snakeBodyParts.Count) * 0.2f;
            deltaPos += Mathf.Abs(snakeSizeBasedSpeed) * Time.deltaTime * moveDir.magnitude;
            

            // If we have moved at least 1 grid cell
            if (deltaPos >= gridSystemSingleton.GetCellSize)
            {
                // Remove the distance of one cell from delta
                deltaPos -= gridSystemSingleton.GetCellSize;

                // Save the head's current position for body movement
                Vector3 lastHeadPos = targetPos;

                // Advance target position by one cell in current direction
                targetPos += moveDir * gridSystemSingleton.GetCellSize;

                // Move body segments forward
                MoveBodyParts(lastHeadPos);

                // Check if the new target position causes death
                isDead = snakeCollisionDetection.CheckIfDead(targetPos, snakeBodyParts);
                if (isDead)
                {
                    audiosrc.clip = dyingSfx;
                    audiosrc.Play();
                    if (deathEvent != null)
                    {
                        deathEvent.Invoke();
                    }
                }
            }

            // Smoothly move snake toward target position
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed * moveSpeed);

            // Check for food collision and grow if eaten
            if (snakeCollisionDetection.DetectCollisionWithFood(targetPos))
            {
                if (foodSpawnerSingleton.EatTheFood())
                {
                    audiosrc.clip = eatingSfx;
                    audiosrc.Play();
                    score++;
                    if (eatEvent != null)
                    {
                        eatEvent(score);
                    }

                    
                    GrowBigger();
                }
            }
        }
    }


    /// <summary>
    /// Gets the movement direction from input utility.
    /// Only updates if there is a valid (non-zero) direction.
    /// </summary>
    private void AssignMoveDir()
    {
        Vector3 newMoveDir = snakeInputUtility.GetMoveDir();

        //  change direction if there is actual input
        if (newMoveDir.magnitude > 0)
        {
            // Prevent 180° turn: dot product will be -1 if opposite
            if (Vector3.Dot(newMoveDir, moveDir) != -1)
            {
                moveDir = newMoveDir;
            }
        }
    }


    /// <summary>
    /// Adds a new segment to the end of the snake.
    /// </summary>
    private void GrowBigger()
    {
        Vector3 spawnPos;

        // First body part follows the head, others follow the last tail position
        if (snakeBodyParts.Count == 0)
        {
            spawnPos = transform.position;
        }
        else
        {
            spawnPos = snakeBodyParts[snakeBodyParts.Count - 1].position;
        }

        Transform newPart = Instantiate(snakeBodyPartPrefab, spawnPos, Quaternion.identity);
        snakeBodyParts.Add(newPart);
    }


    /// <summary>
    /// Moves all body parts forward, following the head.
    /// </summary>
    private void MoveBodyParts(Vector3 lastHeadPos)
    {
        if (snakeBodyParts.Count > 0)
        {
            Vector3 prevPos = lastHeadPos;

            // Iterate through body parts and move each to the previous part's position
            for (int i = 0; i < snakeBodyParts.Count; i++)
            {
                Vector3 tempPos = snakeBodyParts[i].position;
                snakeBodyParts[i].position = prevPos;
                prevPos = tempPos;
            }
        }
    }
}
