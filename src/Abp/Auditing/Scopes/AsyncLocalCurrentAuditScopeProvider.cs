using System.Threading;
using Abp.Dependency;
using Castle.Core;
using Castle.Core.Logging;

namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentAuditScopeProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class AsyncLocalCurrentAuditScopeProvider : ICurrentAuditScopeProvider, ITransientDependency
    {
        /// <inheritdoc />
        [DoNotWire]
        public IAuditScope Current
        {
            get { return GetCurrentAuditScope(); }
            set { SetCurrentAuditScope(value); }
        }

        public ILogger Logger { get; set; }

        private static readonly AsyncLocal<LocalAuditScopeWrapper> AsyncLocalAuditScope = new AsyncLocal<LocalAuditScopeWrapper>();

        public AsyncLocalCurrentAuditScopeProvider()
        {
            Logger = NullLogger.Instance;
        }

        private static IAuditScope GetCurrentAuditScope()
        {
            var auditScope = AsyncLocalAuditScope.Value?.AuditScope;
            if (auditScope == null)
            {
                return null;
            }

            if (auditScope.IsDisposed)
            {
                AsyncLocalAuditScope.Value = null;
                return null;
            }

            return auditScope;
        }

        private static void SetCurrentAuditScope(IAuditScope value)
        {
            lock (AsyncLocalAuditScope)
            {
                if (value == null)
                {
                    if (AsyncLocalAuditScope.Value == null)
                    {
                        return;
                    }

                    if (AsyncLocalAuditScope.Value.AuditScope?.Outer == null)
                    {
                        AsyncLocalAuditScope.Value.AuditScope = null;
                        AsyncLocalAuditScope.Value = null;
                        return;
                    }

                    AsyncLocalAuditScope.Value.AuditScope = AsyncLocalAuditScope.Value.AuditScope.Outer;
                }
                else
                {
                    if (AsyncLocalAuditScope.Value?.AuditScope == null)
                    {
                        if (AsyncLocalAuditScope.Value != null)
                        {
                            AsyncLocalAuditScope.Value.AuditScope = value;
                        }

                        AsyncLocalAuditScope.Value = new LocalAuditScopeWrapper(value);
                        return;
                    }

                    value.Outer = AsyncLocalAuditScope.Value.AuditScope;
                    AsyncLocalAuditScope.Value.AuditScope = value;
                }
            }
        }

        private class LocalAuditScopeWrapper
        {
            public IAuditScope AuditScope { get; set; }

            public LocalAuditScopeWrapper(IAuditScope auditScope)
            {
                AuditScope = auditScope;
            }
        }
    }
}