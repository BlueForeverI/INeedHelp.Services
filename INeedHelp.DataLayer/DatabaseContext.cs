using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INeedHelp.Models;

namespace INeedHelp.DataLayer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("INeedHelpDatabase")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; } 
        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Coordinates> Coordinates { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Friends).WithMany()
                .Map(map =>
                {
                    map.ToTable("UsersUsers");
                    map.MapLeftKey("FirstUserId");
                    map.MapRightKey("SecondUserId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
