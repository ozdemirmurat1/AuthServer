using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data
{
    // Üyelik ile de ilgili işlem yapacağımız için IdentityDbContext i kullandık. DbContext yerine
    // IdentityDbContext Role , Claim ve Token ları tutar . aşağıdaki string değeri Id'nin string olmasını sağlar
    public class AppDbContext:IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Projede IEntityTypeConfiguration u implemente eden class ları bul
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(builder);
        }
    }
}
