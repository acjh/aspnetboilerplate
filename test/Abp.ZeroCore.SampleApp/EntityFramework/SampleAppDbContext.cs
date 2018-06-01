using Abp.Domain.Entities;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.EntityFramework
{
    public class UserPermission : Entity, ISoftDelete
    {
        public int UserId { get; set; }
        public string Permission { get; set; }
        public bool IsDeleted { get; set; }
    }

    //TODO: Re-enable when IdentityServer ready
    public class SampleAppDbContext : AbpZeroDbContext<Tenant, Role, User, SampleAppDbContext>, IAbpPersistedGrantDbContext
    {
        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserPermission> UserPermissionss { get; set; }

        public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
