using UnityEngine;
using System.Collections;

public class InputsManager : MonoBehaviour {
    public delegate void OnSwipe();

    public static InputsManager Instance;
    public OnSwipe OnSwipeLeft;
    public OnSwipe OnSwipeRight;

    public float swipeTreshold;

    protected bool isEnabled = true;
    protected bool isTouching;
    protected Vector3 startTouchPos;

    void Awake()
    {
        Instance = this;
    }

    void Update () {
        if (!isEnabled)
            return;

//#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetMouseButtonDown(0))
        {
            isTouching = true;
            startTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isTouching = false;
        }
        else if (isTouching)
        {
            Vector3 currentPos = Input.mousePosition;
            if(startTouchPos.x - currentPos.x > swipeTreshold)
            {
                isTouching = false;
                OnSwipeLeft.Invoke();
            }
            else if (startTouchPos.x - currentPos.x < -swipeTreshold)
            {
                isTouching = false;
                OnSwipeRight.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnSwipeLeft.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnSwipeRight.Invoke();
        }
//#else
//        if (Input.touchCount <= 0)
//            return;

//        Touch touch = Input.GetTouch(0);
//        switch (touch.phase)
//        {
//            case TouchPhase.Began:
//                isTouching = true;
//                startTouchPos = touch.position;
//                break;
//            case TouchPhase.Ended:
//            case TouchPhase.Canceled:
//                isTouching = false;
//                break;
//            case TouchPhase.Moved:
//                Vector3 currentPos = touch.position;
//                if (isTouching && startTouchPos.x - currentPos.x > swipeTreshold)
//                {
//                    isTouching = false;
//                    OnSwipeLeft.Invoke();
//                }
//                else if (isTouching && startTouchPos.x - currentPos.x < -swipeTreshold)
//                {
//                    isTouching = false;
//                    OnSwipeRight.Invoke();
//                }
//                break;
//        }
//#endif
    }

    public void Toggle(bool value)
    {
        isEnabled = value;
    }
}
