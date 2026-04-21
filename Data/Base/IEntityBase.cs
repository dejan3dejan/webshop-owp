namespace webshop_owp.Data.Base
{
    /// <summary>
    /// Represents the base structure for all database entities, ensuring a standardized primary key definition.
    /// </summary>
    public interface IEntityBase
    {
        int Id { get; set; }
    }
}
