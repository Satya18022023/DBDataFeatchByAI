
using Microsoft.EntityFrameworkCore;

namespace AIDBDataFeatch.DataAccessLayer.GetSchemaDetails
{
    public class SchemaDetails(DBContext context) : ISchemaDetails
    {
        public async Task<IList<SchemaColumn>> GetSchema()
        {
                var results = new List<SchemaColumn>();

                var sql = @"
                         SELECT 
                            TABLE_SCHEMA,
                            TABLE_NAME,
                            COLUMN_NAME,
                            DATA_TYPE,
                            CHARACTER_MAXIMUM_LENGTH,
                            IS_NULLABLE
                        FROM INFORMATION_SCHEMA.COLUMNS
                        ORDER BY TABLE_NAME, ORDINAL_POSITION";

                var conn = context.Database.GetDbConnection();

                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new SchemaColumn
                    {
                        TableSchema = reader["TABLE_SCHEMA"]?.ToString(),
                        TableName = reader["TABLE_NAME"]?.ToString(),
                        ColumnName = reader["COLUMN_NAME"]?.ToString(),
                        DataType = reader["DATA_TYPE"]?.ToString(),
                        MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"] as int?,
                        IsNullable = reader["IS_NULLABLE"]?.ToString()
                    });
                }

                return results;
            }
        }
    }

