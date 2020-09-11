Imports System.Configuration.ConfigurationManager
Imports Components
Imports Utility
Imports System.Data
Partial Class controls_GoogleTracking
    Inherits BaseControl

    Public GoogleTrackingNo As String = String.Empty
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GoogleTrackingNo = AppSettings("GlobalGoogleTrackingNo")

        If DataLayer.SysParam.GetValue(DB, "IsGoogleAnalyticsEcommerceTracking") = "1" Then
            If InStr(Request.RawUrl.ToLower, "/store/confirm.aspx") > 0 Then
                GenerateOrderDetails()
            End If
        End If
    End Sub
    Private Sub GenerateOrderDetails()
        Dim dt As DataTable
        Dim sText As New StringBuilder
        Dim dr As DataRow
        Try
            Dim dbStoreOrder As DataLayer.StoreOrderRow = DataLayer.StoreOrderRow.GetRow(DB, Session("LastOrderId"))

            sText.AppendLine("  pageTracker._addTrans(")
            sText.AppendLine("      """ & CStr(dbStoreOrder.OrderNo) & """,")
            sText.AppendLine("      """ & Session("ReferralURL") & """,")
            sText.AppendLine("      """ & dbStoreOrder.Total.ToString & """,")
            sText.AppendLine("      """ & dbStoreOrder.Tax.ToString & """,")
            sText.AppendLine("      """ & dbStoreOrder.Shipping.ToString & """,")
            sText.AppendLine("      """ & dbStoreOrder.BillingCity.ToString & """,")
            sText.AppendLine("      """ & dbStoreOrder.BillingState.ToString & """,")
            sText.AppendLine("      """ & dbStoreOrder.BillingCountry.ToString & """);")

            Dim dbStoreItem As DataLayer.StoreItemRow
            Dim DepartmentName As String
            Dim sSQL As String = "SELECT StoreOrderItem.* "
            sSQL &= " FROM StoreOrderItem "
            sSQL &= " WHERE OrderId = " & DB.NullNumber(dbStoreOrder.OrderId)
            dt = DB.GetDataSet(sSQL).Tables(0)
            For Each dr In dt.Rows
                dbStoreItem = DataLayer.StoreItemRow.GetRow(DB, dr("ItemId"))
                If Not IsDBNull(dr("DepartmentId")) Then
                    DepartmentName = DataLayer.StoreDepartmentRow.GetRow(DB, dr("DepartmentId")).Name
                Else
                    DepartmentName = ""
                End If

                sText.AppendLine("pageTracker._addItem( ")
                sText.AppendLine("      """ & dbStoreOrder.OrderNo & """,")
                sText.AppendLine("      """ & dr("SKU").ToString & """,")
                sText.AppendLine("      """ & Replace(dr("ItemName").ToString, """", "") & """,")
                sText.AppendLine("      """ & Replace(DepartmentName.ToString, """", "") & """,")
                sText.AppendLine("      """ & dr("Price").ToString & """,")
                sText.AppendLine("      """ & dr("Quantity").ToString & """);")

            Next

            sText.AppendLine("pageTracker._trackTrans();")
            ltlGoogleOrderTracking.Text = sText.ToString
        Catch ex As Exception
        End Try
    End Sub
End Class
