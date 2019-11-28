using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonitoringServer.Models;
using Npgsql;
using NpgsqlTypes;

namespace MonitoringServer
{
    public class DbWorker
    {
        private static ILogger _logger = Log.CreateLogger<DbWorker>();

        private static string _connectionString;

        public static void UseDbConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static async void SaveInfo(ComputerInfo compInfo)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                @"INSERT INTO ""Monitoring_CommonInfo""(""ClientIp"", ""TotalRAM"", ""FreeRAM"", ""CPULoad"")
                  VALUES(@ClientIpV, @TotalRAMV, @FreeRAMV, @CPULoadV) RETURNING ""Id""", conn))
                {
                    cmd.Parameters.AddWithValue("ClientIpV", NpgsqlDbType.Varchar, compInfo.ClientIp);
                    cmd.Parameters.AddWithValue("TotalRAMV", NpgsqlDbType.Integer, compInfo.TotalRAM);
                    cmd.Parameters.AddWithValue("FreeRAMV", NpgsqlDbType.Integer, compInfo.FreeRAM);
                    cmd.Parameters.AddWithValue("CPULoadV", NpgsqlDbType.Numeric, compInfo.CPULoad);
                    var id = (int) await cmd.ExecuteScalarAsync();

                    cmd.CommandText = @"INSERT INTO ""Monitoring_DiskInfo""(""InfoId"", ""HDDName"", ""HDDTotalSpace"", ""HDDFreeSpace"")
	                                    VALUES (@InfoIdV, @HDDNameV, @HDDTotalSpaceV, @HDDFreeSpaceV);";
                    foreach (var hddInfo in compInfo.HDDInfo)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("InfoIdV", NpgsqlDbType.Integer, id);
                        cmd.Parameters.AddWithValue("HDDNameV", NpgsqlDbType.Varchar,
                            hddInfo.HDDName.Substring(0, Math.Min(hddInfo.HDDName.Length, 32)));
                        cmd.Parameters.AddWithValue("HDDTotalSpaceV", NpgsqlDbType.Bigint, hddInfo.HDDTotalSpace);
                        cmd.Parameters.AddWithValue("HDDFreeSpaceV", NpgsqlDbType.Bigint, hddInfo.HDDFreeSpace);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DbWorker ERROR");
            }
        }

    }
}
