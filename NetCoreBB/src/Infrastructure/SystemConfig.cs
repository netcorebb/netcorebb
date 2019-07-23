/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.IO;
using NetCoreBB.Domain.Model.SystemConfig;
using NetCoreBB.Interfaces;

namespace NetCoreBB.Infrastructure
{
    public class SystemConfig : ISystemConfig, IDisposable
    {
        public IObservable<Domain.Model.SystemConfig.System> System { get; }

        public IObservable<MySql> MySql { get; }

        private string ConfigFile { get; }
        private FileSystemWatcher Watcher { get; }


        public SystemConfig(string configFile)
        {
            ConfigFile = configFile;
            Watcher = new FileSystemWatcher();
        }


        public void Dispose()
        {
            Watcher.Dispose();
        }
    }
}
