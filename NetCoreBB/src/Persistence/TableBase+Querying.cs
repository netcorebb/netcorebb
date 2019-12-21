/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using Dapper;
using LanguageExt;
using MySql.Data.MySqlClient;
using NetCoreBB.Persistence.Model;
using static LanguageExt.Prelude;

namespace NetCoreBB.Persistence
{
    public abstract partial class TableBase<T>
    {
        private const int Limit = 1000;


        public Option<T> Single(long id)
        {
            try {
                var res = Use(db => db.QuerySingleOrDefault($"select * from {TableName} where id = @Id", new {id}));
                return res.IsSome ? Some(new T()) : None;
            }
            catch (MySqlException) {
                return None;
            }
        }


        public Option<Row<T>> SingleRow(long id)
        {
            try {
                var res = Use(db => db.QuerySingleOrDefault($"select * from {TableName} where id = @Id", new {id}));
                return res.IsSome ? Some(new Row<T>(0, 0, DateTime.Now, new T())) : None;
            }
            catch (MySqlException) {
                return None;
            }
        }


        public Option<T> SingleByRId(long rid)
        {
            try {
                var res = Use(db => db.QuerySingleOrDefault($"select * from {TableName} where row_id = @Id", new {rid}));
                return res.IsSome ? Some(new T()) : None;
            }
            catch (MySqlException) {
                return None;
            }
        }


        public Option<Row<T>> SingleRowByRId(long rid)
        {
            try {
                var res = Use(db => db.QuerySingleOrDefault($"select * from {TableName} where row_id = @Id", new {rid}));
                return res.IsSome ? Some(new Row<T>(0, 0, DateTime.Now, new T())) : None;
            }
            catch (MySqlException) {
                return None;
            }
        }


        public Arr<T> Select(int? limit)
        {
            try {
                var res = Use(db => db.Query($"select * from {TableName} limit @Limit", new {Limit = limit ?? Limit}));
                return Empty;
            }
            catch (MySqlException) {
                return Empty;
            }
        }


        public Arr<Row<T>> SelectRows(int? limit)
        {
            try { }
            catch (MySqlException) {
                return Empty;
            }
            return Empty;
        }
    }
}
