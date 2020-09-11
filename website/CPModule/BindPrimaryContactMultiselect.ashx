<%@ WebHandler Language="C#" Class="BindPrimaryContactMultiselect" %>

using System;
using System.Web;
    using DataAccessLayer;
    using System.Linq;
    using System.Data;
    using System.Collections.Generic;
    using Newtonsoft.Json;


public class BindPrimaryContactMultiselect : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
            DataAccess Da = new DataAccess();
        context.Response.ContentType = "application/json";
            DataSet ds = Da.ExecuteSelectQry("select AdminId,FirstName,LastName,Email from Admin");
        List<PrimaryContact> PrimaryContactList = ds.Tables[0].AsEnumerable().Select(x => new PrimaryContact()
        {
            AdminId = x.Field<int>("AdminId"),
            FirstName = x.Field<string>("FirstName"),
            LastName = x.Field<string>("LastName"),
            Email = x.Field<string>("Email")
        }).ToList();
        context.Response.Write(JsonConvert.SerializeObject(PrimaryContactList));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
    class PrimaryContact
{
    public int AdminId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}