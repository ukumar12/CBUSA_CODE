Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderAccountRow
        Inherits BuilderAccountRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderAccountID As Integer)
            MyBase.New(DB, BuilderAccountID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderAccountID As Integer) As BuilderAccountRow
            Dim row As BuilderAccountRow

            row = New BuilderAccountRow(DB, BuilderAccountID)
            row.Load()

            Return row
        End Function

        Public Shared Function GetPrimaryAccount(ByVal DB As Database, ByVal BuilderId As Integer) As BuilderAccountRow
            Dim row As BuilderAccountRow

            row = New BuilderAccountRow(DB)
            row.LoadByBuilder(BuilderId, True)

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderAccountID As Integer)
            Dim row As BuilderAccountRow

            row = New BuilderAccountRow(DB, BuilderAccountID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderAccount"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetAccountByUsername(ByVal DB As Database, ByVal Username As String) As BuilderAccountRow
            Dim out As New BuilderAccountRow(DB)
            Dim sql As String = "select * from BuilderAccount where Username=" & DB.Quote(Username)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderAccounts(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal ShowActiveOnly As Boolean = False) As DataTable
            Dim sql As String = "select * from BuilderAccount where BuilderID=" & DB.Number(BuilderID) & " "
            If ShowActiveOnly Then
                sql &= " AND IsActive = 1 "
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderAccountsNew(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal ShowActiveOnly As Boolean = False) As DataTable
            Dim sql As String = "select * from BuilderAccount where BuilderID=" & DB.Number(BuilderID) & " "
            If ShowActiveOnly Then
                sql &= " AND FlagForExistingUser is null "
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderAccounts(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select * from BuilderAccount where BuilderID=" & DB.Number(BuilderID) & " "
            'If ShowActiveOnly Then
            sql &= " AND FlagForExistingUser = 1 "
            'End If
            Return DB.GetDataTable(sql)
        End Function


        Public ReadOnly Property GetSelectedRoles() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select BuilderRoleID from BuilderAccountBuilderRole where BuilderAccountID = " & BuilderAccountID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("BuilderRoleID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property
        Public Shared Function GetBuilderAccountRoles(ByVal DB As Database, ByVal BuilderAccountID As Integer) As String

            Dim sql As String = "select br.* from BuilderRole br inner join BuilderAccountBuilderRole babr on babr.BuilderRoleID =br.BuilderRoleID where babr.BuilderAccountID = " & DB.Number(BuilderAccountID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty

            While sdr.Read()
                Result &= Conn & sdr("BuilderRole")
                Conn = "<br/>"
            End While
            sdr.Close()
            Return Result
        End Function
        Public Sub DeleteFromBuilderRoles()
            DB.ExecuteSQL("delete from BuilderAccountBuilderRole where BuilderAccountID = " & BuilderAccountID)
        End Sub

        Public Sub InsertToBuilderAccountBuilderRoles(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InserttoBuilderAccountBuilderRole(Element)
            Next
        End Sub

        Public Sub InserttoBuilderAccountBuilderRole(ByVal BUilderRoleID As Integer)
            Dim SQL As String = "insert into BuilderAccountBuilderRole (BuilderAccountID, BuilderRoleID) values (" & BuilderAccountID & "," & BUilderRoleID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

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
        Public Shared Function getUniqueUsernameFromEmail(ByVal DB As Database, ByVal Email As String) As String
            Dim emailparts() As String = Email.Split("@")
            Dim UserFromEmail As String = Email
            If emailparts.Count > 1 Then
                UserFromEmail = emailparts(0)
            End If
            If CheckUsernameAvailability(DB, UserFromEmail) = False Then
                Dim num As Integer = 1
                While CheckUsernameAvailability(DB, UserFromEmail & num) = False
                    num = num + 1
                End While
                UserFromEmail = UserFromEmail & num
            End If
            Return UserFromEmail

        End Function
    End Class

    Public MustInherit Class BuilderAccountRowBase
        Private m_DB As Database
        Private m_BuilderAccountID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_CRMID As String = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Username As String = Nothing
        Private m_Email As String = Nothing
        Private m_Password As String = Nothing
        Private m_IsPrimary As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_SendNCPReminder As Boolean = Nothing              'Added by Apala (Medullus) - VSO#7117
        Private m_Created As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_Title As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Fax As String = Nothing
        Private m_HistoricPasswordHash As String
        Private m_HistoricPasswordSalt As String
        Private m_HistoricPasswordSha1 As String
        Private m_RequirePasswordUpdate As Boolean
        Private m_PasswordEncrypted As Boolean
        Private m_FlagForExistingUser As Boolean


        Public Property BuilderAccountID() As Integer
            Get
                Return m_BuilderAccountID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderAccountID = Value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = Value
            End Set
        End Property
        Public Property CRMID() As String
            Get
                Return m_CRMID
            End Get
            Set(ByVal Value As String)
                m_CRMID = Value
            End Set
        End Property
        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = Value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = Value
            End Set
        End Property

        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal Value As String)
                m_Username = Value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = Value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal Value As String)
                m_Password = Value
            End Set
        End Property

        Public Property IsPrimary() As Boolean
            Get
                Return m_IsPrimary
            End Get
            Set(ByVal Value As Boolean)
                m_IsPrimary = Value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
            End Set
        End Property

        'Added by Apala (Medullus) - VSO#7117
        Public Property SendNCPReminder() As Boolean
            Get
                Return m_SendNCPReminder
            End Get
            Set(ByVal Value As Boolean)
                m_SendNCPReminder = Value
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

        Public Property HistoricID() As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal value As Integer)
                m_HistoricID = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal value As String)
                m_Title = value
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

        Public Property HistoricPasswordHash() As String
            Get
                Return m_HistoricPasswordHash
            End Get
            Set(ByVal value As String)
                m_HistoricPasswordHash = value
            End Set
        End Property

        Public Property HistoricPasswordSalt() As String
            Get
                Return m_HistoricPasswordSalt
            End Get
            Set(ByVal value As String)
                m_HistoricPasswordSalt = value
            End Set
        End Property

        Public Property HistoricPasswordSha1() As String
            Get
                Return m_HistoricPasswordSha1
            End Get
            Set(ByVal value As String)
                m_HistoricPasswordSha1 = value
            End Set
        End Property

        Public Property RequirePasswordUpdate() As Boolean
            Get
                Return m_RequirePasswordUpdate
            End Get
            Set(ByVal value As Boolean)
                m_RequirePasswordUpdate = value
            End Set
        End Property

        Public ReadOnly Property PasswordEncrypted() As Boolean
            Get
                Return m_PasswordEncrypted
            End Get
        End Property

        Public Property FlagForExistingUser() As Boolean
            Get
                Return m_FlagForExistingUser
            End Get
            Set(ByVal value As Boolean)
                m_FlagForExistingUser = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderAccountID As Integer)
            m_DB = DB
            m_BuilderAccountID = BuilderAccountID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderAccount WHERE BuilderAccountID = " & DB.Number(BuilderAccountID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub LoadByBuilder(ByVal BuilderId As Integer, Optional ByVal IsPrimary As Boolean = False)
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderAccount WHERE BuilderId = " & DB.Number(BuilderId)
            If IsPrimary Then
                SQL &= " AND IsPrimary = 1"
            End If
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderAccountID = Convert.ToInt32(r.Item("BuilderAccountID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_CRMID = Convert.ToString(r.Item("CRMID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_Username = Convert.ToString(r.Item("Username"))
            m_Email = Convert.ToString(r.Item("Email"))
            Try
                m_Password = Utility.Crypt.DecryptTripleDes(Core.GetString(r.Item("Password")))
                m_PasswordEncrypted = True
            Catch ex As Security.Cryptography.CryptographicException
                m_Password = Core.GetString(r.Item("Password"))
                m_PasswordEncrypted = False
            End Try
            m_IsPrimary = Convert.ToBoolean(r.Item("IsPrimary"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_SendNCPReminder = Convert.ToBoolean(r.Item("SendNCPReminder"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            If IsDBNull(r.Item("HistoricID")) Then
                m_HistoricID = Nothing
            Else
                m_HistoricID = Convert.ToInt32(r.Item("HistoricID"))
            End If
            If IsDBNull(r.Item("Title")) Then
                m_Title = Nothing
            Else
                m_Title = Convert.ToString(r.Item("Title"))
            End If
            If IsDBNull(r.Item("Phone")) Then
                m_Phone = Nothing
            Else
                m_Phone = Convert.ToString(r.Item("Phone"))
            End If
            If IsDBNull(r.Item("Mobile")) Then
                m_Mobile = Nothing
            Else
                m_Mobile = Convert.ToString(r.Item("Mobile"))
            End If
            If IsDBNull(r.Item("Fax")) Then
                m_Fax = Nothing
            Else
                m_Fax = Convert.ToString(r.Item("Fax"))
            End If
            If IsDBNull(r.Item("HistoricPasswordHash")) Then
                m_HistoricPasswordHash = Nothing
            Else
                m_HistoricPasswordHash = Convert.ToString(r.Item("HistoricPasswordHash"))
            End If
            If IsDBNull(r.Item("HistoricPasswordSalt")) Then
                m_HistoricPasswordSalt = Nothing
            Else
                m_HistoricPasswordSalt = Convert.ToString(r.Item("HistoricPasswordSalt"))
            End If
            If IsDBNull(r.Item("HistoricPasswordSha1")) Then
                m_HistoricPasswordSha1 = Nothing
            Else
                m_HistoricPasswordSha1 = Convert.ToString(r.Item("HistoricPasswordSha1"))
            End If
            m_RequirePasswordUpdate = Core.GetBoolean(r.Item("RequirePasswordUpdate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderAccount (" _
             & " BuilderID" _
             & ",CRMID" _
             & ",FirstName" _
             & ",LastName" _
             & ",Username" _
             & ",Email" _
             & ",Password" _
             & ",IsPrimary" _
             & ",IsActive" _
             & ",SendNCPReminder" _
             & ",Created" _
             & ",Updated" _
             & ",HistoricID" _
             & ",Title" _
             & ",Phone" _
             & ",Fax" _
             & ",Mobile" _
             & ",HistoricPasswordHash" _
             & ",HistoricPasswordSalt" _
             & ",HistoricPasswordSha1" _
             & ",RequirePasswordUpdate" _
             & ",FlagForExistingUser" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Quote(CRMID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Username) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(Utility.Crypt.EncryptTripleDes(Password)) _
             & "," & CInt(IsPrimary) _
             & "," & CInt(IsActive) _
             & "," & CInt(SendNCPReminder) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Number(HistoricID) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(HistoricPasswordHash) _
             & "," & m_DB.Quote(HistoricPasswordSalt) _
             & "," & m_DB.Quote(HistoricPasswordSha1) _
             & "," & CInt(RequirePasswordUpdate) _
             & "," & CInt(m_FlagForExistingUser) _
             & ")"

            BuilderAccountID = m_DB.InsertSQL(SQL)

            Return BuilderAccountID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderAccount SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",CRMID = " & m_DB.Quote(CRMID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Username = " & m_DB.Quote(Username) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",Password = " & m_DB.Quote(Utility.Crypt.EncryptTripleDes(Password)) _
             & ",IsPrimary = " & CInt(IsPrimary) _
             & ",IsActive = " & CInt(IsActive) _
             & ",SendNCPReminder = " & CInt(SendNCPReminder) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",HistoricID = " & m_DB.Quote(HistoricID) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",HistoricPasswordHash = " & m_DB.Quote(HistoricPasswordHash) _
             & ",HistoricPasswordSalt = " & m_DB.Quote(HistoricPasswordSalt) _
             & ",HistoricPasswordSha1 = " & m_DB.Quote(HistoricPasswordSha1) _
             & ",RequirePasswordUpdate = " & CInt(RequirePasswordUpdate) _
             & " WHERE BuilderAccountID = " & m_DB.Quote(BuilderAccountID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderAccountBuilderRole WHERE BuilderAccountID = " & m_DB.Number(BuilderAccountID)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM BuilderAccount WHERE BuilderAccountID = " & m_DB.Number(BuilderAccountID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove

        Public Shared Function GetBuilderAccountByCRMID(ByVal DB As Database, ByVal CRMID As String) As BuilderAccountRow
            Dim out As New BuilderAccountRow(DB)
            Dim sql As String = "select * from BuilderAccount where CRMID=" & DB.Quote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderAccountByLikeCRMID(ByVal DB As Database, ByVal CRMID As String) As BuilderAccountRow
            Dim out As New BuilderAccountRow(DB)
            Dim sql As String = "select * from BuilderAccount where CRMID Like " & DB.FilterQuote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderAccountByEmailOrName(ByVal DB As Database, ByVal Email As String, ByVal Name As String) As BuilderAccountRow
            Dim out As New BuilderAccountRow(DB)
            Dim sql As String = "select top 1 * from BuilderAccount where (CRMID is NULL or CRMID='') and Email=" & DB.Quote(Email) & " and LastName=" & DB.Quote(Name)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
    End Class

    Public Class BuilderAccountCollection
        Inherits GenericCollection(Of BuilderAccountRow)
    End Class

End Namespace

