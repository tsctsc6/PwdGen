using PwdGen.Models;
using SQLite;
using System.Diagnostics.CodeAnalysis;

namespace PwdGen.Services;

public class PwdGenDbService
{
    public static string DatabasePath => Path.Combine(AppContext.BaseDirectory, "PwdGen.db");

    public const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create;

    public SQLiteAsyncConnection? Database { get; private set; } = null;

    [MemberNotNull(nameof(Database))]
    public async ValueTask InitAsync()
    {
        if (Database is not null) return;
        Database = new SQLiteAsyncConnection(DatabasePath, Flags);
        await Database.CreateTableAsync<AcctData>();
    }

    public async ValueTask CloseAsync()
    {
        if (Database is null) return;
        await Database.CloseAsync();
        Database = null;
    }

    public async Task<int> InsertAsync<T>(T item)
    {
        await InitAsync();
        return await Database.InsertAsync(item);
    }

    public async Task<int> DeleteAsync<T>(T item)
    {
        await InitAsync();
        return await Database.DeleteAsync(item);
    }

    public async Task<int> UpdateAsync<T>(T item)
    {
        await InitAsync();
        return await Database.UpdateAsync(item);
    }

    public async ValueTask<AsyncTableQuery<T>> GetAllAsync<T>() where T : new()
    {
        await InitAsync();
        return Database.Table<T>();
    }
}
