using Abp.BackgroundJobs;
using Abp.EntityHistory.Auditing;
using Abp.ZeroCore.SampleApp.Application.Blogs;
using Castle.MicroKernel.Registration;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace Abp.Zero.EntityHistory
{
    public class EntityHistory_Auditing_Tests : AbpZeroTestBase
    {
        private readonly IBlogAppService _blogAppService;

        private IBackgroundJobManager _backgroundJobManager;

        public EntityHistory_Auditing_Tests()
        {
            _blogAppService = Resolve<IBlogAppService>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            LocalIocManager.IocContainer.Register(
                Component.For<IBackgroundJobManager>().Instance(_backgroundJobManager).LifestyleSingleton()
                );
        }

        [Fact]
        public async Task Should_Enqueue_Update_Job()
        {
            //Arrange
            var input = new BlogDto { Name = "Blog" };

            //Act
            await _blogAppService.Create(input);

            //Assert
            await _backgroundJobManager.Received()
                .EnqueueAsync<EntityChangeSetUpdateJob, EntityChangeSetUpdateJobArgs>(
                    Arg.Is<EntityChangeSetUpdateJobArgs>(
                        a => a.AuditLogId != default(long) &&
                             a.EntityChangeSetId != default(long)
                        )
                );
        }
    }
}
