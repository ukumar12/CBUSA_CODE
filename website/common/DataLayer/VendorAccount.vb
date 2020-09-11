Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports Utility

Namespace DataLayer

    Public Class VendorAccountRow
        Inherits VendorAccountRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorAccountID As Integer)
            MyBase.New(DB, VendorAccountID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorAccountID As Integer) As VendorAccountRow
            Dim row As VendorAccountRow

            row = New VendorAccountRow(DB, VendorAccountID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorAccountID As Integer)
            Dim row As VendorAccountRow

            row = New VendorAccountRow(DB, VendorAccountID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorAccount"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal ShowActiveOnly As Boolean = False) As DataTable
            Dim SQL As String = "select * from VendorAccount where VendorId = " & VendorId

            If ShowActiveOnly Then
                SQL &= " AND Isactive = 1 "
            End If


            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If

            Return DB.GetDataTable(SQL)
        End Function


        Public Shared Function GetListByVendorNew(ByVal DB As Database, ByVal VendorId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal ShowActiveOnly As Boolean = False) As DataTable
            Dim SQL As String = "select * from VendorAccount where VendorId = " & VendorId

            If ShowActiveOnly Then
                SQL &= " AND FlagForExistingUser is null "
            End If

            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorId As Integer) As DataTable
            Dim SQL As String = "select * from VendorAccount where VendorId = " & VendorId

            'If ShowActiveOnly Then
            SQL &= " AND FlagForExistingUser=1 "
            'End If

            Return DB.GetDataTable(SQL)
        End Function



        Public ReadOnly Property GetSelectedVendorRoles() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorRoleID from VendorAccountVendorRole where VendorAccountID = " & VendorAccountID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorRoleID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public ReadOnly Property GetSelectedVendorRoleLabels() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select vr.VendorRole from VendorAccountVendorRole vavr JOIN VendorRole vr ON vavr.VendorRoleId = vr.VendorRoleId where VendorAccountID = " & VendorAccountID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorRole")
                    Conn = ", "
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllVendorRoles()
            DB.ExecuteSQL("delete from VendorAccountVendorRole where VendorAccountID = " & VendorAccountID)
        End Sub

        Public Sub InsertToVendorRoles(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToVendorRole(Element)
            Next
        End Sub

        Public Sub InsertToVendorRole(ByVal VendorRoleID As Integer)
            Dim SQL As String = "insert into VendorAccountVendorRole (VendorAccountID, VendorRoleID) values (" & VendorAccountID & "," & VendorRoleID & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub SetAsPrimary(ByVal VendorId As Integer)
            Dim SQL As String = "update VendorAccount set IsPrimary = 0 WHERE VendorId = " & VendorId
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetVendorByUsername(ByVal DB As Database, ByVal Username As String) As VendorAccountRow
            Dim out As New VendorAccountRow(DB)
            Dim sql As String = "select * from VendorAccount where Username=" & DB.Quote(Username)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetPrimaryAccount(ByVal DB As Database, ByVal VendorID As Integer) As VendorAccountRow
            Dim out As New VendorAccountRow(DB)
            Dim sql As String = "select * from VendorAccount where IsPrimary=1 and VendorID=" & DB.Number(VendorID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
        Public Shared Function GetAllVendorUserRoles(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
            Dim sql As String = " select r.VendorAccountID, v.VendorRole, v.VendorRoleID " _
                & " from VendorAccount a inner join VendorAccountVendorRole r on a.VendorAccountID=r.VendorAccountID " _
                & "     inner join VendorRole v on r.VendorRoleID=v.VendorRoleID " _
                & " where a.VendorID = " & DB.Number(VendorID) _
                & " order by VendorAccountID"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetUniqueUserName(ByVal DB As Database, ByVal FirstName As String, ByVal LastName As String) As String
            Dim basename As String = FirstName & "." & LastName
            Dim cnt As Integer = 1
            Dim username As String = basename.Replace(" ", "")

            Dim sql As String = "select v.cnt + b.cnt + p.cnt from " _
                              & "(select count(*) as cnt from VendorAccount where Username={0}) as v, " _
                              & "(select count(*) as cnt from BuilderAccount where Username={0}) as b, " _
                              & "(select count(*) as cnt from PIQAccount where Username={0}) as p"
            While DB.ExecuteScalar(String.Format(sql, DB.Quote(username))) > 0
                username = basename & cnt
                cnt += 1
            End While
            Return username
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
        Public Shared Function GetVendorAccountByCRMID(ByVal DB As Database, ByVal CRMID As String) As VendorAccountRow
            Dim out As New VendorAccountRow(DB)
            Dim sql As String = "select * from VendorAccount where CRMID=" & DB.Quote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetVendorAccountByLikeCRMID(ByVal DB As Database, ByVal CRMID As String) As VendorAccountRow
            Dim out As New VendorAccountRow(DB)
            Dim sql As String = "select * from VendorAccount where CRMID Like " & DB.FilterQuote(CRMID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetBuilderAccountByEmailOrName(ByVal DB As Database, ByVal Email As String, ByVal Name As String) As VendorAccountRow
            Dim out As New VendorAccountRow(DB)
            Dim sql As String = "select top 1 * from VendorAccount  where (CRMID is NULL or CRMID='') and Email=" & DB.Quote(Email) & " and LastName=" & DB.Quote(Name)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
    End Class

    Public MustInherit Class VendorAccountRowBase
        Private m_DB As Database
        Private m_VendorAccountID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_VendorBranchOfficeID As Integer = Nothing
        Private m_CRMID As String = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Title As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Fax As String = Nothing
        Private m_Email As String = Nothing
        Private m_Username As String = Nothing
        Private m_Password As String = Nothing
        Private m_HistoricPasswordHash As String = Nothing
        Private m_HistoricPasswordSalt As String = Nothing
        Private m_HistoricPasswordSha1 As String = Nothing
        Private m_IsPrimary As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Created As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_RequirePasswordUpdate As Boolean = Nothing
        Private m_PasswordEncrypted As Boolean = Nothing
        Private m_FlagForExistingUser As Boolean


        Public Property VendorAccountID() As Integer
            Get
                Return m_VendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_VendorAccountID = Value
            End Set
        End Property

        Public Property HistoricID() As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricID = Value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
            End Set
        End Property

        Public Property VendorBranchOfficeID() As Integer
            Get
                Return m_VendorBranchOfficeID
            End Get
            Set(ByVal Value As Integer)
                m_VendorBranchOfficeID = Value
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

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = Value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = Value
            End Set
        End Property

        Public Property Mobile() As String
            Get
                Return m_Mobile
            End Get
            Set(ByVal Value As String)
                m_Mobile = Value
            End Set
        End Property

        Public Property Fax() As String
            Get
                Return m_Fax
            End Get
            Set(ByVal Value As String)
                m_Fax = Value
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

        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal Value As String)
                m_Username = Value
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

        Public ReadOnly Property PasswordEncrypted() As Boolean
            Get
                Return m_PasswordEncrypted
            End Get
        End Property

        Public Property HistoricPasswordHash() As String
            Get
                Return m_HistoricPasswordHash
            End Get
            Set(ByVal Value As String)
                m_HistoricPasswordHash = Value
            End Set
        End Property

        Public Property HistoricPasswordSalt() As String
            Get
                Return m_HistoricPasswordSalt
            End Get
            Set(ByVal Value As String)
                m_HistoricPasswordSalt = Value
            End Set
        End Property

        Public Property HistoricPasswordSha1() As String
            Get
                Return m_HistoricPasswordSha1
            End Get
            Set(ByVal Value As String)
                m_HistoricPasswordSha1 = Value
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
            Set(ByVal Value As Boolean)
                m_RequirePasswordUpdate = Value
            End Set
        End Property


        Public ReadOnly Property EncryptedPassword() As String
            Get
                If m_Password = String.Empty Then
                    Return String.Empty
                End If
                Return Crypt.EncryptTripleDes(Password)
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
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorAccountID As Integer)
            m_DB = DB
            m_VendorAccountID = VendorAccountID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorAccount WHERE VendorAccountID = " & DB.Number(VendorAccountID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_VendorAccountID = Core.GetInt(r.Item("VendorAccountID"))
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_VendorBranchOfficeID = Core.GetInt(r.Item("VendorBranchOfficeID"))
            m_FirstName = Core.GetString(r.Item("FirstName"))
            m_CRMID = Core.GetString(r.Item("CRMID"))
            m_LastName = Core.GetString(r.Item("LastName"))
            m_Title = Core.GetString(r.Item("Title"))
            m_Phone = Core.GetString(r.Item("Phone"))
            m_Mobile = Core.GetString(r.Item("Mobile"))
            m_Fax = Core.GetString(r.Item("Fax"))
            m_Email = Core.GetString(r.Item("Email"))
            m_Username = Core.GetString(r.Item("Username"))
            If IsDBNull(r.Item("Password")) Then
                m_Password = Nothing
            Else
                Try
                    m_Password = Crypt.DecryptTripleDes(r.Item("Password"))
                    m_PasswordEncrypted = True
                Catch ex As Exception
                    m_Password = r.Item("Password")
                    m_PasswordEncrypted = False
                End Try
            End If
            m_HistoricPasswordHash = Core.GetString(r.Item("HistoricPasswordHash"))
            m_HistoricPasswordSalt = Core.GetString(r.Item("HistoricPasswordSalt"))
            m_HistoricPasswordSha1 = Core.GetString(r.Item("HistoricPasswordSha1"))
            m_IsPrimary = Core.GetBoolean(r.Item("IsPrimary"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_Updated = Core.GetDate(r.Item("Updated"))
            m_RequirePasswordUpdate = Core.GetBoolean(r.Item("RequirePasswordUpdate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorAccount (" _
             & " HistoricID" _
             & ",VendorID" _
             & ",VendorBranchOfficeID" _
             & ",CRMID" _
             & ",FirstName" _
             & ",LastName" _
             & ",Title" _
             & ",Phone" _
             & ",Mobile" _
             & ",Fax" _
             & ",Email" _
             & ",Username" _
             & ",Password" _
             & ",HistoricPasswordHash" _
             & ",HistoricPasswordSalt" _
             & ",HistoricPasswordSha1" _
             & ",IsPrimary" _
             & ",IsActive" _
             & ",Created" _
             & ",Updated" _
             & ",RequirePasswordUpdate" _
             & ",FlagForExistingUser" _
             & ") VALUES (" _
             & m_DB.NullNumber(HistoricID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(VendorBranchOfficeID) _
             & "," & m_DB.Quote(CRMID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(Username) _
             & "," & m_DB.Quote(EncryptedPassword) _
             & "," & m_DB.Quote(HistoricPasswordHash) _
             & "," & m_DB.Quote(HistoricPasswordSalt) _
             & "," & m_DB.Quote(HistoricPasswordSha1) _
             & "," & CInt(IsPrimary) _
             & "," & CInt(IsActive) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & CInt(RequirePasswordUpdate) _
             & "," & CInt(FlagForExistingUser) _
             & ")"

            VendorAccountID = m_DB.InsertSQL(SQL)

            Return VendorAccountID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorAccount SET " _
             & " HistoricID = " & m_DB.NullNumber(HistoricID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",VendorBranchOfficeID = " & m_DB.NullNumber(VendorBranchOfficeID) _
             & ",CRMID = " & m_DB.Quote(CRMID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",Username = " & m_DB.Quote(Username) _
             & ",Password = " & m_DB.Quote(EncryptedPassword) _
             & ",HistoricPasswordHash = " & m_DB.Quote(HistoricPasswordHash) _
             & ",HistoricPasswordSalt = " & m_DB.Quote(HistoricPasswordSalt) _
             & ",HistoricPasswordSha1 = " & m_DB.Quote(HistoricPasswordSha1) _
             & ",IsPrimary = " & CInt(IsPrimary) _
             & ",IsActive = " & CInt(IsActive) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",RequirePasswordUpdate = " & CInt(RequirePasswordUpdate) _
             & " WHERE VendorAccountID = " & m_DB.Quote(VendorAccountID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorAccount WHERE VendorAccountID = " & m_DB.Number(VendorAccountID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class


    Public Class VendorAccountCollection
        Inherits GenericCollection(Of VendorAccountRow)
    End Class

End Namespace


