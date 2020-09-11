Option Strict Off

Imports Components
Imports DataLayer
Imports Controls
Imports System.Configuration

Partial Class HeaderCtrl
    Inherits ModuleControl
    Implements IHeaderControl




    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ID = "ctrlHeader"

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ulBuilderLinks.Visible = (Session("BuilderId") IsNot Nothing)
            ulVendorLinks.Visible = (Session("VendorId") IsNot Nothing)
            ulPIQLinks.Visible = (Session("PIQId") IsNot Nothing)
            ulBuilderDashboard.Visible = (Session("BuilderId") IsNot Nothing)
            ulVendorDashboard.Visible = (Session("VendorId") IsNot Nothing)

            Dim ShowNCPLink As Boolean = DataLayer.SysParam.GetValue(DB, "ShowNCPLink")

            Dim MedullusNCPLink As String = IIf(CInt(SysParam.GetValue(DB, "TestMode")) = 1, SysParam.GetValue(DB, "MedullusNCPLinkTEST"), SysParam.GetValue(DB, "MedullusNCPLinkLive"))

            If ulBuilderLinks.Visible Then
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, CInt(Session("BuilderId")))
                spnCompanyName.InnerText = dbBuilder.CompanyName.Substring(0, IIf(dbBuilder.CompanyName.Length > 25, 25, dbBuilder.CompanyName.Length))

                Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, CInt(Session("BuilderAccountId")))
                spnAccountUser.InnerText = dbBuilderAccount.FirstName.Substring(0, 1) & ". " & dbBuilderAccount.LastName

                lnkNCPContractCentral.HRef = ConfigurationManager.AppSettings.Get("NCP_URL") & "CbusaBuilder/ContractCentral/Index?UserId=" & Convert.ToBase64String(Encoding.UTF8.GetBytes(Session("BuilderAccountId")))
                lnkNCPReporting.HRef = MedullusNCPLink & Convert.ToBase64String(Encoding.UTF8.GetBytes(Session("BuilderAccountId")))
                lnkNCPHelp.HRef = ConfigurationManager.AppSettings.Get("NCP_URL") & "Help.html"
                'lnkNCPHelp.HRef = MedullusNCPLink & Convert.ToBase64String(Encoding.UTF8.GetBytes(Session("BuilderAccountId")))

                If ShowNCPLink Then
                    'hypNationalContracts.NavigateUrl = MedullusNCPLink & Convert.ToBase64String(Encoding.UTF8.GetBytes(Session("BuilderAccountId")))
                    'hypNationalContracts.Target = "_blank"
                    'lnkNationalContracts.Visible = True
                End If

                If dbBuilder.QuarterlyReportingOn Then
                    hypBuilderReporting.Enabled = True
                Else
                    hypBuilderReporting.Enabled = False
                End If


            End If

            If ulVendorLinks.Visible Then
                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, CInt(Session("VendorId")))
                spnCompanyName.InnerText = dbVendor.CompanyName

                Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, CInt(Session("VendorAccountId")))
                spnAccountUser.InnerText = dbVendorAccount.FirstName & " " & dbVendorAccount.LastName

                Dim CurrentYear As Integer = DatePart(DateInterval.Year, Now)
                Dim CurrentQuarter As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
                Dim LastQuarter As Integer = IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
                Dim LastYear As Integer = IIf(LastQuarter = 4, CurrentYear - 1, CurrentYear)

                Dim LastQuarterEnd As DateTime = New Date(LastYear, (LastQuarter * 3), Date.DaysInMonth(LastYear, LastQuarter * 3)).Date

                Dim DaysVendorJoined As Integer = DateDiff(DateInterval.Day, dbVendor.Submitted, LastQuarterEnd)

                If Not dbVendor.QuarterlyReportingOn Or DaysVendorJoined < 0 Then
                    lnkVendorReporting.HRef = ""
                    lnkVendorReporting.Disabled = True
                    lnkVendorReporting.Style.Add("color", "lightgray")
                End If
            End If


            If Not Request.Url.AbsolutePath.ToLower.Contains("/takeoff/edit.aspx") Then
                phSearch.Visible = False
            End If
            slPreferredVendor.WhereClause = " VendorID In (Select VendorID from LLCVendor l inner join Builder b On l.LLCID=b.LLCID where exists (Select * from VendorProductPrice where VendorId=l.VendorId) And b.BuilderID=" & DB.Number(Session("BuilderID")) & ")"

        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Session("MemberName") Is Nothing Then
            'ltlMemberWelcome.Text = "Welcome <b>" & Session("MemberName") & "</b>"
        End If
    End Sub

    Protected Sub slPreferredVendor_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slPreferredVendor.ValueChanged
        Session("CurrentPreferredVendor") = slPreferredVendor.Value
        RaiseEvent VendorSelected(Me, System.EventArgs.Empty)
    End Sub

    Public Event VendorSelected(ByVal sender As Object, ByVal e As System.EventArgs) Implements Controls.IHeaderControl.ControlEvent

    Public Property ReturnValue() As String Implements Controls.IHeaderControl.ReturnValue
        Get
            Return slPreferredVendor.Value
        End Get
        Set(ByVal value As String)
            slPreferredVendor.Value = value
        End Set
    End Property
End Class
