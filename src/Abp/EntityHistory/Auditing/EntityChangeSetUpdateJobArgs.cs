using System;

namespace Abp.EntityHistory.Auditing
{
    [Serializable]
    public class EntityChangeSetUpdateJobArgs
    {
        public long AuditLogId { get; set; }

        public long EntityChangeSetId { get; set; }
    }
}
