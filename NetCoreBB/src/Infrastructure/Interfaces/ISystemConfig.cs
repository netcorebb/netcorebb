/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;

namespace NetCoreBB.Infrastructure.Interfaces
{
    /// <summary>
    /// Todo: Live-reloaded configuration from config.toml
    /// </summary>
    public interface ISystemConfig
    {
        /// <summary>
        /// Tries to read the data from config.toml and materializes it into a model, otherwise defaults are returned.
        /// </summary>
        /// <returns>Tuple of models</returns>
        (Domain.Model.SystemConfig.System, Domain.Model.SystemConfig.MySql) Read();

        // <summary>
        // Observable system configuration; fires on distinct file changes to config.toml and user/dev config files.
        // </summary>
        //IObservable<Domain.Model.SystemConfig.System> System { get; }

        // <summary>
        // Observable MySQL configuration; fires on distinct file changes to config.toml and user/dev config files.
        // </summary>
        //IObservable<Domain.Model.SystemConfig.MySql> MySql { get; }

        // <summary>
        // Starts watching changes to config.toml and user/dev config files.
        // </summary>
        // <returns>True upon success; false if already started or watcher cannot be started.</returns>
        //bool StartWatching();

        // <summary>
        // Stops watching changes to config.toml and user/dev config files.
        // </summary>
        //void StopWatching();
    }
}
