namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// Used to get/set current <see cref="IAuditScope"/>. 
    /// </summary>
    public interface ICurrentAuditScopeProvider
    {
        /// <summary>
        /// Gets/sets current <see cref="IAuditScope"/>.
        /// Setting to null returns back to outer audit scope where possible.
        /// </summary>
        IAuditScope Current { get; set; }
    }
}
