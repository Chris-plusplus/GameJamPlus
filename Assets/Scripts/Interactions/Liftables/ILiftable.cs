namespace Interactables
{
    public interface ILiftable
    {
        bool IsEnabled { get; }
        public void PickUp(ILiftableHolder holder);
        void Drop();
    }
}