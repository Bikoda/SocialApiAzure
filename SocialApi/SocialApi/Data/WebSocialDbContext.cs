using Microsoft.EntityFrameworkCore;
using SocialApi.Models.Domain;
using System.Collections.Generic;

namespace SocialApi.Data
{
    public class WebSocialDbContext : DbContext
    {
        public WebSocialDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Records> LogRecord { get; set; }
    }
}
