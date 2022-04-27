using System.Data;
using KBT.WebAPI.Training.Example.DatabaseFactory.Databases;

namespace KBT.WebAPI.Training.Example.DatabaseFactory.Factory;

public sealed class DBFactory
{
    private DBFactory()
    {
    }

    public static Database CreateDatabase(string type, string connectionString)
    {
        try
        {
            Database createdObject = null;
            switch (type)
            {
                case "MSSQL":
                    createdObject = new MSSQLDatabase(connectionString);
                    break;
                case "Oracle":
                    createdObject = new OracleDatabase(connectionString);
                    break;
                default:
                    throw new Exception($"Not support this database type : {type}");
                    break;
            }

            // Pass back the instance as a Database
            return createdObject;
        }
        catch (Exception excep)
        {
            throw new Exception("Error instantiating database . " + excep.Message);
        }
    }
}