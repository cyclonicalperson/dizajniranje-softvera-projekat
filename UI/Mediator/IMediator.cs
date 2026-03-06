using System.Windows;

namespace CoWorkingManager.Mediator
{
    public interface IMediator
    {
        void Notify(Window sender, string eventCode);
    }
}