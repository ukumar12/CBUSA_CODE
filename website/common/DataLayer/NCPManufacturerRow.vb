Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class NCPManufacturerRow
        Inherits NCPManufacturerRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, NCPManufacturerID As Integer)
            MyBase.New(DB, NCPManufacturerID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal NCPManufacturerID As Integer) As NCPManufacturerRow
            Dim row As NCPManufacturerRow

            row = New NCPManufacturerRow(DB, NCPManufacturerID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal NCPManufacturerID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM NCPManufacturer WHERE NCPManufacturerID = " & DB.Number(NCPManufacturerID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, NCPManufacturerID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from NCPManufacturer"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

End Namespace

