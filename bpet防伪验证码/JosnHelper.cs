using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Data;


namespace aicheWebService
{
    /// <summary>
    /// 将对象序列化 为JSON
    /// </summary>
    public class JsonHelper
    {
        public JsonHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //// 自定义md5加密函数 
        //public static string md5(string strTemp)
        //{

        //    string strMd5 = null;
        //    if (strTemp != null)
        //    {
        //        strMd5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strTemp, "MD5").ToLower();
        //    }
        //    else
        //    {
        //        strMd5 = "imil51aspx,aryouok?";
        //    }
        //    return strMd5;
        //}
        /// <summary>
        /// 把对象序列化 JSON 字符串 
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实体</param>
        /// <returns>JSON字符串</returns>
        public static string ObjectToJson(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        /// <summary>
        /// 将DataTable转化为JSON
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt)
        {

            StringBuilder JsonString = new StringBuilder();

                JsonString.Append("{ ");
                JsonString.Append("\"ResultCount\":\"" + dt.Rows.Count + "\"");
                JsonString.Append(",\"Result\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
           
        }
    }
}
