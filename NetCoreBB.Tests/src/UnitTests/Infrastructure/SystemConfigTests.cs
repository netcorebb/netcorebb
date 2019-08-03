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
        public void Read_returns_defaults_if_no_config_is_present()
        {
            var sysDef = new Domain.Model.SystemConfig.System();
            var mysDef = new Domain.Model.SystemConfig.MySql();

            var (sysRes, mysRes) = Config.Read();

            sysRes.Installed.ShouldBe(sysDef.Installed);
            sysRes.Maintenance.ShouldBe(sysDef.Maintenance);
            sysRes.Development.ShouldBe(sysDef.Development);
            sysRes.UnderAttack.ShouldBe(sysDef.UnderAttack);

            mysRes.Server.ShouldBe(mysDef.Server);
            mysRes.SslMode.ShouldBe(mysDef.SslMode);
            mysRes.Database.ShouldBe(mysDef.Database);
            mysRes.Username.ShouldBe(mysDef.Username);
            mysRes.Password.ShouldBe(mysDef.Password);
            mysRes.Protocol.ShouldBe(mysDef.Protocol);
            mysRes.Port.ShouldBe(mysDef.Port);
            mysRes.Other.ShouldBe(mysDef.Other);
        }


        [Fact]
        public void Read_parses_system_section()
        {
            const string main1 = @"[System]
                SystemInstalled = true
                MaintenanceMode = true
                DevelopmentMode = true
                UnderAttackMode = true";

            File.WriteAllText(Path.Combine(EtcPath, "config.toml"), main1);
            var (system1, _) = Config.Read();
            system1.Installed.ShouldBeTrue();
            system1.Maintenance.ShouldBeTrue();
            system1.Development.ShouldBeTrue();
            system1.UnderAttack.ShouldBeTrue();

            const string main2 = @"[System]
                SystemInstalled = false
                MaintenanceMode = false
                DevelopmentMode = false
                UnderAttackMode = false";

            File.WriteAllText(Path.Combine(EtcPath, "config.toml"), main2);
            var (system2, _) = Config.Read();
            system2.Installed.ShouldBeFalse();
            system2.Maintenance.ShouldBeFalse();
            system2.Development.ShouldBeFalse();
            system2.UnderAttack.ShouldBeFalse();
        }


        [Fact]
        public void Read_parses_mysql_section()
        {
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
            var (_, mysql) = Config.Read();
            mysql.Server.ShouldBe("a");
            mysql.SslMode.ShouldBe("b");
            mysql.Database.ShouldBe("c");
            mysql.Username.ShouldBe("d");
            mysql.Password.ShouldBe("e");
            mysql.Protocol.ShouldBe("f");
            mysql.Port.ShouldBe(12345);
            mysql.Other.ShouldBe(Map(("g", "xxx"), ("h", "123"), ("i", "true")));
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
