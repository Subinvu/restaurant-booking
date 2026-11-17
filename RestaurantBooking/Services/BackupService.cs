using RestaurantBooking.Config;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Xử lý gọi Process mongodump/mongorestore, mongoexport/mongoimport và Memurai BGSAVE ngay từ FrmAdmin.
/// </summary>
public class BackupService
{
    private readonly DbContext _db = DbContext.Instance;

    // TODO (TV2 - Giai đoạn 1):
    // 1. RunMongoDumpAsync(targetFolder) -> mongodump --uri="mongodb://127.0.0.1:27017" --db=db_booking_nhahang --out=...
    // 2. RunMongoRestoreAsync(backupFolder) -> mongorestore --drop ...
    // 3. ExportCollectionsToJsonAsync(targetFolder) -> mongoexport từng collection
    // 4. TriggerMemuraiBgSave() -> RedisDb.Execute("BGSAVE")
}
