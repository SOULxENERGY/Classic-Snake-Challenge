using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeInputUtility
{
    PlayerInput playerInput;
    SwipeDetection swipeDetection;

    public SnakeInputUtility(PlayerInput playerInput, SwipeDetection swipeDetection)
    {
        this.playerInput = playerInput;
        this.swipeDetection = swipeDetection;
    }
    public void Update() {
        swipeDetection.Update();
    }

    public Vector3 GetMoveDir()
    {
        Vector3 inputDirection = GetInputDirection();
       
        if (inputDirection.magnitude > 0)
        {
            Vector3 gridDirection = GetGridDirection(inputDirection);
            
            if (gridDirection.magnitude > 0)
            {
                
                return gridDirection;
            }
        }

        return Vector3.zero;
    }
    private Vector3 GetGridDirection(Vector3 inputDirection)
    {
        
        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            return new Vector3(Mathf.Sign(inputDirection.x), 0, 0);
        }
        else
        {
            return new Vector3(0, Mathf.Sign(inputDirection.y), 0);
        }
    }

    private Vector3 GetInputDirection()
    {
        if (playerInput.currentControlScheme == "Touch")
        {
            return swipeDetection.GetSwipeValue().normalized;
        }
        else
        {
            return playerInput.actions["Move"].ReadValue<Vector2>().normalized;
        }
    }

    
}
