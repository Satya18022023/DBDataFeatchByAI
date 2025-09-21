
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AIDBDataFeatch.DataAccessLayer.GetDataByQuery;
    public class GetDataByQuery(DBContext context) : IGetDataByQuery
    {
        public async Task GetDatafromDB(string query)
        {
                //using var conn = context.Database.GetDbConnection();

                using var conn = new SqlConnection(context.Database.GetConnectionString());

                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = query;

                using var reader = await cmd.ExecuteReaderAsync();
                
                bool headerPrinted = false;
                while (await reader.ReadAsync())
                {
                    if (!headerPrinted)
                    {
                        // Print column headers
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader.GetName(i),-20}");  // Left-align, 20-character wide
                        }
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 20 * reader.FieldCount)); // Divider line
                        headerPrinted = true;
                    }

                    // Print row values
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetValue(i),-20}");
                    }
                    Console.WriteLine();
                }

            
        }
    }
