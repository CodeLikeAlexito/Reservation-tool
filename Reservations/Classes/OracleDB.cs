using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;

namespace Reservations.Classes
{
    public class OracleDB
    {
        private OracleConnection _conn = null;

        private string connectSettings = null;

        protected bool isDBConnectionOpened = false;

        protected OracleDB()
        {
            connectSettings = ConfigurationManager.ConnectionStrings["DBConnSelect"].ToString();
        }

        protected OracleDB(string connString)
        {
            connectSettings = ConfigurationManager.ConnectionStrings[connString].ToString();
        }

        protected OracleConnection GetDBConnection()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
                openDBConnection();

            return _conn;
        }

        protected void closeDBConnection()
        {
            if (_conn != null)
                _conn.Close();

            _conn = null;
            isDBConnectionOpened = false;

        }

        protected void openDBConnection()
        {
            _conn = new OracleConnection(connectSettings);

            _conn.Open();

            isDBConnectionOpened = true;
        }

        protected OracleParameter GetStringParameter(string value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.String;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetIntParameter(int? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Int32;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetEnumParameter(Enum value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Int32;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetFloatParameter(float? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Double;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetDoubleParameter(double value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Double;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetDecimalParameter(decimal? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Double;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetByteParameter(byte? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.Byte;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetCharParameter(char? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.String;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetDateParameter(DateTime? value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.DateTime;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected OracleParameter GetNotNullableDateParameter(DateTime value, string codbColumn)
        {
            OracleParameter op9 = new OracleParameter();
            op9.DbType = DbType.DateTime;
            op9.Value = value;
            op9.ParameterName = codbColumn;

            return op9;
        }

        protected int GetNextval(string seqName)
        {
            int code = -1;
            OracleCommand cmd = new OracleCommand("", GetDBConnection());

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("select {0}.Nextval as newval from dual ", seqName);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    code = Convert.ToInt32(reader["newval"]);
                }
            }

            return code;
        }
    }
}