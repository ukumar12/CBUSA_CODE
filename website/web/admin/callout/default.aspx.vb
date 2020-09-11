Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("CALLOUTS")

        If Not IsPostBack Then

            Dim dtb As DataTable = DB.GetDataTable("SELECT Callout1, Callout2 FROM BuilderCallout")

            Me.divBuilderCallout1.InnerHtml = dtb.Rows(0).Item("Callout1").ToString
            Me.divBuilderCallout2.InnerHtml = dtb.Rows(0).Item("Callout2").ToString

            Dim dtv As DataTable = DB.GetDataTable("SELECT Callout1, Callout2 FROM VendorCallout")

            Me.divVendorCallout1.InnerHtml = dtv.Rows(0).Item("Callout1").ToString
            Me.divVendorCallout2.InnerHtml = dtv.Rows(0).Item("Callout2").ToString


            Dim dtp As DataTable = DB.GetDataTable("SELECT Callout1, Callout2 FROM PIQCallout")

            Me.divPIQCallout1.InnerHtml = dtp.Rows(0).Item("Callout1").ToString
            Me.divPIQCallout2.InnerHtml = dtp.Rows(0).Item("Callout2").ToString

        End If
    End Sub

End Class
