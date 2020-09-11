
Option Strict Off
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Email_BuilderRebatesSummary
    Inherits ModuleControl

    Dim sHistoricVendorId As String
    Protected sBuilderName As String
    Protected sVendorName, excludedVendors As String
    Protected sumBuilder As String
    Protected sumAllBuilder As String
    Protected amountDueTotals As Double = 0.0
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsAdminDisplay Then
            Dim AccDB As New Database
            Try
                excludedVendors = DB.ExecuteScalar("SELECT coalesce( stuff((    SELECT ', ' + cast(HistoricID as varchar(max))    FROM Vendor where excludedVendor = 1    FOR XML PATH('')    ), 1, 1, ''),'')")


                '  Dim dtExVendors As DataTable = DB.GetDataTable("SELECT DISTINCT(VendorId) FROM Vendor WHERE excludedVendor =1")
                AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
                Dim sVendors As New System.Text.StringBuilder
                sVendors.Append("SELECT [vndrname], " & vbCrLf)
                '  sVendors.Append("       [phone], " & vbCrLf)
                sVendors.Append("       [invoice], " & vbCrLf)
                sVendors.Append("       [period] + ' - ' + [year] AS [Qtr/Yr], " & vbCrLf)
                sVendors.Append("       [Period], " & vbCrLf)
                sVendors.Append("       [Year], " & vbCrLf)
                sVendors.Append("       [date], " & vbCrLf)
                sVendors.Append("       [dayspastdue], " & vbCrLf)
                sVendors.Append("       [purchvol], " & vbCrLf)
                sVendors.Append("       [rebaterate], " & vbCrLf)
                sVendors.Append("       [amountdue] " & vbCrLf)
                sVendors.Append("FROM   [RG_ARReport] " & vbCrLf)
                sVendors.Append("WHERE DaysPastDue >= 30 AND  bldrid =  " & Request.QueryString("BuilderId").ToString() & vbCrLf)
                If Not excludedVendors = String.Empty Then
                    sVendors.Append("and vndrid not in ( " & excludedVendors & " ) " & vbCrLf)
                End If
                sVendors.Append(" ORDER BY vndrid")


                rptVendors.DataSource = AccDB.GetDataTable(sVendors.ToString())
                rptVendors.DataBind()


            Catch ex As Exception
                AddError(ErrHandler.ErrorText(ex))
            Finally
                AccDB.Close()

            End Try
        End If

    End Sub

    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            amountDueTotals += Core.GetMoney(DataBinder.Eval(e.Item.DataItem, "amountdue"))
        ElseIf e.Item.ItemType = ListItemType.Footer Then
            CType(e.Item.FindControl("litTotalAmount"), Literal).Text = FormatCurrency(amountDueTotals, 2)
        End If

    End Sub


End Class
