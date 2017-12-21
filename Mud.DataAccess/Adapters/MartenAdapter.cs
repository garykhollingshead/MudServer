using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marten;
using Mud.Data;
using Mud.Data.Character;
using Mud.Data.Configuration;
using Mud.Data.Persistance;
using Mud.DataAccess.IdGenerators;
using Npgsql;

namespace Mud.DataAccess.Adapters
{
    public class MartenAdapter
    {
        private readonly DocumentStore DocumentStore;
        private readonly PostgresSettings _settings;


        public MartenAdapter(PostgresSettings settings)
        {
            _settings = settings;

            CreateDatabase();

            DocumentStore = DocumentStore.For(Configure);
        }

        private void Configure(StoreOptions storeOptions)
        {
            storeOptions.Connection(GetNpgsqlConnectionString(_settings.Database));

            storeOptions.Schema.For<Character>().IdStrategy(new CustomdIdGeneration());
        }

        private string GetNpgsqlConnectionString(string database = "postgres")
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = _settings.Server,
                Port = _settings.Port,
                Username = _settings.Username,
                Password = _settings.Password,
                Database = database,
                Pooling = true,
                MinPoolSize = 1,
                MaxPoolSize = 20,
                ConnectionIdleLifetime = 15
            };

            return builder.ConnectionString;
        }

        public void CreateDatabase()
        {
            using (var conn = new NpgsqlConnection(GetNpgsqlConnectionString()))
            {
                var createDbCmd = new NpgsqlCommand($@"CREATE DATABASE {_settings.Database};", conn);
                conn.Open();
                try
                {
                    createDbCmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }

                conn.Close();
            }
            CreateExtension();
        }

        public void CreateExtension()
        {
            using (var conn = new NpgsqlConnection(GetNpgsqlConnectionString(_settings.Database)))
            {
                var createExtensionCmd = new NpgsqlCommand($"CREATE EXTENSION PLV8;", conn);
                conn.Open();
                try
                {
                    createExtensionCmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
                conn.Close();
            }
        }

        public void DestroyDatabase()
        {
            DocumentStore.Dispose();
            using (var conn = new NpgsqlConnection(GetNpgsqlConnectionString()))
            {
                var getOpenDbConnsCmd = new NpgsqlCommand($@"SELECT pg_terminate_backend(pg_stat_activity.pid)
                                                            FROM pg_stat_activity
                                                            WHERE pg_stat_activity.datname = '{_settings.Database}'
                                                            AND pid <> pg_backend_pid();", conn);
                var deleteDbCmd = new NpgsqlCommand($@"DROP DATABASE {_settings.Database};", conn);
                conn.Open();
                try
                {
                    getOpenDbConnsCmd.ExecuteNonQuery();
                    deleteDbCmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
                conn.Close();
            }
        }

        public void ClearTable(Type type)
        {
            DocumentStore.Advanced.Clean.DeleteDocumentsFor(type);
        }

        public void Delete<T>(string id) where T : ModelWithIdentity
        {
            using (var session = GetOpenSession())
            {
                session.Delete<T>(id);
                session.SaveChanges();
            }
        }

        public void Delete<T>(List<string> ids) where T : ModelWithIdentity
        {
            using (var session = GetOpenSession())
            {
                ids.ForEach(i => session.Delete<T>(i));
                session.SaveChanges();
            }
        }

        public void Delete<T>(T item) where T : ModelWithIdentity
        {
            using (var session = GetOpenSession())
            {
                session.Delete(item);
                session.SaveChanges();
            }
        }

        public void Delete<T>(List<T> items) where T : ModelWithIdentity
        {
            using (var session = GetOpenSession())
            {
                items.ForEach(i => session.Delete(i));
                session.SaveChanges();
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> query) where T : ModelWithIdentity
        {
            using (var session = GetOpenSession())
            {
                session.DeleteWhere(query);
                session.SaveChanges();
            }
        }

        public async Task<T> UpsertAsync<T>(T thing) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(thing);
                await session.SaveChangesAsync();
            }

            return thing;
        }

        public async Task<List<T>> UpsertAsync<T>(List<T> things) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.StoreObjects(things);
                await session.SaveChangesAsync();
            }

            return things;
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(Expression<Func<T, bool>> query) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.QuerySession())
            {
                return await session.Query<T>().Where(query).FirstOrDefaultAsync();
            }
        }

        public T QuerySingle<T>(Expression<Func<T, bool>> query) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.QuerySession())
            {
                return session.Query<T>().Where(query).Single();
            }
        }

        public List<T> QueryList<T>(Expression<Func<T, bool>> query) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.QuerySession())
            {
                return session.Query<T>().Where(query)?.ToList() ?? new List<T>();
            }
        }

        public async Task<T> GetByIdAsync<T>(string id) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.QuerySession())
            {
                return await session.LoadAsync<T>(id);
            }
        }

        public async Task<List<T>> GetListByIds<T>(List<string> ids) where T : ModelWithIdentity
        {
            using (var session = DocumentStore.QuerySession())
            {
                return (await session.LoadManyAsync<T>(ids.ToArray())).ToList();
            }
        }

        public IDocumentSession GetOpenSession()
        {
            return DocumentStore.OpenSession();
        }

        public IQuerySession GetQuerySession()
        {
            return DocumentStore.QuerySession();
        }
    }
}
