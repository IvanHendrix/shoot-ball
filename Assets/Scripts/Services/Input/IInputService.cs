namespace Services.Input
{
    public interface IInputService : IService
    {
        bool IsTouchStarted();
        bool IsTouching();
        bool IsTouchEnded();
    }
}