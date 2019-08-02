/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.IO;
using System.Linq;
using LanguageExt;
using MoreLinq;
using NetCoreBB.Infrastructure;
using NetCoreBB.Interfaces;
using ServiceStack;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace NetCoreBB.UnitTests.Infrastructure
{
    public class SystemConfigTests : IDisposable
    {
        [Fact]
        public void Read_has_parsed_system_section()
        {
            var defaults = new Domain.Model.SystemConfig.System();
            var (system1, _) = Config.Read();
            system1.Installed.ShouldBe(defaults.Installed);
            system1.Maintenance.ShouldBe(defaults.Maintenance);
            system1.Development.ShouldBe(defaults.Development);
            system1.UnderAttack.ShouldBe(defaults.UnderAttack);

            const string main1 = @"[System]
                SystemInstalled = true
                MaintenanceMode = true
                DevelopmentMode = true
                UnderAttackMode = true";

            File.WriteAllText(Path.Combine(EtcPath, "config.toml"), main1);
            var (system2, _) = Config.Read();
            system2.Installed.ShouldBeTrue();
            system2.Maintenance.ShouldBeTrue();
            system2.Development.ShouldBeTrue();
            system2.UnderAttack.ShouldBeTrue();

            const string main2 = @"[System]
                SystemInstalled = false
                MaintenanceMode = false
                DevelopmentMode = false
                UnderAttackMode = false";

            File.WriteAllText(Path.Combine(EtcPath, "config.toml"), main2);
            var (system3, _) = Config.Read();
            system3.Installed.ShouldBeFalse();
            system3.Maintenance.ShouldBeFalse();
            system3.Development.ShouldBeFalse();
            system3.UnderAttack.ShouldBeFalse();
        }


        [Fact]
        public void Read_has_parsed_mysql_section()
        {
            var defaults = new Domain.Model.SystemConfig.MySql();
            var (_, mysql1) = Config.Read();
            mysql1.Server.ShouldBe(defaults.Server);
            mysql1.SslMode.ShouldBe(defaults.SslMode);
            mysql1.Database.ShouldBe(defaults.Database);
            mysql1.Username.ShouldBe(defaults.Username);
            mysql1.Password.ShouldBe(defaults.Password);
            mysql1.Protocol.ShouldBe(defaults.Protocol);
            mysql1.Port.ShouldBe(defaults.Port);
            mysql1.Other.ShouldBe(defaults.Other);

            const string main = @"[MySQL]
                Server = ""a""
                SslMode = ""b""
                Database = ""c""
                Username = ""d""
                Password = ""e""
                Protocol = ""f""
                Port = 12345
                [MySQL.Other]
                ""g"" = ""xxx""
                ""h"" = 123
                ""i"" = true";

            File.WriteAllText(Path.Combine(EtcPath, "config.toml"), main);
            var (_, mysql2) = Config.Read();
            mysql2.Server.ShouldBe("a");
            mysql2.SslMode.ShouldBe("b");
            mysql2.Database.ShouldBe("c");
            mysql2.Username.ShouldBe("d");
            mysql2.Password.ShouldBe("e");
            mysql2.Protocol.ShouldBe("f");
            mysql2.Port.ShouldBe(12345);
            mysql2.Other.ShouldBe(Map(("g", "xxx"), ("h", "123"), ("i", "true")));
        }


        // --- Setup ---

        public class PathLocatorMock : IPathLocator
        {
            public static string ConfigPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), "etc_test");
            public Option<string> Config => ConfigPath.DirectoryExists() ? Some(ConfigPath) : None;
        }

        public class EnvironmentMock : IEnvironment
        {
            public bool IsProduction => !DevMode;
            public bool IsDevelopment => DevMode;

            public EnvironmentMock(bool devMode = false)
            {
                DevMode = devMode;
            }

            public bool DevMode { get; set; }
        }

        private string EtcPath { get; } = PathLocatorMock.ConfigPath;

        private ISystemConfig Config { get; }
        private ITestOutputHelper Output { get; }

        public SystemConfigTests(ITestOutputHelper helper)
        {
            Output = helper;
            Config = new SystemConfig(new PathLocatorMock(), new EnvironmentMock());

            // Create directory if not exists
            Directory.CreateDirectory(EtcPath);
            Output.WriteLine("Path: " + EtcPath);

            // If it exists and contains *.toml files, delete them
            Directory.EnumerateFiles(EtcPath)
                .Where(x => x.EndsWith(".toml"))
                .ForEach(File.Delete);
        }

        public void Dispose()
        {
            ((IDisposable)Config).Dispose();
        }
    }
}
