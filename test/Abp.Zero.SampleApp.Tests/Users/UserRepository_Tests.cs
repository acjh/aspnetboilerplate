using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Zero.SampleApp.EntityFramework;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using System.Linq;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserRepository_Tests : SampleAppTestBase
    {
        [Fact]
        public void Should_Insert_And_Retrieve_User()
        {
            var userRepository = LocalIocManager.Resolve<IRepository<User, long>>();

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@aspnetboilerplate.com").ShouldBe(null);

            userRepository.Insert(new User
            {
                TenantId = null,
                UserName = "admin",
                Name = "System",
                Surname = "Administrator",
                EmailAddress = "admin@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@aspnetboilerplate.com").ShouldNotBe(null);
        }

        [Fact]
        public void Test()
        {
            var _unitOfWorkManager = LocalIocManager.Resolve<IUnitOfWorkManager>();
            var _permissionRepo = LocalIocManager.Resolve<IRepository<UserPermission>>();
            var _userRepo = LocalIocManager.Resolve<IRepository<User, long>>();

            LocalIocManager.Resolve<IAbpSession>().Use(1, null);

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
