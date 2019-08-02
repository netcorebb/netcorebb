/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;

namespace NetCoreBB.Interfaces
{
    /// <summary>
    /// Live-reloaded configuration from config.toml
    /// </summary>
    public interface ISystemConfig
    {
        /// <summary>
        /// Tries to read the data from config.toml and materializes it into a model, otherwise defaults are returned.
        /// </summary>
        /// <returns>Tuple of models</returns>
        (Domain.Model.SystemConfig.System, Domain.Model.SystemConfig.MySql) Read();

        /// <summary>
        /// Observable system configuration; fires immediately upon subscription if StartWatching() has been called.
        /// </summary>
        IObservable<Domain.Model.SystemConfig.System> System { get; }

        /// <summary>
        /// Observable MySQL configuration; fires immediately upon subscription if StartWatching() has been called.
        /// </summary>
        IObservable<Domain.Model.SystemConfig.MySql> MySql { get; }

        /// <summary>
        /// Starts watching changes to config.toml.
        /// </summary>
        /// <returns>True upon success; false if already started</returns>
        bool StartWatching();

        /// <summary>
        /// Stops watching changes to config.toml.
        /// </summary>
        void StopWatching();
    }
}
