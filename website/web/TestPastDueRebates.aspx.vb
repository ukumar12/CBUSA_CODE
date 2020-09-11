Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class TestPastDueRebates
    Inherits System.Web.UI.Page

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        Dim AccDB As New Database
        Dim DB As New Database

        DB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), "", ""))

        AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
        Dim dtAECReport As DataTable = AccDB.GetDataTable("SELECT * From RG_ARReport Where DaysPastDue = 101 ")

        For Each row As DataRow In dtAECReport.Rows
            Response.Write(row("VNDRID"))
            Response.Write(" / ")
            Response.Write(Core.GetString(row("DaysPastDue")).Trim)
            Response.Write("<br /> *************************** <br />")

            Dim sHistoricIdGroups As String = AccDB.ExecuteScalar("select CAST(stuff((SELECT  distinct  ',' + t1.BLDRID FROM RG_ARReport t1 where t1.VNDRID = 4391 FOR XML PATH('')),1,1,'') as varchar(max))  ")

            Dim dtBuilderGroups As DataTable = DB.GetDataTable("Select distinct(BuilderGroup) AS BuilderGroup ,OperationsManager ,NotificationEmailList from LLC l inner join Builder b on l.LLCID=b.LLCID where b.RebatesEmailPreferences= 1 AND b.HistoricID in (" & sHistoricIdGroups & ")")

            Dim sHistoricVendorId As String = "4391"

            For Each drBuilderGroup As DataRow In dtBuilderGroups.Rows
                Dim builderGroup As String = Core.GetString(drBuilderGroup("BuilderGroup"))
                Dim OperationsManager As String = Core.GetString(drBuilderGroup("OperationsManager"))
                Dim NotificationEmailList As String = String.Empty
                If Not IsDBNull(drBuilderGroup("NotificationEmailList")) Then
                    NotificationEmailList = Core.GetString(drBuilderGroup("NotificationEmailList"))
                End If

                Dim HistoricBuilderIdByGroup As String = DB.ExecuteScalar("select CAST(stuff((SELECT distinct ',' + CAST( b.HistoricId as varchar)  FROM Builder b  WHERE b.LLCID = l.LLCID and b.HistoricID in (" & sHistoricIdGroups & ")  FOR XML PATH('')),1,1,'') as varchar(max)) from LLC l where  l.BuilderGroup = " & DB.Quote(builderGroup))

                Dim dtVendor As DataTable = DB.GetDataTable("SELECT CompanyName, Email FROM Vendor WHERE HistoricId = " & sHistoricVendorId)
                If dtVendor.Rows.Count > 0 Then
                    Response.Write(Core.GetString(dtVendor.Rows(0)("CompanyName")))
                    Response.Write("<br />")
                    Response.Write(Core.GetString(dtVendor.Rows(0)("Email")))
                End If


                Dim sVendorId = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sHistoricVendorId)
                'Dim sDaysPastdue = DB.GetDataTable("SELECT DaysPastdue  from RG_ARReport")
                Dim sDaysPastDue As String = Core.GetString(row("DaysPastDue")).Trim
                Dim sVendorEmails As String = ""

                sVendorEmails = DB.ExecuteScalar("SELECT COALESCE(STUFF((SELECT ',' + VendorEmail.Email FROM(SELECT DISTINCT VA.Email FROM VendorAccount VA JOIN VendorAccountVendorRole VAVR ON VA.VendorAccountID= VAVR.VendorAccountID WHERE VA.VendorID = " & sVendorId & " AND VA.IsActive = 1 AND VAVR.VendorRoleID IN (1, 3, 2, 5)) AS VendorEmail FOR XML PATH('')),1,1,''), 'NA') VendorEmails")

                Response.Write(sVendorEmails)

            Next
        Next
    End Sub

End Class
