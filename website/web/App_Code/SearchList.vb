Imports Utility
Imports Components
Imports DataLayer
Imports IDevSearch
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data.SqlClient

<WebService(Namespace:="http://www.cbusa.us/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Script.Services.ScriptService()> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class SearchList
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetSearchList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal MaxResults As Integer) As String
        Table = Core.ProtectParam(Table)
        TextField = Core.ProtectParam(TextField)
        ValueField = Core.ProtectParam(ValueField)
        Dim sql As String
        If MaxResults = 0 Then
            sql = "select "
        Else
            sql = "select top " & (MaxResults) & " "
        End If
        If ValueField <> String.Empty Then
			sql &= TextField & "," & ValueField & " from [" & Table & "] where " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%") & " order by CHARINDEX(" & GlobalDB.DB.Quote(Text) & "," & TextField & ") ASC, " & TextField
        Else
			sql &= TextField & " from [" & Table & "] where " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%") & " order by CHARINDEX(" & GlobalDB.DB.Quote(Text) & "," & TextField & ") ASC, " & TextField
        End If

		Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(" & TextField & ") from [" & Table & "] where " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%"))

        Dim sdr As SqlClient.SqlDataReader = GlobalDB.DB.GetReader(sql)
        Dim items As New StringBuilder()
        Dim conn As String = String.Empty
        items.Append("[")

        While sdr.Read()
            Dim value As String = String.Empty
            If ValueField <> String.Empty Then value = IIf(IsDBNull(sdr(ValueField)), Nothing, sdr(ValueField))
            items.Append(conn & "{'text':'" & Convert.ToString(sdr(TextField)).Replace("'", "\'") & "','value':'" & value & "'}")
            conn = ","
        End While
        sdr.Close()
        items.Append("]")

        Return "{'count':" & count & ",'items':" & items.ToString & "}"
    End Function

    <WebMethod()> _
    Public Function GetFilteredSearchList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Table = Core.ProtectParam(Table)
        TextField = Core.ProtectParam(TextField)
        ValueField = Core.ProtectParam(ValueField)
        WhereClause = Core.ProtectParam(WhereClause)
        Dim sql As String
        If MaxResults = 0 Then
            sql = "select "
        Else
            sql = "select top " & (MaxResults) & " "
        End If
        If ValueField <> String.Empty Then
            sql &= TextField & "," & ValueField & " from [" & Table & "] where " & WhereClause & " and " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%") & " order by CHARINDEX(" & GlobalDB.DB.Quote(Text) & "," & TextField & ") ASC, " & TextField
        Else
            sql = TextField & " from [" & Table & "] where " & WhereClause & " and " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%") & " order by CHARINDEX(" & GlobalDB.DB.Quote(Text) & "," & TextField & ") ASC, " & TextField
        End If

		Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(" & TextField & ") from [" & Table & "] where " & TextField & " like " & GlobalDB.DB.Quote("%" & Text & "%") & " and " & WhereClause)

        Dim sdr As SqlClient.SqlDataReader = GlobalDB.DB.GetReader(sql)
        Dim items As New StringBuilder()
        Dim conn As String = String.Empty

        items.Append("[")
        While sdr.Read()
            Dim value As String = String.Empty
            If ValueField <> String.Empty Then value = sdr(ValueField)
            items.Append(conn & "{'text':'" & Convert.ToString(sdr(TextField)).Replace("'", "\'") & "','value':'" & value & "'}")
            conn = ","
        End While

        sdr.Close()
        items.Append("]")
        Return "{'count':" & count & ",'items':" & items.ToString & "}"
    End Function

#Region "IDev SearchList"
    <WebMethod(EnableSession:=True)> _
    Public Function GetIDevSearchList(ByVal text As String, ByVal table As String, ByVal field As String, ByVal MaxResults As Integer) As String
        Dim items As New StringBuilder
        Dim conn As String = String.Empty
        Dim sr As SearchResult
        Dim total As Integer = 0
        Dim idx As New SearchIndex
        idx.Directory = System.Configuration.ConfigurationManager.AppSettings("SearchIndexDirectory")
        idx.IndexName = System.Configuration.ConfigurationManager.AppSettings("SearchIndexName")
        idx.DefaultOperator = "OR"
        Dim q As IndexQuery = GetIndexQuery(MaxResults, text)

        sr = idx.Search(q)

        If Not text = String.Empty Then
            Dim Words() As String = text.Split(" "c)
            If Words.Length > 1 Then
                If sr.Count = 0 Then
                    idx.DefaultOperator = "OR"
                    sr = idx.Search(q)
                End If
            End If
        End If

        items.Append("[")
        For i As Integer = 0 To sr.ds.Tables(0).Rows.Count
            Dim value As String = sr.ds.Tables(0).Rows(i).Item("ProductID")
            Dim name As String = sr.ds.Tables(0).Rows(i).Item("Product")
            items.Append(conn & "{'text':'" & name.Replace("'", "\'") & "','value':'" & value.Replace("'", "\'") & "'}")
            conn = ","
        Next
        items.Append("]")

        Return "{'count':" & sr.Count & ",'items':" & items.ToString & "}"
    End Function

    Private Function GetLabelOrValueText(ByVal text As String, ByVal row As DataRow, ByVal fieldvalue As String) As String
        Dim CountText As String = String.Empty
        If row("count") >= 0 Then
            CountText = "(" & row("count") & ")"
        End If
        If row("Count") = -1 Then
            Return row("label") & " " & CountText
        Else
            Dim value As String = RemovePipe(row(fieldvalue))
            Return value & " " & CountText
        End If
    End Function

    Private Function RemovePipe(ByVal s As String) As String
        Dim pipe As String = InStrRev(s, "|")
        Return Right(s, Len(s) - pipe)
    End Function

    Private Function GetIndexQuery(ByVal MaxLength As Integer, ByVal Keyword As String, Optional ByVal Sort As String = "") As IndexQuery
        Dim q As String = IIf(Keyword = "*", Keyword, Trim(Keyword & IIf(Keyword.IndexOf(" ") = -1, "*", "")))

        Dim facets As New GenericCollection(Of Facet)
        Dim facetcache As New GenericCollection(Of Facet)
        Dim query As New IndexQuery
        query.Keywords = Keyword
        query.MaxDocs = 1000
        query.Facets = facets
        query.FacetCache = facetcache
        query.SortBy = String.Empty
        query.SortReverse = False
        query.BestFragmentField = "Description"
        query.PageNo = 1
        query.MaxPerPage = MaxLength
        query.ForceRefresh = False
        query.FacetCacheDuration = 60 * 10 'ten minutes
        query.MaxHitsForCache = 100

        Select Case Sort
            Case "priceasc"
                query.SortBy = "SalePrice"
                query.SortReverse = False
            Case "pricedesc"
                query.SortBy = "SalePrice"
                query.SortReverse = True
            Case "score", ""
                query.SortBy = String.Empty
                query.SortReverse = False
        End Select

        'AddFacet(bc, facets, facetcache, "Brand", "Brand", "brand")
        'AddFacet(bc, facets, facetcache, "ProductType", "ProductType", "producttype")
        'AddFacet(bc, facets, facetcache, "Manufacturer", "Manufacturer", "manufacturer")
        'AddFacet(bc, facets, facetcache, "UnitOfMeasure", "UnitOfMeasure", "unitofmeasure")
        Return query
    End Function
#End Region

    '#Region "Custom AutoComplete"
    '    <WebMethod(EnableSession:=True)> _
    '    Public Function GetFilteredBuilderList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal MaxResults As Integer) As String
    '        Text = Core.ProtectParam(Text)
    '        Dim out As New StringBuilder
    '        Dim conn As String = String.Empty
    '        Dim isComplete As Boolean
    '        out.Append("[")
    '        If HttpContext.Current.Session("SelectedLLCID") IsNot Nothing Then
    '            Dim dt As DataTable = BuilderRow.GetListByLLC(GlobalDB.DB, HttpContext.Current.Session("SelectedLLCID"), Text)
    '            Dim e As Generic.IEnumerator(Of DataRow) = dt.Rows.GetEnumerator
    '            Dim count As Integer = 0
    '            While count < MaxResults AndAlso e.MoveNext
    '                out.Append(conn & "{'text':'" & Convert.ToString(e.Current("CompanyName")).Replace("'", "\'") & "','value':'" & e.Current("BuilderId") & "'}")
    '                conn = ","
    '                count += 1
    '            End While
    '            isComplete = (dt.Rows.Count <= MaxResults)
    '        End If
    '        out.Append("]")

    '        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & out.ToString & "}"
    '    End Function

    '    <WebMethod(EnableSession:=True)> _
    '    Public Function GetCountyList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
    '        Text = Core.ProtectParam(Text)
    '        WhereClause = Core.ProtectParam(WhereClause)
    '        Dim sql As String
    '        If MaxResults = 0 Then
    '            sql = "select"
    '        Else
    '            sql = "select top " & (MaxResults + 1)
    '        End If
    '        sql &= " distinct CountyName, CountyFIPS from ZipCode where CountyName like " & GlobalDB.DB.Quote(Text & "%") & " and " & WhereClause & " order by CountyName"
    '        Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql)
    '        Dim out As New StringBuilder("[")
    '        Dim conn As String = String.Empty
    '        Dim count As Integer = 0
    '        While count < MaxResults AndAlso sdr.Read()
    '            out.Append(conn & "{'text':'" & Convert.ToString(sdr.Item("CountyName")).Replace("'", "\'") & "','value':'" & sdr.Item("CountyFips") & "'}")
    '            conn = ","
    '            count += 1
    '        End While
    '        Dim isComplete As Boolean = Not sdr.Read
    '        sdr.Close()
    '        out.Append("]")
    '        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & out.ToString & "}"
    '    End Function
    '#End Region

    <WebMethod(EnableSession:=True)> _
    Public Function GetCountyList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Try
            Text = Core.ProtectParam(Text)
            WhereClause = Core.ProtectParam(WhereClause)
            Dim sql As String
            Dim fields As String = String.Empty
            If MaxResults = 0 Then
                sql = "select"
            Else
                sql = "select top " & (MaxResults)
            End If
            fields = " CountyName, CountyFIPS from ZipCode where CountyName like " & GlobalDB.DB.Quote(Text & "%")
            If WhereClause <> String.Empty Then
                fields &= " and " & WhereClause
            End If
            fields &= " group by CountyName, CountyFips "
            Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(*) from (select " & fields & " ) as temp")

            Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql & fields & " order by CountyName")
            Dim out As New StringBuilder("[")
            Dim conn As String = String.Empty

            While sdr.Read()
                out.Append(conn & "{'text':'" & Convert.ToString(sdr.Item("CountyName")).Replace("'", "\'") & "','value':'" & sdr.Item("CountyFips") & "'}")
                conn = ","
            End While

            sdr.Close()
            out.Append("]")

            Return "{'count':" & count & ",'items':" & out.ToString & "}"
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetVendorUsers(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Try
            Text = Core.ProtectParam(Text)
            WhereClause = Core.ProtectParam(WhereClause)
            Dim sql As String
            Dim fields As String = String.Empty
            If MaxResults = 0 Then
                sql = "select"
            Else
                sql = "select top " & (MaxResults)
            End If
            fields = " VendorAccountID, FirstName, LastName from VendorAccount where (LastName + ', ' + FirstName) like " & GlobalDB.DB.Quote("%" & Text & "%")
            If WhereClause <> String.Empty Then
                fields &= " and " & WhereClause
            End If
            Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(*) from (select " & fields & " ) as temp")

            Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql & fields & " order by LastName, FirstName")
            Dim out As New StringBuilder("[")
            Dim conn As String = String.Empty

            While sdr.Read()
                out.Append(conn & "{'text':'" & Convert.ToString(Core.GetString(sdr.Item("LastName")) & ", " & Core.GetString(sdr.Item("FirstName"))).Replace("'", "\'") & "','value':'" & sdr.Item("VendorAccountID") & "'}")
                conn = ","
            End While

            sdr.Close()
            out.Append("]")

            Return "{'count':" & count & ",'items':" & out.ToString & "}"
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetEagleOneTakeoffList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Try
            Text = Core.ProtectParam(Text)
            WhereClause = Core.ProtectParam(WhereClause)
            Dim sql As String
            Dim fields As String
            Dim conn As String = " where "
            If MaxResults = 0 Then
                sql = "select"
            Else
                sql = "select top " & MaxResults
            End If
            fields = " (l.LLC + ' - ' + b.CompanyName + ': ' + t.Title) as Label, t.TakeoffID " _
                   & " from Takeoff t inner join Builder b on t.BuilderID=b.BuilderID" _
                   & "  inner join LLC l on l.LLCID=b.LLCID"

            If Text <> String.Empty Then
                fields &= conn & "  (l.LLC + ' - ' + b.CompanyName + ': ' + t.Title) like " & GlobalDB.DB.Quote("%" & Text & "%")
                conn = " and "
            End If

            If WhereClause <> String.Empty Then
                fields &= conn & WhereClause
                conn = " and "
            End If

            Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(*) from (select " & fields & ") as tmp")
            Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql & fields & " order by Label")
            Dim out As New StringBuilder("[")
            conn = String.Empty

            While sdr.Read
                out.Append(conn & "{'text':'" & Convert.ToString(sdr.Item("Label")).Replace("'", "\'") & "','value':'" & sdr.Item("TakeoffID") & "'}")
                conn = ","
            End While
            sdr.Close()
            out.Append("]")

            Return "{'count':" & count & ",'items':" & out.ToString & "}"
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Function GetAllOrders(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Try
            Text = Core.ProtectParam(Text)
            WhereClause = Core.ProtectParam(WhereClause)
            Dim sql As String
            Dim fields As String
            Dim conn As String = " where "
            If MaxResults = 0 Then
                sql = "select o.* "
            Else
                sql = "select top  " & MaxResults & " o.* "
            End If

            fields = " from  ( Select OrderID,OrderstatusID, BuilderID,Title,0 As TwoPriceCampaignId FROM [Order]  " _
                   & " UNION ALL  " _
                   & " Select TwoPriceOrderID,OrderstatusID ,BuilderID,Title ,TwoPriceCampaignId From TwoPriceOrder " _
                  & " ) o left outer join OrderStatus s on o.OrderStatusID=s.OrderStatusID "
            If Text <> String.Empty Then
                fields &= conn & "  Title like " & GlobalDB.DB.Quote("%" & Text & "%")
                conn = " and "
            End If

            If WhereClause <> String.Empty Then
                fields &= conn & WhereClause
                conn = " and "
            End If
            fields &= " and s.OrderStatus != 'Unsaved' "
            Dim count As Integer = GlobalDB.DB.ExecuteScalar("select count(*)    " & fields)
            Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql & fields & " order by Title")
            Dim out As New StringBuilder("[")
            conn = String.Empty

            While sdr.Read

                out.Append(conn & "{'text':'" & Convert.ToString(sdr.Item("Title")).Replace("'", "\'") & "','value':'" & sdr.Item("OrderID") & "-" & sdr.Item("TwoPriceCampaignId") & "'}")
                conn = ","
            End While
            sdr.Close()
            out.Append("]")

            Return "{'count':" & count & ",'items':" & out.ToString & "}"
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Function

End Class
