Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Controls
Imports Components
Imports DataLayer
Imports System.IO

Public Class oldajax
    Inherits SitePage

    Private Function Escape(ByVal s As String)
        Dim t As String

        t = Replace(s, "'", "\'")
        t = Trim(t)

        Return "'" & t & "'"
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

        Dim FunctionName As String = Request("f")
        Select Case FunctionName
            Case "DisplayItems"
                DisplayItems()
            Case "DisplayItemsAndDepartments"
                DisplayItemsAndDepartments()
            Case "GetDepartmentInfo"
                GetDepartmentInfo()
            Case "GetItemInfo"
                GetItemInfo()
            Case "CheckAvailbilityBuilder"
                CheckAvailbilityBuilder()
            Case "CheckAvailbilityVendor"
                CheckAvailbilityVendor()
            Case "CheckAvailbility"
                CheckAvailbility()
            Case "DisplaySuggest"
                DisplaySuggest()
        End Select
    End Sub

    Private Sub DisplayItems()
        Dim ItemId, SQL, q
        Dim sArray1, sArray2, sConn
        sConn = ""

        ItemId = Request("ItemId")
        q = Request("q")

        sArray1 = "new Array("
        sArray2 = "new Array("

        SQL = " select top 10 * from StoreItem where (ItemName like " & DB.Quote(q & "%") & " or SKU like " & DB.Quote(q & "%") & ") order by ItemName asc"
        Dim dr As SqlDataReader = DB.GetReader(SQL)
        While dr.Read
            sArray1 &= sConn & Escape(dr("ItemName") & ", (" & dr("SKU") & ")")
            sArray2 &= sConn & Escape(dr("ItemId"))
            sConn = ","
        End While
        dr.Close()
        sArray1 = sArray1 & ")"
        sArray2 = sArray2 & ")"

        Response.Write("showQueryDiv('al', " & sArray1 & "," & sArray2 & ")")
    End Sub

    Private Sub DisplayItemsAndDepartments()
        Dim ItemId, SQL, q
        Dim sArray1, sArray2, sConn
        sConn = ""

        ItemId = Request("ItemId")
        q = Request("q")

        sArray1 = "new Array("
        sArray2 = "new Array("

        SQL = " select top 5 ItemId As RecId, ItemName As RecordName, 'Item' As RecType, SKU from StoreItem where (ItemName like " & DB.Quote(q & "%") & " or SKU like " & DB.Quote(q & "%") & ")"
        SQL &= " UNION "
        SQL &= " select top 5 DepartmentId As RecId, Name As RecordName, 'Department' As RecType, '' AS SKU from StoreDepartment where Name like " & DB.Quote(q & "%")
        SQL &= " order by RecordName asc"
        Try
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                If Convert.ToString(dr("SKU")) <> "" Then
                    sArray1 &= sConn & Escape(dr("RecordName") & " [" & dr("SKU") & "]")
                Else
                    sArray1 &= sConn & Escape(dr("RecordName") & " [" & dr("RecType") & "]")
                End If
                sArray2 &= sConn & Escape(dr("RecId") & "-" & dr("RecType"))
                sConn = ","
            End While
            dr.Close()
        Catch ex As Exception
            'Response.Write("ERRROR!")
        End Try
        sArray1 = sArray1 & ")"
        sArray2 = sArray2 & ")"

        Response.Write("showQueryDiv('al', " & sArray1 & "," & sArray2 & ")")
    End Sub

    Private Sub GetDepartmentInfo()
        Dim DepartmentId, SQL, sResult
        sResult = ""

        DepartmentId = Request("DepartmentId")

        SQL = " select * from StoreDepartment where DepartmentId  = " & DB.Quote(DepartmentId)
        Dim dr As SqlDataReader = DB.GetReader(SQL)
        If dr.Read Then
            sResult = dr("DepartmentId") & "|" & dr("Name")
        End If
        dr.Close()
        Response.Write(sResult)
    End Sub

    Private Sub GetItemInfo()
        Dim ItemId, SQL, sResult
        sResult = ""

        ItemId = Request("ItemId")

        SQL = " select * from StoreItem where ItemId  = " & DB.Quote(ItemId)
        Dim dr As SqlDataReader = DB.GetReader(SQL)
        If dr.Read Then
            sResult = dr("SKU") & "|" & dr("ItemName")
        End If
        dr.Close()

        Response.Write(sResult)
    End Sub

    Sub CheckAvailbility()
        If Request("Username") = String.Empty Then
            Response.Write("FAIL")
            Response.End()
        End If


        If CheckAvailbilityVendor() And CheckAvailbilityBuilder() Then
            Response.Write("OK")
        Else
            Response.Write("FAIL")
        End If
    End Sub

    Function CheckAvailbilityVendor() As Boolean
        If Request("Username") = String.Empty Then
            Return False
        End If

        Dim MemberId As Integer = Nothing
        If Request("VendorAccountId") <> String.Empty Then
            MemberId = Convert.ToInt32(Request("VendorAccountId"))
        End If

        Dim SQL As String = "SELECT top 1 Username FROM VendorAccount WHERE Username = " & DB.Quote(Request("Username"))
        If MemberId <> Nothing Then
            SQL &= " and VendorAccountId <> " & MemberId
        End If
        Dim Username As String = DB.ExecuteScalar(SQL)
        If Username = String.Empty Then
            Return True
        Else
            Return False
        End If
    End Function

    Function CheckAvailbilityBuilder() As Boolean
        If Request("Username") = String.Empty Then
            Return False
        End If

        Dim BuilderId As Integer = Nothing
        If Request("BuilderAccountId") <> String.Empty Then
            BuilderId = Convert.ToInt32(Request("BuilderAccountId"))
        End If

        Dim SQL As String = "SELECT top 1 Username FROM BuilderAccount WHERE Username = " & DB.Quote(Request("Username"))
        If BuilderId <> Nothing Then
            SQL &= " and BuilderAccountId <> " & DB.Number(BuilderId)
        End If
        Dim Username As String = DB.ExecuteScalar(SQL)
        If Username = String.Empty Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub DisplaySuggest()
        Dim SQL, q As String
        Dim sArray1, sArray2, sArray3, sArray4, sConn
        Dim InactiveLft As String = String.Empty, InactiveRgt As String = String.Empty
        sConn = ""

        Dim RecordCount As Integer = -1

        q = Request("q")
        If Trim(q) = String.Empty Then Exit Sub

        sArray1 = "new Array("
        sArray2 = "new Array("
        sArray3 = "new Array("
        sArray4 = "new Array("

        SQL = " from (SELECT ItemId, CustomUrl, ItemName, coalesce(Image,(select top 1 productimage from storeitemattribute sia where sia.itemid = si.itemid and productimage is not null)) as image from StoreItem si WHERE (ItemName LIKE " & DB.Quote("%" & q & "%") _
         & " OR Sku LIKE " & DB.Quote("%" & q & "%") & ") AND si.IsActive = 1) as tmp"

        RecordCount = DB.ExecuteScalar("select count(*) " & SQL)

        Dim dr As SqlDataReader = DB.GetReader("select top 5 ItemId, CustomUrl, ItemName, Image " & SQL & " ORDER BY ItemName ASC")
        While dr.Read
            sArray1 &= sConn & Escape(dr("ItemName"))
            sArray2 &= sConn & Escape(dr("ItemId"))
            sArray3 &= sConn & Escape(IIf(IsDBNull(dr("Image")), "", dr("Image")))
            sArray4 &= sConn & Escape(GlobalRefererName & IIf(IsDBNull(dr("CustomUrl")), "/store/item.aspx?ItemId=" & dr("ItemId"), dr("CustomUrl")))
            sConn = ","
        End While
        dr.Close()

        sArray1 &= ")"
        sArray2 &= ")"
        sArray3 &= ")"
        sArray4 &= ")"

        Response.Write("showQueryDiv('al', " & sArray1 & "," & sArray2 & "," & sArray3 & "," & sArray4 & ",'" & RecordCount & "')")
    End Sub
End Class