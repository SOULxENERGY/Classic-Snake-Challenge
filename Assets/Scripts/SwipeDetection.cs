using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class SwipeDetection
{
    private bool isTouchEnabled;

    private List<int> uiIgnoreLayers;

    private List<Touch> touchConsideredForDragging = new List<Touch>();
    private float minSwipeMagnitude = 10f;

    public SwipeDetection(bool enableTouchScreen,List<int> allUiLayersThatShouldBeIgnoredWhenDetectingIfTouchIsOverAnyUiElement)
    {
        
        this.uiIgnoreLayers = allUiLayersThatShouldBeIgnoredWhenDetectingIfTouchIsOverAnyUiElement;

        SetTouchEnabled(enableTouchScreen);
    }

    public void SetTouchEnabled(bool enabled)
    {
        isTouchEnabled = enabled;
        if (enabled)
        {
            EnhancedTouchSupport.Enable();
        }
        else
        {
            EnhancedTouchSupport.Disable();
        }
    }

    public void Update()
    {
        if (!isTouchEnabled)
            return;

        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began && !IsTouchOverUI(touch))
            {
                touchConsideredForDragging.Add(touch);
            }
        }

        // Clean up removed touches
        for (int i = touchConsideredForDragging.Count - 1; i >= 0; i--)
        {
            bool isActive = false;
            foreach (var activeTouch in Touch.activeTouches)
            {
                if (touchConsideredForDragging[i].finger.index == activeTouch.finger.index)
                {
                    touchConsideredForDragging[i] = activeTouch;
                    isActive = true;
                    break;
                }
            }

            if (!isActive)
            {
                touchConsideredForDragging.RemoveAt(i);
            }
        }
    }

    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touch.screenPosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var r in results)
        {
            if (!uiIgnoreLayers.Contains(r.gameObject.layer))
                return true;
        }

        return false;
    }

    private Vector2 GetTouchDeltaVec()
    {
         

        for (int i = 0; i < touchConsideredForDragging.Count; i++)
        {
            Touch touch = touchConsideredForDragging[i];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta;

                if (delta.magnitude >= minSwipeMagnitude)
                {
                    // Valid swipe, remove touch so it doesn't repeat
                    touchConsideredForDragging.RemoveAt(i);
                    return delta;
                }
            }
        }

        return Vector2.zero;
    }

    public Vector2 GetSwipeValue()
    {
        if (!isTouchEnabled)
        {
            if (Mouse.current != null)
            {
                return Mouse.current.delta.ReadValue();
            }
            return Vector2.zero;
        }

        Vector2 delta = GetTouchDeltaVec();
        return  delta;
    }
}
