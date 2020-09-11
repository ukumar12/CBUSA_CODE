Option Strict Off

Imports Components
Imports DataLayer

Partial Class Text
    Inherits ModuleControl

    Private Sub Text_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Dim strHTMLContent As String = HTMLContent

        'If strHTMLContent.Contains("lnkLeftMenuVendorReporting") Then
        '    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, CInt(Session("VendorId")))

        '    If Not dbVendor.QuarterlyReportingOn Then
        '        HTMLContent = strHTMLContent.Replace("<li><a id=""lnkLeftMenuVendorReporting"" href=""/rebates/vendor-sales.aspx"">Quarterly Sales</a></li>", "")
        '    End If
        'End If

        Dim strHTMLContent As String = HTMLContent
        Dim strQueryString As String = String.Empty

        If strHTMLContent.Contains("**home_starts_param**") Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, CInt(Session("BuilderId")))
            Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, CInt(Session("BuilderAccountId")))

            strQueryString = String.Concat("oid=", dbBuilder.CRMID, "&cid=", dbBuilderAccount.CRMID)
            HTMLContent = strHTMLContent.Replace("**home_starts_param**", strQueryString)

        End If

    End Sub

End Class
