/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LanguageExt;
using NetCoreBB.Interfaces;
using ServiceStack;
using Tomlyn;
using Tomlyn.Model;

namespace NetCoreBB.Infrastructure
{
    using Sys = Domain.Model.SystemConfig.System;
    using MyS = Domain.Model.SystemConfig.MySql;


    public class SystemConfig : ISystemConfig, IDisposable
    {
        public IObservable<Sys> System => _system.DistinctUntilChanged();
        public IObservable<MyS> MySql => _mysql.DistinctUntilChanged();

        private readonly Subject<Sys> _system = new Subject<Sys>();
        private readonly Subject<MyS> _mysql = new Subject<MyS>();

        private FileSystemWatcher Watcher { get; } = new FileSystemWatcher();
        private IPathLocator Locator { get; }
        private IEnvironment Environment { get; }

        private const string MainCfg = "config.toml";
        private const string UserCfg = "config.user.toml";
        private const string DevCfg = "config.dev.toml";


        public SystemConfig(Some<IPathLocator> locator, Some<IEnvironment> environment)
        {
            Locator = locator.Value;
            Environment = environment.Value;

            Locator.Config.IfSome(path => {
                Watcher.Path = path;

                Watcher.Filters.Add(MainCfg);
                Watcher.Filters.Add(UserCfg);
                Watcher.Filters.Add(DevCfg);

                Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                Watcher.IncludeSubdirectories = false;

                Watcher.Changed += (sender, args) => {
                    var (system, mysql) = Read();
                    _system.OnNext(system);
                    _mysql.OnNext(mysql);
                };
            });
        }


        public bool StartWatching()
        {
            if (Watcher.EnableRaisingEvents) {
                return false;
            }
            if (!Watcher.Path.IsEmpty()) {
                Watcher.EnableRaisingEvents = true;
                return true;
            }
            return false;
        }


        public void StopWatching()
        {
            Watcher.EnableRaisingEvents = false;
        }


        public void Dispose()
        {
            StopWatching();
            Watcher.Dispose();
            _system.Dispose();
            _mysql.Dispose();
        }


        public (Sys, MyS) Read()
        {
            var system = new Sys();
            var mysql = new MyS();

            Locator.Config.IfSome(cfg => {
                var mainCfgFile = Path.Combine(cfg, MainCfg);
                var userCfgFile = Path.Combine(cfg, UserCfg);
                var devCfgFile = Path.Combine(cfg, DevCfg);

                if (!Process(mainCfgFile, ref system, ref mysql)) {
                    return;
                }
                Process(userCfgFile, ref system, ref mysql);

                if (Environment.IsDevelopment) {
                    Process(devCfgFile, ref system, ref mysql);
                }
            });

            return (system, mysql);
        }

        private static bool Process(string file, ref Sys system, ref MyS mysql)
        {
            const string kSystem = "System";
            const string kMySql = "MySQL";

            if (!file.FileExists()) {
                return false;
            }
            var toml = Toml.Parse(file.ReadAllText());
            if (toml.HasErrors) {
                return false;
            }
            var model = toml.ToModel();

            if (model.ContainsKey(kSystem)) {
                Parse(ref system, (TomlTable)model[kSystem]);
            }
            if (model.ContainsKey(kMySql)) {
                Parse(ref mysql, (TomlTable)model[kMySql]);
            }

            return true;
        }

        private static void Parse(ref Sys system, TomlTable table)
        {
            try {
                {
                    var val = GetBool(table, "SystemInstalled");
                    if (val != null) system.Installed = val.Value;
                }
                {
                    var val = GetBool(table, "MaintenanceMode");
                    if (val != null) system.Maintenance = val.Value;
                }
                {
                    var val = GetBool(table, "DevelopmentMode");
                    if (val != null) system.Development = val.Value;
                }
                {
                    var val = GetBool(table, "UnderAttackMode");
                    if (val != null) system.UnderAttack = val.Value;
                }
            }
            catch (InvalidCastException) { }
        }

        private static void Parse(ref MyS mysql, TomlTable table)
        {
            try {
                {
                    var val = GetString(table, "Server");
                    if (val != null) mysql.Server = val;
                }
                {
                    var val = GetString(table, "SslMode");
                    if (val != null) mysql.SslMode = val;
                }
                {
                    var val = GetString(table, "Database");
                    if (val != null) mysql.Database = val;
                }
                {
                    var val = GetString(table, "Username");
                    if (val != null) mysql.Username = val;
                }
                {
                    var val = GetString(table, "Password");
                    if (val != null) mysql.Password = val;
                }
                {
                    var val = GetString(table, "Protocol");
                    if (val != null) mysql.Protocol = val;
                }
                {
                    var val = GetInt(table, "Port");
                    if (val != null) mysql.Port = val.Value;
                }
                if (!table.ContainsKey("Other")) {
                    return;
                }
                var map = ((TomlTable)table["Other"])
                    .GetTomlEnumerator()
                    .Select(x => (x.Key, x.Value.Kind switch {
                        ObjectKind.Boolean => ((TomlBoolean)x.Value).Value.ToString().ToLower(),
                        ObjectKind.Integer => ((TomlInteger)x.Value).Value.ToString(),
                        ObjectKind.String => ((TomlString)x.Value).Value,
                        _ => string.Empty
                        }));
                mysql.Other = new Map<string, string>(map);
            }
            catch (InvalidCastException) { }
        }

        private static bool? GetBool(TomlTable table, string key)
        {
            return table.ContainsKey(key) ? (bool)table[key] : (bool?)null;
        }

        private static int? GetInt(TomlTable table, string key)
        {
            return table.ContainsKey(key) ? Convert.ToInt32((long)table[key]) : (int?)null;
        }

        private static string? GetString(TomlTable table, string key)
        {
            return table.ContainsKey(key) ? (string)table[key] : null;
        }
    }
}
