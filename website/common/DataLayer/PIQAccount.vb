Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PIQAccountRow
        Inherits PIQAccountRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PIQAccountID As Integer)
            MyBase.New(DB, PIQAccountID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PIQAccountID As Integer) As PIQAccountRow
            Dim row As PIQAccountRow

            row = New PIQAccountRow(DB, PIQAccountID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PIQAccountID As Integer)
            Dim row As PIQAccountRow

            row = New PIQAccountRow(DB, PIQAccountID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PIQAccount"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetRowByUsername(ByVal DB As Database, ByVal Username As String) As PIQAccountRow
            Dim out As New PIQAccountRow(DB)
            Dim sql As String = "select * from PIQAccount where Username=" & DB.Quote(Username)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function CheckUsernameAvailability(ByVal DB As Database, ByVal Username As String) As Boolean
            If DB.ExecuteScalar("select count(*) from BuilderAccount where Username=" & DB.Quote(Username)) > 0 Then
                Return False
            ElseIf DB.ExecuteScalar("select count(*) from VendorAccount where Username=" & DB.Quote(Username)) > 0 Then
                Return False
            ElseIf DB.ExecuteScalar("select count(*) from PIQAccount where Username=" & DB.Quote(Username)) > 0 Then
                Return False
            End If
            Return True
        End Function

        Public Shared Function GetListByPIQ(ByVal DB As Database, ByVal PIQID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "Asc") As DataTable
            Dim sql As String = "select * from PIQAccount where PIQID = " & DB.Number(PIQID)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        'LLCs
        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from PiqAccountLLC where PIQAccountID = " & PIQAccountID)
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
            DB.ExecuteSQL("delete from PiqAccountLLC where PIQAccountID = " & PIQAccountID)
        End Sub

        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub

        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into PiqAccountLLC (PIQAccountID, LLCID) values (" & PIQAccountID & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetLLCCount(ByVal DB As Database, ByVal PiqAccountId As Integer) As Integer
            Return DB.ExecuteScalar("select count(*) from PiqAccountLLC where PiqAccountId=" & DB.Number(PiqAccountId))
        End Function
    End Class

    Public MustInherit Class PIQAccountRowBase
        Private m_DB As Database
        Private m_PIQAccountID As Integer = Nothing
        Private m_PIQID As Integer = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Username As String = Nothing
        Private m_Password As String = Nothing
        Private m_IsPrimary As Boolean = Nothing
        Private m_Created As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_RequirePasswordUpdate As Boolean = Nothing
        Private m_Email As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Fax As String = Nothing


        Public Property PIQAccountID() As Integer
            Get
                Return m_PIQAccountID
            End Get
            Set(ByVal Value As Integer)
                m_PIQAccountID = value
            End Set
        End Property

        Public Property PIQID() As Integer
            Get
                Return m_PIQID
            End Get
            Set(ByVal Value As Integer)
                m_PIQID = value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = value
            End Set
        End Property

        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal Value As String)
                m_Username = value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal Value As String)
                m_Password = value
            End Set
        End Property

        Public Property IsPrimary() As Boolean
            Get
                Return m_IsPrimary
            End Get
            Set(ByVal Value As Boolean)
                m_IsPrimary = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
        End Property

        Public Property RequirePasswordUpdate() As Boolean
            Get
                Return m_RequirePasswordUpdate
            End Get
            Set(ByVal value As Boolean)
                m_RequirePasswordUpdate = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal value As String)
                m_Email = value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal value As String)
                m_Phone = value
            End Set
        End Property

        Public Property Mobile() As String
            Get
                Return m_Mobile
            End Get
            Set(ByVal value As String)
                m_Mobile = value
            End Set
        End Property

        Public Property Fax() As String
            Get
                Return m_Fax
            End Get
            Set(ByVal value As String)
                m_Fax = value
            End Set
        End Property

        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PIQAccountID As Integer)
            m_DB = DB
            m_PIQAccountID = PIQAccountID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PIQAccount WHERE PIQAccountID = " & DB.Number(PIQAccountID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PIQAccountID = Convert.ToInt32(r.Item("PIQAccountID"))
            m_PIQID = Convert.ToInt32(r.Item("PIQID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_Username = Convert.ToString(r.Item("Username"))
            m_Password = Convert.ToString(r.Item("Password"))
            m_IsPrimary = Convert.ToBoolean(r.Item("IsPrimary"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            m_RequirePasswordUpdate = Core.GetBoolean(r.Item("RequirePasswordUpdate"))
            m_Email = Core.GetString(r.Item("Email"))
            m_Phone = Core.GetString(r.Item("Phone"))
            m_Mobile = Core.GetString(r.Item("Mobile"))
            m_Fax = Core.GetString(r.Item("Fax"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PIQAccount (" _
             & " PIQID" _
             & ",FirstName" _
             & ",LastName" _
             & ",Username" _
             & ",Password" _
             & ",IsPrimary" _
             & ",Created" _
             & ",Updated" _
             & ",RequirePasswordUpdate" _
             & ",Email" _
             & ",Phone" _
             & ",Mobile" _
             & ",Fax" _
             & ") VALUES (" _
             & m_DB.NullNumber(PIQID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Username) _
             & "," & m_DB.Quote(Password) _
             & "," & CInt(IsPrimary) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(RequirePasswordUpdate) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(Fax) _
             & ")"

            PIQAccountID = m_DB.InsertSQL(SQL)

            Return PIQAccountID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PIQAccount SET " _
             & " PIQID = " & m_DB.NullNumber(PIQID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Username = " & m_DB.Quote(Username) _
             & ",Password = " & m_DB.Quote(Password) _
             & ",IsPrimary = " & CInt(IsPrimary) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",RequirePasswordUpdate = " & CInt(RequirePasswordUpdate) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & " WHERE PIQAccountID = " & m_DB.Quote(PIQAccountID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PIQAccount WHERE PIQAccountID = " & m_DB.Number(PIQAccountID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PIQAccountCollection
        Inherits GenericCollection(Of PIQAccountRow)
    End Class

End Namespace

