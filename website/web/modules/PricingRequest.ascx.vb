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

            Me.lnkViewAll.HRef = "/vendor/pricingrequest/default.aspx"

            SQL = RepeaterSQL()

            If SQL <> String.Empty Then
                dt = DB.GetDataTable(SQL)

                Me.rptPricingRequests.DataSource = dt
                Me.rptPricingRequests.DataBind()

                If DisplayViewAllLink Then
                    Me.lnkViewAll.Visible = True
                Else
                    Me.lnkViewAll.Visible = False
                End If

                If dt.Rows.Count > 0 Then
                    divNoCurrentPriceRequests.Visible = False
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

        Dim SQL As String = String.Empty 

        If Not IsAdminDisplay Then

            SQL &= "SELECT" & IIf(ReturnCount > 0, " TOP " & ReturnCount.ToString, "") & vbcrlf
            SQL &= "b.CompanyName Builder," & vbcrlf
            SQL &= "p.ProductID," & vbcrlf
            SQL &= "p.Product," & vbcrlf
            SQL &= "CONVERT(VARCHAR(10), pc.Created, 101) Submitted" & vbcrlf
            SQL &= "FROM" & vbcrlf
            SQL &= "PriceComparison pc" & vbcrlf
            SQL &= "JOIN PriceComparisonVendorProductPrice pcvpp ON pc.PriceComparisonID = pcvpp.PriceComparisonID" & vbcrlf
            SQL &= "JOIN Product p ON pcvpp.ProductID = p.ProductID" & vbcrlf
            SQL &= "JOIN Builder b ON pc.BuilderID = b.BuilderID" & vbcrlf
            SQL &= "WHERE" & vbcrlf
            SQL &= "(pcvpp.UnitPrice < 0 OR pcvpp.UnitPrice IS NULL)" & vbcrlf
           'SQL &= "AND pcvpp.VendorID = " & Session("VendorID") & vbcrlf

        End If

        Return SQL

    End Function

    Protected Sub rptPricingRequests_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptPricingRequests.ItemCommand
        If e.CommandName = "EditPricing" Then
            'Me.ctrlProductPriceUpdate.show
        End If
    End Sub

    Protected Sub rptPricingRequests_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPricingRequests.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim btnUpdatePricing As LinkButton = e.Item.FindControl("btnUpdatePricing")
        btnUpdatePricing.CommandArgument = e.Item.DataItem("ProductID").ToString
        btnUpdatePricing.CommandName = "EditPricing"

    End Sub



End Class
