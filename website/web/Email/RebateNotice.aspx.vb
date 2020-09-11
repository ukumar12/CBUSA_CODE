Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Email_RebateNotice
    Inherits BasePage

    Protected amountDueTotals As Double = 0.0

    Protected Sub Email_RebateNotice_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim AccDB As New Database
        Try
            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))
            Dim sVendors As New System.Text.StringBuilder
            sVendors.Append("SELECT [vndrname], " & vbCrLf)
            '    sVendors.Append("       [phone], " & vbCrLf)
            sVendors.Append("       [invoice], " & vbCrLf)
            sVendors.Append("       [period] + ' - ' + [year] AS [Qtr/Yr], " & vbCrLf)
            sVendors.Append("       [date], " & vbCrLf)
            sVendors.Append("       [dayspastdue], " & vbCrLf)
            sVendors.Append("       [purchvol], " & vbCrLf)
            sVendors.Append("       [rebaterate], " & vbCrLf)
            sVendors.Append("       [amountdue] " & vbCrLf)
            sVendors.Append("FROM   [aec_arreport] " & vbCrLf)
            sVendors.Append("WHERE  bldrid =  " & Request.QueryString("BuilderId").ToString() & " ORDER BY vndrid")

            rptVendors.DataSource = AccDB.GetDataTable(sVendors.ToString())
            rptVendors.DataBind()


        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
        Finally
            AccDB.Close()

        End Try

    End Sub

    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            amountDueTotals = Core.GetMoney(DataBinder.Eval(e.Item.DataItem, "amountdue"))
        ElseIf e.Item.ItemType = ListItemType.Footer Then
            CType(e.Item.FindControl("litTotalAmount"), Literal).Text = amountDueTotals
        End If

    End Sub
End Class
