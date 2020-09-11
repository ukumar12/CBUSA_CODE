Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Collections.Generic

Namespace DataLayer

    Public Class NationalContractRow
        Inherits NationalContractBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BannerId As Integer)
            MyBase.New(DB, BannerId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ContractID As Integer) As NationalContractRow
            Dim row As NationalContractRow

            row = New NationalContractRow(DB, ContractID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ContractID As Integer)
            Dim row As NationalContractRow

            row = New NationalContractRow(DB, ContractID)
            row.Remove()
        End Sub
    End Class

    Public Class NationalContractBase
        Public Property DB As Database

        Public Property ContractID As Integer = Nothing
        Public Property Title As String = Nothing
        Public Property DescriptionPage As String = Nothing
        Public Property Manufacturer As String = Nothing
        Public Property ManufacturerSite As String = Nothing
        Public Property Products As String = Nothing
        Public Property StartDate As DateTime = Nothing
        Public Property EndDate As DateTime = Nothing
        Public Property ArchiveDate As Boolean = Nothing
        Public Property ContractTerm As String = Nothing

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            Me.DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ContractID As Integer)
            Me.DB = DB
            Me.ContractID = ContractID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM NationalContract WHERE ContractID = " & DB.Number(ContractID)
            r = DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As DataRow)
            ContractID = Convert.ToInt32(r.Item("ContractID"))
            If Not IsDBNull(r.Item("Title")) Then Title = Convert.ToString(r.Item("Title"))
            If Not IsDBNull(r.Item("Manufacturer")) Then Manufacturer = Convert.ToString(r.Item("Manufacturer"))
            If Not IsDBNull(r.Item("Products")) Then Products = Convert.ToString(r.Item("Products"))
            If Not IsDBNull(r.Item("StartDate")) Then StartDate = Convert.ToDateTime(r.Item("StartDate"))
            If Not IsDBNull(r.Item("EndDate")) Then EndDate = Convert.ToDateTime(r.Item("EndDate"))
            If Not IsDBNull(r.Item("ManufacturerSite")) Then ManufacturerSite = Convert.ToString(r.Item("ManufacturerSite"))
            If Not IsDBNull(r.Item("DescriptionPage")) Then DescriptionPage = Convert.ToString(r.Item("DescriptionPage"))
            If Not IsDBNull(r.Item("ArchiveDate")) Then ArchiveDate = Core.GetBoolean(r.Item("ArchiveDate"))
            If Not IsDBNull(r.Item("ContractTerm")) Then ContractTerm = Convert.ToString(r.Item("ContractTerm"))
        End Sub 'Load

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            ContractID = Convert.ToInt32(r.Item("ContractID"))
            If Not IsDBNull(r.Item("Title")) Then Title = Convert.ToString(r.Item("Title"))
            If Not IsDBNull(r.Item("Manufacturer")) Then Manufacturer = Convert.ToString(r.Item("Manufacturer"))
            If Not IsDBNull(r.Item("Products")) Then Products = Convert.ToString(r.Item("Products"))
            If Not IsDBNull(r.Item("StartDate")) Then StartDate = Convert.ToDateTime(r.Item("StartDate"))
            If Not IsDBNull(r.Item("EndDate")) Then EndDate = Convert.ToDateTime(r.Item("EndDate"))
            If Not IsDBNull(r.Item("ManufacturerSite")) Then ManufacturerSite = Convert.ToString(r.Item("ManufacturerSite"))
            If Not IsDBNull(r.Item("DescriptionPage")) Then DescriptionPage = Convert.ToString(r.Item("DescriptionPage"))
            If Not IsDBNull(r.Item("ArchiveDate")) Then ArchiveDate = Core.GetBoolean(r.Item("ArchiveDate"))
            If Not IsDBNull(r.Item("ContractTerm")) Then ContractTerm = Convert.ToString(r.Item("ContractTerm"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO NationalContract (" _
             & " Title" _
             & ",Manufacturer" _
             & ",Products" _
             & ",StartDate" _
             & ",EndDate" _
             & ",ManufacturerSite" _
             & ",DescriptionPage" _
             & ",ArchiveDate" _
             & ",ContractTerm" _
             & ") VALUES (" _
             & DB.Quote(Title) _
             & "," & DB.Quote(Manufacturer) _
             & "," & DB.Quote(Products) _
             & "," & DB.Quote(StartDate) _
             & "," & DB.Quote(EndDate) _
             & "," & DB.Quote(ManufacturerSite) _
             & "," & DB.Quote(DescriptionPage) _
             & "," & CInt(ArchiveDate) _
             & "," & DB.Quote(ContractTerm) _
             & ")"

            ContractID = DB.InsertSQL(SQL)

            Return ContractID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE NationalContract SET " _
             & " Title = " & DB.Quote(Title) _
             & ",Manufacturer = " & DB.Quote(Manufacturer) _
             & ",Products = " & DB.Quote(Products) _
             & ",StartDate = " & DB.Quote(StartDate) _
             & ",EndDate = " & DB.Quote(EndDate) _
             & ",ManufacturerSite = " & DB.Quote(ManufacturerSite) _
             & ",DescriptionPage = " & DB.Quote(DescriptionPage) _
             & ",ArchiveDate = " & CInt(ArchiveDate) _
             & ",ContractTerm = " & DB.Quote(ContractTerm) _
             & " WHERE ContractId = " & DB.Number(ContractID)

            DB.ExecuteSQL(SQL)
        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM NationalContractBuilder WHERE ContractID = " & DB.Number(ContractID)
            DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM NationalContractLLC WHERE ContractID = " & DB.Number(ContractID)
            DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM NationalContract WHERE ContractID = " & DB.Number(ContractID)
            DB.ExecuteSQL(SQL)
        End Sub 'Remove

#Region "LLCs"

        Public Sub UpdateLLCs(LLCIDsToInsert As List(Of String))
            ClearLLCs()


            For Each LLCID As Integer In LLCIDsToInsert.Where(Function(id) IsNumeric(id)).Select(Function(id) id)
                InsertLLC(LLCID)
            Next
        End Sub

        Public Sub UpdateLLCs(LLCsToInsert As List(Of LLCRow))
            ClearLLCs()

            For Each LLC As LLCRow In LLCsToInsert
                InsertLLC(LLC.LLCID)
            Next
        End Sub

        Protected Sub InsertLLC(LLCID As Integer)
            DB.ExecuteSQL(" INSERT INTO NationalContractLLC (LLCID, ContractID) VALUES (" & LLCID & "," & ContractID & ")")
        End Sub

        Protected Sub ClearLLCs()
            DB.ExecuteSQL("DELETE FROM NationalContractLLC WHERE ContractID = " & ContractID)
        End Sub

        Public Function GetLLCs() As List(Of LLCRow)
            Dim ret As New List(Of LLCRow)
            Dim IDs As Object = DB.ExecuteScalar("SELECT Stuff(" & _
                                                 "(SELECT N', ' + CONVERT(NVARCHAR(MAX), LLCID) FROM NationalContractLLC WHERE ContractID = " & ContractID & " FOR XML PATH(''),TYPE)" & _
                                                 ".value('text()[1]','nvarchar(max)'),1,2,N'')")

            If Not IsDBNull(IDs) Then
                For Each sec As Integer In CStr(IDs).Split(",")
                    Dim LLCID As Integer = 0
                    If Integer.TryParse(sec, LLCID) Then ret.Add(LLCRow.GetRow(DB, LLCID))
                Next
            End If

            Return ret
        End Function

#End Region

#Region "Builders"
        Public Sub ClearBuilders()
            DB.ExecuteSQL("DELETE FROM NationalContractBuilder WHERE ContractID = " & ContractID)
        End Sub

        Public Sub UpdateBuilders(BuilderIDsToInsert As List(Of String))
            ClearBuilders()

            For Each BuilderID As Integer In BuilderIDsToInsert.Where(Function(id) IsNumeric(id)).Select(Function(id) id)
                InsertBuilder(BuilderID)
            Next
        End Sub

        Public Sub UpdateBuilders(BuildersToInsert As List(Of BuilderRow))
            ClearBuilders()

            For Each Builder As BuilderRow In BuildersToInsert
                InsertBuilder(Builder.BuilderID)
            Next
        End Sub

        Protected Sub InsertBuilder(BuilderID As Integer)
            DB.ExecuteSQL(" INSERT INTO NationalContractBuilder (BuilderID, ContractID) VALUES (" & BuilderID & "," & ContractID & ")")
        End Sub
        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from NationalContract"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function
        Public Function GetBuilders() As List(Of BuilderRow)
            Dim ret As New List(Of BuilderRow)
            Dim IDs As Object = DB.ExecuteScalar("SELECT Stuff(" & _
                                                 "(SELECT N', ' + CONVERT(NVARCHAR(MAX), BuilderID) FROM NationalContractBuilder WHERE ContractID = " & ContractID & " FOR XML PATH(''),TYPE)" & _
                                                 ".value('text()[1]','nvarchar(max)'),1,2,N'')")

            If Not IsDBNull(IDs) Then
                For Each sec As Integer In CStr(IDs).Split(",")
                    Dim BuilderID As Integer = 0
                    If Integer.TryParse(sec, BuilderID) Then ret.Add(BuilderRow.GetRow(DB, BuilderID))
                Next
            End If

            Return ret
        End Function
#End Region
    End Class

End Namespace
