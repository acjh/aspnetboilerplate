using Abp.Domain.Entities;
using Abp.Threading;
using System.Threading.Tasks;

namespace Abp.Auditing
{
    public interface IDatabaseAuditingStore : IAuditingStore
    {
        /// <summary>
        /// Should save audits to a persistent store.
        /// </summary>
        /// <param name="auditInfo">Audit informations</param>
        /// <returns>The audit entity</returns>
        Task<IEntity<long>> SaveAndGetEntityAsync(AuditInfo auditInfo);
    }

    public static class DatabaseAuditingStoreExtensions
    {
        public static IEntity<long> SaveAndGetEntity(this IDatabaseAuditingStore auditingStore, AuditInfo auditInfo)
        {
            return AsyncHelper.RunSync(() => auditingStore.SaveAndGetEntityAsync(auditInfo));
        }
    }
}
