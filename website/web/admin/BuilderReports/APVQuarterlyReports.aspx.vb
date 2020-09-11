Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic

Partial Class Index
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")


        Dim i As Integer = 1
        If Not IsPostBack Then
            For i = 1 To 4
                Me.F_StartPeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
                Me.F_EndPeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            For i = 1 To 15
                Me.F_StartPeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
                Me.F_EndPeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
            Next

            Me.F_StartPeriodQuarter.SelectedValue = ((Now.AddMonths(-6).Month - 1) \ 3) + 1
            Me.F_EndPeriodQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1

            Me.F_StartPeriodYear.SelectedValue = Now.AddMonths(-6).Year
            Me.F_EndPeriodYear.SelectedValue = Now.AddMonths(-3).Year
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")

            F_BuilderID.DataValueField = "HistoricID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))
            F_BuilderID.SelectedValue = Request("F_BuilderID")


        End If
    End Sub

    Private Function GetBuilders(ByVal db As Database) As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " Where "



        SQLFields = "SELECT b.HistoricId,b.companyname,b.llcid,b.IsActive , l.LLC, b.submitted "
        SQL = " FROM Builder b LEFT OUTER JOIN  LLC l   On b.LLCID = l.LLCID"

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & db.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If

        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsActive  = " & db.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If

        If Not F_LLC.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.LLCID = " & db.Number(F_LLC.SelectedValue)
            Conn = " AND "
        End If





        Dim res As DataTable = db.GetDataTable(SQLFields & SQL & " ORDER BY CompanyName ")
        Return res
    End Function

    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        Dim dttoexport As DataTable = ExportList(DB)
        Dim dtAPV As New DataTable

        Dim dtbuilders As DataTable = GetBuilders(DB)
        Dim dtGetQuarters As DataTable = GetQuarterAndYearfromDate(DB, GetDateFromQuarterAndYear(DB, F_StartPeriodQuarter.SelectedValue, F_StartPeriodYear.SelectedValue), GetDateFromQuarterAndYear(DB, F_EndPeriodQuarter.SelectedValue, F_EndPeriodYear.SelectedValue))

        dtAPV.Columns.Add(New DataColumn("BuilderID", System.Type.GetType("System.String")))
        dtAPV.Columns.Add(New DataColumn("CompanyName", System.Type.GetType("System.String")))
        dtAPV.Columns.Add(New DataColumn("Market", System.Type.GetType("System.String")))
        dtAPV.Columns.Add(New DataColumn("IsActive", System.Type.GetType("System.String")))
        dtAPV.Columns.Add(New DataColumn("CreateDate", System.Type.GetType("System.String")))
        dtAPV.Columns.Add(New DataColumn("Createyear", System.Type.GetType("System.String")))
        For Each drGetQuarters In dtGetQuarters.Rows
            dtAPV.Columns.Add(New DataColumn(drGetQuarters("DisplayValue"), System.Type.GetType("System.String")))
        Next


        For Each row In dtbuilders.Rows
            Dim drAPV As DataRow = dtAPV.NewRow
            drAPV("BuilderID") = row("HistoricID")
            drAPV("CompanyName") = row("CompanyName")
            drAPV("Market") = row("LLC")
            drAPV("IsActive") = Core.GetInt((row("IsActive")))
            drAPV("CreateDate") = CType(row("Submitted"), Date).ToShortDateString
            drAPV("Createyear") = CType(row("Submitted"), Date).Year
            For k = 6 To dtAPV.Columns.Count - 1
                drAPV.Item(k) = FilterFinalAmountByQuarterAndYearAndBuilder(dttoexport, drAPV.Table.Columns(k).ColumnName.Split("|")(1), drAPV.Table.Columns(k).ColumnName.Split("|")(0), row("HistoricID"))
            Next

            dtAPV.Rows.Add(drAPV)
        Next


        Export(DB, dtAPV)




    End Sub




    Protected Function FilterFinalAmountByQuarterAndYearAndBuilder(ByVal dtExportFilter As DataTable, ByVal PeriodYear As Integer, ByVal PeriodQuarter As Integer, ByVal HistoricBuilderID As Integer) As Double
        Dim dtExportview As DataView = dtExportFilter.DefaultView
        Dim filters As New List(Of String)
        If HistoricBuilderID <> Nothing Then
            filters.Add("HistoricBuilderID = " & DB.Number(HistoricBuilderID))
        End If

        filters.Add("PeriodQuarter = " & DB.Number(PeriodQuarter))
        filters.Add("PeriodYear = " & DB.Number(PeriodYear))

        dtExportview.RowFilter = String.Join(" AND ", filters.ToArray)

        If dtExportview.Count > 0 Then
            Return FormatCurrency(dtExportview.Item(0)("Amount"), 2)
        Else
            Return FormatCurrency(0, 2)
        End If



    End Function

    Protected Sub Export(ByVal db As Database, ByVal dt As DataTable)
        Dim res As DataTable = dt
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        Dim Conn As String = String.Empty
        Dim s As String = String.Empty
        Dim IgnoredFields As String() = {}
        For Each col As DataColumn In res.Columns
            If Array.IndexOf(IgnoredFields, col.ColumnName) = -1 Then
                If col.ColumnName.Contains("|") Then
                    s &= Conn & Core.QuoteCSV("Q" & col.ColumnName.ToUpper)
                    Conn = ","
                Else
                    s &= Conn & Core.QuoteCSV(col.ColumnName.ToUpper)
                    Conn = ","
                End If

             
            End If
        Next
        sw.WriteLine(s)
        For Each row As DataRow In res.Rows

            s = String.Empty
            Conn = String.Empty
            For Each col As DataColumn In res.Columns
                If Array.IndexOf(IgnoredFields, col.ColumnName) = -1 Then
                    Dim val As String = Convert.ToString(row(col.ColumnName))
                    s &= Conn & Core.QuoteCSV(val)
                    Conn = ","
                End If
            Next
            sw.WriteLine(s)
        Next

        sw.Flush()
        sw.Close()
        sw.Dispose()
        divDownload.Visible = True
        lnkDownload.NavigateUrl = Folder & FileName


    End Sub
    Protected Function GetQuarterAndYearfromDate(ByVal db As Database, ByVal startdate As Date, ByVal enddate As Date) As DataTable




        Dim varname1 As New System.Text.StringBuilder
        varname1.Append("WITH mycte AS " & vbCrLf)
        varname1.Append("( " & vbCrLf)
        varname1.Append("  SELECT CAST(" & db.Quote(startdate) & " AS DATE) DateValue " & vbCrLf)
        varname1.Append("  UNION ALL " & vbCrLf)
        varname1.Append("  SELECT  DATEADD(Q,1,DateValue)    FROM    mycte  WHERE   DATEADD(Q,1,DateValue) <= " & db.Quote(enddate) & vbCrLf)
        varname1.Append(") " & vbCrLf)
        varname1.Append(" " & vbCrLf)
        varname1.Append("SELECT  CONVERT(varchar(3), DATENAME(QUARTER,DateValue)) As PeriodQuarter, " & vbCrLf)
        varname1.Append("Convert(varchar(4),DATEPART(YYYY,DateValue)) As PeriodYear, " & vbCrLf)
        varname1.Append(" CONVERT(varchar(3), DATENAME(QUARTER,DateValue)) + '|' +Convert(varchar(4),DATEPART(YYYY,DateValue)) As DisplayValue FROM  mycte")


        Dim dtGetQuarters As DataTable = db.GetDataTable(varname1.ToString)
        Return dtGetQuarters
    End Function

    Protected Function GetDateFromQuarterAndYear(ByVal db As Database, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer) As DateTime

        Dim DateGromQuarters As DateTime = db.ExecuteScalar("select CAST((CAST((" & db.Number(PeriodQuarter) & " - 1) * 3 + 1 AS varchar) + '/1/' + CAST(" & db.Number(PeriodYear) & " AS varchar)) as DateTime)")

        Return DateGromQuarters


    End Function

    Protected Function ExportList(ByVal db As Database) As DataTable
        Dim dt As DataTable = Nothing

        Dim CacheKey As String = "APVExportList_" & Date.Now.ToString("MMddyyyy")
        'To do later
        If Not System.Web.HttpContext.Current Is Nothing Then
            If Not TypeOf (System.Web.HttpContext.Current.Cache(CacheKey)) Is DataTable Then
                System.Web.HttpContext.Current.Cache.Remove(CacheKey)
            End If
            dt = System.Web.HttpContext.Current.Cache(CacheKey)
        End If

        If dt Is Nothing Then

            Dim varname1 As New System.Text.StringBuilder
            varname1.Append("with CTE as ( " & vbCrLf)
            varname1.Append("SELECT  *  " & vbCrLf)
            varname1.Append("FROM   vAPVQuarterlyReports AS APV " & vbCrLf)
            varname1.Append(") " & vbCrLf)
            varname1.Append("select DISTINCT SUM(Coalesce(FinalAmount,0)) OVER(Partition by HistoricBuilderID,PeriodQuarter,PeriodYear) as amount, PeriodQuarter , PeriodYear  ,HistoricBuilderID,QTRYEARDATE " & vbCrLf)
            varname1.Append("from cte " & vbCrLf)



            dt = db.GetDataTable(varname1.ToString, 1200)
            System.Web.HttpContext.Current.Cache.Insert(CacheKey, dt, Nothing, Date.UtcNow.AddSeconds(1800), TimeSpan.Zero)
        End If

        Dim dtview As DataView = dt.DefaultView
        Dim filters As New List(Of String)
        If F_BuilderID.SelectedValue <> Nothing Then
            filters.Add("HistoricBuilderID = " & db.Number(F_BuilderID.SelectedValue))
        End If

        filters.Add("QTRYEARDATE >= " & db.Quote(GetDateFromQuarterAndYear(db, F_StartPeriodQuarter.SelectedValue, F_StartPeriodYear.SelectedValue)))
        filters.Add("QTRYEARDATE <= " & db.Quote(GetDateFromQuarterAndYear(db, F_EndPeriodQuarter.SelectedValue, F_EndPeriodYear.SelectedValue)))

        dtview.RowFilter = String.Join(" AND ", filters.ToArray)
        Return dtview.ToTable


    End Function

End Class
