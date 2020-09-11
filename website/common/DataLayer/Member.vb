Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports Utility

Namespace DataLayer

    Public Class MemberRow
        Inherits MemberRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal MemberId As Integer)
            MyBase.New(database, MemberId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal MemberId As Integer) As MemberRow
            Dim row As MemberRow
            row = New MemberRow(_Database, MemberId)
            row.Load()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal MemberId As Integer)
            Dim row As MemberRow
            row = New MemberRow(_Database, MemberId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function ValidateMemberCredentials(ByVal DB As Database, ByVal Username As String, ByVal Password As String) As Integer
            Dim Decrypted As String = String.Empty, MemberId As Integer = 0
			Dim SQL As String = "select MemberId, Password FROM Member where IsActive = 1 AND Username = " & DB.Quote(Username)
            Dim dr As SqlDataReader = DB.GetReader(SQL)

            If dr.Read Then
                MemberId = dr("MemberId")
                Decrypted = Crypt.DecryptTripleDES(dr("Password"))
            End If

            dr.Close()
            If Decrypted = Password Then Return MemberId Else Return 0
        End Function

        Public Shared Function GetRowByUsername(ByVal DB As Database, ByVal Username As String) As MemberRow
            Dim SQL As String = "SELECT * FROM Member WHERE Username = " & DB.Quote(Username)

            Dim r As SqlDataReader
            Dim row As MemberRow = New MemberRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then row.Load(r)

            r.Close()
            Return row
        End Function

        Public Shared Function GetRowByGuid(ByVal DB As Database, ByVal Guid As String) As MemberRow
            Dim SQL As String = "SELECT * FROM Member WHERE Guid = " & DB.Quote(Guid)

            Dim r As SqlDataReader
            Dim row As MemberRow = New MemberRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then row.Load(r)

            r.Close()
            Return row
        End Function

        Public Function GetMemberOrderHistory() As DataTable
            Return DB.GetDataTable("select OrderId, OrderNo, BillingFirstName + ' ' + BillingLastName As BillingName, Total, " & _
                                " (SELECT Name FROM StoreOrderStatus sos where sos.Code = so.Status) As Status, ProcessDate " & _
                                " from storeorder so where MemberId = " & MemberId & " AND ProcessDate is not null order by processdate desc")
        End Function

        Public Function GetAddressBook() As DataTable
            Return DB.GetDataTable("SELECT * FROM MemberAddress WHERE MemberId=" & MemberId & " AND AddressType = 'AddressBook'")
        End Function

        Public Function GetFullAddressBook() As DataTable
            Return DB.GetDataTable("SELECT * FROM MemberAddress WHERE MemberId=" & MemberId)
        End Function

        Public Function GetReminders() As DataTable
            Return DB.GetDataTable("SELECT ReminderId, Coalesce(Name,'') As Name, IsRecurrent, EventDate, Coalesce(cast(DaysBefore1 as varchar(3)),'') As DaysBefore1, Coalesce(cast(DaysBefore2 as varchar(3)),'') As DaysBefore2 FROM MemberReminder WHERE MemberId=" & MemberId)
        End Function

        Public Function GetWishlistItems() As DataTable
            Return DB.GetDataTable("select WishlistItemId, Quantity, si.ItemName, si.Price, si.SalePrice, si.IsOnSale, si.ItemId, si.SKU, si.Image, " & _
                                 " FROM StoreItem si, MemberWishlistItem mwi where si.IsActive = 1 AND mwi.ItemId = si.ItemId AND mwi.MemberId = " & MemberId)
        End Function

        Public Shared Function PerformMemberLogin(ByVal DB As Database, ByVal sUsername As String, ByVal sPassword As String, ByVal bIsPersist As Boolean) As Boolean
            Dim MemberId As Integer = MemberRow.ValidateMemberCredentials(DB, sUsername, sPassword)
            If MemberId > 0 Then
                System.Web.HttpContext.Current.Session("MemberId") = MemberId
                Dim FullName As String = DB.ExecuteScalar("select FirstName + ' ' +LastName as Fullname from MemberAddress where AddressType='Billing' and MemberId=" & DB.Number(MemberId))
                System.Web.HttpContext.Current.Session("MemberName") = FullName
                If bIsPersist = True Then Utility.CookieUtil.SetTripleDESEncryptedCookie("MemberId", System.Web.HttpContext.Current.Session("MemberId").ToString, Today.AddDays(15))

                'Update MemberId for Tracking search terms Used bv the search reports.
                If Not System.Web.HttpContext.Current.Session("SearchMemberTracking") Is Nothing Then
                    Dim SearchTermTracking As String = System.Web.HttpContext.Current.Session("SearchMemberTracking").ToString
                    DB.ExecuteSQL("Update SearchTerm Set MemberId=" & DB.Number(MemberId) & " Where TermId in (" & SearchTermTracking & ")")
                    System.Web.HttpContext.Current.Session("SearchMemberTracking") = Nothing
                End If
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    Public MustInherit Class MemberRowBase
        Private m_DB As Database
        Private m_MemberId As Integer = Nothing
        Private m_Username As String = Nothing
        Private m_Password As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_IsSameDefaultAddress As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_Guid As String = Nothing

        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = value
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
                m_Password = value
            End Set
        End Property

        Public ReadOnly Property EncryptedPassword() As String
            Get
                If m_Password = String.Empty Then
                    Return String.Empty
                End If
                Return Crypt.EncryptTripleDES(Password)
            End Get
        End Property

        Public Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
            Set(ByVal Value As DateTime)
                m_CreateDate = value
            End Set
        End Property

        Public Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
            Set(ByVal Value As DateTime)
                m_ModifyDate = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property

        Public Property Guid() As String
            Get
                Return m_Guid
            End Get
            Set(ByVal Value As String)
                m_Guid = Value
            End Set
        End Property

        Public Property IsSameDefaultAddress() As Boolean
            Get
                Return m_IsSameDefaultAddress
            End Get
            Set(ByVal value As Boolean)
                m_IsSameDefaultAddress = value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal MemberId As Integer)
            m_DB = database
            m_MemberId = MemberId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Member WHERE MemberId = " & DB.Quote(MemberId)
            r = m_DB.GetReader(SQL)
            If r.Read Then Me.Load(r)

            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            m_Username = Convert.ToString(r.Item("Username"))
            m_Password = Crypt.DecryptTripleDes(r.Item("Password"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_IsSameDefaultAddress = Convert.ToBoolean(r.Item("IsSameDefaultAddress"))
            If IsDBNull(r.Item("CreateDate")) Then m_CreateDate = Nothing Else m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("ModifyDate")) Then m_ModifyDate = Nothing Else m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            If IsDBNull(r.Item("Guid")) Then m_Guid = Nothing Else m_Guid = Convert.ToString(r.Item("Guid"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO Member (" _
             & " Username" _
             & ",Password" _
             & ",IsActive" _
             & ",IsSameDefaultAddress" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ",Guid" _
             & ") VALUES (" _
             & m_DB.Quote(Username) _
             & "," & m_DB.Quote(EncryptedPassword) _
             & "," & CInt(IsActive) _
             & "," & CInt(IsSameDefaultAddress) _
             & "," & m_DB.Quote(CreateDate) _
             & "," & m_DB.Quote(ModifyDate) _
             & "," & m_DB.Quote(Guid) _
             & ")"

            MemberId = m_DB.InsertSQL(SQL)

            Return MemberId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Member SET " _
             & " Username = " & m_DB.Quote(Username) _
             & ",Password = " & m_DB.Quote(EncryptedPassword) _
             & ",CreateDate = " & m_DB.Quote(CreateDate) _
             & ",ModifyDate = " & m_DB.Quote(ModifyDate) _
             & ",IsActive = " & CInt(IsActive) _
             & ",IsSameDefaultAddress = " & CInt(IsSameDefaultAddress) _
             & ",Guid = " & m_DB.Quote(Guid) _
             & " WHERE MemberId = " & m_DB.Quote(MemberId)

            m_DB.ExecuteSQL(SQL)
        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MemberReminderLog WHERE ReminderId IN (SELECT ReminderId FROM MemberReminder WHERE MemberId = " & m_DB.Quote(MemberId) & ")"
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM MemberReminder WHERE MemberId = " & m_DB.Quote(MemberId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM MemberWishlistItem WHERE MemberId = " & m_DB.Quote(MemberId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM MemberAddress WHERE MemberId = " & m_DB.Quote(MemberId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM Member WHERE MemberId = " & m_DB.Quote(MemberId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MemberCollection
        Inherits GenericCollection(Of MemberRow)
    End Class
End Namespace