using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;

namespace Abp.ZeroCore.SampleApp.Application.Blogs
{
    [AutoMap(typeof(Blog))]
    public class BlogDto : EntityDto
    {
        public string Name { get; set; }
    }
}
