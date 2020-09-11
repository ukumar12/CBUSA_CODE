Imports Components
Imports DataLayer

Partial Class admin_ajax_attributes
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

        CheckAccess("STORE")

        Dim sOutput As String = ""
        Dim F As String = Request.QueryString("F")
        Dim TemplateId As Integer = Request.QueryString("TemplateId")
        Dim TempAttributeId As Integer = Request.QueryString("TempAttributeId")
        Dim TemplateAttributeId As Integer = Request.QueryString("TemplateAttributeId")
        Dim sSortDirection As String = Request.QueryString("SortDirection")

        Dim SKU As String = Request.QueryString("SKU")
        Dim VALUE As String = Request.QueryString("VALUE")
        Dim PRICE As Double, CurrentTempAttributeId As Integer
        If Trim(Request.QueryString("PRICE")) = "" Then PRICE = 0 Else PRICE = Request.QueryString("PRICE")
        If Trim(Request.QueryString("CurrentTempAttributeId")) = "" Then CurrentTempAttributeId = 0 Else CurrentTempAttributeId = Request.QueryString("CurrentTempAttributeId")

        If F = "LoadAttributes" Then
            sOutput = RefreshScreen(TemplateId)
        ElseIf F = "ResetAttributes" Then
            CleanAttributeTable()
            sOutput = RefreshScreen(TemplateId)
        ElseIf F = "MoveAttribute" Then
            Core.ChangeSortOrder(DB, "TempAttributeId", "StoreItemAttributeTemp", "SortOrder", "TemplateAttributeId=" & TemplateAttributeId & " AND AdminId=" & LoggedInAdminId, TempAttributeId, sSortDirection)
            sOutput = RefreshScreen(TemplateId)
        ElseIf F = "SaveAttribute" Then
            SaveAttribute(SKU, PRICE, VALUE, TemplateAttributeId, CurrentTempAttributeId)
            sOutput = RefreshScreen(TemplateId)
        ElseIf F = "DeleteAttribute" Then
            DeleteAttribute(TempAttributeId)
            sOutput = RefreshScreen(TemplateId)
        End If

        ltlOutput.Text = sOutput
    End Sub

    Private Sub CleanAttributeTable()
        DB.ExecuteSQL("DELETE FROM StoreItemAttributeTemp WHERE AdminId=" & LoggedInAdminId)
    End Sub

    Private Sub DeleteAttribute(ByVal TempAttributeId As Integer)
        DB.ExecuteSQL("DELETE FROM StoreItemAttributeTemp WHERE TempAttributeId=" & TempAttributeId)
    End Sub
    Private Sub SaveAttribute(ByVal SKU As String, ByVal Price As Double, ByVal Value As String, ByVal TemplateAttributeId As Integer, ByVal CurrentTempAttributeId As Integer)
        If CurrentTempAttributeId > 0 Then
            DB.ExecuteSQL("UPDATE StoreItemAttributeTemp SET AttributeValue = " & DB.Quote(Value) & ", SKU = " & DB.Quote(SKU) & ", Price = " & DB.Quote(Price) & " WHERE TempAttributeId=" & DB.Number(CurrentTempAttributeId))
        Else
            Dim MaxSortOrder As Integer = DB.ExecuteScalar("SELECT COALESCE(MAX(SortOrder),0)+1 FROM StoreItemAttributeTemp WHERE TemplateAttributeId=" & TemplateAttributeId & " AND AdminId=" & LoggedInAdminId)
            DB.ExecuteSQL("INSERT INTO StoreItemAttributeTemp (AdminId, TemplateAttributeId, AttributeValue, SKU, Price, SortOrder) VALUES (" & DB.Quote(LoggedInAdminId) & "," & DB.Quote(TemplateAttributeId) & "," & DB.Quote(Value) & "," & DB.Quote(SKU) & "," & DB.Quote(Price) & "," & DB.Quote(MaxSortOrder) & ")")
        End If
    End Sub

    Private Function RefreshScreen(ByVal TemplateId As Integer) As String
        Dim sReturn As String = "", TemplateAttributeId As Integer, AttributeName As String = ""
        Dim dv As DataView = StoreItemTemplateRow.GetTemplateAttributes(DB, TemplateId).DefaultView
        If dv.Count = 0 Then
            sReturn = "This item template has no specific attributes"
        Else
            For iLoop As Integer = 0 To dv.Count - 1
                TemplateAttributeId = dv(iLoop).Item("TemplateAttributeId")
                AttributeName = dv(iLoop).Item("AttributeName")

                sReturn &= "<b>" & AttributeName & "</b>: &nbsp;&nbsp;&nbsp; <a href=""javascript:void(0);"" id=""lnk_" & TemplateAttributeId & """ onClick=""if (getElement('tbl_" & TemplateAttributeId & "').style.display == 'block') { HideDiv('tbl_" & TemplateAttributeId & "'); getElement('lnk_" & TemplateAttributeId & "').innerHTML = 'add'; } else { ShowDiv('tbl_" & TemplateAttributeId & "'); getElement('lnk_" & TemplateAttributeId & "').innerHTML = 'hide'; }"" class='smaller'>add</a><br />" & vbCrLf
                sReturn &= "<div id=""tbl_" & TemplateAttributeId & """ style=""display: none;"">" & vbCrLf
                sReturn &= "<input type='hidden' name='CurrentTempAttributeId_" & TemplateAttributeId & "' id='CurrentTempAttributeId value=''>"
                sReturn &= "<table border=0><tr>" & vbCrLf
                sReturn &= " <td>Value: <input type=text class='smaller' size='25' maxlength='255' name='VALUE_" & TemplateAttributeId & "'></td>" & vbCrLf
                sReturn &= " <td>Add SKU: <input type=text class='smaller' size='10' maxlength='25' name='SKU_" & TemplateAttributeId & "'></td>" & vbCrLf
                sReturn &= " <td>Add cost: $<input type=text class='smaller' size='5' maxlength='10' name='PRICE_" & TemplateAttributeId & "'></td>" & vbCrLf
                sReturn &= " <td><input type=button class='btn' onClick=""SaveAttribute(" & TemplateAttributeId & ");"" value='submit'></td>" & vbCrLf
                sReturn &= "</tr></table>" & vbCrLf
                sReturn &= "</div>" & vbCrLf
                sReturn &= BuildTable(TemplateAttributeId)
            Next
        End If
        Return sReturn
    End Function

    Private Function BuildTable(ByVal TemplateAttributeId) As String
        Dim sReturn As String = ""
        Dim sValue As String = "", sSKU As String = ""
        Dim dv As DataView = DB.GetDataTable("SELECT * FROM StoreItemAttributeTemp WHERE TemplateAttributeId=" & TemplateAttributeId & " AND AdminId=" & LoggedInAdminId & " ORDER BY SortOrder").DefaultView

        If dv.Count > 0 Then
            sReturn = "<table border=0 cellpadding=3>"
            sReturn &= "<tr><th>&nbsp;</th><th>&nbsp;</th><th>Value</th><th>Add SKU</th><th>Add Price</th><th>&nbsp;</th></tr>"
            For iLoop As Integer = 0 To dv.Count - 1
                sValue = Replace(dv(iLoop).Item("AttributeValue"), "'", "\'")
                sSKU = Replace(dv(iLoop).Item("SKU"), "'", "\'")
                sValue = Replace(sValue, """", "\""")
                sSKU = Replace(sSKU, """", "\""")

                sReturn &= "<tr>"
                sReturn &= "<td><a href=""javascript:void(0);"" onClick=""EditAttribute(" & dv(iLoop).Item("TempAttributeId") & "," & dv(iLoop).Item("TemplateAttributeId") & ",'" & sValue & "','" & sSKU & "','" & FormatNumber(dv(iLoop).Item("PRICE"), 2) & "');""><img src='/images/admin/edit.gif' border='0'></a></td>"
                sReturn &= "<td><a href=""javascript:void(0);"" onClick=""DeleteAttribute(" & dv(iLoop).Item("TempAttributeId") & ");""><img src='/images/admin/delete.gif' border='0'></a></td>"
                sReturn &= "<td>" & Server.HtmlEncode(dv(iLoop).Item("AttributeValue")) & "</td>"
                sReturn &= "<td>" & Server.HtmlEncode(dv(iLoop).Item("SKU")) & "</td>"
                sReturn &= "<td>" & FormatCurrency(dv(iLoop).Item("Price")) & "</td>"
                sReturn &= "<td><a href=""javascript:void(0);"" onClick=""MoveAttribute(" & dv(iLoop).Item("TempAttributeId") & "," & dv(iLoop).Item("TemplateAttributeId") & ",'UP');""><img src='/images/admin/moveup.gif' border='0'></a> &nbsp; <a href=""javascript:void(0);"" onClick=""MoveAttribute(" & dv(iLoop).Item("TempAttributeId") & "," & dv(iLoop).Item("TemplateAttributeId") & ",'DOWN');""><img src='/images/admin/movedown.gif' border='0'></a></td>"
                sReturn &= "</tr>"
            Next
            sReturn &= "</table>"
        End If
        Return sReturn
    End Function
End Class
