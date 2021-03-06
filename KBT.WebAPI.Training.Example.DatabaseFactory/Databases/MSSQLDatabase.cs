using System.Data;
using KBT.WebAPI.Training.Example.DatabaseFactory.Factory;
using Microsoft.Data.SqlClient;

namespace KBT.WebAPI.Training.Example.DatabaseFactory.Databases;

public class MSSQLDatabase : Database
{
    public override IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public override IDbCommand CreateCommand()
    {
        return new SqlCommand();
    }

    public override IDbConnection CreateOpenConnection()
    {
        SqlConnection connection = (SqlConnection)CreateConnection();
        connection.Open();

        return connection;
    }

    public override IDbCommand CreateCommand(string commandText, IDbConnection connection)
    {
        SqlCommand command = (SqlCommand)CreateCommand();

        command.CommandText = commandText;
        command.Connection = (SqlConnection)connection;
        command.CommandType = CommandType.Text;

        return command;
    }

    public override IDbCommand CreateStoredProcCommand(string procName, IDbConnection connection)
    {
        SqlCommand command = (SqlCommand)CreateCommand();

        command.CommandText = procName;
        command.Connection = (SqlConnection)connection;
        command.CommandType = CommandType.StoredProcedure;

        return command;
    }

    public override IDataParameter CreateParameter(string parameterName, object parameterValue)
    {
        return new SqlParameter(parameterName, parameterValue);
    }

    public MSSQLDatabase(string connectionString) : base(connectionString)
    {
    }
}