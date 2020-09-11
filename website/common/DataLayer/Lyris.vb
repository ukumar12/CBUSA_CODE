Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text.RegularExpressions
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Namespace Components

    Public Class Lyris
        Private m_aLinks As ArrayList
        Private m_HTMLMessage As String
        Private m_TextMessage As String
        Private m_NewsletterDate As String
        Private m_SavedText As String
        Private m_HTMLTemplate As String
        Private m_TextTemplate As String
        Private m_TemplateSlots As MailingTemplateSlotCollection
        Private m_MessageSlots As GenericSerializableCollection(Of MailingMessageSlot)
        Private m_MIMEType As String

        Public Sub New()
            m_aLinks = New ArrayList
        End Sub

        Public Property MIMEType() As String
            Get
                Return m_MIMEType
            End Get
            Set(ByVal value As String)
                m_MIMEType = value
            End Set
        End Property

        Public Property TemplateSlots() As MailingTemplateSlotCollection
            Get
                Return m_TemplateSlots
            End Get
            Set(ByVal value As MailingTemplateSlotCollection)
                m_TemplateSlots = value
            End Set
        End Property

        Public Property MessageSlots() As GenericSerializableCollection(Of MailingMessageSlot)
            Get
                Return m_MessageSlots
            End Get
            Set(ByVal value As GenericSerializableCollection(Of MailingMessageSlot))
                m_MessageSlots = value
            End Set
        End Property

        Public Property NewsletterDate() As String
            Get
                Return m_NewsletterDate
            End Get
            Set(ByVal value As String)
                m_NewsletterDate = value
            End Set
        End Property

        Public Property SavedText() As String
            Get
                Return m_SavedText
            End Get
            Set(ByVal value As String)
                m_SavedText = value
            End Set
        End Property

        Public Property HTMLTemplate() As String
            Get
                Return m_HTMLTemplate
            End Get
            Set(ByVal value As String)
                m_HTMLTemplate = value
            End Set
        End Property

        Public Property TextTemplate() As String
            Get
                Return m_TextTemplate
            End Get
            Set(ByVal value As String)
                m_TextTemplate = value
            End Set
        End Property

        Public ReadOnly Property Links() As ArrayList
            Get
                Return m_aLinks
            End Get
        End Property

        Public ReadOnly Property HTMLMessage() As String
            Get
                Return m_HTMLMessage
            End Get
        End Property

        Public ReadOnly Property TextMessage() As String
            Get
                Return m_TextMessage
            End Get
        End Property

        Public Shared ReadOnly Property GetDateString(ByVal dt As String) As String
            Get
                Dim y, m, d, h, mi, s
                y = Year(dt)
                m = Month(dt)
                If Len(m) = 1 Then m = "0" & m
                d = Day(dt)
                If Len(d) = 1 Then d = "0" & d
                h = Hour(dt)
                If Len(h) = 1 Then h = "0" & h
                mi = Minute(dt)
                If Len(mi) = 1 Then mi = "0" & mi
                s = Second(dt)
                If Len(s) = 1 Then s = "0" & s
                Return y & m & d & h & mi & s
            End Get
        End Property

        Private Function GenerateHTMLLinks(ByVal sInput As String) As String
            If sInput Is Nothing Then Return ""

            Dim Matches As MatchCollection = Regex.Matches(sInput, "href[\s\n]*=[\s\n]*(""([^""]*)""|'([^']*)'|([\S]+))", RegexOptions.IgnoreCase)
            If Matches.Count = 0 Then
                Return sInput
            End If

            Dim Added As Integer = 0
            For Each Match As Match In Matches
                Dim lnk As New LyrisLink
                lnk.URL = Match.Value
                lnk.Type = LyrisLink.LinkType.HTML

                If Not Match.Groups(2).Value = String.Empty Then
                    lnk.Href = Match.Groups(2).Value
                ElseIf Not Match.Groups(3).Value = String.Empty Then
                    lnk.Href = Match.Groups(3).Value
                ElseIf Not Match.Groups(4).Value = String.Empty Then
                    lnk.Href = Match.Groups(4).Value
                End If

                'only if the href is not the stylesheen and mailto
                Dim r As Regex = New Regex("\.css$|mailto:|^#", RegexOptions.IgnoreCase)
                If Not r.IsMatch(lnk.Href) Then
                    Dim Index As Integer = Match.Index + Added
                    Dim Start As Integer = InStr(Match.Value, lnk.Href)
                    Index = Index + Start - 1

                    Dim l As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/link.aspx?l=" & (Links.Count + 1) & "&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"
                    sInput = Left(sInput, Index) & l & Right(sInput, Len(sInput) - Index - Len(lnk.Href))
                    lnk.Replaced = l

                    Added = Added + Len(l) - Len(lnk.Href)
                    m_aLinks.Add(lnk)
                End If
            Next
            Return sInput
        End Function

        Private Function GenerateTextLinks(ByVal sInput As String) As String
            Dim Matches As MatchCollection = Regex.Matches(sInput, "(http(s?)://.*?)\s", RegexOptions.IgnoreCase)
            If Matches.Count = 0 Then
                Return sInput
            End If

            Dim Added As Integer = 0
            For Each Match As Match In Matches
                Dim lnk As New LyrisLink
                lnk.URL = Match.Value
                lnk.Type = LyrisLink.LinkType.TEXT
                lnk.Href = Match.Groups(1).Value

                'if the link contains mail merge tags, then ignore
                Dim r As Regex = New Regex(".*%%(.*?)%%.*", RegexOptions.IgnoreCase)
                If Not r.IsMatch(lnk.Href) Then
                    Dim Index As Integer = Match.Index + Added

                    Dim l As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/link.aspx?l=" & (Links.Count + 1) & "&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"
                    sInput = Left(sInput, Index) & l & Right(sInput, Len(sInput) - Index - Len(lnk.Href))
                    lnk.Replaced = l

                    Added = Added + Len(l) - Len(lnk.Href)
                    m_aLinks.Add(lnk)
                End If
            Next
            Return sInput
        End Function

        Private Function GeneratePlainText(ByVal sInput As String) As String
            If sInput = String.Empty Then
                Return sInput
            End If

            'Replace nbsp with regular spaces
            sInput = Replace(sInput, "&nbsp;", " ")
            sInput = Replace(sInput, "&amp;", "&")

            Dim Matches As MatchCollection = Regex.Matches(sInput, "<[\s\n]*a.*?href[\s\n]*=[\s\n]*(""([^""]*)""|'([^']*)'|([\S]+)).*?>.*?<[\s\n]*/[\s\n]*a[\s\n]*>", RegexOptions.IgnoreCase)

            'I need to keep all links that are in the html format
            Dim Added As Integer = 0
            For Each Match As Match In Matches
                Dim l As String = String.Empty
                If Not Match.Groups(2).Value = String.Empty Then
                    l = Match.Groups(2).Value
                ElseIf Not Match.Groups(3).Value = String.Empty Then
                    l = Match.Groups(3).Value
                ElseIf Not Match.Groups(4).Value = String.Empty Then
                    l = Match.Groups(4).Value
                End If
                l = Replace(l, "&amp;", "&")

                Dim r As Regex = New Regex("\.css$|mailto:|^#", RegexOptions.IgnoreCase)
                'if this is just stylesheet, then ignore
                If r.IsMatch(l) Then
                    'Do nothing
                Else
                    Dim Text As String = String.Empty
                    Text = Core.StripHTML(Match.Value)
                    Text = Regex.Replace(Text, "\r\n|\r", "", RegexOptions.IgnoreCase)
                    Text = Trim(Text)

                    Dim Index = Match.Index + Added
                    Dim TmpLink As String = Text & " " & l
                    sInput = Left(sInput, Index) & TmpLink & Right(sInput, Len(sInput) - Index - Len(Match.Value))
                    Added = Added + Len(TmpLink) - Len(Match.Value)
                End If
            Next

            'REMOVE ALL HTML TAGS
            sInput = Regex.Replace(sInput, "(<\s*/td\s*>)", "#AE:TAB#", RegexOptions.IgnoreCase)

            Dim Pattern As String = "<\s*br\s*>|<\s*br/\s*>|<\s*/pre\s*>|<\s*/textarea\s*>|" _
                                  & "<\s*/blockquote\s*>|<\s*/body\s*>|<\s*/dd\s*>|<\s*/DIR\s*>|<\s*/DIV\s*>|" _
                                  & "<\s*/DL\s*>|<\s*/DT\s*>|<\s*/h1\s*>|<\s*/h2\s*>|<\s*/h3\s*>|<\s*/h4\s*>|" _
                                  & "<\s*/h5\s*>|<\s*/h6\s*>|<\s*/head\s*>|<\s*/li\s*>|<\s*/menu\s*>|<\s*/noframes\s*>|" _
                                  & "<\s*/ol\s*>|<\s*/p\s*>|<\s*/pre\s*>|<\s*/table\s*>|<\s*/th\s*>|" _
                                  & "<\s*/title\s*>|<\s*/tr\s*>|<\s*/ul\s*>"

            sInput = Regex.Replace(sInput, Pattern, "#AE:CRLF#", RegexOptions.IgnoreCase)
            sInput = Core.StripHTML(sInput)

            'Replace all linebreaks
            sInput = Regex.Replace(sInput, "\s{2,}", " ", RegexOptions.IgnoreCase)
            sInput = Regex.Replace(sInput, "#AE:TAB#", vbTab, RegexOptions.IgnoreCase)
            sInput = Regex.Replace(sInput, "#AE:CRLF#", vbCrLf, RegexOptions.IgnoreCase)

            sInput = Replace(sInput, vbCrLf & " ", vbCrLf)
            sInput = Replace(sInput, vbCrLf & vbTab, vbCrLf)

            sInput = Regex.Replace(sInput, "^\s*", "", RegexOptions.IgnoreCase)

            Return sInput
        End Function

        Public Shared Function AddList(ByVal sListName As String, ByVal sListDescription As String) As Integer
            Dim DB As New Database

            Dim SQL As String = "INSERT INTO lists_ ( PgmUnsub2_, AllowInfo_, ErrReset_, NoBodyOk_, NameReqd_, Topic_, PgmBefore_, Security_, Additional_, Accepted_, Comment_, URLLogo_, SimpleSub_, VisibGlobl_, ArchivNum_, MergeAllowBody_, ArchivDays_, ExpireDays_, MaxMessNum_, ConfDays_, ConfNotify_, PrimLang_, Visitors_, NoSearch_, HdrRemove_, NoEmail_, ApproveNum_, PgmAfter_, SMTPFrom_, AnyonePost_, ArchivSize_, PrivApprov_, AllowCross_, To_, MessageHdr_, CleanNotif_, DigestFtr_, SndActMess_, AllowDupe_, ReplyTo_, NoListHdr_, NoEmailSub_, RelPending_, ErrHold_, PrivDays_, NoConfirm_, SubPasswd_, SubNotDays_, PrsrvXTags_, CleanDays_, SMTPHdrs_, NoNNTP_, PasswdReqd_, ReferralPurgeDays_, NoMidRewr_, DescShort_, CleanUnsub_, RelHour_, Admin_, MaxMembers_, MergeCapabilities_, ArchivKeep_, AdminSend_, ConfUnsub_, BouncngNum_, CreatStamp_, MaxPerUser_, Moderated_, BlnkSubjOk_, ReviewPerm_, URLList_, Banned_, Name_, Anonymous_, ModHdrDate_, Child_, Hidden_, MaxMessSiz_, PostPass_, MaxQuoting_, CrossClean_, MessRecips_, BouncngTim_, PgmSub1_, Transact_, Disabled_, MessageFtr_, PgmUnsub1_, DelSubTop_, ReferralsPerDay_, CleanAuto_, ConfDate_, From_, SecLang_, ConfInterv_, DigestHdr_, NoArchive_, PgmSub2_, Keywords_, ListSubj_ ) VALUES " _
             & "( NULL, 'F', 10, 'F', 0, 'americaneagle', NULL, 'open', NULL, NULL, NULL, NULL, 'T', 'F', 0, 0, 0, 0, 300, 0, 0, 'English', 'F', 'F', '', 'F', -1, NULL, NULL, 'F', 0, NULL, 'F', '%%nameemail%%', NULL, 0, '', 'F', 'F', 'nochange', 'F', 'F', 0, 2, 0, 'F', NULL, NULL, 'F', 0, NULL, 'T', 0, 0, 'F', " & DB.Quote(sListDescription) & ", 0, 0, 'Admin', 0, 2, 0, 'T', 2, 0, GetDate(), 0, 'all', 'F', 0, NULL, NULL, " & DB.Quote(sListName) & ", 'F', 'F', 'T', 'F', 1000, 0, 0, 'F', 'T', 0, NULL, NULL, 'F', '', NULL, 'F', 20, 'F', NULL, NULL, 'English', 0, NULL, 'F', NULL, NULL, 'F' )"

            DB.Open(AppSettings("LyrisConnectionString"))
            Dim ListId As Integer = DB.InsertSQL(SQL)
            DB.Close()

            Return ListId
        End Function

        Public Shared Function AddListVersion85(ByVal sListName As String, ByVal sListDescription As String) As Integer
            Dim DB As New Database

            Dim SQL As String = "INSERT INTO lists_ ( PgmUnsub2_, AllowInfo_, NoBodyOk_, NameReqd_, DetectHtmlByDefault_, KeepOutmailPostings_, Topic_, PgmBefore_, Security_, RecencyDayCount_, Additional_, RecencyWebEnabled_, AddHeadersAndFooters_, Comment_, URLLogo_, SimpleSub_, VisibGlobl_, ArchivNum_, MergeAllowBody_, ArchivDays_, ExpireDays_, MaxMessNum_, ConfDays_, ConfNotify_, RecipientLoggingLevel_, Visitors_, NoSearch_, HdrRemove_, NoEmail_, ApproveNum_, PgmAfter_, SMTPFrom_, MriVisibility_, AnyonePost_, PrivApprov_, AllowCross_, DefaultFrom_, To_, CleanNotif_, MessageHdr_, DigestFtr_, AllowDupe_, TclMergeInit_, ReplyTo_, NoListHdr_, NoEmailSub_, RelPending_, ErrHold_, PrivDays_, NoConfirm_, SubPasswd_, SubNotDays_, PrsrvXTags_, CleanDays_, SMTPHdrs_, NoNNTP_, PasswdReqd_, ReferralPurgeDays_, RecencySequentialEnabled_, NoMidRewr_, DescShort_, CleanUnsub_, DefaultTo_, RelHour_, Admin_, MaxMembers_, MergeCapabilities_, AdminSend_, ConfUnsub_, TrackAllUrls_, CreatStamp_, MaxPerUser_, RecencyMailCount_, Moderated_, BlnkSubjOk_, ReviewPerm_, URLList_, Name_, Anonymous_, ModHdrDate_, Child_, MaxMessSiz_, RecencyOperator_, PostPass_, CrossClean_, DefaultSubject_, MaxQuoting_, PgmSub1_, Disabled_, RecencyTriggeredEnabled_, MessageFtr_, PgmUnsub1_, ReferralsPerDay_, DetectOpenByDefault_, RecencyEmailEnabled_, CleanAuto_, From_, MergeCapOverride_, DigestHdr_, NoArchive_, PgmSub2_, Keywords_, ListSubj_ ) VALUES " _
             & "( NULL, 'F', 'F', 0, 'T', 0, 'americaneagle', NULL, 'open', 7, NULL, 'F', 'N', NULL, NULL, 'T', 'F', 0, 0, 0, 0, 300, 0, 0, 'E', 'F', 'F', '', 'F', -1, NULL, NULL, 'H', 'F', NULL, 'F', 'login', '%%nameemail%%', 0, NULL, '', 'F', NULL, 'nochange', 'F', 'F', 0, 2, 0, 'F', NULL, NULL, 'F', 0, NULL, 'T', 1, 0, 'F', 'F', " & DB.Quote(sListDescription) & ", 0, '%%nameemail%%', 0, 'Admin', 0, 1, 'T', 2, 'F', GetDate(), 0, 3, 'all', 'F', 0, NULL, " & DB.Quote(sListName) & ", 'F', 'F', 'T', 1000, 'm', 0, 'F', NULL, 0, NULL, 'F', 'F', '', NULL, 20, 'T', 'F', 'F', NULL, 3, NULL, 'F', NULL, NULL, 'F' )"

            DB.Open(AppSettings("LyrisConnectionString"))
            Dim ListId As Integer = DB.InsertSQL(SQL)
            DB.Close()

            Return ListId
        End Function

        Public Shared Sub AddMember(ByVal sListName As String, ByVal ID As Integer, ByVal sName As String, ByVal sEmail As String, ByVal bIsAdmin As Boolean, ByVal sUsername As String, ByVal sPassword As String)
            Dim SQL As String, MemberId As Integer
            Dim sDomain As String, sIsAdmin As String
            Dim DB As New Database

            If sEmail = String.Empty Then
                Exit Sub
            End If
            If bIsAdmin Then sIsAdmin = "T" Else sIsAdmin = "F"
            If sUsername = String.Empty Then sUsername = LCase(Left(sEmail, InStr(sEmail, "@") - 1))
            If sPassword = String.Empty Then sPassword = ID

            sDomain = Right(sEmail, Len(sEmail) - InStrRev(sEmail, "@"))

            DB.Open(AppSettings("LyrisConnectionString"))

            Dim bUpdate As Boolean = False
            SQL = "SELECT UserID_ FROM members_ WHERE EmailAddr_ = " & DB.Quote(sEmail) & " AND List_ = " & DB.Quote(sListName)
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            If dr.HasRows Then bUpdate = True
            dr.Close()

            If bUpdate Then
                If Not UpdateMember(sListName, ID, sName, sEmail, bIsAdmin, sUsername, sPassword) Then
                    DB.Close()
                    Exit Sub
                End If
            Else
                SQL = "INSERT INTO members_ ( ReceiveAck_, CanAppPend_, MemberType_, EmailAddr_, SubType_, List_, NumAppNeed_, AppNeeded_, NotifySubm_, Domain_, NotifyErr_, IsListAdm_, Password_, CleanAuto_, FullName_, NoRepro_, NumBounces_, UserNameLC_, DateJoined_, RcvAdmMail_, UserID_ ) VALUES ( 'F', 'F', 'normal', " & DB.Quote(sEmail) & ", 'mail', " & DB.Quote(sListName) & ", 0, 'F', 'F', " & DB.Quote(sDomain) & ", 'F', " & DB.Quote(sIsAdmin) & ", " & DB.Quote(sPassword) & ", 'F', " & DB.Quote(sName) & ", 'F', 0, " & DB.Quote(sUsername) & ", GetDate(), 'F'," & DB.Number(ID) & ")"
                MemberId = DB.InsertSQL(SQL)
                If MemberId = 0 Then
                    DB.Close()
                    Exit Sub
                End If
            End If
            DB.Close()
        End Sub

        Private Shared Function UpdateMember(ByVal sListName As String, ByVal ID As Integer, ByVal sName As String, ByVal sEmail As String, ByVal bIsAdmin As Boolean, ByVal sUsername As String, ByVal sPassword As String) As Boolean
            Dim SQL As String, iRowsAffected As Integer
            Dim sIsAdmin As String

            If sEmail = String.Empty Then
                Return False
            End If

            If bIsAdmin Then sIsAdmin = "T" Else sIsAdmin = "F"
            If sUsername = String.Empty Then sUsername = LCase(Left(sEmail, InStr(sEmail, "@") - 1))
            If sPassword = String.Empty Then sPassword = ID
            Dim sDomain As String = Right(sEmail, Len(sEmail) - InStrRev(sEmail, "@"))

            Dim DB As New Database
            DB.Open(AppSettings("LyrisConnectionString"))

            SQL = " UPDATE members_ SET " _
              & " Domain_ = " & DB.Quote(sDomain) & "," _
              & " EmailAddr_ = " & DB.Quote(sEmail) & "," _
              & " FullName_ = " & DB.Quote(sName) & "," _
              & " Password_ = " & DB.Quote(sPassword) & "," _
              & " UserID_ = " & DB.Quote(ID) & "," _
              & " IsListAdm_ = " & DB.Quote(sIsAdmin) & "," _
              & " UserNameLC_ = " & DB.Quote(sUsername) _
              & " WHERE EmailAddr_ = " & DB.Quote(sEmail) & " AND List_ = " & DB.Quote(sListName)

            iRowsAffected = DB.ExecuteSQL(SQL)
            If iRowsAffected = 0 Then
                DB.Close()
                Return False
            End If
            DB.Close()

            Return True
        End Function

        Private Shared Function GetUTCFormattedDate() As String
            Dim DB As New Database
            DB.Open(AppSettings("LyrisConnectionString"))
            Dim dt As String = DB.ExecuteScalar("select GetUTCDate()")
            DB.Close()

            Dim sMonths As String = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
            Dim aMonths As String() = Split(sMonths, ",")
            Dim sDays As String = ",Sun,Mon,Tue,Wed,Thu,Fri,Sat"
            Dim aDays As String() = Split(sDays, ",")
            Dim dtime As String = aDays(DatePart("W", dt)) & ", " & Day(dt) & " " & aMonths(Month(dt) - 1) & " " & Year(dt)

            Dim hh, mm, ss
            hh = DatePart("H", dt) : If Len(hh) = 1 Then hh = "0" & hh
            mm = DatePart("N", dt) : If Len(mm) = 1 Then mm = "0" & mm
            ss = DatePart("S", dt) : If Len(ss) = 1 Then ss = "0" & ss

            dtime = dtime & " " & hh & ":" & mm & ":" & ss & " -0000"

            Return dtime
        End Function

        Private Shared Function GenNameEmail(ByVal sFromName As String, ByVal sFromEmail As String) As String
            Return """" & sFromName & """ <" & sFromEmail & ">"
        End Function

        Public Shared Sub SendHTMLMessage(ByVal sListName As String, ByVal sFromName As String, ByVal sFromEmail As String, ByVal sReplyTo As String, ByVal sBody As String, ByVal sSubject As String, ByVal dt As String)
            Dim sFromNameEmail As String = GenNameEmail(sFromName, sFromEmail)
            Dim sCharset As String = "iso-8859-1"

            Dim sHdrAll As String = "" _
                & "From: " & sFromNameEmail & vbCrLf _
                & "To: %%nameemail%%" & vbCrLf _
                & "Reply-To: " & sReplyTo & vbCrLf _
                & "Subject: " & sSubject & vbCrLf _
                & "Date: " & GetUTCFormattedDate() & vbCrLf _
                & "MIME-Version: 1.0" & vbCrLf _
                & "Content-Type: text/html; charset=""" & sCharset & """" & vbCrLf _
                & "Content-Transfer-Encoding: ""8bit"""

            Dim DB As New Database
            DB.Open(AppSettings("LyrisConnectionString"))

            Dim SQL As String = "INSERT INTO moderate_ ( Type_, SubsetID_, ReSubmit_, PurgeMess_, List_, ModHdrDate_, Transact_, AutoApDate_, HdrTo_, Title_, HdrFrom_, To_, Status_, HdrDate_, MaxRecips_, MemberID_, Body_, HdrSubject_, HdrAll_, HdrFromSpc_ ) VALUES ( 'admin-send', 0, 0, NULL, " & DB.Quote(sListName) & ", 'T', 'Message created by the website', " & DB.Quote(dt) & ", '%%nameemail%%', " & DB.Quote(sListName & " : " & Now()) & ", " & DB.Quote(sFromEmail) & ", " & DB.Quote(sListName) & ", 'new', " & DB.Quote(GetUTCFormattedDate()) & ", 0, 0, " & DB.Quote(sBody) & ", " & DB.Quote(sSubject) & ", " & DB.Quote(sHdrAll) & ", NULL )"

            DB.ExecuteSQL(SQL)
            DB.Close()
        End Sub

        Public Shared Sub SendTextMessage(ByVal sListName As String, ByVal sFromName As String, ByVal sFromEmail As String, ByVal sReplyTo As String, ByVal sBody As String, ByVal sSubject As String, ByVal dt As String)
            Dim sFromNameEmail As String = GenNameEmail(sFromName, sFromEmail)
            Dim sCharset As String = "iso-8859-1"

            Dim sHdrAll As String = "" _
                & "From: " & sFromNameEmail & vbCrLf _
                & "To: %%nameemail%%" & vbCrLf _
                & "Reply-To: " & sReplyTo & vbCrLf _
                & "Subject: " & sSubject & vbCrLf _
                & "Date: " & GetUTCFormattedDate() & vbCrLf _
                & "MIME-Version: 1.0" & vbCrLf _
                & "Content-Type: text/plain; charset=""" & sCharset & """" & vbCrLf _
                & "Content-Transfer-Encoding: ""8bit"""

            Dim DB As New Database
            DB.Open(AppSettings("LyrisConnectionString"))
            Dim SQL As String = "INSERT INTO moderate_ ( Type_, SubsetID_, ReSubmit_, PurgeMess_, List_, ModHdrDate_, Transact_, AutoApDate_, HdrTo_, Title_, HdrFrom_, To_, Status_, HdrDate_, MaxRecips_, MemberID_, Body_, HdrSubject_, HdrAll_, HdrFromSpc_ ) VALUES ( 'admin-send', 0, 0, NULL, " & DB.Quote(sListName) & ", 'T', 'Message created by the website', " & DB.Quote(dt) & ", '%%nameemail%%', " & DB.Quote(sListName & " : " & Now()) & ", " & DB.Quote(sFromEmail) & ", " & DB.Quote(sListName) & ", 'new', " & DB.Quote(GetUTCFormattedDate()) & ", 0, 0, " & DB.Quote(sBody) & ", " & DB.Quote(sSubject) & ", " & DB.Quote(sHdrAll) & ", NULL )"
            DB.ExecuteSQL(SQL)
            DB.Close()
        End Sub

        Public Sub GenerateMessages(ByVal GenerateLinks As Boolean)
            Dim oTplHTML As XTemplate = New XTemplate
            Dim oTplText As XTemplate = New XTemplate

            'Clear links array list
            m_aLinks.Clear()

            oTplHTML.Content = HTMLTemplate
            oTplText.Content = TextTemplate

            Dim iOrdinal As Integer = 1
            Dim Headlines As String = String.Empty
            Dim ConnStr As String = String.Empty
            For i As Integer = 0 To TemplateSlots.Count - 1
                Dim TemplateSlot As MailingTemplateSlotRow = TemplateSlots(i)
                Dim MessageSlot As MailingMessageSlot = MessageSlots(i)
                Dim Slot As String = String.Empty

                oTplHTML.Assign("HEADLINE" & (i + 1), MessageSlot.Headline)
                If GenerateLinks Then
                    oTplHTML.Assign("SLOT" & (i + 1), GenerateHTMLLinks(MessageSlot.Slot))
                Else
                    oTplHTML.Assign("SLOT" & (i + 1), MessageSlot.Slot)
                End If

                If MIMEType = "TEXT" Or MIMEType = "BOTH" Then
                    Slot = GeneratePlainText(MessageSlot.Slot)
                Else
                    Slot = MessageSlot.Slot
                End If

                If Not Slot = String.Empty Then
                    'Generate headlines
                    If Not MessageSlot.Headline = String.Empty Then
                        iOrdinal += 1
                        Headlines = Headlines & ConnStr & iOrdinal & "." & vbTab & GeneratePlainText(MessageSlot.Headline)
                        ConnStr = vbCrLf
                    End If
                    oTplText.Assign("HEADLINE" & (i + 1), GeneratePlainText(MessageSlot.Headline))
                    oTplText.Assign("SLOT" & (i + 1), Slot)
                    oTplText.Parse("BODY.CONTENT")
                End If
            Next

            oTplHTML.Assign("DATE", NewsletterDate)
            oTplHTML.Assign("BROWSER_LINK", Replace(Configuration.ConfigurationManager.AppSettings("LyrisBrowserLinkHTML"), "{LINK}", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/message.aspx?e=%%EmailAddr_%%&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"))
            oTplHTML.Assign("WEBSITE_URL", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
            oTplHTML.Assign("WEBSITE_NAME", System.Configuration.ConfigurationManager.AppSettings("GlobalWebsiteName"))
            oTplHTML.Parse("BODY")

            m_HTMLMessage = oTplHTML.GetBlock("BODY")

            If Not SavedText = String.Empty Then
                m_TextMessage = SavedText
            Else
                oTplText.Assign("DATE", NewsletterDate)
                oTplText.Assign("BROWSER_LINK", Replace(Configuration.ConfigurationManager.AppSettings("LyrisBrowserLinkText"), "{LINK}", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/message.aspx?e=%%EmailAddr_%%&mid=%%userid_%%&lid=%%merge lists_.ListId_%%"))
                oTplText.Assign("WEBSITE_URL", System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
                oTplText.Assign("WEBSITE_NAME", System.Configuration.ConfigurationManager.AppSettings("GlobalWebsiteName"))
                Call oTplText.Assign("HEADLINES", Headlines)
                Call oTplText.Parse("BODY")
                m_TextMessage = oTplText.GetBlock("BODY")
            End If
            If GenerateLinks Then
                m_TextMessage = GenerateTextLinks(m_TextMessage)
            Else
                m_TextMessage = m_TextMessage
            End If
        End Sub

    End Class

    Public Class LyrisLink
        Private m_URL As String
        Public Property URL() As String
            Get
                Return m_URL
            End Get
            Set(ByVal value As String)
                m_URL = value
            End Set
        End Property

        Private m_Type As LinkType
        Public Property Type() As LinkType
            Get
                Return m_Type
            End Get
            Set(ByVal value As LinkType)
                m_Type = value
            End Set
        End Property

        Private m_Href As String
        Public Property Href() As String
            Get
                Return m_Href
            End Get
            Set(ByVal value As String)
                m_Href = value
            End Set
        End Property

        Private m_Replaced As String
        Public Property Replaced() As String
            Get
                Return m_Replaced
            End Get
            Set(ByVal value As String)
                m_Replaced = value
            End Set
        End Property

        Public Enum LinkType
            HTML
            TEXT
        End Enum
    End Class


End Namespace
