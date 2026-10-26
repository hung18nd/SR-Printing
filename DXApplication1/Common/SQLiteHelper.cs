using System;
using System.Collections.Generic;
using System.Text;
using Finisar.SQLite;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace DXApplication1
{
    static class SQLite
    {
        static string db_file_name_default = Global.ApplicationPath() + "\\DBRegister" + "\\DBDataLogs.db3";
        static string db_file_name_data = Global.ApplicationPath() + "\\AppData.db3";
        static bool db_busy = false;
        static public bool ExecuteNonQuery(string db_file_name, string query_string)
        {
            string connString = "Data Source=" + db_file_name + ";Version=3;New=False;Compress=True;";
            bool bCommOK = false;
            while (db_busy) Thread.Sleep(1);
            db_busy = true;
            try
            {
                SQLiteConnection myConnection = new SQLiteConnection(connString);
                myConnection.Open();
                SQLiteCommand myCommand = new SQLiteCommand(query_string, myConnection);
                myCommand.ExecuteNonQuery();
                myConnection.Close();
                bCommOK = true;
            }
            catch (Exception ex)
            {
                Debug.Print("[ExecuteNonQuery] - [" + query_string + "] - " + ex.ToString());
                //MessageBox.Show("[ExecuteNonQuery] - [" + query_string + "] - " + ex.ToString());
                Global.WriteLogFile("[ExecuteNonQuerySQLite] - [" + query_string + "] - " + ex.ToString());
            }
            db_busy = false;
            return bCommOK;
        }

        static public bool ExecuteNonQuery(string query_string)
        {
            return ExecuteNonQuery(db_file_name_default, query_string);
        }

        static public bool ExecuteNonQueryData(string query_string)
        {
            return ExecuteNonQuery(db_file_name_data, query_string);
        }

        static public DataTable ExecuteDataTable(string db_file_name, string query_string)
        {
            DataTable data_table = null;
            string connString = "Data Source=" + db_file_name + ";Version=3;New=False;Compress=True;";
            try
            {
                SQLiteConnection myConnection = new SQLiteConnection(connString);
                myConnection.Open();
                SQLiteDataAdapter myAdapter = new SQLiteDataAdapter();
                myAdapter.SelectCommand = new SQLiteCommand(query_string, myConnection);
                DataSet dataSet = new DataSet();
                try
                {
                    myAdapter.Fill(dataSet);
                }
                catch (SQLiteException ex)
                {
                    Debug.Print("[ExecuteDataTable/myAdapter.Fill(dataSet)] - [" + query_string + "] - " + ex.ToString());
                    //MessageBox.Show("[ExecuteDataTable/myAdapter.Fill(dataSet)] - [" + query_string + "] - " + ex.ToString());
                    Global.WriteLogFile("[ExecuteDataTableSQLite / myAdapter.Fill(dataSet)] - [" + query_string + "] - " + ex.ToString());
                }

                if (dataSet.Tables.Count > 0)
                    data_table = dataSet.Tables[0];
                myConnection.Close();
            }
            catch (SQLiteException ex)
            {
                Debug.Print("[ExecuteDataTable] - [" + query_string + "] - " + ex.ToString());
                //MessageBox.Show("[ExecuteDataTable] - [" + query_string + "] - " + ex.ToString());
                Global.WriteLogFile("[ExecuteDataTableSQLite] - [" + query_string + "] - " + ex.ToString());
            }
            if (data_table == null) data_table = new DataTable();
            return data_table;
        }

        static public DataTable ExecuteDataTable(string query_string)
        {
            return ExecuteDataTable(db_file_name_default, query_string);
        }

        static public DataTable ExecuteDataTableData(string query_string)
        {
            return ExecuteDataTable(db_file_name_data, query_string);
        }

        static public object ExecuteScalar(string db_file_name, string query_string)
        {
            string connString = "Data Source=" + db_file_name + ";Version=3;New=False;Compress=True;";
            object retOBJ = null;
            while (db_busy) Thread.Sleep(1);
            db_busy = true;
            try
            {
                SQLiteConnection myConnection = new SQLiteConnection(connString);
                myConnection.Open();
                SQLiteCommand myCommand = new SQLiteCommand(query_string, myConnection);
                retOBJ = myCommand.ExecuteScalar();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Debug.Print("[ExecuteScalar] - [" + query_string + "] - " + ex.ToString());
                MessageBox.Show("[ExecuteScalar] - [" + query_string + "] - " + ex.ToString());
            }
            db_busy = false;
            return retOBJ;
        }

        static public object ExecuteScalar(string query_string)
        {
            return ExecuteScalar(db_file_name_default, query_string);
        }
		
    }
}
