using System;
using Abp.Dependency;
using Abp.Extensions;
using Castle.Core;

namespace Abp.Auditing.Scopes
{
    public class AuditScope : IAuditScope, ITransientDependency
    {
        public string Id { get; }

        [DoNotWire]
        public IAuditScope Outer { get; set; }

        /// <inheritdoc/>
        public event EventHandler<AuditScopeEventArgs> Completed;

        /// <inheritdoc/>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value that indicates if this audit scope is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AuditScope()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        /// <inheritdoc/>
        public void Begin()
        {
        }

        /// <inheritdoc/>
        public void Complete(long auditLogId)
        {
            Completed.InvokeSafely(this, new AuditScopeEventArgs(auditLogId));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            IsDisposed = true;
            Disposed.InvokeSafely(this);
        }
    }
}
