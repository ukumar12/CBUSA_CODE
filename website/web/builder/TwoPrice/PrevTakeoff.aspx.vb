Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Collections.Generic
Imports System.Web.Services
Imports Utility
Imports System.Web.UI.WebControls

Partial Class builder_TwoPrice_PrevTakeoff
    Inherits SitePage
    Private TotalProducts As Integer
    Private TotalPrice As Double
    Private CountUnpricedProducts As Integer
    Protected ReadOnly Property TwoPriceTakeOffId As Integer
        Get
            Return Request("TwoPriceTakeOffId")
        End Get
    End Property
    Protected ReadOnly Property BuilderId As Integer
        Get
            Return Session("BuilderId")
        End Get
    End Property

    Private m_VendorId As Integer
    Protected ReadOnly Property VendorId As Integer
        Get
            If m_VendorId = Nothing Then
                m_VendorId = DB.ExecuteScalar("SELECT TOP 1 AwardedVendorId FROM TwoPriceCampaign " &
                                            "WHERE TwoPriceCampaignId IN (SELECT TOP 1 TwoPriceCampaignId FROM TwoPriceTakeOff WHERE TwoPriceTakeOffId = " & DB.Number(TwoPriceTakeOffId) & ")")
            End If

            Return m_VendorId
        End Get
    End Property
    Private _dbTwoPriceTakeOff As TwoPriceTakeOffRow
    Protected ReadOnly Property dbTwoPriceTakeOff As TwoPriceTakeOffRow
        Get
            If _dbTwoPriceTakeOff Is Nothing And Request("TwoPriceTakeOffId") IsNot Nothing Then
                _dbTwoPriceTakeOff = TwoPriceTakeOffRow.GetRow(DB, DB.Number(Request("TwoPriceTakeOffId")))
            End If
            Return _dbTwoPriceTakeOff
        End Get
    End Property
    Private _dbTwoPriceCampaign As TwoPriceCampaignRow
    Protected ReadOnly Property dbTwoPriceCampaign As TwoPriceCampaignRow
        Get
            If _dbTwoPriceCampaign Is Nothing Then
                _dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, dbTwoPriceTakeOff.TwoPriceCampaignId)
            End If
            Return _dbTwoPriceCampaign
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            'ViewState("F_SortBy") = gvListPrevTakeOff.SortBy
            'ViewState("F_SortOrder") = gvListPrevTakeOff.SortOrder
	    
	    'gvListPrevTakeOff.PageIndex = ViewState("GridPageIndex")

	    If Not IsPostBack Then
		BindPreviousTakeoffData()
                
	    End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub gvListPrevTakeOff_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvListPrevTakeOff.PageIndexChanging
        gvListPrevTakeOff.PageIndex = e.NewPageIndex
	ViewState("GridPageIndex") = e.NewPageIndex
        BindPreviousTakeoffData()
    End Sub

    Private Sub BindPreviousTakeoffData()
        'ViewState("F_SortBy") = gvListPrevTakeOff.SortBy
        'ViewState("F_SortOrder") = gvListPrevTakeOff.SortOrder

        gvListPrevTakeOff.DataKeyNames = New String() {"TakeoffId"}
        'gvListPrevTakeOff.Pager.NofRecords = TakeOffRow.GetBuilderTakeoffCount(DB, BuilderId)
        Dim Res As DataTable = TakeOffRow.GetBuilderTakeoffs(DB, BuilderId, "", 1, gvListPrevTakeOff.PageIndex, gvListPrevTakeOff.PageSize)
        gvListPrevTakeOff.DataSource = Res.DefaultView
        gvListPrevTakeOff.DataBind()
        'If gvList.Rows.Count > 0 Then
        '    gvList.UseAccessibleHeader = True
        '    gvList.HeaderRow.TableSection = TableRowSection.TableHeader
        '    'gvList.FooterRow.TableSection = TableRowSection.TableFooter
        'End If
    End Sub
End Class