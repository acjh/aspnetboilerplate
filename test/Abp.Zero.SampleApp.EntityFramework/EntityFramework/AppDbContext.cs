using System.Data.Common;
using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Zero.EntityFramework;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;

namespace Abp.Zero.SampleApp.EntityFramework
{
    public class UserPermission : Entity, ISoftDelete
    {
        public int UserId { get; set; }
        public string Permission { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class AppDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        public DbSet<UserPermission> UserPermissionss { get; set; }

        public AppDbContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {

        }
    }
}