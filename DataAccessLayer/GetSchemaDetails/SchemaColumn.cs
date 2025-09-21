namespace AIDBDataFeatch.DataAccessLayer.GetSchemaDetails
{
    public class SchemaColumn
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public string IsNullable { get; set; }
    }
}
