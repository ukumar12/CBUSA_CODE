Imports Utility
Imports Components
Imports DataLayer
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data.SqlClient

<WebService(Namespace:="http://www.cbusa.us/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Script.Services.ScriptService()> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class AutoComplete
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetAutoCompleteList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal MaxResults As Integer) As String
        Table = Core.ProtectParam(Table)
        TextField = Core.ProtectParam(TextField)
        ValueField = Core.ProtectParam(ValueField)
        Dim sql As String
        If MaxResults = 0 Then
            sql = "select "
        Else
            sql = "select top " & (MaxResults + 1) & " "
        End If
        If ValueField <> String.Empty Then
            sql &= TextField & "," & ValueField & " from " & Table & " where " & TextField & " like " & GlobalDB.DB.Quote(Text & "%") & " order by " & TextField
        Else
            sql &= TextField & " from " & Table & " where " & TextField & " like " & GlobalDB.DB.Quote(Text & "%") & " order by " & TextField
        End If
        Dim sdr As SqlClient.SqlDataReader = GlobalDB.DB.GetReader(sql)
        Dim items As New StringBuilder()
        Dim conn As String = String.Empty
        items.Append("[")
        Dim count As Integer = 0
        While count < MaxResults AndAlso sdr.Read()
            Dim value As String = String.Empty
            If ValueField <> String.Empty Then value = sdr(ValueField)
            items.Append(conn & "{'text':'" & Convert.ToString(sdr(TextField)).Replace("'", "\'") & "','value':'" & value & "'}")
            conn = ","
            count += 1
        End While
        Dim isComplete As Boolean = Not sdr.Read
        sdr.Close()
        items.Append("]")

        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & items.ToString & "}"
    End Function

    <WebMethod()> _
    Public Function GetFilteredACList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Table = Core.ProtectParam(Table)
        TextField = Core.ProtectParam(TextField)
        ValueField = Core.ProtectParam(ValueField)
        WhereClause = Core.ProtectParam(WhereClause)
        Dim sql As String
        If MaxResults = 0 Then
            sql = "select "
        Else
            sql = "select top " & (MaxResults + 1) & " "
        End If
        If ValueField <> String.Empty Then
            sql = "select " & TextField & "," & ValueField & " from [" & Table & "] where " & WhereClause & " and " & TextField & " like " & GlobalDB.DB.Quote(Text & "%") & " order by " & TextField
        Else
            sql = "select " & TextField & " from [" & Table & "] where " & WhereClause & " and " & TextField & " like " & GlobalDB.DB.Quote(Text & "%") & " order by " & TextField
        End If
        Dim sdr As SqlClient.SqlDataReader = GlobalDB.DB.GetReader(sql)
        Dim items As New StringBuilder()
        Dim conn As String = String.Empty
        Dim count As Integer = 0
        items.Append("[")
        While count < MaxResults AndAlso sdr.Read()
            Dim value As String = String.Empty
            If ValueField <> String.Empty Then value = sdr(ValueField)
            items.Append(conn & "{'text':'" & Convert.ToString(sdr(TextField)).Replace("'", "\'") & "','value':'" & value & "'}")
            conn = ","
        End While
        Dim isComplete As Boolean = Not sdr.Read
        sdr.Close()
        items.Append("]")
        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & items.ToString & "}"
    End Function


#Region "Custom AutoComplete"
    <WebMethod(EnableSession:=True)> _
    Public Function GetFilteredBuilderList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal MaxResults As Integer) As String
        Text = Core.ProtectParam(Text)
        Dim out As New StringBuilder
        Dim conn As String = String.Empty
        Dim isComplete As Boolean
        out.Append("[")
        If HttpContext.Current.Session("SelectedLLCID") IsNot Nothing Then
            Dim dt As DataTable = BuilderRow.GetListByLLC(GlobalDB.DB, HttpContext.Current.Session("SelectedLLCID"), Text)
            Dim e As Generic.IEnumerator(Of DataRow) = dt.Rows.GetEnumerator
            Dim count As Integer = 0
            While count < MaxResults AndAlso e.MoveNext
                out.Append(conn & "{'text':'" & Convert.ToString(e.Current("CompanyName")).Replace("'", "\'") & "','value':'" & e.Current("BuilderId") & "'}")
                conn = ","
                count += 1
            End While
            isComplete = (dt.Rows.Count <= MaxResults)
        End If
        out.Append("]")

        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & out.ToString & "}"
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetCountyList(ByVal Table As String, ByVal TextField As String, ByVal ValueField As String, ByVal Text As String, ByVal WhereClause As String, ByVal MaxResults As Integer) As String
        Text = Core.ProtectParam(Text)
        WhereClause = Core.ProtectParam(WhereClause)
        Dim sql As String
        If MaxResults = 0 Then
            sql = "select"
        Else
            sql = "select top " & (MaxResults + 1)
        End If
        sql &= " distinct CountyName, CountyFIPS from ZipCode where CountyName like " & GlobalDB.DB.Quote(Text & "%") & " and " & WhereClause & " order by CountyName"
        Dim sdr As SqlDataReader = GlobalDB.DB.GetReader(sql)
        Dim out As New StringBuilder("[")
        Dim conn As String = String.Empty
        Dim count As Integer = 0
        While count < MaxResults AndAlso sdr.Read()
            out.Append(conn & "{'text':'" & Convert.ToString(sdr.Item("CountyName")).Replace("'", "\'") & "','value':'" & sdr.Item("CountyFips") & "'}")
            conn = ","
            count += 1
        End While
        Dim isComplete As Boolean = Not sdr.Read
        sdr.Close()
        out.Append("]")
        Return "{'isComplete':" & isComplete.ToString.ToLower & ",'items':" & out.ToString & "}"
    End Function
#End Region
End Class
