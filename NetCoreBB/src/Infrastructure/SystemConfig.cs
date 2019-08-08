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
using System.Text;
using LanguageExt;
using NetCoreBB.Interfaces;
using ServiceStack;
using Tomlyn;
using Tomlyn.Model;

namespace NetCoreBB.Infrastructure
{
    using Sys = Domain.Model.SystemConfig.System;
    using MyS = Domain.Model.SystemConfig.MySql;


    // Todo: Implement safe file watcher and event emitter
    public class SystemConfig : ISystemConfig, IDisposable
    {
        //public IObservable<Sys> System => _system.DistinctUntilChanged();
        //public IObservable<MyS> MySql => _mysql.DistinctUntilChanged();

        //private readonly Subject<Sys> _system = new Subject<Sys>();
        //private readonly Subject<MyS> _mysql = new Subject<MyS>();

        private (Sys, MyS)? _lastValidConfig;

        //private FileSystemWatcher Watcher { get; } = new FileSystemWatcher();
        private IPathLocator Locator { get; }
        private IEnvironment Environment { get; }

        private const string MainCfg = "config.toml";
        private const string UserCfg = "config.user.toml";
        private const string DevCfg = "config.dev.toml";


        public SystemConfig(Some<IPathLocator> locator, Some<IEnvironment> environment)
        {
            Locator = locator.Value;
            Environment = environment.Value;

            /*Locator.Config.IfSome(path => Watcher.Path = path);

            Watcher.Filters.Add(MainCfg);
            Watcher.Filters.Add(UserCfg);
            Watcher.Filters.Add(DevCfg);

            Watcher.NotifyFilter = NotifyFilters.LastWrite;
            Watcher.IncludeSubdirectories = false;

            Watcher.Changed += (sender, args) => {
                var (system, mysql) = Read();
                _system.OnNext(system);
                _mysql.OnNext(mysql);
            };*/
        }


        public bool StartWatching()
        {
/*            Locator.Config.IfSome(path => Watcher.Path = path);

            if (Watcher.EnableRaisingEvents) {
                return false;
            }
            if (!Watcher.Path.IsEmpty()) {
                Watcher.EnableRaisingEvents = true;
                return true;
            }*/
            return false;
        }


        public void StopWatching()
        {
/*            Watcher.EnableRaisingEvents = false;*/
        }


        public void Dispose()
        {
/*            StopWatching();
            Watcher.Dispose();
            _system.Dispose();
            _mysql.Dispose();*/
        }


        public (Sys, MyS) Read()
        {
            (Sys, MyS)? res = null;
            ReadFromConfigFiles().Match(tuple => {
                res = tuple;
                _lastValidConfig = res;
            }, () => { res = _lastValidConfig ?? (new Sys(), new MyS()); });
            return res ?? throw new Exception();
        }


        private Option<(Sys, MyS)> ReadFromConfigFiles()
        {
            var res = (new Sys(), new MyS());
            var (system, mysql) = res;
            var success = false;

            Locator.Config.IfSome(path => {
                var mainCfgFile = Path.Combine(path, MainCfg);
                var userCfgFile = Path.Combine(path, UserCfg);
                var devCfgFile = Path.Combine(path, DevCfg);

                if (!Process(mainCfgFile, ref system, ref mysql)) {
                    return;
                }
                Process(userCfgFile, ref system, ref mysql);

                if (Environment.IsDevelopment) {
                    Process(devCfgFile, ref system, ref mysql);
                }
                success = true;
            });

            return success ? res : Option<(Sys, MyS)>.None;
        }

        private static bool Process(string file, ref Sys system, ref MyS mysql)
        {
            const string kSystem = "System";
            const string kMySql = "MySQL";

            if (!file.FileExists()) {
                return false;
            }
            string? data;
            try {
                data = File.ReadAllText(file, Encoding.UTF8);
            }
            catch (Exception) {
                return false;
            }
            var toml = Toml.Parse(data);
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
                    var val = GetUInt(table, "Port");
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

        private static uint? GetUInt(TomlTable table, string key)
        {
            return table.ContainsKey(key) ? Convert.ToUInt32((long)table[key]) : (uint?)null;
        }

        private static string? GetString(TomlTable table, string key)
        {
            return table.ContainsKey(key) ? (string)table[key] : null;
        }
    }
}
