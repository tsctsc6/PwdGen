using System.Data;
using PwdGen.Models;
using Microsoft.Data.Sqlite;
using RustSharp;

namespace PwdGen.Services;

public class DbService
{
    public static string DatabasePath => Path.Combine(AppContext.BaseDirectory, "PwdGen.db");
    private static string ConnectionString => $"Data Source={DatabasePath}";
    
    private SqliteConnection? Connection { get; set; } = null;
    
    public async ValueTask<Result<int, string>> InitAsync()
    {
        if (Connection is not null && Connection.State == ConnectionState.Open)
            return Result.Ok(0);
        
        Connection = new SqliteConnection(ConnectionString);
        await Connection.OpenAsync();
        switch (Connection.State)
        {
            case ConnectionState.Closed:
                return Result.Err("Connection is closed.");
            case ConnectionState.Open:
                break;
            case ConnectionState.Broken:
                return Result.Err($"Data Source is Broken.");
            default:
                return Result.Err($"Connection.State is {Connection.State}.");
        }
        
        try
        {
            var isTableExits = false;
            await using (var cmd = new SqliteCommand(
                             """
                             PRAGMA table_info("AcctData");
                             """
                             , Connection))
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    isTableExits = reader.GetBoolean(0);
                }
            }
            if (isTableExits) return Result.Ok(0);
            await using (var cmd = new SqliteCommand(
                             """
                             CREATE TABLE "AcctData" (
                             "Id" integer primary key autoincrement not null ,
                             "UserName" varchar(50) not null ,
                             "Platform" varchar(50) not null ,
                             "Remark" varchar(100) not null ,
                             "SkipCount" integer not null ,
                             "UseUpLetter" integer not null ,
                             "UseLowLetter" integer not null ,
                             "UseNumber" integer not null ,
                             "UseSpChar" integer not null ,
                             "PwdLen" integer not null ,
                             "DateModified" integer not null );
                             """, Connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            await using (var cmd = new SqliteCommand(
                             """
                             CREATE INDEX "AcctData_Id"
                             ON "AcctData" (Id);
                             """, Connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            await using (var cmd = new SqliteCommand(
                             """
                             CREATE INDEX "AcctData_Platform"
                             ON "AcctData" (Platform);
                             """, Connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            await using (var cmd = new SqliteCommand(
                             """
                             CREATE INDEX "AcctData_UserName"
                             ON "AcctData" (UserName);
                             """, Connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
        return Result.Ok(0);
    }

    public async ValueTask<Result<int, string>> CloseAsync()
    {
        if (Connection is null) return Result.Ok(0);
        await Connection.CloseAsync();
        switch (Connection.State)
        {
            case ConnectionState.Closed:
                return Result.Ok(0);
            case ConnectionState.Open:
                return Result.Err("Connection is open.");
            case ConnectionState.Broken:
                return Result.Err($"Data Source is Broken.");
            default:
                return Result.Err($"Connection.State is {Connection.State}.");
        }
    }

    public async Task<Result<int, string>> InsertAcctDataAsync(AcctData item)
    {
        var initResult = await InitAsync();
        if (initResult.IsErr)
            return initResult.MapErr(err => "Init Database Failed." + Environment.NewLine + err);
        try
        {
            await using var cmd = new SqliteCommand(
                """
                INSERT INTO "AcctData"(
                    "UserName",
                    "Platform",
                    "Remark",
                    "SkipCount",
                    "UseUpLetter",
                    "UseLowLetter",
                    "UseNumber",
                    "UseSpChar",
                    "PwdLen",
                    "DateModified"
                ) VALUES (
                          @UserName,
                          @Platform,
                          @Remark,
                          @SkipCount,
                          @UseUpLetter,
                          @UseLowLetter,
                          @UseNumber,
                          @UseSpChar,
                          @PwdLen,
                          @DateModified);
                """, Connection);
            cmd.Parameters.AddWithValue("@UserName", item.UserName);
            cmd.Parameters.AddWithValue("@Platform", item.Platform);
            cmd.Parameters.AddWithValue("@Remark", item.Remark);
            cmd.Parameters.AddWithValue("@SkipCount", item.SkipCount);
            cmd.Parameters.AddWithValue("@UseUpLetter", item.UseUpLetter);
            cmd.Parameters.AddWithValue("@UseLowLetter", item.UseLowLetter);
            cmd.Parameters.AddWithValue("@UseNumber", item.UseNumber);
            cmd.Parameters.AddWithValue("@UseSpChar", item.UseSpChar);
            cmd.Parameters.AddWithValue("@PwdLen", item.PwdLen);
            cmd.Parameters.AddWithValue("@DateModified", item.DateModified);
            return Result.Ok(await cmd.ExecuteNonQueryAsync());
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
    }

    public async Task<Result<int, string>> DeleteAcctDataAsync(AcctData item)
    {
        var initResult = await InitAsync();
        if (initResult.IsErr)
            return initResult.MapErr(err => "Init Database Failed." + Environment.NewLine + err);
        try
        {
            await using var cmd = new SqliteCommand(
                """DELETE FROM "AcctData" WHERE "Id" = @Id;""", Connection);
            cmd.Parameters.AddWithValue("@Id", item.Id);
            return Result.Ok(await cmd.ExecuteNonQueryAsync());
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
    }

    public async Task<Result<int, string>> UpdateAcctDataAsync(AcctData item)
    {
        var initResult = await InitAsync();
        if (initResult.IsErr)
            return initResult.MapErr(err => "Init Database Failed." + Environment.NewLine + err);
        try
        {
            await using var cmd = new SqliteCommand(
                """
                UPDATE "AcctData" SET
                "UserName" = @UserName,
                "Platform" = @Platform,
                "Remark" = @Remark,
                "SkipCount" = @SkipCount,
                "UseUpLetter" = @UseUpLetter,
                "UseLowLetter" = @UseLowLetter,
                "UseNumber" = @UseNumber,
                "UseSpChar" = @UseSpChar,
                "PwdLen" = @PwdLen,
                "DateModified" = @DateModified
                WHERE "Id" = @Id;
                """, Connection);
            cmd.Parameters.AddWithValue("@Id", item.Id);
            cmd.Parameters.AddWithValue("@UserName", item.UserName);
            cmd.Parameters.AddWithValue("@Platform", item.Platform);
            cmd.Parameters.AddWithValue("@Remark", item.Remark);
            cmd.Parameters.AddWithValue("@SkipCount", item.SkipCount);
            cmd.Parameters.AddWithValue("@UseUpLetter", item.UseUpLetter);
            cmd.Parameters.AddWithValue("@UseLowLetter", item.UseLowLetter);
            cmd.Parameters.AddWithValue("@UseNumber", item.UseNumber);
            cmd.Parameters.AddWithValue("@UseSpChar", item.UseSpChar);
            cmd.Parameters.AddWithValue("@PwdLen", item.PwdLen);
            cmd.Parameters.AddWithValue("@DateModified", item.DateModified);
            return Result.Ok(await cmd.ExecuteNonQueryAsync());
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
    }

    public async Task<Result<(AcctData[] Result, int TotolCount), string>> GetAllAcctDataAsync(string searchString, int start, int count)
    {
        var initResult = await InitAsync();
        if (initResult is ErrResult<int, string> errResult)
            return Result.Err("Init Database Failed." + Environment.NewLine + errResult.Value);
        SqliteDataReader? reader = null;
        try
        {
            var baseString =
                """
                SELECT
                "Id",
                "UserName",
                "Platform",
                "Remark",
                "SkipCount",
                "UseUpLetter",
                "UseLowLetter",
                "UseNumber",
                "UseSpChar",
                "PwdLen",
                "DateModified"
                from "AcctData"
                """;
            if (string.IsNullOrEmpty(searchString))
            {
                await using var cmd = new SqliteCommand(
                    $"""
                     {baseString}
                     OFFSET @Offset LIMIT @Count
                     """,
                    Connection);
                cmd.Parameters.AddWithValue("@Offset", start);
                cmd.Parameters.AddWithValue("@Count", count);
                reader = await cmd.ExecuteReaderAsync();
            }
            else
            {
                await using var cmd = new SqliteCommand(
                    $"""
                     {baseString}
                     WHERE "UserName" LIKE @SearchString
                     OR "Platform" LIKE @SearchString
                     OFFSET @Offset LIMIT @Count
                     """,
                    Connection);
                cmd.Parameters.AddWithValue("@Offset", start);
                cmd.Parameters.AddWithValue("@Count", count);
                cmd.Parameters.AddWithValue("@SearchString", searchString);
                reader = await cmd.ExecuteReaderAsync();
            }

            var totolCount = 0;
            List<AcctData> list = [];
            while (await reader.ReadAsync())
            {
                var acctData = new AcctData
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Platform = reader.GetString(2),
                    Remark = reader.GetString(3),
                    SkipCount = reader.GetInt32(4),
                    UseUpLetter = reader.GetBoolean(5),
                    UseLowLetter = reader.GetBoolean(6),
                    UseNumber = reader.GetBoolean(7),
                    UseSpChar = reader.GetBoolean(8),
                    PwdLen = reader.GetInt32(9),
                    DateModified = reader.GetInt64(10)
                };
                list.Add(acctData);
            }
            return Result.Ok((list.ToArray(), totolCount));
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
        finally
        {
            if (reader != null) await reader.DisposeAsync();
        }
    }
}
