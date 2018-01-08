using Abp.Auditing.Scopes;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using System.Transactions;

namespace Abp.EntityHistory.Auditing
{
    public class EntityChangeSetUpdater :
        IEventHandler<EntityCreatingEventData<EntityChangeSet>>,
        ITransientDependency
    {
        private readonly IRepository<EntityChangeSet, long> _changeSetRepository;

        private readonly IAuditScopeManager _auditScopeManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EntityChangeSetUpdater(
            IRepository<EntityChangeSet, long> changeSetRepository,
            IAuditScopeManager auditScopeManager,
            IBackgroundJobManager backgroundJobManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _changeSetRepository = changeSetRepository;

            _auditScopeManager = auditScopeManager;
            _backgroundJobManager = backgroundJobManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void HandleEvent(EntityCreatingEventData<EntityChangeSet> eventData)
        {
            var auditScope = _auditScopeManager.Current;
            if (auditScope == null)
            {
                return;
            }
            
            auditScope.Completed += (sender, args) =>
            {
                _backgroundJobManager.Enqueue<EntityChangeSetUpdateJob, EntityChangeSetUpdateJobArgs>(new EntityChangeSetUpdateJobArgs
                {
                    AuditLogId = args.AuditLogId,
                    EntityChangeSetId = eventData.Entity.Id
                });
            };
        }
    }
}
