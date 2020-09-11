Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VINDICIA_SOAP_LOGS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_SoapId.Text = Request("F_SoapId")
            F_ReturnString.Text = Request("F_ReturnString")
            F_BuilderGUID.Text = Request("F_BuilderGUID")
            F_ReturnCodeLBound.Text = Request("F_ReturnCodeLBound")
            F_ReturnCodeUBound.Text = Request("F_ReturnCodeUBound")
            F_CreateDateLBound.Text = Request("F_CreateDateLBound")
            F_CreateDateUBound.Text = Request("F_CreateDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "VindiciaSoapLogId"
                gvList.SortOrder = "DESC"
            End If


            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM VindiciaSoapLog  "

        If Not F_SoapId.Text = String.Empty Then
            SQL = SQL & Conn & "SoapId LIKE " & DB.FilterQuote(F_SoapId.Text)
            Conn = " AND "
        End If
        If Not F_ReturnString.Text = String.Empty Then
            SQL = SQL & Conn & "ReturnString LIKE " & DB.FilterQuote(F_ReturnString.Text)
            Conn = " AND "
        End If
        If Not F_BuilderGUID.Text = String.Empty Then
            SQL = SQL & Conn & "BuilderGUID = " & DB.Quote(F_BuilderGUID.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateLBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate >= " & DB.Quote(F_CreateDateLBound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUBound.Text = String.Empty Then
            SQL = SQL & Conn & "CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUBound.Text))
            Conn = " AND "
        End If
        If Not F_ReturnCodeLBound.Text = String.Empty Then
            SQL = SQL & Conn & "ReturnCode >= " & DB.Number(F_ReturnCodeLBound.Text)
            Conn = " AND "
        End If
        If Not F_ReturnCodeUBound.Text = String.Empty Then
            SQL = SQL & Conn & "ReturnCode <= " & DB.Number(F_ReturnCodeUBound.Text)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class

