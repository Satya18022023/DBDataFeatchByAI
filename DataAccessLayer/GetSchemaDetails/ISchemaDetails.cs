namespace AIDBDataFeatch.DataAccessLayer.GetSchemaDetails
{
    public interface ISchemaDetails
    {
        Task<IList<SchemaColumn>> GetSchema();
    }
}
