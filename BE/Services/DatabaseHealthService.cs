using System.Data;
using System.Threading.Tasks;
using BE.Constants;
using BE.Data;
using Microsoft.EntityFrameworkCore;

namespace BE.Services;

public class DatabaseHealthService
{
    private readonly ApplicationDbContext _context;

    public DatabaseHealthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetDatabaseVersionAsync()
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State == ConnectionState.Closed;

        try
        {
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = DatabaseConstants.DatabaseHealthQuery;
            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }
        finally
        {
            if (shouldClose && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }
}
