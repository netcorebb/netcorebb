/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;

namespace NetCoreBB.Interfaces
{
    using NetCoreBB.Domain.Model.SystemConfig;

    /// <summary>
    /// Live-reloaded configuration from config.toml
    /// </summary>
    public interface ISystemConfig
    {
        /// <summary>
        /// Observable system configuration
        /// </summary>
        IObservable<System> System { get; }

        /// <summary>
        /// Observable MySQL configuration
        /// </summary>
        IObservable<MySql> MySql { get; }
    }
}
