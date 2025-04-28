namespace Services.Input
{
    public class MouseInputService : IInputService
    {
        public bool IsTouchStarted() => UnityEngine.Input.GetMouseButtonDown(0);
        public bool IsTouching() => UnityEngine.Input.GetMouseButton(0);
        public bool IsTouchEnded() => UnityEngine.Input.GetMouseButtonUp(0);
    }
}