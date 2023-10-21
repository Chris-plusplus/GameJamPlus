namespace Interactables
{
    public interface ILiftable
    {
        public void PickUp(ILiftableHolder holder);
        void Drop();
    }
}