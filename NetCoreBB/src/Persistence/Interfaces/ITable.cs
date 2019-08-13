/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System.Threading.Tasks;
using LanguageExt;

namespace NetCoreBB.Persistence.Interfaces
{
    /// <summary>
    /// Interface for accessing a MySQL table combining serveral CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity from domain model</typeparam>
    public interface ITable<T> : IQueryableTable<T> where T : Record<T>
    {
        /// <summary>
        /// Name of the table; not necessarily the real MySQL table name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the table exists in the database
        /// </summary>
        bool Exists();

        /// <summary>
        /// Whether the table exists in the database (async)
        /// </summary>
        Task<bool> ExistsAsync();
    }
}
