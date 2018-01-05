namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// Defines an audit scope.
    /// This interface is internally used by ABP.
    /// Use <see cref="IAuditScopeManager.Begin()"/> to start a new audit scope.
    /// </summary>
    public interface IAuditScope : IActiveAuditScope, IAuditScopeCompleteHandle
    {
        /// <summary>
        /// Unique id of this audit scope.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Reference to the outer audit scope if exists.
        /// </summary>
        IAuditScope Outer { get; set; }

        /// <summary>
        /// Begins the audit scope.
        /// </summary>
        void Begin();
    }
}
