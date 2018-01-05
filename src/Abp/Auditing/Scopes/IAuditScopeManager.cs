using System;

namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// Audit scope manager.
    /// Used to begin and control an audit scope.
    /// </summary>
    public interface IAuditScopeManager
    {
        /// <summary>
        /// Gets currently active audit scope (or null if not exists).
        /// </summary>
        IActiveAuditScope Current { get; }

        /// <summary>
        /// Begins a new audit scope.
        /// </summary>
        /// <returns>A handle to be able to complete the audit scope</returns>
        IAuditScopeCompleteHandle Begin();
    }
}
