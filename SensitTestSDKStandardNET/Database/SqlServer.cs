using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Common;

namespace Sensit.TestSDK.Database
{
    /// <summary>
    /// Used for connecting with and using the database functionality
    /// </summary>
    /// <remarks>Inspired by: http://csharp.net-informations.com/data-providers/csharp-sqlcommand-executereader.htm </remarks>
    /// <remarks>Inspired by: https://stackoverflow.com/questions/1202935/convert-rows-from-a-data-reader-into-typed-results </remarks>
    public class SqlServer
    {

        private string _server; // Database Server instance
        private string _database; // Name of Database that you're connecting to
        private string _username; // Username
        private string _password; // Password

        /// <summary>
        /// Name of Server instance for the Database
        /// </summary>
        public string Server
        {
            get => _server;
            set => _server = value;
        }

        /// <summary>
        /// Name of Database that you're connecting to
        /// </summary>
        public string Database
        {
            get => _database;
            set => _database = value;
        }

        /// <summary>
        /// Username of Database
        /// </summary>
        public string Username
        {
            get => _username;
            set => _username = value;
        }

        /// <summary>
        /// Password for Database
        /// </summary>
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        /// <summary>
        /// Used to test the connection to the database with the provided credentials from the application settings
        /// </summary>
        public void CheckConnection(DbProviderFactory factory)
        {
            DbConnection cnn = factory.CreateConnection();
            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection
            Console.WriteLine("Connection opened successfully");
        }

        /// <summary>
        /// Inserts a product into the Test Suites table in the database
        /// </summary>
        /// <param name="product">Product that you want to add</param>
        public void InsertIntoTestSuites(DbProviderFactory factory, String product)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "Insert into TestSuites (Product) values (\'" + product + "\');";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Test Cases table in the database
        /// </summary>
        /// <param name="name">Name of Test Case</param>
        /// <param name="objective">Objective</param>
        /// <param name="owner">Owner</param>
        /// <param name="estimatedTime">Estimated Time</param>
        /// <param name="product">Project being tested</param>
        public void InsertIntoTestCases(DbProviderFactory factory, String name, String objective, String owner, String estimatedTime,
            String product, String testCaseNumber)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into TestCases (Name,Objective,Owner,EstimatedTime,TestSuiteID,TestCaseNumber) Values (\'" + name + "\',\'" +
                  objective + "\',\'" + owner + "\',\'" + estimatedTime + "\',(select TestSuiteID from TestSuites where Product = \'" + product + "\')," + Int32.Parse(testCaseNumber) + ");";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Device Under Tests table in the database
        /// </summary>
        public void InsertIntoDeviceUnderTests(DbProviderFactory factory)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert DeviceUnderTests Default Values;";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Test Runs table in the database
        /// </summary>
        /// <param name="date">Date of Test Run</param>
        /// <param name="tester">Tester</param>
        /// <param name="notes">Notes</param>
        /// <param name="issue">Issues from test run</param>
        /// <param name="status">Status of results</param>
        public void InsertIntoTestRuns(DbProviderFactory factory, String date, String tester, String notes, String issue, String status, int testCaseID)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into TestRuns (Date,Tester,TestCaseID,Notes,Issue,Status,EnvironmentID) Values (\'" + date +
                  "\',\'" + tester + "\'," + testCaseID + ",\'" + notes + "\',\'" + issue +
                  "\',\'" + status + "\',(select MAX(EnvironmentID) from DeviceUnderTests));";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Device Componenets table in the database
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        public void InsertIntoDeviceComponents(DbProviderFactory factory, String name, String version)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into DeviceComponents(Name,Version,EnvironmentID) Values ('" + name + "','" + version +
                  "',(select MAX(EnvironmentID) from DeviceUnderTests));";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Equipment table in the database
        /// </summary>
        /// <param name="name">Name of Equipment</param>
        /// <param name="quantity">Quantity</param>
        public void InsertIntoEquipment(DbProviderFactory factory, String name, String quantity)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into Equipment (Name,Quantity,TestSuiteID) Values (\'" + name + "\',\'" + quantity +
                  "\',(select MAX(TestSuiteID) from TestSuites));";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// Complete insertion for the Test Steps table in the database
        /// </summary>
        /// <param name="step">Step</param>
        /// <param name="expectedResult">Expected Result</param>
        public void InsertIntoTestSteps(DbProviderFactory factory, String step, int TestCaseID, String expectedResult, String sequence)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into TestSteps (Step,ExpectedResult,TestCaseID,Sequence) Values (\'" + step +
                  "\',\'" + expectedResult +
                  "\'," + TestCaseID +",\'" + sequence +"\'); ";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
            
        }

        /// <summary>
        /// Complete insertion for the Test Step Results table
        /// </summary>
        /// <param name="actualResult">Actual Result</param>
        /// <param name="status">Status of step</param>
        public void InsertIntoTestStepResults(DbProviderFactory factory, String actualResult, String status, int stepID, int runID)
        {
            DbConnection cnn = factory.CreateConnection();
            string sql = null;

            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            sql = "insert into TestStepResults (ActualResult,Status,TestRunID,TestStepID) Values (\'" + actualResult + "\',\'" +
                  status + "\'," + runID + "," + stepID + ");";

            command.CommandText = sql;

            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }

        /// <summary>
        /// A modular query that allows you to query anything you want from the database and return the result to the console.  Used for testing purposes.
        /// </summary>
        /// <param name="query">Query you want to execute</param>
        /// <param name="table">Name of Database table you're querying from (if you're looking for just the ID parameter from a certain table then use "ID")</param>
        /// <param name="nameOfId">Normally left empty unless you're using the ID table in which case you need to specify the name of the ID that you're expecting</param>
        public string ModularQueryWithResult(DbProviderFactory factory, String query, string table, string nameOfId)
        {

            DbConnection cnn = factory.CreateConnection();
            cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            command.CommandText = query;

            DbDataReader reader;
            reader = command.ExecuteReader();
            ArrayList objs = new ArrayList();

            switch (table)
            {
                case "Device Components":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            DeviceUnderTestID = reader["DeviceUnderTestID"],
                            Name = reader["Name"],
                            Version = reader["Version"],
                            EnvironmentID = reader["EnvironmentID"]
                        });
                    }

                    break;

                case "DeviceUnderTests":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            EnvironmentID = reader["EnvironmentID"]
                        });
                    }

                    break;

                case "Equipment":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            EquipmentID = reader["EquipmentID"],
                            Name = reader["Name"],
                            Quantity = reader["Quantity"],
                            TestSuiteID = reader["TestSuiteID"]
                        });
                    }

                    break;

                case "TestCases":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            TestCaseID = reader["TestCaseID"],
                            Name = reader["Name"],
                            Objective = reader["Objective"],
                            Owner = reader["Owner"],
                            EstimatedTime = reader["EstimatedTime"],
                            TestSuiteID = reader["TestSuiteID"],
                            TestCaseNumber = reader["TestCaseNumber"]
                        });
                    }

                    break;

                case "TestRuns":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            TestRunID = reader["TestRunID"],
                            Date = reader["Date"],
                            Tester = reader["Tester"],
                            TestCaseID = reader["TestCaseID"],
                            Notes = reader["Notes"],
                            Issue = reader["Issue"],
                            Status = reader["Status"],
                            EnvironmentID = reader["EnvironmentID"]
                        });
                    }

                    break;

                case "TestStepResults":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            TestStepResultID = reader["TestStepResultID"],
                            ActualResult = reader["ActualResult"],
                            Status = reader["Status"],
                            TestRunID = reader["TestRunID"]
                        });
                    }

                    break;

                case "TestSteps":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            TestStepID = reader["TestStepID"],
                            Step = reader["Step"],
                            ExpectedResult = reader["ExpectedResult"],
                            TestCaseID = reader["TestCaseID"],
                            Sequence = reader["Sequence"]
                        });
                    }

                    break;

                case "TestSuites":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            TestSuiteID = reader["TestSuiteID"],
                            Product = reader["Product"]
                        });
                    }

                    break;

                case "ID":
                    while (reader.Read())
                    {
                        objs.Add(new
                        {
                            nameOfId = reader[nameOfId]
                        });
                    }

                    break;

            }

            command.Dispose();
            cnn.Close();
            return JsonConvert.SerializeObject(objs);
        }


        /// <summary>
        /// A modular query that will not return anything when called.  Used for queries such as update, insert, delete, patch
        /// </summary>
        /// <param name="query">Query to be executed</param>
        public void ModularQueryNoResult(DbProviderFactory factory, String query)
        {
            DbConnection cnn = factory.CreateConnection();
           cnn.ConnectionString = "Data Source=" + _server + ";Initial Catalog=" + _database + ";User ID=" + _username +
                               ";Password=" + _password; //Sql Server connection string style
            cnn.Open(); // Opens connection

            DbCommand command = factory.CreateCommand();
            command.Connection = cnn;
            command.CommandText = query;
            command.ExecuteNonQuery();
            command.Dispose();
            cnn.Close();
            Console.WriteLine("Query executed successfully");
        }
    }
}
