using System.Data;

namespace KBT.WebAPI.Training.Example.DatabaseFactory.Factory;

public abstract class Database
{
    protected string _connectionString;

    protected Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public abstract IDbConnection CreateConnection();
    public abstract IDbCommand CreateCommand();
    public abstract IDbConnection CreateOpenConnection();
    public abstract IDbCommand CreateCommand(string commandText, IDbConnection connection);
    public abstract IDbCommand CreateStoredProcCommand(string procName, IDbConnection connection);
    public abstract IDataParameter CreateParameter(string parameterName, object parameterValue);
}