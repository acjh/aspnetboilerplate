using System;

namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// Used to complete an audit scope.
    /// This interface cannot be injected or directly used.
    /// Use <see cref="IAuditScopeManager"/> instead.
    /// </summary>
    public interface IAuditScopeCompleteHandle : IDisposable
    {
        /// <summary>
        /// Completes this audit scope.
        /// </summary>
        void Complete(long auditLogId);
    }
}
