using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
namespace bpet防伪验证码
{
    /// <summary>
    /// bpetCode 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class bpetCode : System.Web.Services.WebService
    {

        [WebMethod]
        public void checkCode()
        {
            Context.Response.ContentType = "text/plain";
            SqlConnection conn = new SqlConnection("SERVER=10.1.1.12;DATABASE=QFMainDB_BRJ;PWD=123;UID=sa;");
            conn.Open();

            string sCode = Context.Request["Code"];
            if(sCode==null||sCode=="")
            {

            }
            //string sCode = "8850867822183556";
            string result = "{\"Result\":";
            string strSQL = "";
            if (sCode==null||sCode==""||sCode.Length != 16||!isNum(sCode))
            {
                result += "\"fail\",\"Error\":\"请输入16位数字防伪码\"";
            }
            #region
            else
            {
                string sql = "Select Count(*) as QueryTimes From QueryRecord Where Code='" + sCode + "'";

                /*运行SQL，次数给QueryTimes*/
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);

               // DataSet ds = DbHelperSQL.Query(sql);

                int QueryTimes = 0;
                QueryTimes = Int32.Parse(ds.Tables[0].Rows[0]["QueryTimes"].ToString());
                if (QueryTimes <= 2)
                {
                    strSQL = strSQL + " Select CodeList.*,Product.Code ,Product.Name ,PrdPlan.PrdBatch From CodeList";
                    strSQL = strSQL + " Left Join ProductRecord On (CodeList.PrdRecordID=ProductRecord.ID)";
                    strSQL = strSQL + " Left Join PrdPlan On (ProductRecord.PlanCode=PrdPlan.Code)";
                    strSQL = strSQL + " Left Join Product On (PrdPlan.PrdCode=Product.Code)";
                    strSQL = strSQL + " Where CodeList.Code_0='" + sCode + "' Or CodeList.Code_1='" + sCode + "' Or CodeList.Code_2='" + sCode + "' ";
                    /*运行SQL,判断是否存在存在执行if，不然else*/
                    //SqlCommand cmd2 = new SqlCommand();
                    //cmd2.Connection = conn;
                    cmd.CommandText = strSQL;
                    DataSet ds2 = new DataSet();
                    SqlDataAdapter adp2 = new SqlDataAdapter(cmd);
                    adp2.Fill(ds2);


                    if (ds2.Tables[0].Rows.Count!=0&&ds2.Tables[0].Rows[0]["ID"]!=null)
                    {
                        result += "\"success\",\"PrdName\":\"" + ds2.Tables[0].Rows[0]["Name"].ToString() + "\",\"Batch\":\"" + ds2.Tables[0].Rows[0]["PrdBatch"].ToString() + "\"";
                        strSQL = "Insert Into QueryRecord (Type,Source,QueryTime,Code) Values ('WEB','" + System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString() + "','','" + sCode + "')";
                        /*执行SQL，插入查询次数*/
                        cmd.CommandText = strSQL;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        strSQL = "";
                        strSQL = strSQL + " Select * From MakeCode Where Code='" + sCode + "'";
                        /*执行SQL，判断CODE是否存在，存在执行if，不然else*/
                        cmd.CommandText = strSQL;
                        DataSet ds3= new DataSet();
                        SqlDataAdapter adp3 = new SqlDataAdapter(cmd);
                        adp3.Fill(ds3);

                        if (ds3.Tables[0].Rows.Count != 0)
                        {
                            result += "\"success\",\"PrdName\":\"\",\"Batch\":\"\"";
                            strSQL = "Insert Into QueryRecord (Type,Source,QueryTime,Code) Values ('WEB','" +System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString() +"','','" + sCode + "')";
                            /*执行SQL，插入查询次数*/
                            cmd.CommandText = strSQL;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            result += "\"fail\",\"Error\":\"您输入的数码不存在，请联系比瑞吉客服人员\"";
                        }
                    }
                }
                else
                {
                    result += "\"fail\",\"Error\":\"您查询的数码已经被多次查询过，该识别码已经作废，请仔细核查\"";
                }
            }
            #endregion

            result += "}";
            conn.Close();
            Context.Response.ContentType = "text/plain";
            Context.Response.Write(result);
        }

        //判断是否为纯数字
        public bool isNum(string num)
        {
            for (int i = 0; i < num.Length; i++)
            {
                byte numByte = Convert.ToByte(num[i]);
                if ((numByte < 48) || (numByte > 57))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
