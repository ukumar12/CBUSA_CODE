Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Public Class TotalSales
    Private m_DateStart As Date = Nothing
    Private m_DateEnd As Date = Nothing
    Public Property DateStart() As Date
        Get
            Return m_DateStart
        End Get
        Set(ByVal value As Date)
            m_DateStart = value
        End Set
    End Property
    Public Property DateEnd() As Date
        Get
            Return m_DateEnd
        End Get
        Set(ByVal value As Date)
            m_DateEnd = value
        End Set
    End Property


    Public Function Generate(ByVal DB As DataTable) As DataTable
        Dim dt As DataTable = Nothing





        Return dt

    End Function

End Class
