Option Strict Off

Imports Components
Imports DataLayer

Partial Class _default
    Inherits ModuleControl

    Protected EntityID As Integer = 1


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        EntityID = Session("BuilderId")

        Dim dbBuilderIndicatorVendor As DataLayer.BuilderIndicatorVendorRow
        Dim dbVendor As DataLayer.VendorRow

        Dim dbBuilderIndicatorProduct As DataLayer.BuilderIndicatorProductRow
        Dim dbProduct As DataLayer.ProductRow

        If Me.Page.FindControl("AjaxManager") Is Nothing Then
            Dim sm As New ScriptManager
            sm.ID = "AjaxManager"
            Try
                Me.Controls.AddAt(0, sm)
            Catch ex As Exception

            End Try
        End If

            'Me.gvAdminIndicators.DataSource = DB.GetDataTable(AdminPriceWatchSQL())
            'Me.gvAdminIndicators.DataBind()

            If BuilderPriceWatchSQL <> "" Then
                Me.gvBuilderIndicators.DataSource = DB.GetDataTable(BuilderPriceWatchSQL())
                Me.gvBuilderIndicators.DataBind()
            End If

            If Not IsPostBack Then

                If DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows.Count = 1 Then
                    Me.divMorePricingInfo.InnerHtml = DataLayer.BuilderPricingInformationRow.GetList(Me.DB).Rows(0).Item("PricingInformation").ToString
                Else
                    Me.divMorePricingInfo.InnerHtml = "There is no pricing information to display."
                End If

                dbBuilderIndicatorVendor = DataLayer.BuilderIndicatorVendorRow.GetRowByBuilder(Me.DB, EntityID, 1)
                If dbBuilderIndicatorVendor.BuilderIndicatorVendorID <> 0 Then
                    dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbBuilderIndicatorVendor.VendorID)
                    Me.acdVendorID1.Value = dbBuilderIndicatorVendor.VendorID.ToString
                    Me.acdVendorID1.Text = dbVendor.CompanyName
                End If

                dbBuilderIndicatorVendor = DataLayer.BuilderIndicatorVendorRow.GetRowByBuilder(Me.DB, EntityID, 2)
                If dbBuilderIndicatorVendor.BuilderIndicatorVendorID <> 0 Then
                    dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbBuilderIndicatorVendor.VendorID)
                    Me.acdVendorID2.Value = dbBuilderIndicatorVendor.VendorID.ToString
                    Me.acdVendorID2.Text = dbVendor.CompanyName
                End If

                dbBuilderIndicatorVendor = DataLayer.BuilderIndicatorVendorRow.GetRowByBuilder(Me.DB, EntityID, 3)
                If dbBuilderIndicatorVendor.BuilderIndicatorVendorID <> 0 Then
                    dbVendor = DataLayer.VendorRow.GetRow(Me.DB, dbBuilderIndicatorVendor.VendorID)
                    Me.acdVendorID3.Value = dbBuilderIndicatorVendor.VendorID.ToString
                    Me.acdVendorID3.Text = dbVendor.CompanyName
                End If

                dbBuilderIndicatorProduct = DataLayer.BuilderIndicatorProductRow.GetRowByBuilder(Me.DB, EntityID, 1)
                If dbBuilderIndicatorProduct.BuilderIndicatorProductID <> 0 Then
                    dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbBuilderIndicatorProduct.ProductID)
                    Me.acdProductID1.Value = dbBuilderIndicatorProduct.ProductID.ToString
                    Me.acdProductID1.Text = dbProduct.Product
                End If

                dbBuilderIndicatorProduct = DataLayer.BuilderIndicatorProductRow.GetRowByBuilder(Me.DB, EntityID, 2)
                If dbBuilderIndicatorProduct.BuilderIndicatorProductID <> 0 Then
                    dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbBuilderIndicatorProduct.ProductID)
                    Me.acdProductID2.Value = dbBuilderIndicatorProduct.ProductID.ToString
                    Me.acdProductID2.Text = dbProduct.Product
                End If

                dbBuilderIndicatorProduct = DataLayer.BuilderIndicatorProductRow.GetRowByBuilder(Me.DB, EntityID, 3)
                If dbBuilderIndicatorProduct.BuilderIndicatorProductID <> 0 Then
                    dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbBuilderIndicatorProduct.ProductID)
                    Me.acdProductID3.Value = dbBuilderIndicatorProduct.ProductID.ToString
                    Me.acdProductID3.Text = dbProduct.Product
                End If

                dbBuilderIndicatorProduct = DataLayer.BuilderIndicatorProductRow.GetRowByBuilder(Me.DB, EntityID, 4)
                If dbBuilderIndicatorProduct.BuilderIndicatorProductID <> 0 Then
                    dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbBuilderIndicatorProduct.ProductID)
                    Me.acdProductID4.Value = dbBuilderIndicatorProduct.ProductID.ToString
                    Me.acdProductID4.Text = dbProduct.Product
                End If

                dbBuilderIndicatorProduct = DataLayer.BuilderIndicatorProductRow.GetRowByBuilder(Me.DB, EntityID, 5)
                If dbBuilderIndicatorProduct.BuilderIndicatorProductID <> 0 Then
                    dbProduct = DataLayer.ProductRow.GetRow(Me.DB, dbBuilderIndicatorProduct.ProductID)
                    Me.acdProductID5.Value = dbBuilderIndicatorProduct.ProductID.ToString
                    Me.acdProductID5.Text = dbProduct.Product
                End If

            End If

    End Sub

    Private Function AdminPriceWatchSQL() As String

        Dim SQL As String = ""

        SQL = "SELECT" & vbCrLf
        SQL &= "p.Product," & vbCrLf
        SQL &= "v.CompanyName Vendor," & vbCrLf
        SQL &= "'$' + CAST(CAST(vpp.VendorPrice AS decimal(10,2)) as varchar) [Price]," & vbCrLf
        SQL &= "CAST(((vpp.VendorPrice - h.VendorPrice) / vpp.VendorPrice) * 100 As varchar) + '%' [Percent Change]" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "AdminIndicatorProduct aip" & vbCrLf
        SQL &= "JOIN Product p ON p.ProductID = aip.ProductID" & vbCrLf
        SQL &= "JOIN VendorProductPrice vpp ON aip.ProductID = vpp.ProductID" & vbCrLf
        SQL &= "JOIN" & vbCrLf
        SQL &= "(" & vbCrLf
        SQL &= "	SELECT"
        SQL &= "		ProductID," & vbCrLf
        SQL &= "		VendorID," & vbCrLf
        SQL &= "		VendorPrice," & vbCrLf
        SQL &= "		RANK() OVER (PARTITION BY ProductID, VendorID ORDER BY VendorPrice DESC) AS 'Rank'" & vbCrLf
        SQL &= "    FROM" & vbCrLf
        SQL &= "    VendorProductPriceHistory" & vbCrLf
        SQL &= ") h ON aip.ProductID = h.ProductID AND vpp.VendorID = h.VendorID" & vbCrLf
        SQL &= "JOIN Vendor v ON vpp.VendorID = v.VendorID" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  h.Rank = 1" & vbCrLf
        SQL &= "  AND v.VendorID IN (SELECT VendorID FROM AdminIndicatorVendor)" & vbCrLf
        SQL &= "Group BY" & vbCrLf
        SQL &= "p.Product, v.CompanyName, vpp.VendorPrice, h.VendorPrice, aip.SortOrder" & vbCrLf
        SQL &= "ORDER BY" & vbCrLf
        SQL &= "aip.SortOrder" & vbCrLf

        Return SQL

    End Function

    Private Function BuilderPriceWatchSQL() As String

        Dim SQL As String = ""
        
        If BuilderIndicatorVendor <> "()" then

            SQL = "SELECT" & vbCrLf
            SQL &= "*" & vbCrLf
            SQL &= "FROM" & vbCrLf        
            SQL &= "(" & vbCrLf
            SQL &= "SELECT" & vbCrLf
            SQL &= "p.Product," & vbCrLf
            SQL &= "v.CompanyName Vendor," & vbCrLf
            SQL &= "'$' + CAST(CAST(vpp.VendorPrice AS decimal(10,2)) as varchar) + ', ' + CAST(((vpp.VendorPrice - h.VendorPrice) / vpp.VendorPrice) * 100 As varchar) + '%' [Data]" & vbCrLf
            'SQL &= "CAST(((vpp.VendorPrice - h.VendorPrice) / vpp.VendorPrice) * 100 As varchar) + '%' [Percent Change]" & vbCrLf
            SQL &= "FROM" & vbCrLf
            SQL &= "BuilderIndicatorProduct bip" & vbCrLf
            SQL &= "JOIN Product p ON p.ProductID = bip.ProductID" & vbCrLf
            SQL &= "JOIN VendorProductPrice vpp ON bip.ProductID = vpp.ProductID" & vbCrLf
            SQL &= "JOIN" & vbCrLf
            SQL &= "(" & vbCrLf
            SQL &= "	SELECT"
            SQL &= "		ProductID," & vbCrLf
            SQL &= "		VendorID," & vbCrLf
            SQL &= "		VendorPrice," & vbCrLf
            SQL &= "		RANK() OVER (PARTITION BY ProductID, VendorID ORDER BY VendorPrice DESC) AS 'Rank'" & vbCrLf
            SQL &= "    FROM" & vbCrLf
            SQL &= "    VendorProductPriceHistory" & vbCrLf
            SQL &= ") h ON bip.ProductID = h.ProductID AND vpp.VendorID = h.VendorID" & vbCrLf
            SQL &= "JOIN Vendor v ON vpp.VendorID = v.VendorID" & vbCrLf
            SQL &= "WHERE" & vbCrLf
            SQL &= "  h.Rank = 1" & vbCrLf
            SQL &= "  AND v.VendorID IN (SELECT VendorID FROM BuilderIndicatorVendor WHERE BuilderID = " & EntityID & ")" & vbCrLf
            SQL &= "  AND bip.BuilderID = " & EntityID & vbCrLf
            SQL &= "Group BY" & vbCrLf
            SQL &= "  p.Product, v.CompanyName, vpp.VendorPrice, h.VendorPrice, bip.SortOrder" & vbCrLf
            'SQL &= "ORDER BY" & vbCrLf
            'SQL &= "  bip.SortOrder" & vbCrLf    
            SQL &= ") MyData" & vbCrLf
            SQL &= "PIVOT" & vbCrLf
            SQL &= "(" & vbCrLf
            SQL &= "  MAX(MyData.Data)" & vbCrLf
            SQL &= "  FOR MyData.Vendor IN " & BuilderIndicatorVendor  & vbCrLf
            SQL &= ") AS MyPIVOT" & vbCrLf
        End If
        Return SQL

    End Function

    Private Function BuilderIndicatorVendor() As String
        Dim SQL As String = String.Empty
        Dim buffer As String = String.Empty
        Dim row As DataRow
        Dim dt As DataTable

        SQL = "SELECT v.CompanyName FROM BuilderIndicatorVendor biv JOIN Vendor v ON v.VendorID = biv.VendorID WHERE biv.BuilderID = " & EntityID
        dt = Me.DB.GetDataTable(SQL)

        For Each row In dt.Rows
            If buffer <> String.Empty Then buffer = buffer & ", "
            buffer = buffer & "[" & row.Item("CompanyName").ToString() & "]"
        Next

        Return "(" & buffer & ")"

    End Function

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Dim dbBuilderIndicatorVendor As DataLayer.BuilderIndicatorVendorRow
        Dim dbBuilderIndicatorProduct As DataLayer.BuilderIndicatorProductRow

        Try

            DataLayer.BuilderIndicatorProductRow.DeleteByBuilder(Me.DB, EntityID)
            DataLayer.BuilderIndicatorVendorRow.DeleteByBuilder(Me.DB, EntityID)

            If IsNumeric(Me.acdVendorID1.Value) And Me.acdVendorID1.Text <> String.Empty Then
                dbBuilderIndicatorVendor = New DataLayer.BuilderIndicatorVendorRow(Me.DB)
                dbBuilderIndicatorVendor.BuilderID = EntityID
                dbBuilderIndicatorVendor.VendorID = Me.acdVendorID1.Value
                dbBuilderIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID2.Value) And Me.acdVendorID2.Text <> String.Empty Then
                dbBuilderIndicatorVendor = New DataLayer.BuilderIndicatorVendorRow(Me.DB)
                dbBuilderIndicatorVendor.BuilderID = EntityID
                dbBuilderIndicatorVendor.VendorID = Me.acdVendorID2.Value
                dbBuilderIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdVendorID3.Value) And Me.acdVendorID3.Text <> String.Empty Then
                dbBuilderIndicatorVendor = New DataLayer.BuilderIndicatorVendorRow(Me.DB)
                dbBuilderIndicatorVendor.BuilderID = EntityID
                dbBuilderIndicatorVendor.VendorID = Me.acdVendorID3.Value
                dbBuilderIndicatorVendor.Insert()
            End If

            If IsNumeric(Me.acdProductID1.Value) And Me.acdProductID1.Text <> String.Empty Then
                dbBuilderIndicatorProduct = New DataLayer.BuilderIndicatorProductRow(Me.DB)
                dbBuilderIndicatorProduct.BuilderID = EntityID
                dbBuilderIndicatorProduct.ProductID = Me.acdProductID1.Value
                dbBuilderIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID2.Value) And Me.acdProductID2.Text <> String.Empty Then
                dbBuilderIndicatorProduct = New DataLayer.BuilderIndicatorProductRow(Me.DB)
                dbBuilderIndicatorProduct.BuilderID = EntityID
                dbBuilderIndicatorProduct.ProductID = Me.acdProductID2.Value
                dbBuilderIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID3.Value) And Me.acdProductID3.Text <> String.Empty Then
                dbBuilderIndicatorProduct = New DataLayer.BuilderIndicatorProductRow(Me.DB)
                dbBuilderIndicatorProduct.BuilderID = EntityID
                dbBuilderIndicatorProduct.ProductID = Me.acdProductID3.Value
                dbBuilderIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID4.Value) And Me.acdProductID4.Text <> String.Empty Then
                dbBuilderIndicatorProduct = New DataLayer.BuilderIndicatorProductRow(Me.DB)
                dbBuilderIndicatorProduct.BuilderID = EntityID
                dbBuilderIndicatorProduct.ProductID = Me.acdProductID4.Value
                dbBuilderIndicatorProduct.Insert()
            End If

            If IsNumeric(Me.acdProductID5.Value) And Me.acdProductID5.Text <> String.Empty Then
                dbBuilderIndicatorProduct = New DataLayer.BuilderIndicatorProductRow(Me.DB)
                dbBuilderIndicatorProduct.BuilderID = EntityID
                dbBuilderIndicatorProduct.ProductID = Me.acdProductID5.Value
                dbBuilderIndicatorProduct.Insert()
            End If

            Response.Redirect(Request.Url.AbsoluteUri)

        Catch ex As Exception

        End Try

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        Try

        Catch ex As Exception

        End Try

    End Sub

End Class
