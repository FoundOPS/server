namespace FoundOps.Common.Silverlight.MVVM.Messages
{
    public class SelectItemMessage<T>
    {
        public T ItemToSelect { get; private set; }
        public bool Handled { get; set; }
        public SelectItemMessage(T itemToSelect)
        {
            ItemToSelect = itemToSelect;
        }
    }
}
