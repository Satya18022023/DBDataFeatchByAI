using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AIDBDataFeatch.DataAccessLayer;
    public class DBContext : DbContext
    {
    public DBContext(DbContextOptions<DBContext> options)
                : base(options)
    {
    }
}


