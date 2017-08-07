using System;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace MiddleLayer
{
    class Program
    {
        static ClientsServer _mainServer;

        private delegate void UpdateStatusCallback(string strMessage);

        static void Main(string[] args)
        {
            string _userCommand = String.Empty, _connectionString = String.Empty;
            bool _connected = false, _connected2DB = false, _serverRunning = false;
            SqlConnection _connection = new SqlConnection();
            SqlCommand _command = new SqlCommand();
            SqlDataReader _reader;

            if ((args.Length <= 1) || (args[0] == "-h"))
                ShowHelp();

            try
            {
                while ((_userCommand != "exit") && (_userCommand != "quit"))
                {
                    Console.Write("///> ");
                    _userCommand = Console.ReadLine();

                    switch (_userCommand)
                    {
                        case "command":
                            if (_connected)
                                ExecuteExpression(_connection);
                            break;
                        case "show"://show databases on server
                            if (_connected)
                            {
                                Console.WriteLine("Databases:");
                                _command = new SqlCommand("SELECT * FROM sys.databases;", _connection);
                                _reader = _command.ExecuteReader();
                                while (_reader.Read())
                                    Console.WriteLine(_reader.GetValue(0).ToString());
                                _reader.Close();
                            }
                            else
                                Console.WriteLine("Try to use 'tables' command.");
                            break;
                        case "connect"://connects to server or database on server
                            if (!_connected && !_connected2DB)
                            {
                                Console.Write("Server:");
                                string server = Console.ReadLine();
                                Console.Write("Database:");
                                string db = Console.ReadLine();
                                if (db == String.Empty)
                                {
                                    _connection.ConnectionString = "Data Source=" + server + ";Integrated Security=True";
                                    _connected = true;
                                }
                                else
                                {
                                    _connection.ConnectionString = "Data Source=" + server + ";Initial Catalog=" + db + ";Integrated Security=True";
                                    _connected = true;
                                    _connected2DB = true;
                                }
                                _connection.Open();
                                Console.WriteLine("///\tCONNECTION ESTABLISHED\t///");
                            }
                            else if (_connected && !_connected2DB)
                            {
                                Console.Write("Database:");
                                string db = Console.ReadLine();
                                string connection = "Data Source=" + _connection.DataSource + ";Initial Catalog=" + db + ";Integrated Security=True";
                                _connection.Close();
                                _connection.ConnectionString = connection;
                                _connection.Open();
                                _connected2DB = true;
                                Console.WriteLine("///\tCONNECTION ESTABLISHED\t///");
                            }
                            else
                                Console.WriteLine("Already connected.");
                            break;
                        case "find"://find sql server
                            MSSQL_search();
                            //MySQL_search();
                            //PLSQL_search();
                            break;
                        case "disconnect"://disconnect from server
                            _connection.Close();
                            Console.WriteLine("///\tCONNECTION CLOSED\t///");
                            _connected = false;
                            _connected2DB = false;
                            break;
                        case "tables"://shows tables from database
                            if (_connected2DB)
                            {
                                _command = new SqlCommand("SELECT * FROM information_schema.tables", _connection);
                                _reader = _command.ExecuteReader();
                                while (_reader.Read())
                                    Console.WriteLine(_reader.GetValue(2).ToString());
                                _reader.Close();
                            }
                            else
                                Console.WriteLine("Not connected to database.");
                            break;
                        case "start":
                            if (_connected2DB)
                            {
                                StartServer(_connection);
                                _serverRunning = true;
                            }
                            else
                                Console.WriteLine("Connect first");
                            break;
                        case "stop":
                            if (_serverRunning)
                            {
                                StopServer();
                                _serverRunning = false;
                            }
                            break;
                    }
                }
                Console.Write("Closing...");
            }
            catch (Exception ex)
            { Console.WriteLine("Error! " + ex.Message + "\n" + ex.HResult);
                Console.Write("Closing...");
                Console.Read();
            }
            finally
            { }


            _connection.Close();
            if (_serverRunning)
                StopServer();
            //Console.WriteLine("/// CONNECTION CLOSED ///");
            //Console.Write("\nDONE");
            Thread.Sleep(500);
            //Console.ReadLine();
        }

        static void MSSQL_search()
        {
            //DataTable dt = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();

            Console.WriteLine("\nMS SQL Servers:");
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Console.WriteLine(dr["InstanceName"] + "\trunning on " + dr["ServerName"]);
            //}

            string[] AvailibleSqlServers = SqlLocator.GetServers();
            if (AvailibleSqlServers != null)
                foreach (string NameServer in AvailibleSqlServers)
                    Console.Write(NameServer + Environment.NewLine);
            else
                Console.WriteLine("SQL Servers not found");
        }

        //static void MySQL_search()
        //{
        //    Console.WriteLine("\nMySQL Servers:");

        //    string connectionString = "server=localhost;user=root;database=world;port=3306;password=q23vASB7;";
        //    MySqlConnection _myConnection = new MySqlConnection(connectionString);
        //    try
        //    {
        //        Console.WriteLine("Connecting to MySQL...");
        //        _myConnection.Open();
        //        Console.WriteLine("Successfull connected...");
        //        // Perform database operations
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    _myConnection.Close();
        //    Console.WriteLine("Done.");
        //}

        //static void PLSQL_search()
        //{
        //    Console.WriteLine("\nPL SQL Servers:");
        //}

        static void ShowHelp()
        {
            Console.WriteLine("Commands:\n\tfind\n\tshow\n\tconnect\n\tdisconnect\n\ttables");
        }

        private static void StopServer()
        {
            _mainServer.StopServer();
        }

        private static void StartServer(SqlConnection connection)
        {
            Console.Write("Server IP address(127.0.0.1): ");
            IPAddress _ip;
            string tmp = Console.ReadLine();
            if (tmp == "")
                _ip = IPAddress.Parse("127.0.0.1");
            else
                _ip = IPAddress.Parse(tmp);

            Console.Write("Server IP port(1986): ");
            int _port;
            tmp = Console.ReadLine();
            if (tmp == "")
                _port = 1986;
            else
                _port = Int32.Parse(tmp);
            
            _mainServer = new ClientsServer(_ip, _port);
            //ClientsServer._statusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
            _mainServer.StartListening(connection);
            Console.Write("Monitoring for connections...\r\n");
            
        }

        /*public static void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Call the method that updates the form
            Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }*/

        private static void Invoke(UpdateStatusCallback updateStatusCallback, object[] v)
        {
            throw new NotImplementedException();
        }

        private void UpdateStatus(string strMessage)
        {
            // Updates the log with the message
            Console.WriteLine(strMessage + "\r");
        }

        static void ExecuteExpression(SqlConnection connection)
        {
            string _userCommand = String.Empty;
            SqlCommand _command = new SqlCommand(String.Empty, connection);
            SqlDataReader _reader;

            while (_userCommand != "exit")
            {
                Console.Write("|||> ");
                _userCommand = Console.ReadLine();

                try
                {
                    string[] queryResult = ExecuteQuery(_userCommand, connection);
                    foreach (string str in queryResult)
                        Console.WriteLine(str);
                    //_command.CommandText = _userCommand;
                    //_reader = _command.ExecuteReader();
                    //for (int i = 0; i < _reader.FieldCount; i++)
                    //{
                    //    Console.Write(_reader.GetName(i) + "\t");
                    //}
                    //Console.Write("\n");

                    //while (_reader.Read())
                    //{
                    //    for (int i = 0; i < _reader.FieldCount; i++)
                    //        Console.Write(_reader.GetValue(i) + "\t");
                    //    Console.Write("\n");
                    //}
                    //_reader.Close();
                }
                catch(Exception e)
                {
                    if (_userCommand!="exit")
                        Console.WriteLine(e.Message);
                    continue;
                }
            }
            //connection.Close();
        }

        public static string[] ExecuteQuery(string query, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader;
            List<string> result = new List<string>();

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string tmp = String.Empty;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        tmp += reader.GetValue(i);
                        if (i != reader.FieldCount - 1)
                            tmp += "|";
                    }
                    result.Add(tmp);
                }
                reader.Close();
            }
            catch { }

            return result.ToArray();
        }
    }

    public class SqlLocator
    {
        [System.Runtime.InteropServices.DllImport("odbc32.dll")]
        private static extern short SQLAllocHandle(short hType, IntPtr inputHandle, out IntPtr outputHandle);
        [System.Runtime.InteropServices.DllImport("odbc32.dll")]
        private static extern short SQLSetEnvAttr(IntPtr henv, int attribute, IntPtr valuePtr, int strLength);
        [System.Runtime.InteropServices.DllImport("odbc32.dll")]
        private static extern short SQLFreeHandle(short hType, IntPtr handle);
        [System.Runtime.InteropServices.DllImport("odbc32.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern short SQLBrowseConnect(IntPtr hconn, StringBuilder inString, short inStringLength, StringBuilder outString, short outStringLength, out short outLengthNeeded);

        private const short SQL_HANDLE_ENV = 1;
        private const short SQL_HANDLE_DBC = 2;
        private const int SQL_ATTR_ODBC_VERSION = 200;
        private const int SQL_OV_ODBC3 = 3;
        private const short SQL_SUCCESS = 0;

        private const short SQL_NEED_DATA = 99;
        private const short DEFAULT_RESULT_SIZE = 1024;
        private const string SQL_DRIVER_STR = "DRIVER=SQL SERVER";

        private SqlLocator() { }

        public static string[] GetServers()
        {
            string[] retval = null;
            string txt = String.Empty;
            IntPtr henv = IntPtr.Zero;
            IntPtr hconn = IntPtr.Zero;
            StringBuilder inString = new StringBuilder(SQL_DRIVER_STR);
            StringBuilder outString = new StringBuilder(DEFAULT_RESULT_SIZE);
            short inStringLength = (short)inString.Length;
            short lenNeeded = 0;

            try {
                if (SQL_SUCCESS == SQLAllocHandle(SQL_HANDLE_ENV, henv, out henv))
                {
                    if (SQL_SUCCESS == SQLSetEnvAttr(henv, SQL_ATTR_ODBC_VERSION, (IntPtr)SQL_OV_ODBC3, 0))
                    {
                        if (SQL_SUCCESS == SQLAllocHandle(SQL_HANDLE_DBC, henv, out hconn))
                        {
                            if (SQL_NEED_DATA == SQLBrowseConnect(hconn, inString, inStringLength, outString, DEFAULT_RESULT_SIZE, out lenNeeded))
                            {
                                if (DEFAULT_RESULT_SIZE < lenNeeded)
                                {
                                    outString.Capacity = lenNeeded;
                                    if (SQL_NEED_DATA != SQLBrowseConnect(hconn, inString, inStringLength, outString, lenNeeded, out lenNeeded))
                                    { throw new ApplicationException("Unabled to aquire SQL Servers from ODBC driver."); }
                                }
                                txt = outString.ToString();
                                int start = txt.IndexOf("{") + 1;
                                int len = txt.IndexOf("}") - start;
                                if ((start > 0) && (len > 0))
                                    txt = txt.Substring(start, len);
                                else
                                    txt = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Acqure SQL Server List Error");
                Console.WriteLine(String.Concat(ex.Message, "Acqure SQL Server List Error"));
                txt = string.Empty;
            }
            finally {
                if (hconn != IntPtr.Zero)
                    SQLFreeHandle(SQL_HANDLE_DBC, hconn);
                if (henv != IntPtr.Zero)
                    SQLFreeHandle(SQL_HANDLE_ENV, hconn);
            }

            if (txt.Length > 0)
                retval = txt.Split(",".ToCharArray());

            return retval;
        }
    }
}
