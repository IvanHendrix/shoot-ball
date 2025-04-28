using UnityEngine;

namespace Services.Input
{
    public class MobileInputService : IInputService
    {
        public bool IsTouchStarted()
        {
            return UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began;
        }

        public bool IsTouching()
        {
            return UnityEngine.Input.touchCount > 0 && (
                UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved ||
                UnityEngine.Input.GetTouch(0).phase == TouchPhase.Stationary);
        }

        public bool IsTouchEnded()
        {
            return UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended;
        }
    }
}