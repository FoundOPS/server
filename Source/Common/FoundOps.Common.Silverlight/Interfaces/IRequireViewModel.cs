namespace FoundOps.Common.Silverlight.Interfaces
{
    public interface IRequireVM
    {
        IDataVM RequiredVM { get; set; }
    }

    public interface IDataVM
    {
        void Subscribe();
    }
}
