using AIDBDataFeatch.DataAccessLayer.GetSchemaDetails;

namespace AIDBDataFeatch.AIOperations
{
    public interface IAIOperations
    {
        Task<string> GetDataFromLLM(IList<SchemaColumn> schema, string querytext);
    }
}
