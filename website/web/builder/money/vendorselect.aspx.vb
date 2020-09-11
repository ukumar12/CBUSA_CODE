Imports Components
Imports DataLayer
Imports System.net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim buffer As String = String.Empty
        Dim SQL As String = String.Empty
        Dim SupplyPhases As String = String.Empty
        Dim row As DataRow
        Dim dt As DataTable

        Try
            SupplyPhases = Request("supplyphases")

            'SQL &= "SELECT DISTINCT" & vbCrLf
            'SQL &= "  v.VendorID, v.CompanyName Vendor" & vbCrLf
            'SQL &= "FROM" & vbCrLf
            'SQL &= "  LLCSupplyPhases lsp" & vbCrLf
            'SQL &= "  JOIN LLCVendor lv ON lsp.LLCID = lv.LLCID" & vbCrLf
            'SQL &= "  JOIN Vendor v ON lv.VendorID = v.VendorID" & vbCrLf
            'SQL &= "WHERE" & vbCrLf
            'SQL &= "  lsp.SupplyPhaseID IN (" & SupplyPhases & ")" & vbCrLf
            'SQL &= "ORDER BY" & vbCrLf
            'SQL &= "  v.CompanyName" & vbCrLf

            SQL = "SELECT v.VendorID, v.CompanyName Vendor FROM Vendor v JOIN LLCVendor lv ON v.VendorID = lv.VendorID JOIN Builder b ON lv.LLCID = b.LLCID JOIN VendorRegistration vr ON v.VendorId = vr.VendorId WHERE b.BuilderID = " & DB.Number(Session("BuilderId")) & " AND vr.PrimarySupplyPhaseID IN " & DB.NumberMultiple(SupplyPhases) & " Order By v.CompanyName"

            'Response.Write(SQL)

            dt = Me.DB.GetDataTable(SQL)

            For Each row In dt.Rows
                If buffer <> String.Empty Then buffer = buffer & ","
                buffer = buffer & row.Item(0).ToString & ":" & row.Item(1).ToString
            Next

        Catch ex As Exception
            buffer = String.Empty
        End Try

        MyBase.Response.Write(buffer)
        MyBase.Response.Flush()

    End Sub

End Class
