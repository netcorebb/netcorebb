/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using LanguageExt;
using MySql.Data.MySqlClient;
using NetCoreBB.Persistence.Interfaces;

namespace NetCoreBB.Persistence
{
    /// <summary>
    /// Base class implementing `ITable`
    /// </summary>
    /// <typeparam name="T">Entity from domain model</typeparam>
    public abstract partial class TableBase<T> : ITable<T> where T : Record<T>, new()
    {
        private MySqlConnection Connection { get; }
        protected string? Prefix { get; }

        public abstract string Name { get; }

        public string TableName
        {
            get
            {
                var regex = new Regex("^[0-9a-zA-Z_-]{1,64}$");
                var tableName = Prefix?.ToLower() + Name.ToLower();
                return regex.IsMatch(tableName) ? tableName : throw new Exception("Invalid table name.");
            }
        }

        protected void Use(Action<IDbConnection> func)
        {
            try {
                Connection.Open();
                func(Connection);
            }
            finally {
                Connection.Close();
            }
        }

        protected async Task UseAsync(Action<IDbConnection> func)
        {
            try {
                await Connection.OpenAsync();
                func(Connection);
            }
            finally {
                await Connection.CloseAsync();
            }
        }

        protected Option<TRet> Use<TRet>(Func<IDbConnection, TRet> func)
        {
            try {
                Connection.Open();
                return func(Connection);
            }
            finally {
                Connection.Close();
            }
        }

        protected async Task<Option<TRet>> UseAsync<TRet>(Func<IDbConnection, TRet> func)
        {
            try {
                await Connection.OpenAsync();
                return func(Connection);
            }
            finally {
                await Connection.CloseAsync();
            }
        }


        public virtual bool Exists()
        {
            return Use(db => db.QuerySingleOrDefault("SHOW TABLES LIKE @TableName", new {TableName}))
                .Match(val => val != null, () => false);
        }

        public virtual async Task<bool> ExistsAsync()
        {
            return await UseAsync(db => db.QuerySingleOrDefault("SHOW TABLES LIKE @TableName", new {TableName}))
                .Match(val => val != null, () => false);
        }


        protected TableBase(Some<Domain.Model.SystemConfig.MySql> config)
        {
            var cfg = config.Value;
            var builder = new MySqlConnectionStringBuilder {
                Server = cfg.Server,

                SslMode = cfg.SslMode.ToLower() switch {
                    "prefered" => MySqlSslMode.Prefered,
                    "preferred" => MySqlSslMode.Preferred,
                    "required" => MySqlSslMode.Required,
                    "verifyca" => MySqlSslMode.VerifyCA,
                    "verify_ca" => MySqlSslMode.VerifyCA,
                    "verifyfull" => MySqlSslMode.VerifyFull,
                    "verify_full" => MySqlSslMode.VerifyFull,
                    _ => MySqlSslMode.None
                },

                Database = cfg.Database,
                UserID = cfg.Username,
                Password = cfg.Password,
                Port = cfg.Port,

                ConnectionProtocol = cfg.Protocol.ToLower() switch {
                    // Windows only
                    "memory" => MySqlConnectionProtocol.Memory,
                    "sharedmemory" => MySqlConnectionProtocol.Memory,
                    "shared_memory" => MySqlConnectionProtocol.Memory,
                    "pipe" => MySqlConnectionProtocol.Pipe,
                    "namedpipe" => MySqlConnectionProtocol.Pipe,
                    "named_pipe" => MySqlConnectionProtocol.Pipe,
                    // Unix only
                    "unix" => MySqlConnectionProtocol.Unix,
                    "unixsocket" => MySqlConnectionProtocol.Unix,
                    "unix_socket" => MySqlConnectionProtocol.Unix,
                    // Universal, TCP/IP
                    "socket" => MySqlConnectionProtocol.Socket,
                    "tcp" => MySqlConnectionProtocol.Socket,
                    _ => MySqlConnectionProtocol.Socket,
                }
            };

            Connection = new MySqlConnection(builder.ToString());
            Prefix = cfg.Prefix;
        }


        protected TableBase(Some<string> connectionString, string? prefix = null)
        {
            Connection = new MySqlConnection(connectionString.Value);
            Prefix = prefix;
        }
    }
}
