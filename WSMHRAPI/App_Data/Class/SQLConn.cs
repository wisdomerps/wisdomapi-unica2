using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace WSM.Conn
{
    public class SQLConn
    {
        private System.Data.SqlClient.SqlCommand Cmd;
        private System.Data.SqlClient.SqlConnection Cnn;
        private System.Data.SqlClient.SqlTransaction Tran;
        private System.Data.SqlClient.SqlCommand Cmd2;
        private System.Data.SqlClient.SqlConnection Cnn2;
        private System.Data.SqlClient.SqlTransaction Tran2;
        private string ConnString;

        #region " CONNECTTION "


        public void ConnectionString(WSM.Conn.DB.DataBaseName DbName) {

            ConnString = WSM.Conn.DB.ConnectionString(DbName);
        }

        private void SqlConnectionOpen()
        {
            if (Cnn == null) { Cnn = new System.Data.SqlClient.SqlConnection(); };
            if (Cnn.State == ConnectionState.Open)
            {
                Cnn.Close();
            };
            Cnn.ConnectionString = ConnString;
            Cnn.Open();

        }

        public void SqlBeginTransaction()
        {
            if (Cnn == null) { Cnn = new System.Data.SqlClient.SqlConnection(); }

            if (Cnn.State == ConnectionState.Open)
            {
                Cnn.Close();
            };
            Cnn.ConnectionString = ConnString;
            Cnn.Open();
            Cmd = Cnn.CreateCommand();
            Tran = Cnn.BeginTransaction();
        }

        private System.Data.SqlClient.SqlConnection SqlConnectionOpen(System.Data.SqlClient.SqlConnection _cnn)
        {
            if (_cnn == null) { _cnn = new System.Data.SqlClient.SqlConnection(); };
            if (_cnn.State == ConnectionState.Open)
            {
                _cnn.Close();
            };
            _cnn.ConnectionString = "";
            _cnn.Open();
            return _cnn;
        }

        private System.Data.SqlClient.SqlConnection SqlBeginTransaction(System.Data.SqlClient.SqlConnection _cnn)
        {
            if (_cnn == null) { _cnn = new System.Data.SqlClient.SqlConnection(); }

            if (_cnn.State == ConnectionState.Open)
            {
                _cnn.Close();
            };
            _cnn.ConnectionString = "";
            _cnn.Open();
            Tran = _cnn.BeginTransaction();
            return _cnn;
        }

        private System.Data.SqlClient.SqlConnection SqlBeginTransaction(System.Data.SqlClient.SqlConnection _cnn, System.Data.SqlClient.SqlTransaction _tran)
        {
            if (_cnn == null) { _cnn = new System.Data.SqlClient.SqlConnection(); };

            if (_cnn.State == ConnectionState.Open)
            {
                _cnn.Close();
            };
            _cnn.ConnectionString = "";
            _cnn.Open();
            _tran = _cnn.BeginTransaction();
            return _cnn;
        }

        public void CommitTransaction() {
            Tran.Commit();
            DisposeSqlTransaction(Tran);
            DisposeSqlConnection(Cmd);
        }

        public void RollbackTransaction()
        {
            Tran.Rollback();
            DisposeSqlTransaction(Tran);
            DisposeSqlConnection(Cmd);
        }

        public  int ExeTransaction(string sqlcpmmand)
        {
            try
            {
                int Complete = 0;

                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sqlcpmmand;
                Cmd.CommandTimeout = 0;
                Cmd.Transaction = Tran;
                Complete = Cmd.ExecuteNonQuery();
                Cmd.Parameters.Clear();

                return Complete;

            }
            catch (Exception ex)
            {             
                return -1;
            }
        }

        public void DisposeSqlConnection(System.Data.SqlClient.SqlConnection _cnn)
        {
            if ((_cnn != null))
            {
                if (_cnn.State == ConnectionState.Open)
                {
                    _cnn.Close();
                }
                _cnn.Dispose();
            }
        }

        private void DisposeSqlConnection(System.Data.SqlClient.SqlCommand _cmd)
        {
            if ((_cmd != null))
            {
                if ((_cmd.Connection != null))
                {
                    if (_cmd.Connection.State == ConnectionState.Open)
                    {
                        _cmd.Connection.Close();
                    }
                    _cmd.Connection.Dispose();
                }
                _cmd.Dispose();
            }
        }

        private void DisposeSqlConnection(System.Data.SqlClient.SqlDataAdapter _adapter)
        {
            if ((_adapter != null))
            {
                if ((_adapter.SelectCommand != null))
                {
                    if ((_adapter.SelectCommand.Connection != null))
                    {
                        if (!(_adapter.SelectCommand.Connection.State == ConnectionState.Open))
                        {
                            _adapter.SelectCommand.Connection.Close();
                        }
                        _adapter.SelectCommand.Connection.Dispose();
                    }
                    _adapter.SelectCommand.Dispose();
                }
                _adapter.Dispose();
            }
        }

        private void DisposeSqlTransaction(System.Data.SqlClient.SqlTransaction _tran)
        {
            if ((_tran != null))
            {
                if ((_tran.Connection != null))
                {
                    if (_tran.Connection.State == ConnectionState.Open)
                    {
                        _tran.Connection.Close();
                    }
                    _tran.Connection.Dispose();
                }
                _tran.Dispose();
            }
        }

        private void ClearParameterObject(System.Data.SqlClient.SqlCommand _cmd)
        {
            if (_cmd.Parameters.Count > 0)
            {
                _cmd.Parameters.Clear();
            }
        }
        #endregion

        #region "   SQL TRANSACTION    "

        private bool Execute_Tran(string[] QryStr, WSM.Conn.DB.DataBaseName DbName)
        {
            try
            {
                int Complete = 0;

                ConnString = WSM.Conn.DB.ConnectionString(DbName);

                SqlConnectionOpen();
                Cmd = Cnn.CreateCommand();
                Tran = Cnn.BeginTransaction();

                foreach (string Str in QryStr)
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = Str;
                    Cmd.Transaction = Tran;
                    Complete = Cmd.ExecuteNonQuery();
                    Cmd.Parameters.Clear();

                    if (Complete <= 0)
                    {
                        Tran.Rollback();
                        DisposeSqlTransaction(Tran);
                        DisposeSqlConnection(Cmd);
                        return false;
                    }

                }

                Tran.Commit();
                DisposeSqlTransaction(Tran);
                DisposeSqlConnection(Cmd);
                return true;

            }
            catch (Exception ex)
            {
                Tran.Rollback();
                DisposeSqlTransaction(Tran);
                DisposeSqlConnection(Cmd);
                return false;
            }
        }

        private int Execute_Tran(string sqlStr, System.Data.SqlClient.SqlCommand sqlcmd, System.Data.SqlClient.SqlTransaction Tr)
        {
            try
            {
                int Complete = 0;

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sqlStr;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Transaction = Tr;
                Complete = sqlcmd.ExecuteNonQuery();
                sqlcmd.Parameters.Clear();

                return Complete;

            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return -1;
            }
        }

        private int ExecuteTran(string sqlStr, System.Data.SqlClient.SqlCommand sqlcmd, System.Data.SqlClient.SqlTransaction Tr)
        {
            int Complete = 0;
            try
            {

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sqlStr;
                sqlcmd.Transaction = Tr;
                Complete = sqlcmd.ExecuteNonQuery();
                sqlcmd.Parameters.Clear();

                return Complete;

            }
            catch
            {
                return Complete;
            }
        }

        #endregion

        #region "NonTransection"

        public bool ExecuteOnly(string QryStr, WSM.Conn.DB.DataBaseName DbName)
        {

            System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
            System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();

            try
            {
                ConnString = WSM.Conn.DB.ConnectionString(DbName);

                if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                _Cnn.ConnectionString = ConnString;
                _Cnn.Open();
                _Cmd = _Cnn.CreateCommand();
                _Cmd.CommandTimeout = 0;
                _Cmd.CommandType = CommandType.Text;
                _Cmd.CommandText = QryStr;
               
                _Cmd.ExecuteNonQuery();
                _Cmd.Parameters.Clear();

                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
                return true;
            }
            catch (Exception ex)
            {
                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
                //Interaction.MsgBox(ex.Message);
                return false;
            }

        }

        public bool ExecuteData(string QryStr, WSM.Conn.DB.DataBaseName DbName)
        {

            System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
            System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();
            System.Data.SqlClient.SqlTransaction _Tran =null ;


            try
            {
                int Complete = 0;

                ConnString = WSM.Conn.DB.ConnectionString(DbName);

                if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                _Cnn.ConnectionString = ConnString;
                _Cnn.Open();
                _Cmd = _Cnn.CreateCommand();
                _Tran = _Cnn.BeginTransaction();

                _Cmd.CommandType = CommandType.Text;
                _Cmd.CommandText = QryStr;
                _Cmd.Transaction = _Tran;
                Complete = _Cmd.ExecuteNonQuery();
                _Cmd.Parameters.Clear();

                if (Complete <= 0)
                {
                    _Tran.Rollback();
                    DisposeSqlTransaction(_Tran);
                    DisposeSqlConnection(_Cmd);
                    return false;
                }

                _Tran.Commit();
                DisposeSqlTransaction(_Tran);
                DisposeSqlConnection(_Cmd);
                return true;

            }

            catch (Exception ex)
            {
                try { _Tran.Rollback(); } catch { }
              
                DisposeSqlTransaction(_Tran);
                DisposeSqlConnection(_Cmd);
                return false;
            }
        }
        private bool ExecuteNonQuery(ref SqlCommand _Cmd, WSM.Conn.DB.DataBaseName DbName)
        {
            try
            {
                int Complete = 0;

                ConnString = WSM.Conn.DB.ConnectionString(DbName);
                SqlConnectionOpen();
                Tran = Cnn.BeginTransaction();

                _Cmd.Connection = Cnn;
                _Cmd.CommandTimeout = 0;
                _Cmd.Transaction = Tran;
                Complete = _Cmd.ExecuteNonQuery();
                _Cmd.Parameters.Clear();

                if (Complete <= 0)
                {
                    Tran.Rollback();
                    DisposeSqlTransaction(Tran);
                    DisposeSqlConnection(_Cmd);
                    return false;
                }

                Tran.Commit();
                DisposeSqlTransaction(Tran);
                DisposeSqlConnection(_Cmd);
                return true;

            }
            catch (Exception ex)
            {
                Tran.Rollback();
                DisposeSqlTransaction(Tran);
                DisposeSqlConnection(Cmd);
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        private object ExecuteScalar(string QryStr, WSM.Conn.DB.DataBaseName DbName)
        {

            try
            {
                ConnString = WSM.Conn.DB.ConnectionString(DbName);
                SqlConnectionOpen();
                Cmd = Cnn.CreateCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = QryStr;
                return Cmd.ExecuteScalar();

            }
            catch (SqlException ex)
            {
                return null;
            }
            finally
            {
                DisposeSqlConnection(Cmd);
            }

        }

        private object ExecuteScalar(ref SqlCommand _Cmd, WSM.Conn.DB.DataBaseName DbName)
        {

            try
            {
                ConnString = WSM.Conn.DB.ConnectionString(DbName);
                SqlConnectionOpen();

                _Cmd.Connection = Cnn;
                return _Cmd.ExecuteScalar();

            }
            catch (SqlException ex)
            {
                return null;
            }
            finally
            {
                DisposeSqlConnection(_Cmd);
            }

        }
        #endregion

        #region " GETDATA  "

        public DataTable GetDataTable(string QryStr, WSM.Conn.DB.DataBaseName DbName, string TableName = "DataTalble1")
        {
            DataTable objDT = new DataTable(TableName);

            System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
            System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();
            try
            {
                ConnString = WSM.Conn.DB.ConnectionString(DbName);

                if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                _Cnn.ConnectionString = ConnString;
                _Cnn.Open();
                _Cmd = _Cnn.CreateCommand();

                var _Adepter = new SqlDataAdapter(_Cmd);
                _Adepter.SelectCommand.CommandTimeout = 0;
                _Adepter.SelectCommand.CommandType = CommandType.Text;
                _Adepter.SelectCommand.CommandText = QryStr;
                _Adepter.Fill(objDT);
                _Adepter.Dispose();

                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }
            catch (Exception ex)
            {
                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }

            return objDT;
        }

        public void GetDataSet(string QryStr, WSM.Conn.DB.DataBaseName DbName, ref DataSet objDataSet, string DefaultTableName = null)
        {

            //SqlDataAdapter objDA = new SqlDataAdapter(QryStr, WSM.Conn.DB.ConnectionString(DbName));
            //if (DefaultTableName == null)
            //{
            //    objDA.Fill(objDataSet);
            //}
            //else
            //{
            //    objDA.Fill(objDataSet, DefaultTableName);
            //}

            //objDA.Dispose(); 
            System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
            System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();
            try
            {
                ConnString = WSM.Conn.DB.ConnectionString(DbName);

                if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                _Cnn.ConnectionString = ConnString;
                _Cnn.Open();
                _Cmd = _Cnn.CreateCommand();

                var _Adepter = new SqlDataAdapter(_Cmd);
                _Adepter.SelectCommand.CommandTimeout = 0;
                _Adepter.SelectCommand.CommandType = CommandType.Text;
                _Adepter.SelectCommand.CommandText = QryStr;
                _Adepter.Fill(objDataSet);
                _Adepter.Dispose();

                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }
            catch (Exception ex)
            {
                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }

            //return objDataSet;
        }

        public DataTable GetDataTableConectstring(string QryStr, string _ConnectionString, string TableName = "DataTalble1")
        {
            DataTable objDT = new DataTable(TableName);

            System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
            System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();
            try
            {
                ConnString = _ConnectionString;

                if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                _Cnn.ConnectionString = ConnString;
                _Cnn.Open();
                _Cmd = _Cnn.CreateCommand();

                var _Adepter = new SqlDataAdapter(_Cmd);
                _Adepter.SelectCommand.CommandTimeout = 0;
                _Adepter.SelectCommand.CommandType = CommandType.Text;
                _Adepter.SelectCommand.CommandText = QryStr;
                _Adepter.Fill(objDT);
                _Adepter.Dispose();

                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }
            catch (Exception ex)
            {
                DisposeSqlConnection(_Cmd);
                DisposeSqlConnection(_Cnn);
            }

            return objDT;
        }

        public DataTable GetDataTableOnbeginTrans(string QryStr, string DefaultTableName = "DataTalble1")
        {
            DataTable objDT = new DataTable(DefaultTableName);
            SqlCommand _cmd = null;

            try
            {

                if (Tran != null)
                {
                    _cmd = new SqlCommand(QryStr, Cnn, Tran);
                }
                else
                {
                    _cmd = new SqlCommand(QryStr, Cnn);
                }

                var _Adepter = new SqlDataAdapter(_cmd);
                _Adepter.SelectCommand.CommandTimeout = 0;
                _Adepter.Fill(objDT);
                _Adepter.Dispose();

                _cmd.Dispose();

            }
            catch (Exception ex)
            {
                _cmd.Dispose();
            }

            return objDT;

        }

        public string GetField(string strSql, WSM.Conn.DB.DataBaseName DbName, object defaultValue = null)
        {
            DataTable dt = new DataTable();
            string _Value = Convert.ToString(defaultValue);

            try
            {
                dt = GetDataTable(strSql, DbName);


                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow R in dt.Rows)
                    {
                        if (R[0] == DBNull.Value) { }
                        else { _Value = R[0].ToString(); };
                        break;
                    };
                }


            }
            catch (Exception ex)
            {
            }

            dt.Dispose();
            return _Value;
        }

        public string GetFieldConectstring(string strSql, string _ConnecttionString, object defaultValue = null)
        {
            DataTable dt = new DataTable();
            string _Value = Convert.ToString(defaultValue);

            try
            {
                dt = GetDataTableConectstring(strSql, _ConnecttionString);


                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow R in dt.Rows)
                    {
                        if (R[0] == DBNull.Value) { }
                        else { _Value = R[0].ToString(); };
                        break;
                    };
                }


            }
            catch (Exception ex)
            {
            }

            dt.Dispose();
            return _Value;
        }

        public string GetFieldByName(string strSql, WSM.Conn.DB.DataBaseName DbName, string FieldName, object defaultValue = null)
        {
            DataTable dt = new DataTable();
            string _Value = Convert.ToString(defaultValue);

            try
            {
                dt = GetDataTable(strSql, DbName);
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow R in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(FieldName) & dt.Columns.IndexOf(FieldName) >= 0)
                        {
                            if (R[FieldName] == DBNull.Value) { }
                            else { _Value = R[FieldName].ToString(); };
                        }
                        else
                        {
                            if (R[0] == DBNull.Value) { }
                            else { _Value = R[0].ToString(); };
                        }
                        break;
                    }
                }
                else
                {
                    _Value = defaultValue.ToString();
                }

            }
            catch (Exception ex)
            {
            }

            dt.Dispose();
            return _Value;
        }

        public string GetFieldOnBeginTrans(string strSql, WSM.Conn.DB.DataBaseName DbName, object defaultValue = null)
        {
            DataTable dt = new DataTable();
            string _Value = defaultValue.ToString();

            try
            {
                dt = GetDataTableOnbeginTrans(strSql, DbName.ToString());

                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow R in dt.Rows)
                    {

                        if (R[0] == DBNull.Value) { }
                        else { _Value = R[0].ToString(); };
                        break;
                    }
                };


            }
            catch (Exception ex)
            {
            }

            dt.Dispose();
            return _Value;

        }

        public string GetFieldByNameOnBeginTrans(string strSql, WSM.Conn.DB.DataBaseName DbName, string FieldName, object defaultValue = null)
        {
            DataTable dt = new DataTable();
            string _Value = Convert.ToString(defaultValue);

            try
            {
                dt = GetDataTableOnbeginTrans(strSql, DbName.ToString());

                if (dt.Rows.Count != 0)
                {

                    foreach (DataRow R in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(FieldName) & dt.Columns.IndexOf(FieldName) >= 0)
                        {
                            if (R[FieldName] == DBNull.Value) { }
                            else { _Value = R[FieldName].ToString(); };
                        }
                        else
                        {
                            if (R[0] == DBNull.Value) { }
                            else { _Value = R[0].ToString(); };
                        }
                        break;
                    }

                };

            }
            catch (Exception ex)
            {
            }

            dt.Dispose();
            return _Value;

        }

        #endregion
    }
}
