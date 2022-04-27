using System.Data;
using AutoMapper;
using KBT.WebAPI.Training.Example.DatabaseFactory.Factory;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.WebAPI.Repositories.Interfaces;

namespace KBT.WebAPI.Training.Example.WebAPI.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(UserRepository));

    private static Database _database;
    private static IMapper _mapper;
    private static string _tableName = "[User]";

    public UserRepository(Database database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public IEnumerable<User> GetUsers()
    {
        IDbConnection connection = null;
        try
        {
            List<User> listObj = new List<User>();
            using (connection = _database.CreateOpenConnection())
            {
                string commandText = $"SELECT * FROM {_tableName}";
                using (IDbCommand command = _database.CreateCommand(commandText, connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User obj = new User();
                            obj = _mapper.Map<User>(reader);
                            listObj.Add(obj);
                        }
                    }
                }
            }

            return listObj;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
            throw ex;
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public User? GetUserById(int userKey)
    {
        IDbConnection connection = null;
        try
        {
            User obj = null;
            using (connection = _database.CreateOpenConnection())
            {
                string commandText = $"SELECT * FROM {_tableName} WHERE UserKey=@UserKey";
                using (IDbCommand command = _database.CreateCommand(commandText, connection))
                {
                    command.Parameters.Add(_database.CreateParameter("@UserKey", userKey));
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = _mapper.Map<User>(reader);
                        }
                    }
                }
            }

            return obj;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
            throw ex;
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public int? Insert(User user)
    {
        IDbConnection connection = null;
        try
        {
            using (connection = _database.CreateOpenConnection())
            {
                string sqlCommand =
                    $"INSERT INTO {_tableName} (UserName, Password, IsActive, EmployeeKey)\n" +
                    $"VALUES (@UserName, @Password, @IsActive, @EmployeeKey);\n" +
                    $"SELECT SCOPE_IDENTITY();";

                using (IDbCommand command = _database.CreateCommand(sqlCommand, connection))
                {
                    command.Parameters.Add(_database.CreateParameter("@UserName", user.UserName));
                    command.Parameters.Add(_database.CreateParameter("@Password", user.Password));
                    command.Parameters.Add(_database.CreateParameter("@IsActive", user.IsActive ?? (object)DBNull.Value));
                    command.Parameters.Add(_database.CreateParameter("@EmployeeKey", user.EmployeeKey ?? (object)DBNull.Value));

                    var newId = command.ExecuteScalar();
                    if (connection.State == ConnectionState.Open) connection.Close();
                    if (newId == null) return null;
                    return Convert.ToInt32(newId);
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
            throw ex;
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public User Update(int userKey, User user)
    {
        IDbConnection connection = null;
        try
        {
            using (connection = _database.CreateOpenConnection())
            {
                string sqlCommand = $"UPDATE {_tableName}\n" +
                                    $"SET UserName=@UserName, Password=@Password, IsActive=@IsActive, EmployeeKey=@EmployeeKey\n" +
                                    $"WHERE UserKey=@UserKey";

                using (IDbCommand command = _database.CreateCommand(sqlCommand, connection))
                {
                    command.Parameters.Add(_database.CreateParameter("@UserName", user.UserName));
                    command.Parameters.Add(_database.CreateParameter("@Password", user.Password));
                    command.Parameters.Add(_database.CreateParameter("@IsActive", user.IsActive ?? (object)DBNull.Value));
                    command.Parameters.Add(_database.CreateParameter("@EmployeeKey", user.EmployeeKey ?? (object)DBNull.Value));
                    command.Parameters.Add(_database.CreateParameter("@UserKey", userKey));
                    command.ExecuteNonQuery();
                }
            }

            User updatedObj = GetUserById(userKey);
            return updatedObj;
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
            throw ex;
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    public void Delete(int userKey)
    {
        IDbConnection connection = null;
        try
        {
            using (connection = _database.CreateOpenConnection())
            {
                string sqlCommand = $"DELETE FROM {_tableName}\n" +
                                    $"WHERE UserKey=@UserKey";

                using (IDbCommand command = _database.CreateCommand(sqlCommand, connection))
                {
                    command.Parameters.Add(_database.CreateParameter("@UserKey", userKey));
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
            throw ex;
        }
        finally
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}