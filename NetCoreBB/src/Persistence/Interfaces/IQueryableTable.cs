/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using LanguageExt;
using NetCoreBB.Persistence.Model;

namespace NetCoreBB.Persistence.Interfaces
{
    /// <summary>
    /// Table read operations
    /// </summary>
    /// <typeparam name="T">Entity from domain model</typeparam>
    public interface IQueryableTable<T> where T : Record<T>
    {
        /// <summary>
        /// Retrieve single entity by id
        /// </summary>
        /// <param name="id">Primary key id</param>
        /// <returns>Entity model upon success</returns>
        Option<T> Single(long id);

        /// <summary>
        /// Retrieve single entity with its meta data by id
        /// </summary>
        /// <param name="id">Primary key id</param>
        /// <returns>Entity meta model upon success</returns>
        Option<Row<T>> SingleRow(long id);

        /// <summary>
        /// Retrieve single entity by row id
        /// </summary>
        /// <param name="rid">Row id</param>
        /// <returns>Entity model upon success</returns>
        Option<T> SingleByRId(long rid);

        /// <summary>
        /// Retrieve single entity with its meta data by row id
        /// </summary>
        /// <param name="rid">Row id</param>
        /// <returns>Entity meta model upon success</returns>
        Option<Row<T>> SingleRowByRId(long rid);

        /// <summary>
        /// Todo: Retrieve some entities from query
        /// </summary>
        /// <param name="limit">Optional limit</param>
        /// <returns>Array of entities (might be empty)</returns>
        Arr<T> Select(int? limit = null);

        /// <summary>
        /// Todo: Retrieve some rows from query
        /// </summary>
        /// <param name="limit">Optional limit</param>
        /// <returns>Array of entities with meta data (might be empty)</returns>
        Arr<Row<T>> SelectRows(int? limit = null);
    }
}
