using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Concurrency;

internal static class DatabaseApplicationLock
{
    public static async Task AcquireAsync(
        FleetWiseDbContext context,
        string resource,
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "A database transaction is required before acquiring a concurrency lock.");
        }

        await context.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @lockResult int;
            EXEC @lockResult = sys.sp_getapplock
                @Resource = {resource},
                @LockMode = N'Exclusive',
                @LockOwner = N'Transaction',
                @LockTimeout = 15000;

            IF @lockResult < 0
                THROW 51000, 'Could not acquire the required concurrency lock.', 1;
            """, cancellationToken);
    }
}
