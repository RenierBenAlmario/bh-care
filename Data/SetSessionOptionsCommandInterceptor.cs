using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Barangay.Data
{
    /// <summary>
    /// Ensures that every executed command in a SQL Server session has the required SET options enabled
    /// (QUOTED_IDENTIFIER, ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT).
    /// Some indexed views, computed columns, and filtered indexes require these options.
    /// </summary>
    public class SetSessionOptionsCommandInterceptor : DbCommandInterceptor
    {
        private const string RequiredSetOptionsSql =
            "SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; SET CONCAT_NULL_YIELDS_NULL ON; SET ARITHABORT ON;";

        private static void EnsureSessionOptions(DbCommand command)
        {
            using var setCmd = command.Connection!.CreateCommand();
            setCmd.CommandText = RequiredSetOptionsSql;
            setCmd.Transaction = command.Transaction; // Assign the transaction
            setCmd.ExecuteNonQuery();
        }

        private static async Task EnsureSessionOptionsAsync(DbCommand command, CancellationToken cancellationToken)
        {
            using var setCmd = command.Connection!.CreateCommand();
            setCmd.CommandText = RequiredSetOptionsSql;
            setCmd.Transaction = command.Transaction; // Assign the transaction
            await setCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            EnsureSessionOptions(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            await EnsureSessionOptionsAsync(command, cancellationToken);
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            EnsureSessionOptions(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await EnsureSessionOptionsAsync(command, cancellationToken);
            return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            EnsureSessionOptions(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            await EnsureSessionOptionsAsync(command, cancellationToken);
            return await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
