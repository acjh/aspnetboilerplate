using Abp.Dependency;

namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// Audit scope manager.
    /// Used to begin and control an audit scope.
    /// </summary>
    internal class AuditScopeManager : IAuditScopeManager, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ICurrentAuditScopeProvider _currentAuditScopeProvider;

        public IActiveAuditScope Current
        {
            get { return _currentAuditScopeProvider.Current; }
        }

        public AuditScopeManager(
            IIocResolver iocResolver,
            ICurrentAuditScopeProvider currentAuditScopeProvider)
        {
            _iocResolver = iocResolver;
            _currentAuditScopeProvider = currentAuditScopeProvider;
        }

        public IAuditScopeCompleteHandle Begin()
        {
            var outerAuditScope = _currentAuditScopeProvider.Current;

            if (outerAuditScope != null)
            {
                return new InnerAuditScopeCompleteHandle();
            }

            var auditScope = _iocResolver.Resolve<IAuditScope>();

            auditScope.Completed += (sender, args) =>
            {
                _currentAuditScopeProvider.Current = null;
            };

            auditScope.Disposed += (sender, args) =>
            {
                _iocResolver.Release(auditScope);
            };

            auditScope.Begin();

            _currentAuditScopeProvider.Current = auditScope;

            return auditScope;
        }
    }
}
