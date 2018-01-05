using System;

namespace Abp.Auditing.Scopes
{
    public interface IActiveAuditScope
    {
        /// <summary>
        /// This event is raised when this audit scope is successfully completed.
        /// </summary>
        event EventHandler<AuditScopeEventArgs> Completed;

        /// <summary>
        /// This event is raised when this audit scope is disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Is this audit scope disposed?
        /// </summary>
        bool IsDisposed { get; }
    }
}
