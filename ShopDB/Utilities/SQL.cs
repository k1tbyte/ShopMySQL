using MySqlConnector;
using System;

namespace ShopDB.Utilities
{
    internal enum SQLResponse : byte
    {
        Success,
        Error,
        AlreadyOpened,
        AlreadyClosed,
        ErrorConnectionNotCreated
    }

    internal static class SQL
    {
        public static MySqlConnection Connection  { get; private set; }
        public static MySqlDataReader MySqlReader { get; private set; }



        public static SQLResponse Connect(string connectionString)
        {
            try
            {
                if(Connection == null)
                    Connection = new MySqlConnection(connectionString);

                if (Connection.State == System.Data.ConnectionState.Open)
                    return SQLResponse.AlreadyOpened;
                
                if(Connection.State == System.Data.ConnectionState.Closed)
                    Connection.Open();
                
            }
            catch
            {
                return SQLResponse.Error;
            }

            return SQLResponse.Success;
        }

        public static SQLResponse Disconnect()
        {
            try
            {
                if (Connection == null)
                    return SQLResponse.ErrorConnectionNotCreated;

                if (Connection.State == System.Data.ConnectionState.Closed)
                    return SQLResponse.AlreadyClosed;
                
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }
            catch
            {
                return SQLResponse.Error;
            }
           
            return SQLResponse.Success;
        }

        public static SQLResponse ExecuteCommand(string command,bool buffered = false)
        {
            try
            {
                if (Connection == null)
                    return SQLResponse.ErrorConnectionNotCreated;

                if(Connection.State == System.Data.ConnectionState.Closed)
                    return SQLResponse.AlreadyClosed;

                var сommandToExec = new MySqlCommand(command, Connection)
                {
                    CommandTimeout = 10
                };


                if (buffered)
                {
                    var reader = сommandToExec.ExecuteReader();

                    if (reader.HasRows)
                    {
                        MySqlReader = reader;
                    }
                }
                else
                {
                    сommandToExec.ExecuteNonQuery();
                }


            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                return SQLResponse.Error;
            }

            return SQLResponse.Success;
        }
    }
}
