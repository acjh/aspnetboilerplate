using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.Auditing.Scopes;
using Abp.Domain.Entities;
using Abp.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditScopeManager _auditScopeManager;

        public AuditingInterceptor(
            IAuditingHelper auditingHelper,
            IAuditScopeManager auditScopeManager)
        {
            _auditingHelper = auditingHelper;
            _auditScopeManager = auditScopeManager;
        }

        public void Intercept(IInvocation invocation)
        {
            if (AbpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, AbpCrossCuttingConcerns.Auditing))
            {
                invocation.Proceed();
                return;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            if (invocation.Method.IsAsync())
            {
                PerformAsyncAuditing(invocation, auditInfo);
            }
            else
            {
                PerformSyncAuditing(invocation, auditInfo);
            }
        }

        private void PerformSyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var auditScope = _auditScopeManager.Begin())
            {
                try
                {
                    invocation.Proceed();
                }
                catch (Exception ex)
                {
                    auditInfo.Exception = ex;
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                    var auditLog = _auditingHelper.Save(auditInfo);
                    auditScope.Complete(auditLog?.Id);
                }
            }
        }

        private void PerformAsyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            using (var auditScope = _auditScopeManager.Begin())
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType == typeof(Task))
                {
                    invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithFinally(
                        (Task)invocation.ReturnValue,
                        exception =>
                        {
                            var auditLog = SaveAuditInfo(auditInfo, stopwatch, exception);
                            auditScope.Complete(auditLog?.Id);
                        });
                }
                else //Task<TResult>
                {
                    invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithFinallyAndGetResult(
                        invocation.Method.ReturnType.GenericTypeArguments[0],
                        invocation.ReturnValue,
                        exception =>
                        {
                            var auditLog = SaveAuditInfo(auditInfo, stopwatch, exception);
                            auditScope.Complete(auditLog?.Id);
                        });
                }
            }
        }

        [CanBeNull]
        private IEntity<long> SaveAuditInfo(AuditInfo auditInfo, Stopwatch stopwatch, Exception exception)
        {
            stopwatch.Stop();
            auditInfo.Exception = exception;
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

            return _auditingHelper.Save(auditInfo);
        }
    }
}