using ChatAppServer.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebApi.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) :  base(dbContextOptions) 
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}
