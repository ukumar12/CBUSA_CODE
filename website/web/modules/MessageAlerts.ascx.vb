Option Strict Off

Imports Components
Imports DataLayer

Partial Class _default
    Inherits ModuleControl

    Protected m_ReturnCount As Integer = 0
    Protected m_DisplayViewAllLink As Boolean = True

    Public Property ReturnCount() As Integer
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpReturnCount.SelectedValue = "" Then
                    Return 0
                Else
                    Return CType(drpReturnCount.SelectedValue, Integer)
                End If
            Else
                Return m_ReturnCount
            End If
        End Get
        Set(ByVal value As Integer)
            If IsAdminDisplay And IsDesignMode Then
                drpReturnCount.SelectedValue = value
            Else
                m_ReturnCount = value
            End If
        End Set
    End Property

    Public Property DisplayViewAllLink() As Boolean
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpDisplayViewAllLink.SelectedValue = "" Then
                    Return False
                Else
                    Return CType(drpDisplayViewAllLink.SelectedValue, Boolean)
                End If
            Else
                Return m_ReturnCount
            End If
        End Get
        Set(ByVal value As Boolean)
            If IsAdminDisplay And IsDesignMode Then
                drpReturnCount.SelectedValue = IIf(value, 1, 0)
            Else
                m_DisplayViewAllLink = value
            End If
        End Set
    End Property

    Public Overrides Property Args() As String
        Get
            Return "ReturnCount=" & ReturnCount & "&DisplayViewAllLink=" & DisplayViewAllLink
        End Get
        Set(ByVal Value As String)
            If Value = String.Empty Then Exit Property
            Dim Pairs() As String = Value.Split("&"c)

            If Pairs.Length >= 1 Then
                Dim ReturnCountValues() As String = Pairs(0).Split("="c)
                If ReturnCountValues.Length > 0 Then
                    ReturnCount = CType(ReturnCountValues(1), Integer)
                End If
            End If

            If Pairs.Length >= 2 Then
                Dim DisplayViewAllLinkValues() As String = Pairs(1).Split("="c)
                If DisplayViewAllLinkValues.Length > 0 Then
                    DisplayViewAllLink = CType(DisplayViewAllLinkValues(1), Boolean)
                End If
            End If

        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        divDesigner.Visible = IsDesignMode

        If IsDesignMode Then
            If IsPostBack Then
                CheckPostData(Controls)
            End If
            hdnField.Value = Args
        End If

    End Sub

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        If Me.Page.FindControl("AjaxManager") Is Nothing Then
            Dim sm As New ScriptManager
            sm.ID = "AjaxManager"
            Try
                Me.Controls.AddAt(0, sm)
            Catch ex As Exception

            End Try
        End If

        If Not IsAdminDisplay Then

            If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                Me.lnkViewAll.HRef = "/builder/message/default.aspx"
            ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                Me.lnkViewAll.HRef = "/vendor/message/default.aspx"
            ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                Me.lnkViewAll.HRef = "/piq/message/default.aspx"
            End If

            SQL = RepeaterSQL()

            'Response.Write(SQL)
            'Response.End()

            If SQL <> String.Empty Then

                dt = DB.GetDataTable(SQL)

                Me.rptMessagesAlerts.DataSource = dt
                Me.rptMessagesAlerts.DataBind()

                If dt.Rows.Count > 0 Then
                    divNoCurrentMessages.Visible = False
                End If

                If DisplayViewAllLink Then
                    Me.lnkViewAll.Visible = True
                Else
                    Me.lnkViewAll.Visible = False
                End If

            End If

        End If

        If IsAdminDisplay And IsDesignMode Then
            drpReturnCount.Items.Insert(0, New ListItem("5", "5"))
            drpReturnCount.Items.Insert(0, New ListItem("10", "10"))
            drpReturnCount.Items.Insert(0, New ListItem("15", "15"))
            drpReturnCount.Items.Insert(0, New ListItem("25", "25"))
            drpReturnCount.Items.Insert(0, New ListItem("ALL", "0"))

            drpDisplayViewAllLink.Items.Insert(0, New ListItem("True", "1"))
            drpDisplayViewAllLink.Items.Insert(0, New ListItem("False", "0"))
        End If

    End Sub

    Private Function RepeaterSQL() As String

        If Not IsAdminDisplay Then
            If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                Return BuilderSQL()
            ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                Return VendorSQL()
            ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                Return PIQSQL()
            Else
                Return String.Empty
            End If
        Else
            Return String.Empty
        End If

    End Function

    Private Function BuilderSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AutomaticMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate,"
        SQL &= "    'Automatic' Type," & vbCrLf
        SQL &= "    0 IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AutomaticMessages am" & vbCrLf
        SQL &= "    JOIN AutomaticMessageBuilderRecipient ambr ON am.AutomaticMessageID = ambr.AutomaticMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsMessage = 1" & vbCrLf
        SQL &= "    AND am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.BuilderID = " & DB.Number(Session("BuilderId")) & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  UNION" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AdminMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate," & vbCrLf
        SQL &= "    'Admin' Type," & vbCrLf
        SQL &= "    am.IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AdminMessage am" & vbCrLf
        SQL &= "    JOIN AdminMessageBuilderRecipient ambr ON am.AdminMessageID = ambr.AdminMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.BuilderID = " & DB.Number(Session("BuilderId")) & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY IsAlert Desc, StartDate" & vbCrLf

        Return SQL


    End Function

    Private Function VendorSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AutomaticMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate,"
        SQL &= "    'Automatic' Type," & vbCrLf
        SQL &= "    0 IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AutomaticMessages am" & vbCrLf
        SQL &= "    JOIN AutomaticMessageVendorRecipient ambr ON am.AutomaticMessageID = ambr.AutomaticMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsMessage = 1" & vbCrLf
        SQL &= "    AND am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.VendorID = " & Session("VendorID") & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  UNION" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AdminMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate," & vbCrLf
        SQL &= "    'Admin' Type," & vbCrLf
        SQL &= "    am.IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AdminMessage am" & vbCrLf
        SQL &= "    JOIN AdminMessageVendorRecipient ambr ON am.AdminMessageID = ambr.AdminMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.VendorID = " & Session("VendorID") & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY IsAlert Desc, StartDate" & vbCrLf

        Return SQL

    End Function

    Private Function PIQSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AutomaticMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate,"
        SQL &= "    'Automatic' Type," & vbCrLf
        SQL &= "    0 IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AutomaticMessages am" & vbCrLf
        SQL &= "    JOIN AutomaticMessagePIQRecipient ambr ON am.AutomaticMessageID = ambr.AutomaticMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsMessage = 1" & vbCrLf
        SQL &= "    AND am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.PIQID = " & Session("PIQId") & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  UNION" & vbCrLf
        SQL &= "  SELECT" & vbCrLf
        SQL &= "    am.AdminMessageID ID," & vbCrLf
        SQL &= "    am.Subject," & vbCrLf
        SQL &= "    am.Title," & vbCrLf
        SQL &= "    LEFT(CAST(am.Message as varchar(100)), 100) LeftMessage," & vbCrLf
        SQL &= "    CAST(am.Message as varchar(MAX)) Message," & vbCrLf
        SQL &= "    am.StartDate," & vbCrLf
        SQL &= "    'Admin' Type," & vbCrLf
        SQL &= "    am.IsAlert," & vbCrLf
        SQL &= "    ambr.ReadDate" & vbCrLf
        SQL &= "  FROM" & vbCrLf
        SQL &= "    AdminMessage am" & vbCrLf
        SQL &= "    JOIN AdminMessagePIQRecipient ambr ON am.AdminMessageID = ambr.AdminMessageID" & vbCrLf
        SQL &= "  WHERE" & vbCrLf
        SQL &= "    am.IsActive = 1" & vbCrLf
        SQL &= "    AND am.StartDate <= getdate()" & vbCrLf
        SQL &= "    AND (am.EndDate >= getdate() OR am.EndDate IS NULL)" & vbCrLf
        SQL &= "    AND ambr.PIQID = " & Session("PIQId") & vbCrLf
        SQL &= "    AND ambr.IsActive = 1" & vbCrLf
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY IsAlert Desc, StartDate" & vbCrLf

        Return SQL

    End Function

    Protected Sub rptMessagesAlerts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMessagesAlerts.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")
        Dim divMsg As HtmlContainerControl = e.Item.FindControl("divMsg")
        Dim divAlert As HtmlContainerControl = e.Item.FindControl("divAlert")
        Dim btnUnRead As Button = e.Item.FindControl("btnUnRead")
        Dim btnSave As Button = e.Item.FindControl("btnSave")
        Dim btnDelete As Button = e.Item.FindControl("btnDelete")

        If e.Item.DataItem("ReadDate").ToString = String.Empty Then
            lnkMessageTitle.Attributes("class") = "amtitlelinkbold"
            divMsg.Attributes("class") = "msgunread"
            btnSave.Visible = True
            btnUnRead.Visible = False
        Else
            btnSave.Visible = False
            btnUnRead.Visible = True
        End If

        btnUnRead.CommandArgument = e.Item.DataItem("Type").ToString & ":" & e.Item.DataItem("ID").ToString
        btnSave.CommandArgument = e.Item.DataItem("Type").ToString & ":" & e.Item.DataItem("ID").ToString
        btnDelete.CommandArgument = e.Item.DataItem("Type").ToString & ":" & e.Item.DataItem("ID").ToString

        If e.Item.DataItem("IsAlert") Then
            divMsg.Visible = False
            divAlert.visible = True
        Else
            divMsg.Visible = True
            divAlert.visible = False
        End If

    End Sub

    Protected Sub rptMessagesAlerts_ItemCommand(ByVal Sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptMessagesAlerts.ItemCommand

        Try

            Select Case e.CommandName
                Case "UnRead"
                    If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                        MarkUnReadByBuilder(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                        MarkUnReadByVendor(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                        MarkUnReadByPIQ(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    End If

                Case "Read"
                    If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                        MarkReadByBuilder(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                        MarkReadByVendor(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                        MarkReadByPIQ(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    End If

                Case "Delete"
                    If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                        MarkDeletedByBuilder(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                        MarkDeletedByVendor(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                        MarkDeletedByPIQ(e.CommandArgument.ToString.Split(":")(0), CType(e.CommandArgument.ToString.Split(":")(1), Integer))
                    End If

            End Select

            Response.Redirect(Request.Url.AbsoluteUri)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub MarkUnReadByBuilder(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkUnReadByBuilder(Me.DB, Session("BuilderId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkUnReadByBuilder(Me.DB, Session("BuilderId"))

        End If

    End Sub

    Private Sub MarkReadByBuilder(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkReadByBuilder(Me.DB, Session("BuilderId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkReadByBuilder(Me.DB, Session("BuilderId"))

        End If

    End Sub

    Private Sub MarkDeletedByBuilder(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkDeletedByBuilder(Me.DB, Session("BuilderId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkDeletedByBuilder(Me.DB, Session("BuilderId"))

        End If

    End Sub

    Private Sub MarkUnReadByVendor(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkUnReadByVendor(Me.DB, Session("VendorId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkUnReadByVendor(Me.DB, Session("VendorId"))

        End If

    End Sub

    Private Sub MarkReadByVendor(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkReadByVendor(Me.DB, Session("VendorId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkReadByVendor(Me.DB, Session("VendorId"))

        End If

    End Sub

    Private Sub MarkDeletedByVendor(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkDeletedByVendor(Me.DB, Session("VendorId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkDeletedByVendor(Me.DB, Session("VendorId"))

        End If

    End Sub

    Private Sub MarkUnReadByPIQ(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkUnReadByPIQ(Me.DB, Session("PIQId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkUnReadByPIQ(Me.DB, Session("PIQId"))

        End If

    End Sub

    Private Sub MarkReadByPIQ(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkReadByPIQ(Me.DB, Session("PIQId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkReadByPIQ(Me.DB, Session("PIQId"))

        End If

    End Sub

    Private Sub MarkDeletedByPIQ(ByVal Type As String, ByVal ID As Integer)

        Dim AutomaticMessage As DataLayer.AutomaticMessagesRow
        Dim AdminMessage As DataLayer.AdminMessageRow

        If Type = "Admin" Then

            AdminMessage = DataLayer.AdminMessageRow.GetRow(Me.DB, ID)
            AdminMessage.MarkDeletedByPIQ(Me.DB, Session("PIQId"))

        ElseIf Type = "Automatic" Then

            AutomaticMessage = DataLayer.AutomaticMessagesRow.GetRow(Me.DB, ID)
            AutomaticMessage.MarkDeletedByPIQ(Me.DB, Session("PIQId"))

        End If

    End Sub

End Class
