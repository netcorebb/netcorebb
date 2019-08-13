/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using LanguageExt;

namespace NetCoreBB.Persistence.Model
{
    /// <summary>
    /// Encapsulating row POCO containing metadata
    /// </summary>
    /// <typeparam name="T">Entity from domain model</typeparam>
    public class Row<T> : Record<Row<T>> where T : Record<T>
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Randomized row id
        /// </summary>
        public long RId { get; }

        /// <summary>
        /// When the row was created
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// When the row was possibly updated
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The other row data
        /// </summary>
        public T Model { get; }

        public Row(long id, long rid, DateTime created, Some<T> model)
        {
            Id = id;
            RId = rid;
            Created = created;
            Model = model.Value;
        }
    }
}
