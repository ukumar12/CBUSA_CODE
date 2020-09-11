Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Threading

Namespace DataLayer

    Public Class AutomaticMessagesRow
        Inherits AutomaticMessagesRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            MyBase.New(DB, AutomaticMessageID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer) As AutomaticMessagesRow
            Dim row As AutomaticMessagesRow

            row = New AutomaticMessagesRow(DB, AutomaticMessageID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            Dim row As AutomaticMessagesRow

            row = New AutomaticMessagesRow(DB, AutomaticMessageID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AutomaticMessages"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetRowByTitle(ByVal DB As Database, ByVal Title As String) As AutomaticMessagesRow
            Dim out As New AutomaticMessagesRow(DB)
            Dim sql As String = "select * from AutomaticMessages where Title=" & DB.Quote(Title)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Sub SendBidSubmitMail(Optional ByVal AdditionalMessage As String = "", Optional ByVal Body As String = "", Optional ByVal TwoPriceCampaignId As Int32 = 0)
            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")

            Dim QryEmailList As String = String.Concat("EXEC sp_GetAdminEmailListForBidCompleteNotification ", DB.NullNumber(TwoPriceCampaignId))
            Dim EmailList As String = DB.ExecuteScalar(QryEmailList)

            EmailList = EmailList.Replace(";", ",")
            If EmailList.Substring(EmailList.Length - 1, 1) = "," Then
                EmailList = EmailList.Substring(0, EmailList.Length - 1)
            End If

            If Body = "" Then
                Body = Message & vbCrLf & vbCrLf & AdditionalMessage
            Else
                Body = Body & vbCrLf & vbCrLf & AdditionalMessage
            End If

            'Core.SendSimpleMail(fromEmail, fromName, fromEmail, fromName, Subject, Body)

            If EmailList <> Nothing Then
                Dim cc As String() = EmailList.Split(",")
                For Each email As String In cc
                    If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                        email = SysParam.GetValue(DB, "AdminEmail")
                    End If
                    If Core.IsEmail(email) Then
                        Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, Body)
                    End If
                Next
            End If
        End Sub


        Public Sub SendAdmin(Optional ByVal AdditionalMessage As String = "", Optional ByVal Body As String = "")
            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")


            If Body = "" Then Body = Message & vbCrLf & vbCrLf & AdditionalMessage

            Core.SendSimpleMail(fromEmail, fromName, fromEmail, fromName, Subject, Body)

            If CCList <> Nothing Then
                Dim cc As String() = CCList.Split(",")
                For Each email As String In cc
                    If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                        email = SysParam.GetValue(DB, "AdminEmail")
                    End If
                    If Core.IsEmail(email) Then
                        Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, Body)
                    End If
                Next
            End If
        End Sub

        Public Sub Send(ByVal Account As BuilderRow)
            Send(Account, String.Empty)
        End Sub

        Public Sub Send(ByVal Account As BuilderRow, ByVal AdditionalMessage As String, Optional ByVal OverwriteMessage As Boolean = False, Optional ByVal CCEmail As String = "", Optional ByVal CCLLCNotification As Boolean = True)

            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
            Dim sBody As String = Message
            If OverwriteMessage Then
                sBody = AdditionalMessage
            Else
                sBody = Message & vbCrLf & vbCrLf & AdditionalMessage
            End If
            sBody = sBody.Replace("%%Builder%%", Account.CompanyName)

            If IsMessage Then
                Dim row As New AutomaticMessageBuilderRecipientRow(DB)
                row.BuilderID = Account.BuilderID
                row.AutomaticMessageID = AutomaticMessageID
                row.Created = Now()
                row.IsActive = True
                row.Insert()
            End If

            If IsEmail Then
                Dim addr As String = Account.Email

                If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                    addr = SysParam.GetValue(DB, "AdminEmail")
                End If

                If Core.IsEmail(addr) Then
                    Dim thEmail As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, addr, Account.CompanyName, Subject, sBody))
                    thEmail.IsBackground = True
                    thEmail.Start()

                    'Core.SendSimpleMail(fromEmail, fromName, addr, Account.CompanyName, Subject, sBody)
                End If
                If CCEmail <> "" Then
                    If Core.IsEmail(CCEmail) Then
                        Dim thCCEmail As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, CCEmail, Account.CompanyName, Subject, sBody))
                        thCCEmail.IsBackground = True
                        thCCEmail.Start()

                        'Core.SendSimpleMail(fromEmail, fromName, CCEmail, Account.CompanyName, Subject, sBody)
                    End If
                End If

                If CCLLCNotification Then
                    If CCList <> Nothing Then
                        Dim cc As String() = CCList.Split(",")
                        For Each email As String In cc
                            If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                email = SysParam.GetValue(DB, "AdminEmail")
                            End If
                            If Core.IsEmail(email) Then
                                Dim thCCLLCEmail As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody))
                                thCCLLCEmail.IsBackground = True
                                thCCLLCEmail.Start()

                                'Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody)
                            End If
                        Next
                    End If

                    'sends emails to assigned person for each llc 
                    Try
                        Dim BuilderLLCEmailList As String = String.Empty
                        BuilderLLCEmailList = LLCRow.GetBuilderLLC(DB, Account.BuilderID).NotificationEmailList
                        If BuilderLLCEmailList <> Nothing Then
                            Dim BuilderLLCEmailListCC() As String = BuilderLLCEmailList.Split(",")
                            For Each BuilderLLCEmailListEmail As String In BuilderLLCEmailListCC
                                'If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                '    BuilderLLCEmailListEmail = SysParam.GetValue(DB, "AdminEmail")
                                'End If
                                If Core.IsEmail(BuilderLLCEmailListEmail) Then
                                    Dim thBuilderLLCEmail As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, BuilderLLCEmailListEmail, BuilderLLCEmailListEmail, Subject, sBody))
                                    thBuilderLLCEmail.IsBackground = True
                                    thBuilderLLCEmail.Start()

                                    'Core.SendSimpleMail(fromEmail, fromName, BuilderLLCEmailListEmail, BuilderLLCEmailListEmail, Subject, sBody)
                                End If
                            Next
                        End If
                    Catch ex As Exception
                    End Try

                End If
            End If
        End Sub


        Public Sub SendLLCNotificationListAReminder(ByVal EmailList As String, ByVal AdditionalMessage As String, Optional ByVal OverwriteMessage As Boolean = False, Optional ByVal CCEmail As String = "")
            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
            Dim sBody As String = Message
            If OverwriteMessage Then
                sBody = AdditionalMessage
            Else
                sBody = Message & vbCrLf & vbCrLf & AdditionalMessage
            End If

            Try
                If EmailList <> String.Empty Then
                    Dim EmailNotificationlist() As String = EmailList.Split(",")
                    For Each Email As String In EmailNotificationlist
                        If Core.IsEmail(Email) Then
                            Core.SendSimpleMail(fromEmail, fromName, Email, Email, Subject, sBody)
                        End If
                    Next
                End If
                'If CCEmail <> "" Then
                '    If Core.IsEmail(CCEmail) Then
                '        Core.SendSimpleMail(fromEmail, fromName, CCEmail, CCEmail, Subject, sBody)
                '    End If
                'End If

                If CCList <> Nothing Then
                    Dim cc As String() = CCList.Split(",")
                    For Each email As String In cc
                        If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                            email = SysParam.GetValue(DB, "AdminEmail")
                        End If
                        If Core.IsEmail(email) Then
                            Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody)
                        End If
                    Next
                End If


            Catch ex As Exception
            End Try
        End Sub

        'Public Sub SendLLCNotificationListAReminder(ByVal Message As String, ByVal AdditionalMessage As String, ByVal LLCEmailList As String)
        '    Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
        '    Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
        '    Dim sBody As String = Message
        '    Try
        '        If LLCEmailList <> String.Empty Then
        '            Dim EmailNotificationlist() As String = LLCEmailList.Split(",")
        '            For Each EmailList As String In EmailNotificationlist
        '                If Core.IsEmail(EmailList) Then
        '                    Core.SendSimpleMail(fromEmail, fromName, EmailList, EmailList, Subject, sBody)
        '                End If
        '            Next
        '        End If
        '    Catch ex As Exception
        '    End Try



        'End Sub

        Public Sub SendCopyToAdmin(ByVal Account As BuilderRow, ByVal AdditionalMessage As String, Optional ByVal OverwriteMessage As Boolean = False, Optional ByVal CCEmail As String = "", Optional ByVal CCLLCNotification As Boolean = True)
            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")
            Dim sBody As String = Message
            If OverwriteMessage Then
                sBody = AdditionalMessage
            Else
                sBody = Message & vbCrLf & vbCrLf & AdditionalMessage
            End If
            sBody = sBody.Replace("%%Builder%%", Account.CompanyName)
            If IsMessage Then
                Dim row As New AutomaticMessageBuilderRecipientRow(DB)
                row.BuilderID = Account.BuilderID
                row.AutomaticMessageID = AutomaticMessageID
                row.Created = Now()
                row.IsActive = True
                row.Insert()
            End If
            If CCEmail <> "" Then
                If Core.IsEmail(CCEmail) Then
                    Core.SendSimpleMail(fromEmail, fromName, CCEmail, Account.CompanyName, Subject, sBody)
                End If
            End If
            If IsEmail Then
                If CCLLCNotification Then
                    If CCList <> Nothing Then
                        Dim cc As String() = CCList.Split(",")
                        For Each email As String In cc
                            If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                email = SysParam.GetValue(DB, "AdminEmail")
                            End If
                            If Core.IsEmail(email) Then
                                Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody)
                            End If
                        Next
                    End If

                    'sends emails to assigned person for each llc 
                    Try
                        Dim BuilderLLCEmailList As String = String.Empty
                        BuilderLLCEmailList = LLCRow.GetBuilderLLC(DB, Account.BuilderID).NotificationEmailList
                        If BuilderLLCEmailList <> Nothing Then
                            Dim BuilderLLCEmailListCC() As String = BuilderLLCEmailList.Split(",")
                            For Each BuilderLLCEmailListEmail As String In BuilderLLCEmailListCC
                                'If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                '    BuilderLLCEmailListEmail = SysParam.GetValue(DB, "AdminEmail")
                                'End If
                                If Core.IsEmail(BuilderLLCEmailListEmail) Then
                                    Core.SendSimpleMail(fromEmail, fromName, BuilderLLCEmailListEmail, BuilderLLCEmailListEmail, Subject, sBody)
                                End If
                            Next
                        End If
                    Catch ex As Exception
                    End Try

                End If
            End If
        End Sub

        Public Sub Send(ByVal Account As VendorRow)
            Send(Account, String.Empty)
        End Sub

        Public Sub Send(ByVal Account As VendorRow, ByVal AdditionalMessage As String, Optional ByVal OverwriteMessage As Boolean = False, Optional ByVal CCEmail As String = "", Optional ByVal CCLLCNotification As Boolean = True)
            Dim fromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")
            Dim fromName As String = SysParam.GetValue(DB, "ContactUsName")

            'Get HTML from Email Template

            Dim sBody As String = Message
            If OverwriteMessage Then
                sBody = AdditionalMessage
            Else
                sBody = AdditionalMessage & vbCrLf & vbCrLf & Message
                'sBody = Message & vbCrLf & vbCrLf & AdditionalMessage
            End If
            sBody = sBody.Replace("%%Vendor%%", Account.CompanyName)

            'Embed body into placeholder in the HTML email template. Use cache.

            If IsMessage Then
                Dim row As New AutomaticMessageVendorRecipientRow(DB)
                row.VendorID = Account.VendorID
                row.AutomaticMessageID = AutomaticMessageID
                row.IsActive = True
                row.Created = Now()
                row.Insert()
            End If
            If IsEmail Then

                Dim sql As String =
                    " select Distinct va.FirstName , va.LastName,va.Email  " _
                    & " from VendorAccount va inner join VendorAccountVendorRole vavr on va.VendorAccountId=vavr.VendorAccountId" _
                    & "     inner join AutomaticMessageVendorRole amvr on vavr.VendorRoleId=amvr.VendorRoleId" _
                    & " where " _
                    & "     va.VendorId=" & Account.VendorID _
                    & " and " _
                    & "     amvr.AutomaticMessageId=" & AutomaticMessageID

                Dim dtRecipients As DataTable = DB.GetDataTable(sql)
                If dtRecipients.Rows.Count = 0 Then
                    Dim addr As String = Account.Email
                    If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                        addr = SysParam.GetValue(DB, "AdminEmail")
                    End If
                    If Core.IsEmail(addr) Then
                        'Use Core.SendHTMLMail
                        '  Core.SendSimpleMail(fromEmail, fromName, addr, Account.CompanyName, Subject, sBody)
                    End If
                Else
                    For Each row As DataRow In dtRecipients.Rows
                        Dim addr As String = row("Email")
                        If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                            addr = SysParam.GetValue(DB, "AdminEmail")
                        End If
                        If Core.IsEmail(addr) Then
                            'Use Core.SendHTMLMail
                            Dim thEmail As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, addr, Core.BuildFullName(Core.GetString(row("FirstName")), "", Core.GetString(row("LastName"))), Subject, sBody))
                            thEmail.IsBackground = True
                            thEmail.Start()

                            'Core.SendSimpleMail(fromEmail, fromName, addr, Core.BuildFullName(Core.GetString(row("FirstName")), "", Core.GetString(row("LastName"))), Subject, sBody)
                        End If
                    Next
                End If
                If CCEmail <> "" Then
                    If Core.IsEmail(CCEmail) Then
                        'Use Core.SendHTMLMail
                        Dim thEmailCC As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, CCEmail, Account.CompanyName, Subject, sBody))
                        thEmailCC.IsBackground = True
                        thEmailCC.Start()

                        'Core.SendSimpleMail(fromEmail, fromName, CCEmail, Account.CompanyName, Subject, sBody)
                    End If
                End If

                If CCLLCNotification Then
                    If CCList <> Nothing Then
                        Dim cc As String() = CCList.Split(",")
                        For Each email As String In cc
                            If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                email = SysParam.GetValue(DB, "AdminEmail")
                            End If
                            If Core.IsEmail(email) Then
                                'Use Core.SendHTMLMail
                                Dim thEmailCCLLC As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody))
                                thEmailCCLLC.IsBackground = True
                                thEmailCCLLC.Start()

                                'Core.SendSimpleMail(fromEmail, fromName, email, email, Subject, sBody)
                            End If
                        Next
                    End If

                    'sends emails to assigned person for each llc 
                    Try
                        Dim dtVendorLLC As DataTable = VendorRow.GetLLCList(DB, Account.VendorID.ToString)
                        Dim llcaSSIGNEDEMAIL As String = String.Empty
                        If dtVendorLLC.Rows.Count > 0 Then
                            For Each drrow As DataRow In dtVendorLLC.Rows
                                Dim VendorLLCEmailList As String = drrow("NotificationEmailList")
                                If VendorLLCEmailList <> Nothing Then
                                    Dim VendorLLCEmailListCC() As String = VendorLLCEmailList.Split(",")
                                    For Each VendorLLCEmailListEmail As String In VendorLLCEmailListCC
                                        'If SysParam.GetValue(DB, "AutoMessageTestMode") Then
                                        '    VendorLLCEmailListEmail = SysParam.GetValue(DB, "AdminEmail")
                                        'End If
                                        If Core.IsEmail(VendorLLCEmailListEmail) Then
                                            Dim thEmailVendorLLC As New Thread(Sub() Core.SendSimpleMail(fromEmail, fromName, VendorLLCEmailListEmail, VendorLLCEmailListEmail, Subject, sBody))
                                            thEmailVendorLLC.IsBackground = True
                                            thEmailVendorLLC.Start()

                                            'Core.SendSimpleMail(fromEmail, fromName, VendorLLCEmailListEmail, VendorLLCEmailListEmail, Subject, sBody)
                                        End If
                                    Next
                                End If
                            Next
                        End If

                    Catch ex As Exception
                    End Try
                End If
            End If
        End Sub

        Public Sub MarkUnReadByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageBuilderRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND BuilderID = " & BuilderID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub
        Public Sub MarkReadByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageBuilderRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND BuilderID = " & BuilderID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageBuilderRecipient SET IsActive = 0 WHERE BuilderID = " & BuilderID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkUnReadByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageVendorRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND VendorID = " & VendorID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkReadByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageVendorRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND VendorID = " & VendorID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessageVendorRecipient SET IsActive = 0 WHERE VendorID = " & VendorID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkUnReadByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessagePIQRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND PIQID = " & PIQID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkReadByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessagePIQRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND PIQID = " & PIQID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AutomaticMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AutomaticMessagePIQRecipient SET IsActive = 0 WHERE PIQID = " & PIQID & " AND AutomaticMessageID = " & Me.AutomaticMessageID)
            End If
        End Sub

        'Vendor Roles
        Public ReadOnly Property GetSelectedVendorRoles() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorRoleID from AutomaticMessageVendorRole where AutomaticMessageID = " & AutomaticMessageID)
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

        Public Sub DeleteFromAllVendorRoles()
            DB.ExecuteSQL("delete from AutomaticMessageVendorRole where AutomaticMessageID = " & AutomaticMessageID)
        End Sub

        Public Sub InsertToVendorRoles(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToVendorRole(Element)
            Next
        End Sub

        Public Sub InsertToVendorRole(ByVal VendorRoleID As Integer)
            Dim SQL As String = "insert into AutomaticMessageVendorRole (AutomaticMessageID, VendorRoleID) values (" & AutomaticMessageID & "," & VendorRoleID & ")"
            DB.ExecuteSQL(SQL)
        End Sub
    End Class

    Public MustInherit Class AutomaticMessagesRowBase
        Private m_DB As Database
        Private m_AutomaticMessageID As Integer = Nothing
        Private m_Subject As String = Nothing
        Private m_Title As String = Nothing
        Private m_Condition As String = Nothing
        Private m_Message As String = Nothing
        Private m_IsEmail As Boolean = Nothing
        Private m_IsMessage As Boolean = Nothing
        Private m_CCList As String = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing


        Public Property AutomaticMessageID() As Integer
            Get
                Return m_AutomaticMessageID
            End Get
            Set(ByVal Value As Integer)
                m_AutomaticMessageID = value
            End Set
        End Property

        Public Property Subject() As String
            Get
                Return m_Subject
            End Get
            Set(ByVal Value As String)
                m_Subject = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property Condition() As String
            Get
                Return m_Condition
            End Get
            Set(ByVal Value As String)
                m_Condition = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = value
            End Set
        End Property

        Public Property IsEmail() As Boolean
            Get
                Return m_IsEmail
            End Get
            Set(ByVal Value As Boolean)
                m_IsEmail = value
            End Set
        End Property

        Public Property IsMessage() As Boolean
            Get
                Return m_IsMessage
            End Get
            Set(ByVal Value As Boolean)
                m_IsMessage = value
            End Set
        End Property

        Public Property CCList() As String
            Get
                Return m_CCList
            End Get
            Set(ByVal Value As String)
                m_CCList = value
            End Set
        End Property

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
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

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            m_DB = DB
            m_AutomaticMessageID = AutomaticMessageID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AutomaticMessages WHERE AutomaticMessageID = " & DB.Number(AutomaticMessageID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AutomaticMessageID = Convert.ToInt32(r.Item("AutomaticMessageID"))
            m_Subject = Convert.ToString(r.Item("Subject"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_Condition = Convert.ToString(r.Item("Condition"))
            If IsDBNull(r.Item("Message")) Then
                m_Message = Nothing
            Else
                m_Message = Convert.ToString(r.Item("Message"))
            End If
            m_IsEmail = Convert.ToBoolean(r.Item("IsEmail"))
            m_IsMessage = Convert.ToBoolean(r.Item("IsMessage"))
            If IsDBNull(r.Item("CCList")) Then
                m_CCList = Nothing
            Else
                m_CCList = Convert.ToString(r.Item("CCList"))
            End If
            m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            If IsDBNull(r.Item("EndDate")) Then
                m_EndDate = Nothing
            Else
                m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AutomaticMessages (" _
             & " Subject" _
             & ",Title" _
             & ",Condition" _
             & ",Message" _
             & ",IsEmail" _
             & ",IsMessage" _
             & ",CCList" _
             & ",StartDate" _
             & ",EndDate" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.Quote(Subject) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(Condition) _
             & "," & m_DB.Quote(Message) _
             & "," & CInt(IsEmail) _
             & "," & CInt(IsMessage) _
             & "," & m_DB.Quote(CCList) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & CInt(IsActive) _
             & ")"

            AutomaticMessageID = m_DB.InsertSQL(SQL)

            Return AutomaticMessageID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AutomaticMessages SET " _
             & " Subject = " & m_DB.Quote(Subject) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Condition = " & m_DB.Quote(Condition) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",IsEmail = " & CInt(IsEmail) _
             & ",IsMessage = " & CInt(IsMessage) _
             & ",CCList = " & m_DB.Quote(CCList) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE AutomaticMessageID = " & m_DB.quote(AutomaticMessageID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AutomaticMessages WHERE AutomaticMessageID = " & m_DB.Number(AutomaticMessageID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AutomaticMessagesCollection
        Inherits GenericCollection(Of AutomaticMessagesRow)
    End Class

End Namespace

