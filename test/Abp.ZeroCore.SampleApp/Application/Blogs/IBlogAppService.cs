using Abp.Application.Services;

namespace Abp.ZeroCore.SampleApp.Application.Blogs
{
    public interface IBlogAppService : IAsyncCrudAppService<BlogDto>
    {
        
    }
}
