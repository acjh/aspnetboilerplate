using System.Threading.Tasks;

namespace Abp.Auditing.Scopes
{
    /// <summary>
    /// This handle is used for inner audit scopes.
    /// A inner audit scope actually uses outer audit scope
    /// and has no effect on <see cref="IAuditScopeCompleteHandle.Complete"/> call.
    /// But if it's not called, an exception is thrown when the scope is disposed.
    /// </summary>
    internal class InnerAuditScopeCompleteHandle : IAuditScopeCompleteHandle
    {
        public const string DidNotCallCompleteMethodExceptionMessage = "Did not call Complete method of an audit.";

        private volatile bool _isCompleteCalled;
        private volatile bool _isDisposed;

        public void Complete(long auditLogId)
        {
            _isCompleteCalled = true;
        }

        public Task CompleteAsync(long auditLogId)
        {
            _isCompleteCalled = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            throw new AbpException(DidNotCallCompleteMethodExceptionMessage);
        }
    }
}
