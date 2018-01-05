using Abp.Domain.Entities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Auditing
{
    public interface IAuditingHelper
    {
        bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false);

        AuditInfo CreateAuditInfo(Type type, MethodInfo method, object[] arguments);

        AuditInfo CreateAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments);

        [CanBeNull]
        IEntity<long> Save(AuditInfo auditInfo);

        [CanBeNull]
        Task<IEntity<long>> SaveAsync(AuditInfo auditInfo);
    }
}