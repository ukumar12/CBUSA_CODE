Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage
    Private dtPaymentterms As DataTable
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim i As Integer = 1


        If Not IsPostBack Then

            For i = 1 To 4
                Me.F_StartPeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
                Me.F_ComparePeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            For i = 1 To 15
                Me.F_StartPeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
                Me.F_ComparePeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
            Next
            BindFields()

        End If


    End Sub

    Private Sub BindFields()

        'gvList.BindList = AddressOf BindList
        F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
        F_VendorID.DataValueField = "VendorID"
        F_VendorID.DataTextField = "CompanyName"
        F_VendorID.DataBind()
        F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))
        F_VendorID.SelectedValue = Request("F_VendorID")
        F_LLC.DataSource = LLCRow.GetList(DB, "LLC", "ASC")
        F_LLC.DataValueField = "LLC"
        F_LLC.DataTextField = "LLC"
        F_LLC.DataBind()
        F_LLC.Items.Insert(0, New ListItem("-- ALL --", ""))


    End Sub

    Private Function GetSearchResults() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "




        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT HistoricVendorID , VendorName , LLC ,SUM(finalAmount) As FinalAmount  "
        SQL = " FROM vAPVforReport  "

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & " VendorID = " & DB.Number(F_VendorID.SelectedValue)
            Conn = " AND "
        End If

        ' if ALL not selected, determine values to include
        If Not F_LLC.Items(0).Selected Then
            Dim LLSString As String = String.Empty
            For Each li As ListItem In F_LLC.Items
                If li.Selected Then
                    LLSString &= DB.Quote(li.Value) & ","
                End If
            Next
            If Not String.IsNullOrEmpty(LLSString) Then
                ' trim last comma
                LLSString = LLSString.Substring(0, LLSString.Length - 1)
                SQL = SQL & Conn & "LLC in (" & LLSString & ")"
                Conn = " AND "
            End If
        End If




        Dim StartYear As Integer = 0
        Dim EndYear As Integer = 0
        Dim StartQuarter As Integer = 0
        Dim EndQuarter As Integer = 0
        Integer.TryParse(F_StartPeriodYear.SelectedValue, StartYear)
        Integer.TryParse(F_StartPeriodQuarter.SelectedValue, StartQuarter)
        Integer.TryParse(F_ComparePeriodYear.SelectedValue, EndYear)
        Integer.TryParse(F_ComparePeriodQuarter.SelectedValue, EndQuarter)


        'if period spans years
        If StartYear > 0 AndAlso StartYear <> EndYear Then
            SQL = SQL & Conn & "("
            ' first do start and end condition
            SQL &= "(PeriodQuarter >= " & DB.Number(StartQuarter) & " AND PeriodYear = " & DB.Number(StartYear)
            SQL &= ") OR ("
            SQL &= "PeriodQuarter <= " & DB.Number(EndQuarter) & " AND PeriodYear = " & DB.Number(EndYear) & ")"
            ' do inbetween conditions if needed
            If StartYear + 1 <> EndYear Then
                For i As Integer = StartYear + 1 To EndYear - 1
                    SQL &= " OR (PeriodYear = " & DB.Number(i) & ")"
                Next
            End If
            SQL &= ")"
            Conn = " AND "
            ' if period within same year
        ElseIf StartYear > 0 AndAlso StartYear = EndYear Then
            SQL = SQL & Conn & "PeriodQuarter >= " & DB.Number(StartQuarter)
            Conn = " AND "
            SQL = SQL & Conn & "PeriodQuarter <= " & DB.Number(EndQuarter)
            Conn = " AND "
            SQL = SQL & Conn & "PeriodYear = " & DB.Number(StartYear)
            Conn = " AND "
        End If

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " group by VendorName ,HistoricVendorID , LLC ORDER BY LLC, VendorName  ", 1200)
        gvList.Pager.NofRecords = res.Rows.Count

        Return res

    End Function
    Private Sub BindList()
        gvList.DataSource = GetSearchResults().DefaultView
        gvList.DataBind()
    End Sub



    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        dtPaymentterms = DB.GetDataTable("Select CompanyName As VendorName, PaymentTerms from Vendor")
        gvList.PageIndex = 0
        BindList()
    End Sub


    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        Dim res As DataTable = GetSearchResults()
        dtPaymentterms = DB.GetDataTable("Select CompanyName As VendorName, PaymentTerms from Vendor")
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("LLC ,VendorName , VendorPaymentTerms , Total Arbitrated Purchase Volume")

        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("VendorName")

                Dim LLC As String = String.Empty
                If Not IsDBNull(row("LLC")) Then
                    LLC = row("LLC")

                End If
                Dim PaymentTerms As String = String.Empty


                If Not IsDBNull(row("VendorName")) Then
                    ' PaymentTerms = row("PaymentTerms")
                    Dim drPaymentterms() As DataRow = dtPaymentterms.Select(String.Format("VendorName = {0}", DB.Quote(row("VendorName"))))
                    For Each drrow As DataRow In drPaymentterms
                        PaymentTerms &= drrow(1)
                    Next
                End If
                Dim TotalArbitratedPurchaseVolume As String = String.Empty
                If Not IsDBNull(row("FinalAmount")) Then
                    TotalArbitratedPurchaseVolume = row("FinalAmount")
                Else
                    TotalArbitratedPurchaseVolume = "0"
                End If
                sw.WriteLine(Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(PaymentTerms) & "," & Core.QuoteCSV(TotalArbitratedPurchaseVolume))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If

        Dim LtlPaymentTerms As Literal = e.Row.FindControl("LtlPaymentTerms")
        Dim drPaymentterms() As DataRow = dtPaymentterms.Select(String.Format("VendorName = {0}", DB.Quote(e.Row.DataItem("VendorName"))))
        LtlPaymentTerms.Text = String.Empty
        For Each row As DataRow In drPaymentterms
            LtlPaymentTerms.Text &= row(1)
        Next

    End Sub




End Class
