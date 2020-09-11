Option Strict Off

Imports Components
Imports DataLayer

Partial Class _default
    Inherits ModuleControl

    Protected m_ReturnCount As Integer = 0
    Protected m_DisplayViewAllLink As Boolean = True

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private BuilderDocumentId As String = ""
    Private VendorDocumentId As String = ""
    Private PIQDocumentID As String = ""

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
        Me.fulDocument.Required = True

        If IsDesignMode Then
            If IsPostBack Then
                CheckPostData(Controls)
            End If
            hdnField.Value = Args
        End If

    End Sub

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Me.rdoCBUSADocuments.Checked = True
        End If

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
                Me.lnkViewAll.HRef = "/builder/document/default.aspx"
            ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                Me.lnkViewAll.HRef = "/vendor/document/default.aspx"
            ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                Me.lnkViewAll.HRef = "/piq/document/default.aspx"
            End If

            BindRepeater()

            
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

        PageURL = Request.Url.ToString()
        If Session("BuilderId") IsNot Nothing Then
        CurrentUserId = Session("BuilderId")
        End If
        UserName = Session("Username")
        Core.DataLog("Documents", PageURL, CurrentUserId, "Builder Left Menu Click", "", "", "", "", UserName)
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
        End If

    End Function

    Private Function BuilderSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf

        If rdoCBUSADocuments.Checked = True Then
            SQL &= "  SELECT DISTINCT " & vbCrLf
            SQL &= "    ad.AdminDocumentID ID," & vbCrLf
            SQL &= "    ad.Title," & vbCrLf
            SQL &= "    ad.FileName," & vbCrLf
            SQL &= "    ad.FileGUID GUID," & vbCrLf
            SQL &= "    ad.Uploaded," & vbCrLf
            SQL &= "    'Admin' Type" & vbCrLf
            SQL &= "  FROM" & vbCrLf
            SQL &= "    AdminDocument ad" & vbCrLf
            SQL &= "    join AdminDocumentDocumentAudienceType addat on addat .AdminDocumentID = ad.AdminDocumentID " & vbCrLf
            SQL &= "    JOIN AdminDocumentLLC  adbr ON ad.AdminDocumentID = adbr.AdminDocumentID " & vbCrLf
            SQL &= "  WHERE" & vbCrLf
            SQL &= "    ad.IsApproved = 1" & vbCrLf
            SQL &= "   AND adbr.LLCID = " & BuilderRow.GetRow(DB, Session("BuilderId")).LLCID & vbCrLf
            SQL &= " And addat.DocumentAudienceTypeId = 1 " & vbCrLf
            SQL &= "  And " & Session("BuilderId") & " IN (select builderid from Builder where HasDocumentsAccess = 1) " & vbCrLf
        End If
        'If rdoAllDocuments.Checked = True Then
        '    SQL &= "  UNION" & vbCrLf
        'End If
        'If rdoAllDocuments.Checked = True or rdoOurDocuments.Checked = True Then
        '    SQL &= "  Select" & vbCrLf
        '    SQL &= "    BuilderDocumentID ID," & vbCrLf
        '    SQL &= "    Title," & vbCrLf
        '    SQL &= "    FileName," & vbCrLf
        '    SQL &= "    GUID GUID," & vbCrLf
        '    SQL &= "    Uploaded," & vbCrLf
        '    SQL &= "    'Account' Type" & vbCrLf
        '    SQL &= "  FROM" & vbCrLf
        '    SQL &= "    BuilderDocument" & vbCrLf
        '    SQL &= "  WHERE" & vbCrLf
        '    SQL &= "    IsApproved = 1" & vbCrLf
        '    SQL &= "    AND BuilderID = " & DB.Number(Session("BuilderId")) & vbCrLf
        'End If
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY Title" & vbCrLf
        
        Return SQL

    End Function

    Private Function VendorSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf
        If rdoCBUSADocuments.Checked = True Then
            SQL &= "  SELECT DISTINCT " & vbCrLf
            SQL &= "    ad.AdminDocumentID ID," & vbCrLf
            SQL &= "    ad.Title," & vbCrLf
            SQL &= "    ad.FileName," & vbCrLf
            SQL &= "    ad.FileGUID GUID," & vbCrLf
            SQL &= "    ad.Uploaded," & vbCrLf
            SQL &= "    'Admin' Type" & vbCrLf
            SQL &= "  FROM" & vbCrLf
            SQL &= "    AdminDocument ad" & vbCrLf
            SQL &= "    join AdminDocumentDocumentAudienceType addat on addat .AdminDocumentID = ad.AdminDocumentID " & vbCrLf
            SQL &= "    JOIN AdminDocumentLLC  adbr ON ad.AdminDocumentID = adbr.AdminDocumentID " & vbCrLf
            SQL &= "  WHERE" & vbCrLf
            SQL &= "    ad.IsApproved = 1" & vbCrLf
            SQL &= "    and addat .DocumentAudienceTypeId = 2 " & vbCrLf
            SQL &= "    AND adbr.LLCID in ( " & VendorRow.GetRow(DB, Session("VendorID")).LLCID & ")" & vbCrLf
            SQL &= "  And " & Session("VendorID") & " IN (select VendorID from Vendor where HasDocumentsAccess = 1) " & vbCrLf
        End If
        'If rdoAllDocuments.Checked = True Then
        '    SQL &= "  UNION" & vbCrLf
        'End If
        'If rdoAllDocuments.Checked = True or rdoOurDocuments.Checked = True Then
        '    SQL &= "  Select" & vbCrLf
        '    SQL &= "    VendorDocumentID ID," & vbCrLf
        '    SQL &= "    Title," & vbCrLf
        '    SQL &= "    FileName," & vbCrLf
        '    SQL &= "    GUID GUID," & vbCrLf
        '    SQL &= "    Uploaded," & vbCrLf
        '    SQL &= "    'Account' Type" & vbCrLf
        '    SQL &= "  FROM" & vbCrLf
        '    SQL &= "    VendorDocument" & vbCrLf
        '    SQL &= "  WHERE" & vbCrLf
        '    SQL &= "    IsApproved = 1" & vbCrLf
        '    SQL &= "    AND VendorID = " & Session("VendorID") & vbCrLf
        'End If
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY Title" & vbCrLf

        Return SQL
    End Function

    Private Function PIQSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  *" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "(" & vbCrLf
        If rdoCBUSADocuments.Checked = True Then
            SQL &= "  SELECT" & vbCrLf
            SQL &= "    ad.AdminDocumentID ID," & vbCrLf
            SQL &= "    ad.Title," & vbCrLf
            SQL &= "    ad.FileName," & vbCrLf
            SQL &= "    ad.FileGUID GUID," & vbCrLf
            SQL &= "    ad.Uploaded," & vbCrLf
            SQL &= "    'Admin' Type" & vbCrLf
            SQL &= "  FROM" & vbCrLf
            SQL &= "    AdminDocument ad" & vbCrLf
            SQL &= "    join AdminDocumentDocumentAudienceType addat on addat .AdminDocumentID = ad.AdminDocumentID " & vbCrLf
            SQL &= "  WHERE" & vbCrLf
            SQL &= "    ad.IsApproved = 1" & vbCrLf
            SQL &= "    and addat .DocumentAudienceTypeId = 3 " & vbCrLf
        End If
        'If rdoAllDocuments.Checked = True  Then
        '    SQL &= "  UNION" & vbCrLf
        'End If
        'If rdoAllDocuments.Checked = True or rdoOurDocuments.Checked = True Then
        '    SQL &= "  SELECT" & vbCrLf
        '    SQL &= "    PIQDocumentID ID," & vbCrLf
        '    SQL &= "    Title," & vbCrLf
        '    SQL &= "    FileName," & vbCrLf
        '    SQL &= "    GUID GUID," & vbCrLf
        '    SQL &= "    Uploaded," & vbCrLf
        '    SQL &= "    'Account' Type" & vbCrLf
        '    SQL &= "  FROM" & vbCrLf
        '    SQL &= "    PIQDocument" & vbCrLf
        '    SQL &= "  WHERE" & vbCrLf
        '    SQL &= "    IsApproved = 1" & vbCrLf
        '    SQL &= "    AND PIQID = " & Session("PIQId") & vbCrLf
        'End If
        SQL &= "  ) d" & vbCrLf
        SQL &= "ORDER BY Title" & vbCrLf

        Return SQL
    End Function

    Protected Sub BindRepeater()
        
        Dim SQL As String = String.Empty
        Dim dt As DataTable

         SQL = RepeaterSQL()

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            Me.rptDocuments.DataSource = dt
            Me.rptDocuments.DataBind()

            If DisplayViewAllLink Then
                Me.lnkViewAll.Visible = True
            Else
                Me.lnkViewAll.Visible = False
            End If

            If dt.Rows.Count > 0 Then
                divNoCurrentDocuments.Visible = False
            End If
        End If
    End Sub
     
    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")
        Dim lnkDelete As Controls.ConfirmLink = e.Item.FindControl("lnkDelete")

        lnkMessageTitle.HRef = "/assets/document/" & e.Item.DataItem("FileName").ToString

        If e.Item.DataItem("Type").ToString = "Admin" Then
            lnkDelete.Visible = False
        End If

        If CType(Me.Page, SitePage).IsLoggedInBuilder Then
            lnkDelete.NavigateUrl = "/builder/document/delete.aspx?ID=" & e.Item.DataItem("ID").ToString
        ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
            lnkDelete.NavigateUrl = "/vendor/document/delete.aspx?ID=" & e.Item.DataItem("ID").ToString
        ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
            lnkDelete.NavigateUrl = "/piq/document/delete.aspx?ID=" & e.Item.DataItem("ID").ToString
        End If

    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        Try

            If Me.fulDocument.NewFileName <> String.Empty Then
                If Not IsAdminDisplay Then
                    If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                        SaveBuilderDocument()
                    ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                        SaveVendorDocument()
                    ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                        SavePIQDocument()
                    Else
                        Response.Redirect("/default.aspx")
                    End If
                End If
            End If

            Response.Redirect(Request.Url.AbsoluteUri)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub SaveBuilderDocument()

        Dim BuilderDocument As New DataLayer.BuilderDocumentRow

        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        BuilderDocument = New DataLayer.BuilderDocumentRow(Me.DB)
        BuilderDocument.BuilderID = Session("BuilderId")
        BuilderDocument.GUID = Guid.NewGuid.ToString
        BuilderDocument.IsApproved = True
        If Me.txtTitle.Text = String.Empty Then
            BuilderDocument.Title = Me.fulDocument.MyFile.FileName
        Else
            BuilderDocument.Title = Me.txtTitle.Text
        End If
        BuilderDocument.Uploaded = Now

        Me.fulDocument.Folder = "/assets/document/"
        Me.fulDocument.SaveNewFile()

        FileInfo = New System.IO.FileInfo(Server.MapPath(Me.fulDocument.Folder & Me.fulDocument.NewFileName))
        OriginalExtension = System.IO.Path.GetExtension(Me.fulDocument.MyFile.FileName)
        NewFileName = Replace(BuilderDocument.GUID, "-", "")
        FileInfo.CopyTo(Server.MapPath(Me.fulDocument.Folder & NewFileName & OriginalExtension))

        BuilderDocument.FileName = NewFileName & OriginalExtension
        BuilderDocument.Insert()

        'log Add Document
        BuilderDocumentId = BuilderDocument.BuilderDocumentID
        Core.DataLog("Document", PageURL, CurrentUserId, "Add Document", BuilderDocumentId, "", "", "", UserName)
        'end log

        FileInfo.Delete()


    End Sub

    Public Sub SaveVendorDocument()

        Dim VendorDocument As New DataLayer.VendorDocumentRow

        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        VendorDocument = New DataLayer.VendorDocumentRow(Me.DB)
        VendorDocument.VendorID = Session("VendorId")
        VendorDocument.GUID = Guid.NewGuid.ToString
        VendorDocument.IsApproved = True
        If Me.txtTitle.Text = String.Empty Then
            VendorDocument.Title = Me.fulDocument.MyFile.FileName
        Else
            VendorDocument.Title = Me.txtTitle.Text
        End If
        VendorDocument.Uploaded = Now

        Me.fulDocument.Folder = "/assets/document/"
        Me.fulDocument.SaveNewFile()

        FileInfo = New System.IO.FileInfo(Server.MapPath(Me.fulDocument.Folder & Me.fulDocument.NewFileName))
        OriginalExtension = System.IO.Path.GetExtension(Me.fulDocument.MyFile.FileName)
        NewFileName = Replace(VendorDocument.GUID, "-", "")
        FileInfo.CopyTo(Server.MapPath(Me.fulDocument.Folder & NewFileName & OriginalExtension))

        VendorDocument.FileName = NewFileName & OriginalExtension
        VendorDocument.Insert()

        'log Add Document
        VendorDocumentId = VendorDocument.VendorDocumentID
        Core.DataLog("Document", PageURL, CurrentUserId, "Add Document", VendorDocumentId, "", "", "", UserName)
        'end log


        FileInfo.Delete()

    End Sub

    Public Sub SavePIQDocument()

        Dim PIQDocument As New DataLayer.PIQDocumentRow

        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        PIQDocument = New DataLayer.PIQDocumentRow(Me.DB)
        PIQDocument.PIQID = Session("PIQId")
        PIQDocument.GUID = Guid.NewGuid.ToString
        PIQDocument.IsApproved = True
        If Me.txtTitle.Text = String.Empty Then
            PIQDocument.Title = Me.fulDocument.MyFile.FileName
        Else
            PIQDocument.Title = Me.txtTitle.Text
        End If
        PIQDocument.Uploaded = Now

        Me.fulDocument.Folder = "/assets/document/"
        Me.fulDocument.SaveNewFile()

        FileInfo = New System.IO.FileInfo(Server.MapPath(Me.fulDocument.Folder & Me.fulDocument.MyFile.FileName))
        OriginalExtension = System.IO.Path.GetExtension(Me.fulDocument.MyFile.FileName)
        NewFileName = Replace(PIQDocument.GUID, "-", "")
        FileInfo.CopyTo(Server.MapPath(Me.fulDocument.Folder & NewFileName & OriginalExtension))

        PIQDocument.FileName = NewFileName & OriginalExtension
        PIQDocument.Insert()

        'log Add Document
        PiqDocumentId = PIQDocument.PIQDocumentID
        Core.DataLog("Document", PageURL, CurrentUserId, "Add Document", PiqDocumentId, "", "", "", UserName)
        'end log

        FileInfo.Delete()

    End Sub

    'Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
       ' Response.Redirect("/builder/default.aspx")
    'End Sub

End Class
