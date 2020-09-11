Option Strict Off

Imports Components
Imports DataLayer

Partial Class BannerCtrl
    Inherits ModuleControl

    Private m_BannerGroupId As Integer
    Public Property BannerGroupId() As Integer
        Get
            If IsAdminDisplay And IsDesignMode Then
                Return drpBannerGroupId.SelectedValue
            Else
                Return m_BannerGroupId
            End If
        End Get
        Set(ByVal value As Integer)
            If IsAdminDisplay And IsDesignMode Then
                drpBannerGroupId.SelectedValue = value
            Else
                m_BannerGroupId = value
            End If
        End Set
    End Property

    Public Overrides Property Args() As String
        Get
            Return "BannerGroupId=" & BannerGroupId
        End Get
        Set(ByVal Value As String)
            If Value = String.Empty Then Exit Property
            Dim aPairs() As String = Value.Split("&"c)

            Dim aBanner() As String = aPairs(0).Split("="c)
            If aBanner.Length > 0 Then
                BannerGroupId = aBanner(1)
            End If
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsAdminDisplay And IsDesignMode Then
            Dim dtBanner As DataTable = BannerGroupRow.GetGroupList(DB, Width)
            drpBannerGroupId.DataSource = dtBanner
            drpBannerGroupId.DataTextField = "Name"
            drpBannerGroupId.DataValueField = "BannerGroupId"
            drpBannerGroupId.DataBind()
            drpBannerGroupId.Items.Insert(0, New ListItem("-- select --", "0"))
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        divDesigner.Visible = IsDesignMode

        If IsDesignMode Then
            If IsPostBack Then
                CheckPostData(Controls)
            End If
            hdnField.Value = Args
        End If

        'in order to remove duplicate banners from the same page, we save banners in the context object
        'the list of displayed banners is passed to the GetRandomRow function below
        Dim Excluded As String = HttpContext.Current.Items("ExcludedBannerList")

        Dim dbLink As BannerRow = BannerRow.GetRandomRow(DB, BannerGroupId, Excluded)

        'add BannerId to list of banners to exlude on next module
        If Not Excluded = String.Empty Then Excluded &= ","
        Excluded &= dbLink.BannerId
        HttpContext.Current.Items("ExcludedBannerList") = Excluded

        Dim Target As String = "target=""_blank"""
        If dbLink.Target = String.Empty Then
            Target = String.Empty
        End If
        If dbLink.HTML = String.Empty Then
            ltlLink.Text = "<a href=""/banner/" & dbLink.BannerId & "/" & ReplaceSpecialCharacters(dbLink.AltText) & ".aspx"" " & Target & "><img border=""0"" src=""/assets/banner/" & dbLink.FileName & """ alt=""" & dbLink.AltText & """/></a><br />"
        Else
            ltlLink.Text = dbLink.HTML
        End If
        ltlLink.Visible = dbLink.IsActive

        If Not IsDesignMode Then
            Visible = dbLink.IsActive

            'Add impression
            If Visible Then
                BannerTrackingRow.AddImpression(DB, dbLink.BannerId, Now())
            End If
        End If
    End Sub

    Private Function ReplaceSpecialCharacters(ByVal s As String) As String
        s = Replace(s, "&", "")
        s = Replace(s, " ", "_")
        s = Replace(s, "-", "_")
        s = Replace(s, ".", "_")
        s = Replace(s, "$", "")
        s = Replace(s, "%", "")
        s = Replace(s, "__", "_")

        Return s
    End Function


End Class
