using System.Threading.Tasks;
using Abp.Authorization;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;
using System.Linq;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.EntityFramework;

namespace Abp.Zero.Users
{
    public class UserManager_Permission_Tests : AbpZeroTestBase
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<UserPermission> _permissionRepo;
        private readonly UserManager _userManager;
        private readonly IRepository<User, long> _userRepo;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager_Permission_Tests()
        {
            _permissionChecker = Resolve<IPermissionChecker>();
            _permissionManager = Resolve<IPermissionManager>();
            _permissionRepo = Resolve<IRepository<UserPermission>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _userManager = Resolve<UserManager>();
            _userRepo = Resolve<IRepository<User, long>>();
        }

        [Fact]
        public async Task Should_Check_IsGranted_Correctly_When_Logged_In_As_Host_Then_Switched_To_Tenant()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            var defaultTenantId = 1;
            var user = UsingDbContext(defaultTenantId, (context) =>
            {
                return context.Users.Single(f => f.TenantId == defaultTenantId && f.UserName == AbpUserBase.AdminUserName);
            });

            await _userManager.GrantPermissionAsync(user, _permissionManager.GetPermission(AppPermissions.TestPermission));

            var isGranted = await _permissionChecker.IsGrantedAsync(user.ToUserIdentifier(), AppPermissions.TestPermission);
            isGranted.ShouldBe(true);

            // Simulate background jobs
            LoginAsHostAdmin();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(user.TenantId))
                {
                    isGranted = await _permissionChecker.IsGrantedAsync(user.ToUserIdentifier(), AppPermissions.TestPermission);
                    isGranted.ShouldBe(true);
                }
            }
        }

        [Fact]
        public void Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                _permissionRepo.Insert(new UserPermission
                {
                    UserId = 2,
                    Permission = "admin"
                });

                uow.Complete();
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                var perm = _permissionRepo.GetAll().First(p => p.UserId == 2 && p.Permission == "admin");

                _permissionRepo.Delete(perm);  // let's say perm.Id == 3

                var permissionsBefore = _permissionRepo.GetAll().ToList();  //permission(3) exists (yes)
                permissionsBefore.ShouldContain(p => p.Id == perm.Id);      //permission(3) exists
                var userBefore = _userRepo.GetAllIncluding(u => u.Permissions).First(u => u.Id == 2);  //permission(3) doesn't exist in userBefore.Permissions (yes)
                userBefore.Permissions.ShouldNotContain(p => p.Id == perm.Id);                         //permission(3) doesn't exist in userBefore.Permissions

                _unitOfWorkManager.Current.SaveChanges();

                var permissionsAfter = _permissionRepo.GetAll().ToList();  //permission(3) disappears (yes)
                permissionsAfter.ShouldNotContain(p => p.Id == perm.Id);   //permission(3) disappears
                var userAfter = _userRepo.GetAllIncluding(u => u.Permissions).First(u => u.Id == 2);  //permission(3) appears again in userAfter.Permissions (no)
                userAfter.Permissions.ShouldNotContain(p => p.Id == perm.Id);                         //permission(3) doesn't exist in userAfter.Permissions

                uow.Complete();
            };
        }
    }
}