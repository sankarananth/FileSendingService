using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileSendingService
{
	
	public class DataLayer
	{
		
		public FileLocationFetchModel GetfileInformation()
		{
			FileLocationFetchModel fdModel = new FileLocationFetchModel();
			try
			{
				using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fileDbConnection"].ConnectionString))
				{
					SqlDataAdapter daFetch = new SqlDataAdapter("Get_FileInformation", conn);
					daFetch.SelectCommand.CommandType = CommandType.StoredProcedure;
					DataTable dt = new DataTable();
					daFetch.Fill(dt);
					if (dt.Rows.Count > 0)
					{
						fdModel.File_id = Convert.ToInt32(dt.Rows[0]["file_id"]);
						fdModel.FileName = dt.Rows[0]["filename"].ToString();
						fdModel.FileLocation = dt.Rows[0]["filelocation"].ToString();
						fdModel.FileExtension = dt.Rows[0]["fileextension"].ToString();
						fdModel.FirstDelieryMethod = dt.Rows[0]["firstdeliveryMethod"].ToString();
						fdModel.SeconddeliveryMethod = dt.Rows[0]["seconddeliveryMethod"].ToString();
						fdModel.Status = dt.Rows[0]["Status"].ToString();
					}
				}
				return fdModel;
			}
			catch (Exception ex)
			{
				string Path = "File Service";
				string Method = MethodBase.GetCurrentMethod().Name;
				string Exmsg = ex.Message;
				InsertException(Path, Method, DateTime.Now, Exmsg);
				return fdModel;
			}

		}
		public void InsertException(string path,string method,DateTime dateTime,string exceptionMsg)
		{
			using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fileDbConnection"].ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("Ins_Exception", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Path", path);
				cmd.Parameters.AddWithValue("@Method", method);
				cmd.Parameters.AddWithValue("@ErrorTime", dateTime);
				cmd.Parameters.AddWithValue("@ExceptionMsg", exceptionMsg);
				cmd.ExecuteNonQuery();
			}
		}
		public void UpdateFileOperationStatus(int iD, string status)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["fileDbConnection"].ConnectionString))
				{
					if (conn.State == ConnectionState.Closed)
					{
						conn.Open();
					}
					SqlCommand cmd = new SqlCommand("Upd_FileOperationStatus", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", iD);
					cmd.Parameters.AddWithValue("@Status", status);

					cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				string Path = "File Service";
				string Method = MethodBase.GetCurrentMethod().Name;
				string Exmsg = ex.Message;
				InsertException(Path, Method, DateTime.Now, Exmsg);
			}
		}
	}
}
