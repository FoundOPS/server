namespace FoundOps.Api.Models
{
    interface IImportable
    {
        int? StatusInt { get; set; }
    }

    enum CellStatus
    {
        Linked = 0,
        New = 1,
        Suggested = 2,
        Error = 3
    }
}