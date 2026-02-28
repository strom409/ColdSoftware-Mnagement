using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Reflection;

namespace ColdStoreManagement.DAL.Helper
{
    public sealed class SQLHelperCore
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor that accepts a connection string directly
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SQLHelperCore(string connectionString)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString)
                ? throw new ArgumentNullException(nameof(connectionString))
                : connectionString;
        }



        #region ---------- Core Helpers ----------

        /// <summary>
        /// This method is used to attach an array of SqlParameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            ArgumentNullException.ThrowIfNull(command);
            if (commandParameters != null)
            {
                foreach (var p in commandParameters.Where(p => p != null))
                {
                    if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
            }
        }

        /// <summary>
        /// Assigns an array of values to an array of SqlParameters.
        /// </summary>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if (commandParameters == null || parameterValues == null) return;

            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            for (int i = 0; i < commandParameters.Length; i++)
            {
                commandParameters[i].Value = parameterValues[i] switch
                {
                    IDbDataParameter paramInstance when paramInstance.Value == null => DBNull.Value,
                    null => DBNull.Value,
                    _ => parameterValues[i]
                };
            }
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, 
            SqlTransaction transaction, CommandType commandType, string commandText,
            SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(commandText);

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        #endregion ---------- Core Helpers ----------

        #region ExecuteNonQuery (Async)

        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText)
        {
            return await ExecuteNonQueryAsync(commandType, commandText, (SqlParameter[])null);
        }

        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, 
            params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException(nameof(_connectionString));

            await using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(); // Use OpenAsync() for non-blocking connection

            return await ExecuteNonQueryAsync(connection, commandType, commandText, commandParameters);
        }
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string sql,
           SqlTransaction transaction, params SqlParameter[] parameters)
        {
            using var cmd = new SqlCommand(sql, transaction.Connection!, transaction)
            {
                CommandType = commandType
            };

            if (parameters?.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return await cmd.ExecuteNonQueryAsync();
        }


        public static async Task<int> ExecuteNonQueryAsync(SqlConnection connection, 
            CommandType commandType, string commandText)
        {
            return await ExecuteNonQueryAsync(connection, commandType, commandText, (SqlParameter[])null);
        }
        private static async Task<int> ExecuteNonQueryAsync(SqlConnection connection,
            CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            await using SqlCommand cmd = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };

            if (commandParameters != null)
            {
                foreach (var param in commandParameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value });
                }
            }

            return await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<int> ExecuteNonQueryAsync(SqlTransaction transaction,
            CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null || transaction.Connection == null)
                throw new ArgumentException("Transaction is null or has been committed/rolled back.", nameof(transaction));

            await using SqlCommand cmd = new SqlCommand(commandText, transaction.Connection, transaction)
            {
                CommandType = commandType
            };

            if (commandParameters != null)
            {
                foreach (var param in commandParameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value });
                }
            }

            return await cmd.ExecuteNonQueryAsync(); // ✅ Use ExecuteNonQueryAsync()
        }

        #endregion ExecuteNonQuery (Async)

        #region TVP Support (Async)
        public async Task<int> ExecuteNonQueryAsync(string spName, string parameterName, DataTable tvp, string typeName, params SqlParameter[] otherParameters)
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using SqlCommand cmd = new SqlCommand(spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlParameter parameter = cmd.Parameters.AddWithValue(parameterName, tvp);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = typeName;

            if (otherParameters != null)
            {
                cmd.Parameters.AddRange(otherParameters);
            }

            return await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        #region ---------- ExecuteScalar ----------

        public async Task<object?> ExecuteScalarAsync(string commandText, CommandType commandType,
            params SqlParameter[] commandParameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };

            if (commandParameters != null && commandParameters.Length > 0)
            {
                foreach (var param in commandParameters)
                {
                    cmd.Parameters.Add(param);
                }
            }

            return await cmd.ExecuteScalarAsync();
        }
        public async Task<T?> ExecuteScalarAsync<T>(string commandText, CommandType commandType,
            params SqlParameter[]? commandParameters)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };

            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }

            var result = await cmd.ExecuteScalarAsync();

            // Handle DBNull and Null results safely
            if (result == null || result == DBNull.Value)
            {
                return default;
            }

            // Attempt to convert the result to the requested type T
            try
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (InvalidCastException)
            {
                // Fallback for types that Convert.ChangeType might struggle with (like Guid)
                return (T)result;
            }
            finally
            { 
                cmd.Parameters.Clear();
                connection.Close();                
            }
        }

        #endregion ---------- End ExecuteScalar ----------


        #region ---------- ExecuteReader (Generic) ----------
        /// <summary>
        /// Execute a query and return the first result without parameters
        /// </summary>
        public async Task<T?> ExecuteSingleAsync<T>(string commandText, CommandType commandType) where T : new()
        {
            return await ExecuteSingleAsync<T>(commandText, commandType, parameters: null);
        }

        /// <summary>
        /// Execute a query and return the first result with optional parameters
        /// </summary>
        public async Task<T?> ExecuteSingleAsync<T>(string commandText, CommandType commandType,
             params SqlParameter[] parameters)
             where T : new()
        {
            var list = await ExecuteReaderAsync<T>(commandText, commandType, parameters);
            return list != null ? list.FirstOrDefault() : default;
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string commandText, CommandType commandType,
             params SqlParameter[] parameters)
             where T : new()
        {
            var result = new List<T>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            await using var reader = await command.ExecuteReaderAsync();

            // Cache properties for performance
            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name.ToLower());

            while (await reader.ReadAsync())
            {
                var item = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i).ToLower();

                    if (!props.TryGetValue(columnName, out var prop))
                        continue;

                    if (reader.IsDBNull(i))
                        continue;

                    var value = reader.GetValue(i);
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType)
                                     ?? prop.PropertyType;

                    try
                    {
                        if (targetType.IsEnum)
                        {
                            prop.SetValue(item, Enum.ToObject(targetType, value));
                        }
                        else
                        {
                            prop.SetValue(item, Convert.ChangeType(value, targetType));
                        }
                    }
                    catch
                    {
                        // optional: log mapping error
                    }
                }

                result.Add(item);
            }

            return result;
        }

        #endregion


        #region ExecuteDataset
        public async Task<DataSet> ExecuteDatasetAsync(CommandType commandType, string commandText, 
            params SqlParameter[] commandParameters)
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();
            return await ExecuteDatasetAsync(connection, commandType, commandText, commandParameters);
        }
        public async Task<DataSet> ExecuteDatasetAsync(string commandText, CommandType commandType,
            params SqlParameter[] commandParameters)
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();
            return await ExecuteDatasetAsync(connection, commandType, commandText, commandParameters);
        }
        private static async Task<DataSet> ExecuteDatasetAsync(SqlConnection connection, 
            CommandType commandType, string commandText, 
            params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            using (SqlCommand cmd = new SqlCommand(commandText, connection))
            {
                cmd.CommandType = commandType;
                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    await Task.Run(() => da.Fill(ds)); // Using Task.Run to keep Fill operation async-compatible
                    return ds;
                }
            }
        }

        public async Task<DataSet> ExecutesDatasetAsync(string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(_connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return await ExecuteDatasetAsync(CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return await ExecuteDatasetAsync(CommandType.StoredProcedure, spName);
            }
        }
        public async Task<DataSet> ExecuteTransactionDatasetAsync(SqlTransaction transaction,
            CommandType commandType, string commandText)
        {
            return await ExecuteTransactionDatasetAsync(transaction, commandType, commandText, (SqlParameter[])null);
        }
        public static async Task<DataSet> ExecuteTransactionDatasetAsync(SqlTransaction transaction, 
            CommandType commandType, string commandText,
            params SqlParameter[] commandParameters)
        {
            if (transaction == null || transaction.Connection == null) throw new ArgumentException("The transaction is not valid or has been committed/rolled back.", nameof(transaction));

            using (SqlCommand cmd = new SqlCommand())
            {
                bool mustCloseConnection;
                PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    await Task.Run(() => da.Fill(ds));
                    cmd.Parameters.Clear();
                    return ds;
                }
            }
        }

        public static async Task<DataSet> ExecuteDatasetAsync(SqlTransaction transaction, 
            string spName, params object[] parameterValues)
        {
            if (transaction == null || transaction.Connection == null) throw new ArgumentException("The transaction is not valid or has been committed/rolled back.", nameof(transaction));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return await ExecuteTransactionDatasetAsync(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return await ExecuteTransactionDatasetAsync(transaction, CommandType.StoredProcedure, spName);
            }
        }


        public sealed class SqlHelperParameterCache
        {
            #region private methods, variables, and constructors

            //Since this class provides only static methods, make the default constructor private to prevent 
            //instances from being created with "new SqlHelperParameterCache()"
            private SqlHelperParameterCache() { }

            private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

            /// <summary>
            /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
            /// </summary>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
            /// <returns>The parameter array discovered.</returns>
            private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, 
                string spName, bool includeReturnValueParameter)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
                connection.Close();

                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                // Init the parameters with a DBNull value
                foreach (SqlParameter discoveredParameter in discoveredParameters)
                {
                    discoveredParameter.Value = DBNull.Value;
                }
                return discoveredParameters;
            }

            /// <summary>
            /// Deep copy of cached SqlParameter array
            /// </summary>
            /// <param name="originalParameters"></param>
            /// <returns></returns>
            private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
            {
                SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

                for (int i = 0, j = originalParameters.Length; i < j; i++)
                {
                    clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
                }

                return clonedParameters;
            }

            #endregion private methods, variables, and constructors

            #region caching functions

            /// <summary>
            /// Add parameter array to the cache
            /// </summary>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="commandText">The stored procedure name or T-SQL command</param>
            /// <param name="commandParameters">An array of SqlParamters to be cached</param>
            public static void CacheParameterSet(string connectionString, string commandText, 
                params SqlParameter[] commandParameters)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

                string hashKey = connectionString + ":" + commandText;

                paramCache[hashKey] = commandParameters;
            }

            /// <summary>
            /// Retrieve a parameter array from the cache
            /// </summary>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="commandText">The stored procedure name or T-SQL command</param>
            /// <returns>An array of SqlParamters</returns>
            public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

                string hashKey = connectionString + ":" + commandText;

                SqlParameter[]? cachedParameters = paramCache[hashKey] as SqlParameter[];
                if (cachedParameters == null)
                {
                    return null;
                }
                else
                {
                    return CloneParameters(cachedParameters);
                }
            }

            #endregion caching functions

            #region Parameter Discovery Functions

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
            {
                return GetSpParameterSet(connectionString, spName, false);
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
                }
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
            {
                return GetSpParameterSet(connection, spName, false);
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
                {
                    return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
                }
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                ArgumentNullException.ThrowIfNull(connection);
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

                SqlParameter[]? cachedParameters;

                cachedParameters = paramCache[hashKey] as SqlParameter[];
                if (cachedParameters == null)
                {
                    SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                    paramCache[hashKey] = spParameters;
                    cachedParameters = spParameters;
                }

                return CloneParameters(cachedParameters);
            }

            #endregion Parameter Discovery Functions
        }

        #endregion
    }
}
