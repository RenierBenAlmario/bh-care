using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Barangay.Data
{
    /// <summary>
    /// Ensures that every SQL Server session opened by Entity Framework Core has QUOTED_IDENTIFIER ON.
    /// This prevents errors related to indexed views, computed columns, or filtered indexes that require the option.
    /// </summary>
    public class QuotedIdentifierConnectionInterceptor : DbConnectionInterceptor
    {
        private const string EnableQuotedIdentifierSql = "SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; SET CONCAT_NULL_YIELDS_NULL ON; SET ARITHABORT ON;";

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            using var command = connection.CreateCommand();
            command.CommandText = EnableQuotedIdentifierSql;
            command.ExecuteNonQuery();
            base.ConnectionOpened(connection, eventData);
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = EnableQuotedIdentifierSql;
            await command.ExecuteNonQueryAsync(cancellationToken);
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }
    }
}
