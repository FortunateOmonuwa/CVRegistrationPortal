global using Microsoft.EntityFrameworkCore;
global using CVRegistrationPortal.Models;

namespace CVRegistrationPortal.DataContext
{
    public class CVContext :DbContext
    {
        public CVContext(DbContextOptions<CVContext> options): base(options) 
        {
            
        }


        public DbSet<User>  Users { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}
