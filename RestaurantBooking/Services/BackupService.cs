using System.Diagnostics;
using System.Text;
using RestaurantBooking.Config;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Thực thi mongodump/mongorestore, mongoexport/mongoimport và Memurai BGSAVE + sao lưu RDB/AOF từ FrmAdmin.
/// </summary>
public class BackupService
{
    public static readonly string[] Collections =
    [
        "users",
        "restaurants",
        "menu_items",
        "bookings_orders",
        "reviews"
    ];

    public static string GetDefaultBackupRoot()
    {
        foreach (var seedPath in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            if (string.IsNullOrWhiteSpace(seedPath)) continue;
            var current = new DirectoryInfo(seedPath);
            while (current != null)
            {
                var slnPath = Path.Combine(current.FullName, "RestaurantBooking.sln");
                var dbBackupPath = Path.Combine(current.FullName, "db", "backup");
                if (File.Exists(slnPath) || Directory.Exists(Path.Combine(current.FullName, "db")))
                {
                    Directory.CreateDirectory(dbBackupPath);
                    return dbBackupPath;
                }
                current = current.Parent;
            }
        }

        var fallback = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RestaurantBookingBackup");
        Directory.CreateDirectory(fallback);
        return fallback;
    }

    public async Task<BackupOperationResult> RunMongoDumpAsync(string targetFolder, bool createTimestampSubfolder = true)
    {
        var rootDir = EnsureDirectory(targetFolder);
        var outDir = createTimestampSubfolder
            ? Path.Combine(rootDir, $"mongodump_{DateTime.Now:yyyyMMdd_HHmmss}")
            : rootDir;
        Directory.CreateDirectory(outDir);

        var args = new[]
        {
            $"--uri={DbContext.MongoConnectionString}",
            $"--db={DbContext.MongoDatabaseName}",
            $"--out={outDir}"
        };

        return await RunCliToolAsync("mongodump", args, outDir);
    }

    public async Task<BackupOperationResult> RunMongoRestoreAsync(string selectedFolder, bool dropBeforeRestore = true)
    {
        if (string.IsNullOrWhiteSpace(selectedFolder) || !Directory.Exists(selectedFolder))
            throw new DirectoryNotFoundException("Thư mục backup cần phục hồi không tồn tại.");

        var dbSubFolder = Path.Combine(selectedFolder, DbContext.MongoDatabaseName);
        var args = new List<string> { $"--uri={DbContext.MongoConnectionString}" };
        if (dropBeforeRestore)
            args.Add("--drop");

        if (Directory.Exists(dbSubFolder) && Directory.GetFiles(dbSubFolder, "*.bson").Length > 0)
        {
            args.Add($"--dir={selectedFolder}");
        }
        else if (Directory.GetFiles(selectedFolder, "*.bson").Length > 0)
        {
            args.Add($"--db={DbContext.MongoDatabaseName}");
            args.Add($"--dir={selectedFolder}");
        }
        else
        {
            throw new InvalidOperationException(
                $"Không tìm thấy file .bson trong '{selectedFolder}' hoặc '{dbSubFolder}'. " +
                "Hãy chọn thư mục đầu ra của mongodump (ví dụ mongodump_yyyyMMdd_HHmmss).");
        }

        return await RunCliToolAsync("mongorestore", args, selectedFolder);
    }

    public async Task<BackupOperationResult> ExportAllCollectionsToJsonAsync(
        string targetFolder,
        bool createTimestampSubfolder = true)
    {
        var rootDir = EnsureDirectory(targetFolder);
        var outDir = createTimestampSubfolder
            ? Path.Combine(rootDir, $"json_export_{DateTime.Now:yyyyMMdd_HHmmss}")
            : rootDir;
        Directory.CreateDirectory(outDir);

        var sbOut = new StringBuilder();
        var sbErr = new StringBuilder();
        var commands = new List<string>();
        bool allOk = true;
        int lastExit = 0;

        foreach (var col in Collections)
        {
            var outFile = Path.Combine(outDir, $"{col}.json");
            var res = await ExportCollectionToJsonAsync(col, outFile);
            commands.Add(res.CommandText);
            if (!string.IsNullOrWhiteSpace(res.OutputLog))
                sbOut.AppendLine($"[{col}] {res.OutputLog.Trim()}");
            if (!string.IsNullOrWhiteSpace(res.ErrorLog))
                sbErr.AppendLine($"[{col}] {res.ErrorLog.Trim()}");

            if (!res.Success)
            {
                allOk = false;
                lastExit = res.ExitCode;
            }
        }

        return new BackupOperationResult
        {
            Success = allOk,
            ExitCode = allOk ? 0 : lastExit,
            CommandText = string.Join(Environment.NewLine, commands),
            OutputLog = sbOut.ToString().Trim(),
            ErrorLog = sbErr.ToString().Trim(),
            TargetPath = outDir
        };
    }

    public async Task<BackupOperationResult> ExportCollectionToJsonAsync(string collectionName, string outputFilePath)
    {
        if (!Collections.Contains(collectionName))
            throw new ArgumentException($"Collection '{collectionName}' không hợp lệ.");

        var parent = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(parent))
            Directory.CreateDirectory(parent);

        var args = new[]
        {
            $"--uri={DbContext.MongoConnectionString}",
            $"--db={DbContext.MongoDatabaseName}",
            $"--collection={collectionName}",
            $"--out={outputFilePath}",
            "--jsonArray",
            "--pretty"
        };

        return await RunCliToolAsync("mongoexport", args, outputFilePath);
    }

    public async Task<BackupOperationResult> ImportCollectionFromJsonAsync(
        string collectionName,
        string jsonFilePath,
        bool dropBeforeImport = false)
    {
        if (!Collections.Contains(collectionName))
            throw new ArgumentException($"Collection '{collectionName}' không hợp lệ.");
        if (string.IsNullOrWhiteSpace(jsonFilePath) || !File.Exists(jsonFilePath))
            throw new FileNotFoundException("Không tìm thấy tập tin JSON cần nhập dữ liệu.", jsonFilePath);
        if (!string.Equals(Path.GetExtension(jsonFilePath), ".json", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Tập tin nhập dữ liệu phải có phần mở rộng .json.");
        if (new FileInfo(jsonFilePath).Length == 0)
            throw new InvalidOperationException("Tập tin JSON đang trống, không có dữ liệu để nhập.");

        var args = new List<string>
        {
            $"--uri={DbContext.MongoConnectionString}",
            $"--db={DbContext.MongoDatabaseName}",
            $"--collection={collectionName}",
            $"--file={jsonFilePath}",
            "--jsonArray"
        };
        if (dropBeforeImport)
            args.Add("--drop");

        return await RunCliToolAsync("mongoimport", args, jsonFilePath);
    }

    public async Task<MemuraiPersistenceStatus> GetMemuraiPersistenceStatusAsync()
    {
        using var adminMux = await ConnectAdminRedisAsync();
        return await ReadPersistenceStatusAsync(adminMux.GetDatabase());
    }

    public async Task<MemuraiPersistenceStatus> TriggerMemuraiBgSaveAsync()
    {
        using var adminMux = await ConnectAdminRedisAsync();
        var redis = adminMux.GetDatabase();
        try
        {
            await redis.ExecuteAsync("BGSAVE");
        }
        catch (RedisServerException ex) when (ex.Message.Contains("already in progress", StringComparison.OrdinalIgnoreCase))
        {
            // BGSAVE đang chạy nền -> chờ hoàn tất bên dưới.
        }

        for (int i = 0; i < 15; i++)
        {
            var status = await ReadPersistenceStatusAsync(redis);
            if (!status.BgSaveInProgress)
                return status;
            await Task.Delay(200);
        }

        return await ReadPersistenceStatusAsync(redis);
    }

    private static async Task<MemuraiPersistenceStatus> ReadPersistenceStatusAsync(IDatabase redis)
    {
        var infoRaw = (await redis.ExecuteAsync("INFO", "persistence")).ToString();
        var infoMap = ParseRedisInfo(infoRaw);

        string dataDir = await GetRedisConfigValueAsync(redis, "dir");
        string dbFilename = await GetRedisConfigValueAsync(redis, "dbfilename");
        string appendOnly = await GetRedisConfigValueAsync(redis, "appendonly");
        string appendDirName = await GetRedisConfigValueAsync(redis, "appenddirname");

        return new MemuraiPersistenceStatus
        {
            DataDirectory = dataDir,
            DbFilename = string.IsNullOrWhiteSpace(dbFilename) ? "dump.rdb" : dbFilename,
            AppendOnlyEnabled = string.Equals(appendOnly, "yes", StringComparison.OrdinalIgnoreCase),
            AppendDirName = string.IsNullOrWhiteSpace(appendDirName) ? "appendonlydir" : appendDirName,
            LastBgSaveStatus = infoMap.GetValueOrDefault("rdb_last_bgsave_status", "unknown"),
            BgSaveInProgress = infoMap.GetValueOrDefault("rdb_bgsave_in_progress", "0") == "1",
            LastSaveEpoch = long.TryParse(infoMap.GetValueOrDefault("rdb_last_save_time", "0"), out var ts) ? ts : 0,
            AofCurrentSize = long.TryParse(infoMap.GetValueOrDefault("aof_current_size", "0"), out var aofSize) ? aofSize : 0,
            RawInfo = infoRaw
        };
    }

    private static async Task<ConnectionMultiplexer> ConnectAdminRedisAsync()
    {
        var opts = ConfigurationOptions.Parse(DbContext.RedisConnectionString);
        opts.AllowAdmin = true;
        opts.AbortOnConnectFail = false;
        return await ConnectionMultiplexer.ConnectAsync(opts);
    }

    public async Task<BackupOperationResult> BackupMemuraiPersistenceFilesAsync(string targetFolder)
    {
        var status = await TriggerMemuraiBgSaveAsync();
        var rootDir = EnsureDirectory(targetFolder);
        var backupDir = Path.Combine(rootDir, $"memurai_backup_{DateTime.Now:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(backupDir);

        var log = new StringBuilder();
        log.AppendLine($"Memurai data dir: {status.DataDirectory}");
        log.AppendLine($"rdb_last_bgsave_status: {status.LastBgSaveStatus}");
        log.AppendLine($"appendonly: {(status.AppendOnlyEnabled ? "yes" : "no")}");

        int copiedCount = 0;

        if (!string.IsNullOrWhiteSpace(status.DataDirectory) && Directory.Exists(status.DataDirectory))
        {
            var rdbSource = Path.Combine(status.DataDirectory, status.DbFilename);
            if (File.Exists(rdbSource))
            {
                var rdbDest = Path.Combine(backupDir, status.DbFilename);
                File.Copy(rdbSource, rdbDest, overwrite: true);
                copiedCount++;
                log.AppendLine($"[OK] Copied RDB: {rdbDest} ({new FileInfo(rdbDest).Length} bytes)");
            }
            else
            {
                log.AppendLine($"[INFO] Không thấy file RDB tại: {rdbSource}");
            }

            var aofDirSource = Path.Combine(status.DataDirectory, status.AppendDirName);
            if (Directory.Exists(aofDirSource))
            {
                var aofDirDest = Path.Combine(backupDir, status.AppendDirName);
                CopyDirectoryRecursive(aofDirSource, aofDirDest);
                var files = Directory.GetFiles(aofDirDest);
                copiedCount += files.Length;
                log.AppendLine($"[OK] Copied AOF dir ({files.Length} files): {aofDirDest}");
            }
            else
            {
                var legacyAof = Path.Combine(status.DataDirectory, "appendonly.aof");
                if (File.Exists(legacyAof))
                {
                    var dest = Path.Combine(backupDir, "appendonly.aof");
                    File.Copy(legacyAof, dest, overwrite: true);
                    copiedCount++;
                    log.AppendLine($"[OK] Copied AOF: {dest}");
                }
            }
        }

        return new BackupOperationResult
        {
            Success = copiedCount > 0,
            ExitCode = copiedCount > 0 ? 0 : 1,
            CommandText = "BGSAVE + Copy RDB/AOF persistence files",
            OutputLog = log.ToString().Trim(),
            ErrorLog = copiedCount > 0 ? string.Empty : "Không sao chép được file RDB/AOF nào từ thư mục dữ liệu Memurai.",
            TargetPath = backupDir
        };
    }

    private static async Task<BackupOperationResult> RunCliToolAsync(
        string toolName,
        IEnumerable<string> arguments,
        string targetPath)
    {
        var exePath = ResolveMongoToolPath(toolName);
        var argList = arguments.ToList();
        var cmdPreview = $"{Path.GetFileName(exePath)} {string.Join(" ", argList.Select(QuoteForDisplay))}";

        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        foreach (var arg in argList)
            psi.ArgumentList.Add(arg);

        try
        {
            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException($"Không thể khởi chạy '{exePath}'.");

            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();

            var stdout = (await stdoutTask).Trim();
            var stderr = (await stderrTask).Trim();

            return new BackupOperationResult
            {
                Success = proc.ExitCode == 0,
                ExitCode = proc.ExitCode,
                CommandText = cmdPreview,
                OutputLog = stdout,
                ErrorLog = stderr,
                TargetPath = targetPath
            };
        }
        catch (Exception ex)
        {
            return new BackupOperationResult
            {
                Success = false,
                ExitCode = -1,
                CommandText = cmdPreview,
                OutputLog = string.Empty,
                ErrorLog = $"Không chạy được '{toolName}': {ex.Message}\nHãy kiểm tra MongoDB Database Tools đã được cài và thêm vào PATH.",
                TargetPath = targetPath
            };
        }
    }

    private static string ResolveMongoToolPath(string toolName)
    {
        var exeName = toolName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            ? toolName
            : $"{toolName}.exe";

        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var dir in pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = Path.Combine(dir.Trim(), exeName);
            if (File.Exists(candidate))
                return candidate;
        }

        var toolsRoot = @"C:\Program Files\MongoDB\Tools";
        if (Directory.Exists(toolsRoot))
        {
            var found = Directory.GetDirectories(toolsRoot)
                .OrderByDescending(d => d)
                .Select(d => Path.Combine(d, "bin", exeName))
                .FirstOrDefault(File.Exists);
            if (!string.IsNullOrEmpty(found))
                return found;
        }

        return exeName;
    }

    private static async Task<string> GetRedisConfigValueAsync(IDatabase redis, string key)
    {
        var res = await redis.ExecuteAsync("CONFIG", "GET", key);
        if (res.Resp2Type == ResultType.Array)
        {
            var arr = (RedisResult[])res!;
            if (arr.Length >= 2)
                return arr[1].ToString();
        }
        return string.Empty;
    }

    private static Dictionary<string, string> ParseRedisInfo(string raw)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in raw.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith('#')) continue;
            var idx = line.IndexOf(':');
            if (idx > 0)
                dict[line[..idx].Trim()] = line[(idx + 1)..].Trim();
        }
        return dict;
    }

    private static void CopyDirectoryRecursive(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), overwrite: true);

        foreach (var sub in Directory.GetDirectories(sourceDir))
            CopyDirectoryRecursive(sub, Path.Combine(destDir, Path.GetFileName(sub)));
    }

    private static string EnsureDirectory(string? folder)
    {
        var path = string.IsNullOrWhiteSpace(folder) ? GetDefaultBackupRoot() : folder.Trim();
        Directory.CreateDirectory(path);
        return path;
    }

    private static string QuoteForDisplay(string value) =>
        value.Contains(' ') ? $"\"{value}\"" : value;
}

public sealed class BackupOperationResult
{
    public bool Success { get; init; }
    public int ExitCode { get; init; }
    public string CommandText { get; init; } = string.Empty;
    public string OutputLog { get; init; } = string.Empty;
    public string ErrorLog { get; init; } = string.Empty;
    public string TargetPath { get; init; } = string.Empty;
}

public sealed class MemuraiPersistenceStatus
{
    public string DataDirectory { get; init; } = string.Empty;
    public string DbFilename { get; init; } = "dump.rdb";
    public bool AppendOnlyEnabled { get; init; }
    public string AppendDirName { get; init; } = "appendonlydir";
    public string LastBgSaveStatus { get; init; } = string.Empty;
    public bool BgSaveInProgress { get; init; }
    public long LastSaveEpoch { get; init; }
    public long AofCurrentSize { get; init; }
    public string RawInfo { get; init; } = string.Empty;
}
