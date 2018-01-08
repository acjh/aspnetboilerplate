using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.EntityHistory.Auditing
{
    public class EntityChangeSetUpdateJob : BackgroundJob<EntityChangeSetUpdateJobArgs>, ITransientDependency
    {
        private readonly IRepository<EntityChangeSet, long> _changeSetRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EntityChangeSetUpdateJob(
            IRepository<EntityChangeSet, long> changeSetRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _changeSetRepository = changeSetRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public override void Execute(EntityChangeSetUpdateJobArgs args)
        {
            EntityChangeSet changeSet;

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                changeSet = _changeSetRepository.FirstOrDefault(args.EntityChangeSetId);
            }

            if (changeSet == null)
            {
                return;
            }

            changeSet.AuditLogId = args.AuditLogId;

            _changeSetRepository.Update(changeSet);
        }
    }
}
