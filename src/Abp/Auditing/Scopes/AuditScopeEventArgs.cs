using System;

namespace Abp.Auditing.Scopes
{
    public class AuditScopeEventArgs : EventArgs
    {
        public long AuditLogId { get; }

        public AuditScopeEventArgs(long auditLogId)
        {
            AuditLogId = auditLogId;
        }
    }
}
