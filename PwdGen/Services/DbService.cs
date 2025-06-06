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
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        SqliteCommand tuplesCmd = null;
        SqliteCommand countCmd = null;
        SqliteDataReader tuplesReader = null;
        SqliteDataReader countReader = null;
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        try
        {
            var baseTuplesString =
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
            var baseCountString =
                """
                SELECT COUNT(*) from "AcctData"
                """;
            if (string.IsNullOrEmpty(searchString))
            {
                tuplesCmd = new SqliteCommand(
                    $"""
                     {baseTuplesString}
                     LIMIT @Count OFFSET @Offset;
                     """, Connection);
                tuplesCmd.Parameters.AddWithValue("@Offset", start);
                tuplesCmd.Parameters.AddWithValue("@Count", count);
                countCmd = new SqliteCommand(
                    $"""
                     {baseCountString}
                     """, Connection);
            }
            else
            {
                tuplesCmd = new SqliteCommand(
                    $"""
                     {baseTuplesString}
                     WHERE "UserName" LIKE @SearchString
                     OR "Platform" LIKE @SearchString
                     LIMIT @Count OFFSET @Offset;
                     """,
                    Connection);
                tuplesCmd.Parameters.AddWithValue("@Offset", start);
                tuplesCmd.Parameters.AddWithValue("@Count", count);
                tuplesCmd.Parameters.AddWithValue("@SearchString", searchString);
                countCmd = new SqliteCommand(
                    $"""
                     {baseCountString}
                     WHERE "UserName" LIKE @SearchString
                     OR "Platform" LIKE @SearchString
                     """, Connection);
            }

            tuplesReader = await tuplesCmd.ExecuteReaderAsync();
            List<AcctData> list = [];
            while (await tuplesReader.ReadAsync())
            {
                var acctData = new AcctData
                {
                    Id = tuplesReader.GetInt32(0),
                    UserName = tuplesReader.GetString(1),
                    Platform = tuplesReader.GetString(2),
                    Remark = tuplesReader.GetString(3),
                    SkipCount = tuplesReader.GetInt32(4),
                    UseUpLetter = tuplesReader.GetBoolean(5),
                    UseLowLetter = tuplesReader.GetBoolean(6),
                    UseNumber = tuplesReader.GetBoolean(7),
                    UseSpChar = tuplesReader.GetBoolean(8),
                    PwdLen = tuplesReader.GetInt32(9),
                    DateModified = tuplesReader.GetInt64(10)
                };
                list.Add(acctData);
            }
            countReader = await countCmd.ExecuteReaderAsync();
            var totalCount = 0;
            while (await countReader.ReadAsync())
            {
                totalCount += countReader.GetInt32(0);
            }
            return Result.Ok((list.ToArray(), totolCount: totalCount));
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
        finally
        {
            if (tuplesReader != null) await tuplesReader.DisposeAsync();
            if (tuplesCmd != null) await tuplesCmd.DisposeAsync();
            if (countReader != null) await countReader.DisposeAsync();
            if (countCmd != null) await countCmd.DisposeAsync();
        }
    }
}
