

namespace CoWorkingManager.Mediator
{
    public interface IMediator
    {
        void Notify(object sender, string eventCode);
    }
}