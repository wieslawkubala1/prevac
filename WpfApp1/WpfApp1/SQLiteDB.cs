using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SQLiteDB
    {
        SQLiteConnection dbConnection;
        public SQLiteCommand command;
        string sqlCommand;
        string dbPath = System.Environment.CurrentDirectory + "\\DB";
        string dbFilePath;
        public void createDbFile()
        {
            if (!string.IsNullOrEmpty(dbPath) && !Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);
            dbFilePath = dbPath + "\\database.db";
            if (!System.IO.File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
            }
        }

        public SQLiteConnection createDbConnection()
        {
            if (dbFilePath == null)
                createDbFile();
            string strCon = string.Format("Data Source={0};", dbFilePath);
            dbConnection = new SQLiteConnection(strCon);
            dbConnection.Open();
            command = dbConnection.CreateCommand();
            return dbConnection;
        }

        public void createTables()
        {
            if (!checkIfExist("LatLonPositions"))
            {
                sqlCommand = @"CREATE TABLE LatLonPositions
                                (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    lat varchar(20),
                                    lon varchar(20),
                                    name varchar(300)
                                )";
                executeQuery(sqlCommand);
            }

            if (!checkIfExist("Units"))
            {
                sqlCommand = @"CREATE TABLE Units
                                (
                                    id INTEGER PRIMARY KEY,
                                    key varchar(30),
                                    name varchar(30)
                                )";
                executeQuery(sqlCommand);

                sqlCommand = @"INSERT INTO Units
                                (id, key, name)
                                VALUES(1, 'standard', 'Kelvin')
                                ";
                executeQuery(sqlCommand);
                sqlCommand = @"INSERT INTO Units
                                (id, key, name)
                                VALUES(2, 'metric', 'Celsius')
                                ";
                executeQuery(sqlCommand);
                sqlCommand = @"INSERT INTO Units
                                (id, key, name)
                                VALUES(3, 'imperial', 'Fahrenheit')
                                ";
                executeQuery(sqlCommand);
            }

            if (!checkIfExist("WeatherJsons"))
            {
                sqlCommand = @"CREATE TABLE WeatherJsons
                                (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    generatedAt datetime,
                                    unitId int not null,
                                    positionId int not null,
                                    json varchar(5000),
                                    foreign key(unitId) references Units(id),
                                    foreign key(positionId) references LatLonPositions(id) 
                                )";
                executeQuery(sqlCommand);
            }

            if (!checkIfExist("ForecastJsons"))
            {
                sqlCommand = @"CREATE TABLE ForecastJsons
                                (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    generatedAt datetime,
                                    unitId int not null,
                                    positionId int not null,
                                    rangeDays int,
                                    periodHours int,
                                    json varchar(5000),
                                    foreign key(unitId) references Units(id),
                                    foreign key(positionId) references LatLonPositions(id) 
                                )";
                /*
                 * In free version of Open Weather API forecasts have range equal 5 days and periods: 3 hours
                 */
                executeQuery(sqlCommand);
            }
        }

        public bool checkIfExist(string tableName)
        {
            command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
            var result = command.ExecuteScalar();

            return result != null && result.ToString() == tableName ? true : false;
        }

        public void executeQuery(string sqlCommand)
        {
            SQLiteCommand triggerCommand = dbConnection.CreateCommand();
            triggerCommand.CommandText = sqlCommand;
            triggerCommand.ExecuteNonQuery();
        }

        public bool checkIfTableContainsData(string tableName)
        {
            if (dbFilePath == null)
                createDbFile();

            using (var connection = createDbConnection())
            {

                command.CommandText = "SELECT count(*) FROM " + tableName;
                var result = command.ExecuteScalar();

                return Convert.ToInt32(result) > 0 ? true : false;
            }
        }


        public void fillTableLatLonPositions()
        {
            if (!checkIfTableContainsData("LatLonPositions"))
            {
                sqlCommand = "insert into LatLonPositions (lat, lon, name) values ('35.88', '76.51', 'K2')";
                executeQuery(sqlCommand);
            }
        }
    }
}