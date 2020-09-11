Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Components
Imports System.Text
Imports System.Web

Namespace DataLayer
    Public Class StoreDepartmentRow
        Inherits StoreDepartmentRowBase

        Private Const CacheKey As String = "DefaultDepartmentId"

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal DepartmentId As Integer)
            MyBase.New(DB, DepartmentId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal DepartmentId As Integer) As StoreDepartmentRow
            Dim row As StoreDepartmentRow

            row = New StoreDepartmentRow(DB, DepartmentId)
            row.Load()

            Return row
        End Function

        'Custom Methods
        Public Shared Function GetRowByCustomURL(ByVal DB As Database, ByVal Url As String) As StoreDepartmentRow
            Dim SQL As String = "SELECT * FROM StoreDepartment WHERE CustomURL = " & DB.Quote(Url)
            Dim r As SqlDataReader
            Dim row As StoreDepartmentRow = New StoreDepartmentRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Function DepartmentInsert(ByVal DB As Database, ByVal DepartmentId As Integer, ByVal Name As String) As Integer
            Dim SQL As String = "exec sp_StoreDepartmentInsert " & DB.Quote(DepartmentId) & "," & DB.Quote(Name)
            DB.ExecuteSQL(SQL)

            Dim DeptId As Integer = DB.ExecuteScalar("SELECT DepartmentId FROM StoreDepartment ORDER BY DepartmentId desc")
            Return DeptId
        End Function

        Public Shared Function ChangeDepartmentSortOrder(ByVal DB As Database, ByVal sKeyField As String, ByVal iKeyValue As Integer, ByVal sAction As String) As Boolean
            Dim sSQL As String
            Dim res As SqlDataReader
            Dim ParentId As Integer
            Dim LFT1 As Integer, LFT2 As Integer, RGT1 As Integer, RGT2 As Integer
            Dim sSortField As String
            Dim iRowsAffected As Integer
            Dim temp As Integer

            ChangeDepartmentSortOrder = False

            'Prevent SQL Injection    
            sKeyField = Replace(sKeyField, ";", "")

            sSQL = "SELECT A.LFT AS LFT1, A.RGT AS RGT1, A.ParentId FROM StoreDepartment AS A WHERE " & sKeyField & " = " & DB.Quote(iKeyValue.ToString)
            res = DB.GetReader(sSQL)
            If res.Read Then
                LFT1 = Convert.ToInt32(res("LFT1"))
                RGT1 = Convert.ToInt32(res("RGT1"))
                ParentId = CInt(IIf(IsDBNull(res("ParentId")), 0, res("ParentId")))
            End If
            res.Close()
            res = Nothing

            If LFT1 = 0 Then Exit Function

            sSQL = "SELECT TOP 1 A.LFT AS LFT2, A.RGT AS RGT2 FROM StoreDepartment AS A WHERE ParentId = " & DB.Quote(ParentId.ToString)
            If sAction = "UP" Then
                sSortField = "RGT DESC"
                sSQL = sSQL & " AND A.RGT < " & LFT1
            Else
                sSortField = "RGT ASC"
                sSQL = sSQL & " AND A.RGT > " & RGT1
            End If
            sSQL = sSQL & " ORDER BY " & sSortField
            res = DB.GetReader(sSQL)
            If res.Read Then
                LFT2 = Convert.ToInt32(res("LFT2"))
                RGT2 = Convert.ToInt32(res("RGT2"))
            End If
            res.Close()
            res = Nothing

            If LFT2 = 0 Then Exit Function

            If sAction = "UP" Then
                temp = LFT1
                LFT1 = LFT2
                LFT2 = temp

                temp = RGT1
                RGT1 = RGT2
                RGT2 = temp
            End If

            sSQL = "UPDATE StoreDepartment SET lft = CASE WHEN lft BETWEEN " & LFT1 & " AND " & RGT1 & " THEN lft + " & (RGT2 - RGT1) & " WHEN lft BETWEEN " & LFT2 & " AND " & RGT2 & " THEN lft - " & (LFT2 - LFT1) & " ELSE lft END, rgt = CASE WHEN rgt BETWEEN " & LFT1 & " AND " & RGT1 & " THEN rgt + " & (RGT2 - RGT1) & " WHEN rgt BETWEEN " & LFT2 & " AND " & RGT2 & " THEN rgt - " & (LFT2 - LFT1) & " ELSE rgt END"
            iRowsAffected = DB.ExecuteSQL(sSQL)
            If iRowsAffected = 0 Then
                Exit Function
            End If

            ChangeDepartmentSortOrder = True
        End Function

        Public Shared Function GetDepartmentsToKeepOpened(ByVal DB As Database, ByVal DEPARTMENTS As String) As String
            Dim Str, sConn, sSQL As String
            Dim rsDepartments As SqlDataReader

            GetDepartmentsToKeepOpened = String.Empty

            If DB.IsEmpty(DEPARTMENTS) Then Exit Function

            sSQL = " SELECT P2.DepartmentId FROM StoreDepartment AS P1, StoreDepartment AS P2" _
              & " WHERE P1.lft BETWEEN P2.lft AND P2.rgt" _
              & " AND P1.DepartmentId IN " & DB.NumberMultiple(DEPARTMENTS)

            Str = ""
            sConn = ""
            rsDepartments = DB.GetReader(sSQL)
            While rsDepartments.Read
                Str = Str & sConn & CStr(rsDepartments("DepartmentId"))
                sConn = ","
            End While
            rsDepartments.Close()
            rsDepartments = Nothing

            GetDepartmentsToKeepOpened = Str
        End Function

        Public Shared Function GetDepartments(ByVal DB As Database, ByVal sPrefix As String, ByVal sLink As String, ByVal sDepartments As String, ByVal sFieldName As String, ByVal iDepartment As Integer, ByVal bHideRoot As Boolean, ByVal HideInactive As Boolean) As String
            Dim sSQL As String, rsDepartments As SqlDataReader
            Dim iLft, iRgt, iIndent As Integer
            Dim Levels() As String = {}
            Dim bHasChildren As Boolean, bLastChild As Boolean
            Dim aDepartments() As String
            Dim ParentId, DepartmentId As Integer
            Dim sChecked As String
            Dim iMaxLft As Integer, aCloseTags() As Integer
            Dim sb As New StringBuilder
            Dim IsInactive As Boolean
            Dim IsSpecial As Boolean

            ' CREATE ARRAY OF Departments TO KEEP OPENED
            aDepartments = Split(sDepartments, ",")

            sSQL = " select MAX(lft) AS MaxLft from StoreDepartment"
            iMaxLft = CInt(DB.ExecuteScalar(sSQL))
            ReDim aCloseTags(iMaxLft)
            For iLoop As Integer = 1 To iMaxLft
                aCloseTags(iLoop) = 0
            Next

            sSQL = " SELECT COUNT(P2.DepartmentId)-1 AS indent, P1.DepartmentId, P1.lft, p1.rgt,p1.NAME, p1.ParentId, (select DepartmentId from StoreDepartment where rgt = p1.rgt+1) AS LastChild, (SELECT COUNT(*) FROM StoreItem i, StoreDepartmentItem di WHERE i.ItemId = di.ItemId and di.DepartmentId = P1.DepartmentId) AS NofItems," _
                & " (select top 1 lft from StoreDepartment where rgt < p1.rgt and p1.rgt - p1.lft > 1 and rgt - lft = 1 order by lft desc) AS LftClose, p1.IsInactive, p1.IsSpecial" _
                & " FROM StoreDepartment P1, StoreDepartment P2" _
                & " WHERE P1.lft BETWEEN P2.lft AND P2.rgt" _
                & " GROUP BY P1.DepartmentId, P1.lft, p1.rgt, p1.NAME, p1.ParentId, p1.IsInactive, p1.IsSpecial" _
                & " ORDER BY P1.lft"

            rsDepartments = DB.GetReader(sSQL)
            While rsDepartments.Read
                DepartmentId = Convert.ToInt32(rsDepartments("DepartmentId"))
                ParentId = CInt(IIf(IsDBNull(rsDepartments("ParentId")), 0, rsDepartments("ParentId")))
                iIndent = Convert.ToInt32(rsDepartments("Indent"))
                iLft = Convert.ToInt32(rsDepartments("Lft"))
                iRgt = Convert.ToInt32(rsDepartments("Rgt"))
                IsInactive = Convert.ToBoolean(rsDepartments("IsInactive"))
                IsSpecial = Convert.ToBoolean(rsDepartments("IsSpecial"))
                bHasChildren = False
                bLastChild = False

                If Not DB.IsEmpty(Convert.ToString(rsDepartments("LftClose"))) And iLft <> 1 Then
                    aCloseTags(Convert.ToInt32(rsDepartments("LftClose"))) = aCloseTags(Convert.ToInt32(rsDepartments("LftClose"))) + 1
                End If

                sb.Append("<table border=0 cellpadding=0 cellspacing=0><tr>")

                Dim iLoop As Integer
                For iLoop = LBound(Levels) + 1 To UBound(Levels)
                    If iLoop < iIndent Then
                        If Not DB.IsEmpty(Levels(iLoop)) Then
                            sb.Append("<td><img src=""/images/i2.gif"" width=19 height=22></td>")
                        Else
                            sb.Append("<td><img src=""/images/spacer.gif"" width=19 height=16></td>")
                        End If
                    End If
                Next

                ' CHECK IF THIS ITEM HAS OTHER CHILDREN
                If iRgt - iLft > 1 Then
                    bHasChildren = True
                    If iIndent > UBound(Levels) Then ReDim Preserve Levels(iIndent)
                    Levels(iIndent) = "Y"
                End If

                ' IS THIS THE LAST CHILD?
                If Not DB.IsEmpty(Convert.ToString(rsDepartments("LastChild"))) Or iLft = 1 Then
                    bLastChild = True
                    If iIndent > UBound(Levels) Then ReDim Preserve Levels(iIndent)
                    Levels(iIndent) = ""
                End If

                If bHasChildren Then
                    If iLft <> 1 Then
                        If bLastChild Then
                            If Array.IndexOf(aDepartments, CStr(DepartmentId)) > -1 And iDepartment <> DepartmentId Then
                                sb.Append("<td style=""cursor:hand"" onClick=""expandit('" & sPrefix & "', this, '" & DepartmentId & "')""><img id=""" & sPrefix & "IMG" & DepartmentId & """ src=""/images/lminus2.gif"" width=19 height=22></td>")
                            Else
                                sb.Append("<td style=""cursor:hand"" onClick=""expandit('" & sPrefix & "', this, '" & DepartmentId & "')""><img id=""" & sPrefix & "IMG" & DepartmentId & """ src=""/images/lplus2.gif"" width=19 height=22></td>")
                            End If
                        Else
                            If Array.IndexOf(aDepartments, CStr(DepartmentId)) > -1 And iDepartment <> DepartmentId Then
                                sb.Append("<td style=""cursor:hand"" onClick=""expandit('" & sPrefix & "', this, '" & DepartmentId & "')""><img id=""" & sPrefix & "IMG" & DepartmentId & """ src=""/images/tminus2.gif"" width=19 height=22></td>")
                            Else
                                sb.Append("<td style=""cursor:hand"" onClick=""expandit('" & sPrefix & "', this, '" & DepartmentId & "')""><img id=""" & sPrefix & "IMG" & DepartmentId & """ src=""/images/tplus2.gif"" width=19 height=22></td>")
                            End If
                        End If
                    End If
                    If Array.IndexOf(aDepartments, CStr(DepartmentId)) > -1 And iDepartment <> DepartmentId Then
                        sb.Append("<td><img id=""" & sPrefix & "FLD" & DepartmentId & """ src=""/images/folderminus2.gif"" width=16 height=22></td>")
                    Else
                        sb.Append("<td><img id=""" & sPrefix & "FLD" & DepartmentId & """ src=""/images/folderplus2.gif"" width=16 height=22></td>")
                    End If
                Else
                    If iLft <> 1 Then
                        If bLastChild Then
                            sb.Append("<td><img src=""/images/l2.gif"" width=19 height=22></td>")
                        Else
                            sb.Append("<td><img src=""/images/t2.gif"" width=19 height=22></td>")
                        End If
                    End If
                    If Array.IndexOf(aDepartments, CStr(DepartmentId)) > -1 And iDepartment <> DepartmentId Then
                        sb.Append("<td><img src=""/images/folderminus2.gif"" width=16 height=22></td>")
                    Else
                        sb.Append("<td><img src=""/images/folderplus2.gif"" width=16 height=22></td>")
                    End If
                End If

                ' DEPARTMENT SORT ORDER BUTTONS
                If sFieldName <> "TARGET_ID" And sFieldName <> "SOURCE_ID" And ParentId <> 0 Then
                    sb.Append("<TD width=4><IMG src=""/images/spacer.gif"" width=4 height=16></TD>")
                    sb.Append("<TD align=right><A href=""move.aspx?ACTION=UP&DepartmentId=" & DepartmentId & "&ParentId=" & ParentId & """><IMG src=""/images/admin/moveup.gif"" width=16 height=16 border=0 alt=""Move Up""></A></TD>")
                    sb.Append("<TD align=right><IMG src=""/images/spacer.gif"" width=1 height=1><A href=""move.aspx?ACTION=DOWN&DepartmentId=" & DepartmentId & "&ParentId=" & ParentId & """><IMG src=""/images/admin/movedown.gif"" width=16 height=16 border=0 alt=""Move Down""></A></TD>")
                End If

                sb.Append("<td><img src=""/images/spacer.gif"" width=4 height=16></td>")

                sChecked = ""
                If iDepartment = DepartmentId Then sChecked = "checked"

                If (bHideRoot And iLft = 1) Or (IsInactive And HideInactive) Or IsSpecial Then
                    sb.Append("<td width=""20"">&nbsp;</td>")
                Else
                    sb.Append("<td><input type=radio name=""" & sFieldName & """ value=""" & DepartmentId & """ " & sChecked & "></td>")
                End If

                If CInt(rsDepartments("NofItems")) <> 0 Then
                    If Not DB.IsEmpty(sLink) Then
                        sb.Append("<td nowrap><a href=""" & sLink & "?DepartmentId=" & DepartmentId & """>")
                    Else
                        sb.Append("<td nowrap>")
                    End If
                    sb.Append(Convert.ToString(rsDepartments("NAME")))
                    If Not DB.IsEmpty(sLink) Then
                        sb.Append("</a>")
                    End If
                    sb.Append("<sup>(<a href=""/admin/store/items/default.aspx?F_DepartmentId=" & CStr(rsDepartments("DepartmentId")) & """>" & CStr(rsDepartments("NofItems")) & "</a>)</sup></td>")
                Else
                    If Not DB.IsEmpty(sLink) Then
                        sb.Append("<td nowrap><a href=""" & sLink & "?DepartmentId=" & DepartmentId & """>" & CStr(rsDepartments("NAME")) & "</a></td>")
                    Else
                        sb.Append("<td nowrap>" & CStr(rsDepartments("NAME")) & "</td>")
                    End If
                End If
                sb.Append("</tr></table>")

                For iLoop = 1 To aCloseTags(iLft)
                    sb.Append("</span>")
                Next

                ' CHECK IF THIS ITEM HAS OTHER CHILDREN
                If bHasChildren And iLft <> 1 Then
                    If Array.IndexOf(aDepartments, CStr(DepartmentId)) > -1 And iDepartment <> DepartmentId Then
                        sb.Append("<span id=""" & sPrefix & "SPAN" & DepartmentId & """ style=""display:block"" style=&{head};>")
                    Else
                        sb.Append("<span id=""" & sPrefix & "SPAN" & DepartmentId & """ style=""display:none"" style=&{head};>")
                    End If
                End If
            End While
            rsDepartments.Close()
            rsDepartments = Nothing

            Return sb.ToString
        End Function

		Public Shared Function DisplayBreadCrumb(ByVal DB As Database, ByVal DepartmentId As Integer, ByVal IsLastLink As Boolean, Optional ByVal ItemName As String = Nothing)
			Dim SQL As String
			Dim dt As DataTable
			Dim Result As String = String.Empty
			Dim Level As Integer = StoreDepartmentRow.GetDepartmentLevel(DB, DepartmentId)
			Dim Count As Integer = 0

			If Not ItemName = Nothing Then
				'if the itemname exists then override the IsLastLink to True
				IsLastLink = True
			End If

			SQL = ""
			SQL &= " SELECT D2.* FROM StoreDepartment AS D1, StoreDepartment AS D2"
			SQL &= " WHERE D1.lft BETWEEN D2.lft AND D2.rgt"
			SQL &= " AND D1.DepartmentId = " & DB.Quote(DepartmentId)
			SQL &= " AND D1.IsInactive = 0 and d2.ParentId is not null"
			SQL &= " ORDER BY D2.LFT"

			If Level = 0 Then Level = 1

			dt = DB.GetDataTable(SQL)
			If dt.Rows.Count > 0 Then
				Result = "<a href=""/"">Home</a> &gt; <a href=""/store/"">Store</a> "
			Else
				Result = "<a href=""/"">Home</a> &gt; <span>Store</span> "
			End If
			For Each row As DataRow In dt.Rows
				Dim PageName As String = "default.aspx"
				If row("ParentId") = StoreDepartmentRow.GetDefaultDepartmentId(DB) Then
					PageName = "main.aspx"
				End If
				If Trim(DepartmentId) = Trim(row("DepartmentId")) Then
					If IsLastLink Then
						Result &= " &gt; <a href=""" & IIf(IsDBNull(row("CustomURL")), "/store/" & PageName & "?DepartmentId=" & row("DepartmentId"), row("CustomURL")) & """>" & row("Name") & "</a>"
						Level -= 1
					Else
						Result &= " &gt; <span>" & row("NAME") & "</span>"
						Level -= 1
					End If
				Else
					Result &= " &gt; <a href=""" & IIf(IsDBNull(row("CustomURL")), "/store/" & PageName & "?DepartmentId=" & row("DepartmentId"), row("CustomURL")) & """>" & row("Name") & "</a>"
					Level -= 1
				End If
			Next
			If IsLastLink AndAlso Not ItemName = Nothing Then Result &= " &gt; " & ItemName
			Return Result
		End Function

        Public Shared Function GetDefaultDepartmentId(ByVal DB As Database) As Integer
            Dim DefaultDepartmentId As Integer = Nothing
            Dim context As HttpContext = HttpContext.Current
            If Not context Is Nothing Then
                DefaultDepartmentId = CType(context.Cache(StoreDepartmentRow.CacheKey), Integer)
            End If
            If DefaultDepartmentId > 0 Then Return DefaultDepartmentId

            Dim SQL As String = "select DepartmentId from StoreDepartment where ParentId is null and IsInactive = 0"
            DefaultDepartmentId = DB.ExecuteScalar(SQL)
            If Not context Is Nothing Then
                context.Cache.Insert(StoreDepartmentRow.CacheKey, DefaultDepartmentId, Nothing, DateTime.Now.AddSeconds(30), TimeSpan.Zero)
            End If
            Return DefaultDepartmentId
        End Function

        Public Shared Function GetDefaultDepartment(ByVal DB As Database) As StoreDepartmentRow
            Dim SQL As String = "select * from StoreDepartment where ParentId is null and IsInactive = 0"
            Dim r As SqlDataReader
            Dim row As StoreDepartmentRow = New StoreDepartmentRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Function GetChildrenDepartments() As DataTable
            Dim SQL As String = String.Empty

			SQL &= " select distinct sdp.DepartmentId, sdp.ParentId, sdp.Name, sdp.ViewImage, sdp.ViewImageAlt, sdp.ThumbnailWidth, sdp.ThumbnailHeight, sdp.Lft, sdp.CustomURL  from StoreDepartment sd1, StoreDepartmentItem sdi1, StoreItem si, StoreDepartment sdp"
            SQL &= " where "
            SQL &= " 	sdi1.DepartmentId = sd1.DepartmentId "
            SQL &= " and 	si.ItemId = sdi1.ItemId "
            SQL &= " and 	si.IsActive = 1 "
            SQL &= " and	sd1.lft >= " & Lft & " and sd1.rgt <= " & Rgt
            SQL &= " and	sdp.ParentId = " & DepartmentId
            SQL &= " and	sdp.lft <= sd1.lft and sdp.rgt >= sd1.rgt"
            SQL &= " and    sdp.IsInactive = 0"
            SQL &= " order by sdp.Lft"

            Return DB.GetDataTable(SQL)
        End Function

        Public Function GetSiblingDepartments() As DataTable
            Dim SQL As String = "SELECT * FROM StoreDepartment WHERE ParentId IN (SELECT ParentId FROM StoreDepartment WHERE DepartmentId = " & DB.Quote(DepartmentId) & ") AND IsInactive = 0"

            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetDepartmentLevel(ByVal Db As Database, ByVal DepartmentId As Integer) As Integer
            Dim SQL As String = String.Empty

            SQL &= " Select Count(P2.DepartmentId)-1 As Indent, P1.DepartmentId, P1.Lft, P1.Rgt,P1.Name"
            SQL &= " From StoreDepartment P1, StoreDepartment P2"
            SQL &= " Where P1.Lft Between P2.Lft And P2.Rgt and p1.DepartmentId = " & DepartmentId
            SQL &= " Group By P1.DepartmentId, P1.Lft, P1.Rgt, P1.Name"

            Return Db.ExecuteScalar(SQL)
        End Function

        Public Shared Function GetMainLevelDepartments(ByVal DB As Database) As DataTable
            Dim SQL As String
            Dim DefaultDepartmentId As Integer = GetDefaultDepartmentId(DB)

            SQL = ""
            SQL &= " select * from StoreDepartment where ParentId = " & DefaultDepartmentId & " and IsInactive = 0"
            SQL &= " order by lft"

            Dim dt As DataTable = DB.GetDataTable(SQL)
            Return dt
        End Function

        Public Shared Function GetSecondLevelDepartments(ByVal DB As Database, ByVal SectionId As Integer, ByVal Grade As Integer) As DataTable
            Dim SQL As String = String.Empty
            Dim dbDefault As StoreDepartmentRow = StoreDepartmentRow.GetRow(DB, GetDefaultDepartmentId(DB))

			SQL &= " select distinct sdp.DepartmentId, sdp.ParentId, sdp.Name, sdp.ViewImage, sdp.ViewImageAlt, sdp.Lft from StoreDepartment sd1, StoreDepartmentItem sdi1, StoreItem si, StoreDepartment sdp"
            SQL &= " where "
            SQL &= " 	    sdi1.DepartmentId = sd1.DepartmentId "
            SQL &= " and 	si.ItemId = sdi1.ItemId "
            SQL &= " and 	si.IsActive = 1 "
            SQL &= " and	sd1.lft >= " & dbDefault.Lft & " and sd1.rgt <= " & dbDefault.Rgt
            SQL &= " and	sdp.ParentId in (select departmentid from storedepartment where parentid = " & dbDefault.DepartmentId & ") "
            SQL &= " and	sdp.lft <= sd1.lft and sdp.rgt >= sd1.rgt"
            SQL &= " and    sdp.IsInactive = 0"
            SQL &= " order by sdp.lft"
            Dim dt As DataTable = DB.GetDataTable(SQL)
            Return dt
        End Function

        Public Shared Function GetMenuDepartments(ByVal DB As Database, ByVal ParentId As Integer) As DataTable
            Dim SQLExclude As String = "SELECT DD1.DepartmentId FROM StoreDepartment AS DD1, StoreDepartment AS DD2 WHERE DD1.lft BETWEEN DD2.lft AND DD2.rgt AND DD2.DepartmentId = (SELECT TOP 1 DepartmentId FROM StoreDepartment WHERE IsInactive = 1)"
            Dim SQL As String = String.Empty

            SQL &= "SELECT * FROM ( SELECT"
            SQL &= "(SELECT COUNT(*) AS NofGreater FROM StoreDepartment dd WHERE dd.ParentId = P1.ParentId AND dd.LFT > P1.LFT AND dd.DepartmentId NOT IN (" & SQLExclude & ")) AS HasMore,"
            SQL &= "(select top 1 lft from StoreDepartment where rgt < p1.rgt and p1.rgt - p1.lft > 1 and rgt - lft = 1 order by lft desc) AS LftClose,"
            SQL &= "p1.LFT, p1.RGT, p1.DepartmentId, p1.ParentId, CASE WHEN (p1.RGT-p1.LFT)>1 THEN 1 ELSE 0 END AS HasChildren, p1.Name, p1.CustomURL"
            SQL &= " FROM StoreDepartment AS P1, StoreDepartment AS P2"
            SQL &= " WHERE P1.lft BETWEEN P2.lft AND P2.rgt"
            SQL &= "   AND P2.DepartmentId = " & DB.Number(ParentId)
            SQL &= "   AND P1.DepartmentId NOT IN (" & SQLExclude & ")"
            SQL &= "   AND P1.DepartmentId <> " & DB.Number(ParentId)
            SQL &= ") AS TMP ORDER BY TMP.LFT"

            Dim dt As DataTable = DB.GetDataTable(SQL)
            Return dt
        End Function

        Public Function GetMainLevelDepartment() As StoreDepartmentRow
            Dim DefaultDepartmentId As Integer = StoreDepartmentRow.GetDefaultDepartmentId(DB)
            Dim SQL As String = " select * from StoreDepartment where ParentId = " & DefaultDepartmentId & " and IsInactive = 0 and lft <= " & Lft & " and rgt >= " & Rgt
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            Dim row As StoreDepartmentRow = New StoreDepartmentRow(DB)
            If dr.Read Then
                row.Load(dr)
            End If
            dr.Close()
            Return row
        End Function

        Public Shared Function GetMainLevelDepartmentsHome(ByVal DB As Database) As DataTable
            Dim dt As DataTable = GetMainLevelDepartments(DB)

            dt.Columns.Add("ItemId", GetType(Integer))
            dt.Columns.Add("Image", GetType(String))
            dt.Columns.Add("ItemName", GetType(String))

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim r As DataRow = dt.Rows(i)
                Dim dr As SqlDataReader = DB.GetReader("select top 1 si.image,si.itemid,si.itemname from storeitem si inner join storedepartmentitem sdi on si.itemid = sdi.itemid where si.image is not null and departmentid in (select departmentid from storedepartment where lft between " & r("lft") & " and " & r("rgt") & ") order by isfeatured desc, newid()")
                If dr.Read Then
                    r("ItemId") = dr("ItemId")
                    r("Image") = dr("Image")
                    r("ItemName") = dr("ItemName")
                End If
                dr.Close()
            Next

            Return dt
        End Function

        Public ReadOnly Property IsActive() As Boolean
            Get
                Dim SQL As String = " select * from StoreDepartment where IsInactive = 1"
                Dim dr As SqlDataReader = DB.GetReader(SQL)
                Dim row As StoreDepartmentRow = New StoreDepartmentRow(DB)
                If dr.Read Then
                    row.Load(dr)
                End If
                dr.Close()

                If Lft >= row.Lft And Rgt <= row.Rgt Then
                    Return False
                End If
                Return True
            End Get
        End Property

    End Class

    Public MustInherit Class StoreDepartmentRowBase
        Private m_DB As Database
        Private m_DepartmentId As Integer = Nothing
        Private m_Lft As Integer = Nothing
        Private m_Rgt As Integer = Nothing
        Private m_ParentId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_PageTitle As String = Nothing
		Private m_ViewImage As String = Nothing
		Private m_ViewImageAlt As String = Nothing
        Private m_MetaDescription As String = Nothing
        Private m_MetaKeywords As String = Nothing
        Private m_CustomURL As String = Nothing
        Private m_IsInactive As Boolean = Nothing
        Private m_ThumbnailWidth As Integer = Nothing
        Private m_ThumbnailHeight As Integer = Nothing
        Private m_Description As String = Nothing
        Private OriginalCustomURL As String = Nothing

        Public Property DepartmentId() As Integer
            Get
                Return m_DepartmentId
            End Get
            Set(ByVal Value As Integer)
                m_DepartmentId = Value
            End Set
        End Property

        Public Property Lft() As Integer
            Get
                Return m_Lft
            End Get
            Set(ByVal Value As Integer)
                m_Lft = Value
            End Set
        End Property

        Public Property Rgt() As Integer
            Get
                Return m_Rgt
            End Get
            Set(ByVal Value As Integer)
                m_Rgt = Value
            End Set
        End Property

        Public Property ParentId() As Integer
            Get
                Return m_ParentId
            End Get
            Set(ByVal Value As Integer)
                m_ParentId = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        Public Property ViewImage() As String
            Get
                Return m_ViewImage
            End Get
            Set(ByVal Value As String)
                m_ViewImage = Value
            End Set
		End Property

		Public Property ViewImageAlt() As String
			Get
				Return m_ViewImageAlt
			End Get
			Set(ByVal value As String)
				m_ViewImageAlt = value
			End Set
		End Property

        Public Property PageTitle() As String
            Get
                Return m_PageTitle
            End Get
            Set(ByVal Value As String)
                m_PageTitle = Value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Return m_MetaDescription
            End Get
            Set(ByVal Value As String)
                m_MetaDescription = Value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Return m_MetaKeywords
            End Get
            Set(ByVal Value As String)
                m_MetaKeywords = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = Value
            End Set
        End Property


        Public Property CustomURL() As String
            Get
                Return m_CustomURL
            End Get
            Set(ByVal Value As String)
                m_CustomURL = Value
            End Set
        End Property

        Public Property IsInactive() As Boolean
            Get
                Return m_IsInactive
            End Get
            Set(ByVal Value As Boolean)
                m_IsInactive = Value
            End Set
        End Property

        Public Property ThumbnailWidth() As Integer
            Get
                Return m_ThumbnailWidth
            End Get
            Set(ByVal Value As Integer)
                m_ThumbnailWidth = Value
            End Set
        End Property

        Public Property ThumbnailHeight() As Integer
            Get
                Return m_ThumbnailHeight
            End Get
            Set(ByVal Value As Integer)
                m_ThumbnailHeight = Value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal DepartmentId As Integer)
            m_DB = DB
            m_DepartmentId = DepartmentId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreDepartment WHERE DepartmentId = " & DB.Quote(DepartmentId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_DepartmentId = Convert.ToInt32(r.Item("DepartmentId"))
            If IsDBNull(r.Item("Lft")) Then
                m_Lft = Nothing
            Else
                m_Lft = Convert.ToInt32(r.Item("Lft"))
            End If
            If IsDBNull(r.Item("Rgt")) Then
                m_Rgt = Nothing
            Else
                m_Rgt = Convert.ToInt32(r.Item("Rgt"))
            End If
            If IsDBNull(r.Item("ParentId")) Then
                m_ParentId = Nothing
            Else
                m_ParentId = Convert.ToInt32(r.Item("ParentId"))
            End If
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("ViewImage")) Then
                m_ViewImage = Nothing
            Else
                m_ViewImage = Convert.ToString(r.Item("ViewImage"))
            End If
			If IsDBNull(r.Item("ViewImageAlt")) Then
				m_ViewImageAlt = Nothing
			Else
				m_ViewImageAlt = Convert.ToString(r.Item("ViewImageAlt"))
			End If
			If IsDBNull(r.Item("PageTitle")) Then
				m_PageTitle = Nothing
			Else
				m_PageTitle = Convert.ToString(r.Item("PageTitle"))
			End If
            If IsDBNull(r.Item("MetaDescription")) Then
                m_MetaDescription = Nothing
            Else
                m_MetaDescription = Convert.ToString(r.Item("MetaDescription"))
            End If
            If IsDBNull(r.Item("MetaKeywords")) Then
                m_MetaKeywords = Nothing
            Else
                m_MetaKeywords = Convert.ToString(r.Item("MetaKeywords"))
            End If
            If IsDBNull(r.Item("CustomURL")) Then
                m_CustomURL = Nothing
                OriginalCustomURL = Nothing
            Else
                m_CustomURL = Convert.ToString(r.Item("CustomURL"))
                OriginalCustomURL = Convert.ToString(r.Item("CustomURL"))
            End If
            m_IsInactive = Convert.ToBoolean(r.Item("IsInactive"))
            If IsDBNull(r.Item("ThumbnailWidth")) Then
                m_ThumbnailWidth = Nothing
            Else
                m_ThumbnailWidth = Convert.ToInt32(r.Item("ThumbnailWidth"))
            End If
            If IsDBNull(r.Item("ThumbnailHeight")) Then
                m_ThumbnailHeight = Nothing
            Else
                m_ThumbnailHeight = Convert.ToInt32(r.Item("ThumbnailHeight"))
            End If
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

			SQL = " INSERT INTO StoreDepartment (" _
			 & " Lft" _
			 & ",Rgt" _
			 & ",ParentId" _
			 & ",Name" _
			 & ",ViewImage" _
			 & ",ViewImageAlt" _
			 & ",PageTitle" _
			 & ",MetaDescription" _
			 & ",MetaKeywords" _
			 & ",CustomURL" _
			 & ",IsInactive" _
			 & ",ThumbnailWidth" _
			 & ",ThumbnailHeight" _
			 & ",Description" _
			 & ") VALUES (" _
			 & m_DB.Quote(Lft) _
			 & "," & m_DB.Quote(Rgt) _
			 & "," & m_DB.NullNumber(ParentId) _
			 & "," & m_DB.Quote(Name) _
			 & "," & m_DB.Quote(ViewImage) _
			 & "," & m_DB.Quote(ViewImageAlt) _
			 & "," & m_DB.Quote(PageTitle) _
			 & "," & m_DB.Quote(MetaDescription) _
			 & "," & m_DB.Quote(MetaKeywords) _
			 & "," & m_DB.Quote(CustomURL) _
			 & "," & CInt(IsInactive) _
			 & "," & m_DB.NullNumber(ThumbnailWidth) _
			 & "," & m_DB.NullNumber(ThumbnailHeight) _
			 & "," & m_DB.Quote(Description) _
			 & ")"

            DepartmentId = m_DB.InsertSQL(SQL)

            Return DepartmentId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String
            If OriginalCustomURL <> String.Empty And OriginalCustomURL <> m_CustomURL Then
                If m_CustomURL = "" Then
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, "/store/default.aspx?departmentid=" & m_DepartmentId.ToString)
                Else
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, m_CustomURL)
                End If
            End If
			SQL = " UPDATE StoreDepartment SET " _
			 & " Lft = " & m_DB.Quote(Lft) _
			 & ",Rgt = " & m_DB.Quote(Rgt) _
			 & ",ParentId = " & m_DB.NullNumber(ParentId) _
			 & ",Name = " & m_DB.Quote(Name) _
			 & ",ViewImage = " & m_DB.Quote(ViewImage) _
			 & ",ViewImageAlt = " & m_DB.Quote(ViewImageAlt) _
			 & ",PageTitle = " & m_DB.Quote(PageTitle) _
			 & ",MetaDescription = " & m_DB.Quote(MetaDescription) _
			 & ",MetaKeywords = " & m_DB.Quote(MetaKeywords) _
			 & ",CustomURL = " & m_DB.Quote(CustomURL) _
			 & ",IsInactive = " & CInt(IsInactive) _
			 & ",ThumbnailWidth = " & m_DB.NullNumber(ThumbnailWidth) _
			 & ",ThumbnailHeight = " & m_DB.NullNumber(ThumbnailHeight) _
			 & ",Description = " & m_DB.Quote(Description) _
			 & " WHERE DepartmentId = " & m_DB.Quote(DepartmentId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "exec sp_RemoveDepartment " & DepartmentId
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreDepartmentCollection
        Inherits GenericCollection(Of StoreDepartmentRow)
    End Class
End Namespace