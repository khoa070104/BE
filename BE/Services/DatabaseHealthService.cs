using System.Threading.Tasks;
using Npgsql;

namespace BE.Services;

public class DatabaseHealthService
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseHealthService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<string> GetDatabaseVersionAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = BE.Constants.DatabaseConstants.DatabaseHealthQuery;
        var result = await command.ExecuteScalarAsync();
        return result?.ToString() ?? string.Empty;
    }
}
