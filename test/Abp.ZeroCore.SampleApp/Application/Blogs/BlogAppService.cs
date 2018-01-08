using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;

namespace Abp.ZeroCore.SampleApp.Application.Blogs
{
    public class BlogAppService : AsyncCrudAppService<Blog, BlogDto>, IBlogAppService
    {
        public BlogAppService(IRepository<Blog> repository) 
            : base(repository)
        {

        }
    }
}
