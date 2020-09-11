Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TakeOffServiceRow
        Inherits TakeOffServiceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, TakeOffServiceID As Integer)
            MyBase.New(DB, TakeOffServiceID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TakeOffServiceID As Integer) As TakeOffServiceRow
            Dim row As TakeOffServiceRow

            row = New TakeOffServiceRow(DB, TakeOffServiceID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TakeOffServiceID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TakeOffService WHERE TakeOffServiceID = " & DB.Number(TakeOffServiceID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, TakeOffServiceID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TakeOffService"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        'Custom Methods
        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from TakeOffServiceLLC where TakeOffServiceID = " & DB.Number(TakeOffServiceID))
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("LLCID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllLLCs()
            DB.ExecuteSQL("delete from TakeOffServiceLLC where TakeOffServiceID = " & DB.Number(TakeOffServiceID))
        End Sub
        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub
        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into TakeOffServiceLLC (TakeOffServiceID, LLCID) values (" & DB.Number(TakeOffServiceID) & "," & DB.Number(LLCID) & ")"
            DB.ExecuteSQL(SQL)
        End Sub
    End Class

End Namespace

