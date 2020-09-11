using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DataAccessLayer
{
    public class DataAccess
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public enum DefaultType
        {
            All = 1,
            Select = 2,
            Blank = 3, 
            NotItem = 4 //Only DB Element
           
        }
        public DataSet ExecuteSelectQry(string Qry, List<SqlParameter> plist = null)
        {
            using (SqlCommand cmd = new SqlCommand(Qry,con))
            {
                if(plist != null)
                {
                    foreach (SqlParameter p in plist)
                    {
                        cmd.Parameters.Add(p);
                    }
                }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            return ds;
                        }
                    }
                
            }
        }

        public DataSet ExecuteSelectQryNullParam(string Qry)
        {
            using (SqlCommand cmd = new SqlCommand(Qry, con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        da.Fill(ds);
                        return ds;
                    }
                }

            }
        }

        public int CallStoreProcedure(string SpName, List<SqlParameter> plist, ref DataSet dsResult)
        {
            //if (string.IsNullOrEmpty(ConStringName))
            //{
            //    ConStringName = "MyCBUSAConn";
            //}
            
            string ConnectionString = null;
            int SpReturn = 0;

            try
            {
                //ConnectionString = ConfigurationManager.ConnectionStrings[ConStringName].ConnectionString;
                //ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

                //con = new SqlConnection(ConnectionString);
                con.Open();

                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (plist != null)
                {
                    foreach (SqlParameter p in plist)
                    {
                        cmd.Parameters.Add(p);
                    }
                }
                // for Return parameter
                SqlParameter paramreturn = new SqlParameter();
                paramreturn.ParameterName = "@ReturnValue";
                paramreturn.SqlDbType = SqlDbType.Int;
                paramreturn.Direction = ParameterDirection.ReturnValue;
                paramreturn.Value = SpReturn;
                cmd.Parameters.Add(paramreturn);
                cmd.CommandTimeout = 0;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dsResult);

                SpReturn = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return SpReturn;
        }
        public void BindDropDownListFromDB(ref DropDownList ddlSample, DataTable dtResult, string SelectedByValue, string SelectedByText, DefaultType defaultType)
        {

            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                ddlSample.DataSource = dtResult;
                ddlSample.DataTextField = "Name";
                ddlSample.DataValueField = "ID";
                ddlSample.DataBind();
            }
            else
            {
                ddlSample.DataSource = null;
                ddlSample.DataBind();
            }
            if (defaultType != DefaultType.NotItem)
            {
                if (defaultType == DefaultType.All)
                {
                    ddlSample.Items.Insert(0, new ListItem("All", ""));
                }
                else if (defaultType == DefaultType.Select)
                {
                    ddlSample.Items.Insert(0, new ListItem("Select", ""));
                }
                else if (defaultType == DefaultType.Blank)
                {
                    ddlSample.Items.Insert(0, new ListItem("", ""));
                }
            }
            if (!string.IsNullOrEmpty(SelectedByValue))
            {
                ddlSample.ClearSelection();
                ddlSample.Items.FindByValue(SelectedByValue).Selected = true;
            }
            else if (!string.IsNullOrEmpty(SelectedByText))
            {
                ddlSample.ClearSelection();
                ddlSample.Items.FindByText(SelectedByText).Selected = true;
            }

        }
    }
}
