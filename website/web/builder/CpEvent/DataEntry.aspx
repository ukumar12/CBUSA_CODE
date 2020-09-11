<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataEntry.aspx.vb" Inherits="BuilderDataEntry" EnableEventValidation="false" %>

<CT:MasterPage ID="CTMain" runat="server">
<asp:PlaceHolder runat="server">
    <script type="text/javascript">


        function CloseAddPtfDiv() {
            var div = document.getElementById("AddPtfProject");
            document.getElementById("AddPtfProject").style.display = "none";
        };

        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }
        function getUrlParam(parameter, defaultvalue) {
            var urlparameter = defaultvalue;
            if (window.location.href.indexOf(parameter) > -1) {
                urlparameter = getUrlVars()[parameter];
            }
            return urlparameter;
        }


        $(document).ready(function () {
            var VargvPtflist = document.getElementById("gvPtfList")
            var VargvCustlist = document.getElementById("gvCustList")
            var VarCampaign = getUrlParam('Tcam', 'empty');
            var VarOpt = getUrlParam('Opt', 'empty');
            document.getElementById("HdnTwoPriceCampaignId").value = VarCampaign;

            if (document.getElementById("RblSpecificProjects").checked) {

            } else {

                if (document.getElementById('HdnRet') = 1) {
                    document.getElementById("RblNoProjects").disabled = true;
                    document.getElementById("RblSpecificProjects").disabled = true;
                    document.getElementById("RblNonParticipant").disabled = true;
                }
                else {
                    document.getElementById("RblNoProjects").disabled = false;
                    document.getElementById("RblSpecificProjects").disabled = false;
                    document.getElementById("RblNonParticipant").disabled = false;
                }
            }
            $(".QAnsRequired").change(function () {
                RequiredField();
            });

            RequiredField()
        });

        function SelectSpecificProjects() {
            if (document.getElementById("RblSpecificProjects").checked = true) {
                document.getElementById("RblNoProjects").checked = false;
                document.getElementById("RblNonParticipant").checked = false;
            }
        }
        function SelectNoProjects() {
            debugger;
            var month = moment().month() + 1;
            var date = month + '/' + moment().date() + '/' + moment().year()
            if (document.getElementById("RblSpecificProjects").checked) {
                if ((document.getElementById("HdnPurchases").value == 'True') && (document.getElementById("Deadline").innerHTML > date)) {
                    var result = confirm("Are you sure you want to change your participation? This action will delete all of the projects you have already entered. You won't be able to undo this deletion.");
                    if (result) {
                        document.getElementById("RblNoProjects").checked = true;
                        document.getElementById("RblSpecificProjects").checked = false;
                        document.getElementById("RblNonParticipant").checked = false;
                        
                    } else {
                        document.getElementById("RblSpecificProjects").checked = true;
                        document.getElementById("RblNonParticipant").checked = false;
                        document.getElementById("RblNoProjects").checked = false;
                    }
                } else {
                    document.getElementById("RblNoProjects").checked = true;
                    document.getElementById("RblSpecificProjects").checked = false;
                    document.getElementById("RblNonParticipant").checked = false;                   
                }
            }
            else {
                document.getElementById("RblNoProjects").checked = true;
                document.getElementById("RblSpecificProjects").checked = false;
                document.getElementById("RblNonParticipant").checked = false;
             
            }
        }
        function SelectNonParticipant() {
            var VargvPtflist = document.getElementById("gvPtfList")
            var VargvCustlist = document.getElementById("gvCustList")
            if (document.getElementById("RblSpecificProjects").checked) {
                if ((HdnPurchases.value == 'True') || (document.getElementById("Deadline").innerHTML > moment())) {
                    var result = confirm("Are you sure you want to change your participation? This action will delete all of the projects you have already entered. You won't be able to undo this deletion.");
                    if (result) {
                        document.getElementById("RblNonParticipant").checked = true;
                        document.getElementById("RblSpecificProjects").checked = false;
                        document.getElementById("RblNoProjects").checked = false;
                       
                    } else {
                        document.getElementById("RblNonParticipant").checked = false;
                        document.getElementById("RblSpecificProjects").checked = true;
                        document.getElementById("RblNoProjects").checked = false;
                    }
                } else {
                    document.getElementById("RblNonParticipant").checked = false;
                    document.getElementById("RblSpecificProjects").checked = true;
                    document.getElementById("RblNoProjects").checked = false;
             
                }
            }
            else {
                document.getElementById("RblNoProjects").checked = false;
                document.getElementById("RblSpecificProjects").checked = false;
                document.getElementById("RblNonParticipant").checked = true;
                
            }
        }

        function CnfDeleteProject() {
            var result = confirm("Are you sure you want to delete this project.");
            document.getElementById("HdnCnfDelete").value = result
        }
     



        function RequiredField() {
            var isValid = true;
            if ($(".QAnsRequired").length > 0) {

                $(".QAnsRequired").each(function () {
                    // Test if the div element is empty
                    if ($(this).val() == '') {
                        isValid = false;
                        $(this).addClass("redborder")
                    } else {
                        $(this).removeClass("redborder")
                    }
                });
                if (!isValid) {
                    $('#btnSavePtfProjectData').attr('disabled', true);
                    $('#btnSavePtfProjectData').addClass("btngray")
                } else {
                    $('#btnSavePtfProjectData').attr('disabled', false);
                    $('#btnSavePtfProjectData').removeClass("btngray")
                }
            }
            return isValid;
        }

        function ShowCopyProjectDialog(BuilderProjId) {
            document.getElementById("ddlCopyProjectCounter").value = "1";
            document.getElementById("hdnSelectedProjectId").value = BuilderProjId;
            document.getElementById("divCopyProjectCounter").style.display = "block";
            return false;

        }
        function OpenProjectList(ProjectType) {
            //document.getElementById("ddlCopyProjectCounter").value = "1";
            //document.getElementById("hdnSelectedProjectId").value = BuilderProjId;
            document.getElementById("divProjectList").style.display = "block";
            return false;
        }
        function HideProjectListDialog() {
            document.getElementById("divProjectList").style.display = "none";
            return false;
        }
        function HideCopyProjectDialog() {
            document.getElementById("divCopyProjectCounter").style.display = "none";
            location.href = location.href;
            return false;
        }

        function ShowCreateBlankProjectDialog(ConstructionType) {
            document.getElementById("ddlBlankProjectCounter").value = "1";
            document.getElementById("hdnConstructionType").value = ConstructionType;
            document.getElementById("divCreateMultipleBlankProjCounter").style.display = "block";
            return false;
        }

        function HideCreateBlankProjectDialog() {
            document.getElementById("divCreateMultipleBlankProjCounter").style.display = "none";
            return false;
        }
</script>
    <script src="/includes/moment.js"></script>
    <style type="text/css">
        .windowPerformance {
            position: absolute;
            z-index: 10;
            background-color: #e1e1e1;
            border: 2px solid #c2c2c2;
            margin-bottom: 15px;
            margin-left: 624px;
            margin-top: 71px;
        }

        .hiddencol {
            display: none;
            width: 0px;
        }

        .overlay {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            background: rgba(0, 0, 0, 0.7);
            transition: opacity 500ms;
            visibility: hidden;
            opacity: 0;
        }

            .overlay:target {
                visibility: visible;
                opacity: 1;
            }
    </style>

</asp:PlaceHolder> 


 

 <div align="center" class="pckghdgred" style="background: 0; padding-bottom: 30px">Enrollment Details : <span id="SpnCpevent" runat="server"></span></div>
    <table  border="0" cellpadding="0" cellspacing="0"  style=" width: 100%"><tr><td>
         <asp:HiddenField ID="HdnTwoPriceCampaignId" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnCheckPtfNewEntry" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnCheckCustNewEntry" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnPtfBuilderProjectId" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnCustBuilderProjectId" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnPurchases" runat="server" value="False"></asp:HiddenField>
         <asp:HiddenField ID="HdnParticipationType" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnPtfProject" runat="server"></asp:HiddenField>         
         <asp:HiddenField ID="HdnCnfDelete" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnPtfProjDelete" runat="server"></asp:HiddenField>
         <asp:HiddenField ID="HdnCustProjDelete" runat="server"></asp:HiddenField>
          <asp:HiddenField ID="HdnRet" runat="server"></asp:HiddenField>
        </td></tr></table>

	<div  class="bdywrpr" >
        <div class="enrollment-details" id="mainTotalForm" runat="server">
            <div  class="corwrpr" style="width: 1024px;">
                <div class="enrollment-left">
                    <div class="enrollment-left-top">
                        <label><strong>Start Date : </strong><span id="StartDate" runat="server"></span></label>
                        <label class="right"><strong>End Date : </strong><span id="EndDate" runat="server"></span></label>

                    </div>
                    <div class="bdbtblhdg">
                        <div>Participation <span id="spnDeadLine" runat="server">(Enrollment Deadline <span id="Deadline" runat="server"></span>)</span>

                        </div>
                    </div>
                    <ul>
                         <li id="liSpecificProjects" runat="server" >
                                <span class="input-radio">
                                <asp:RadioButton ID="RblSpecificProjects" runat="server" AutoPostBack="true" onclick="SelectSpecificProjects()"></asp:RadioButton>
                            </span>

                            <span class="enrollment-left-body">I will participate and have specific projects that will require materials <span class="enrollment-chekbox-area"><asp:HiddenField ID="hdnUncheckedProjectType" runat="server" value=""></asp:HiddenField>
                       
                            </span>
                            </span>
                        </li>
                        <li id="liNoProjects" runat="server">
                            <span class="input-radio">
                             <asp:RadioButton ID="RblNoProjects" runat="server" AutoPostBack="true" onclick="SelectNoProjects()"></asp:RadioButton>
                            </span>
                            <span class="enrollment-left-body">I currently have no projected starts, but I will buy from the awarded supplier if the opportunity arises </span>

                        </li>
                        <li id="liNonParticipant" runat="server">
                            <span class="input-radio">
                                <asp:RadioButton ID="RblNonParticipant" runat="server" AutoPostBack="true" onclick="SelectNonParticipant()"></asp:RadioButton>
                            </span>
                            <span class="enrollment-left-body">I will not participate in this committed purchase </span>
                        </li>

                    </ul>
                    <div class="bdbtblhdg">Event Description </div>
                    <div id="TxtEventDiscription" runat="server" class="cpdiscription"></div>
             </div>
             <div class="enrollment-right" style="width: 68%; height:auto!important">
                 <div id="divPtf" runat="server" class="enrollment-right-body">
                       <h3>
                         <label id="lblPort">My Projects</label>
                         <label id="lblViewProject" class="view-project">View/ Copy Pojects 
                                 <a id="btnviewProjects" href="#" runat="server" onclick="OpenProjectList('Portfolio Homes')"><i aria-hidden="true" class="fa fa-eye"></i></a>

                             </label>
                         <label id="lblAddMultiplePortfolio" class="add-project" style="margin-left: 20px;">Add Multiple 
                                <a id="lnkAddMultiplePortfolio" href="#" onclick="return ShowCreateBlankProjectDialog('1');">
                                    <i aria-hidden="true" class="fa fa-plus-square"></i></a></label>
                         <label id="lblAddPort" class="add-project">Add project 
                          <a id="BtnAddPtfProjects" runat="server"><i aria-hidden="true" class="fa fa-plus-square"></i></a></label>

                        </h3>
                        <div id="tblPtfProjects" runat="server" class="enrollment-right-table" style="height: auto; width: 100%; overflow: auto; max-height: 350px;">
                            <CC:GridView ID="gvPtfList" runat="server" AllowSorting="True" OnRowDataBound="gvPtfList_RowDataBound" OnRowCommand="gvPtfList_RowCommand" AutoGenerateColumns="true" border="0" 
                                CausesValidation="false" class="tblcompr" DataKeyNames="BuilderProjectId" OnRowEditing="gvPtfList_RowEditing" 
                                style="margin: 0px; background-color: transparent;" width="100%">
                                <AlternatingRowStyle CssClass="Cprow" Font-Size="Larger" VerticalAlign="Top"></AlternatingRowStyle>
	                            <RowStyle CssClass="Cprow" Font-Bold="true" Font-Size="Larger" VerticalAlign="Top"></RowStyle>                              
                                 <Columns>
                                  
                                        <asp:TemplateField HeaderText="Action" >
                                            <ItemTemplate>
                                                        <div id="divPtfEditDeleteCopy" class="GridIcon" runat="server" style="display: block; width:65px;">
                                                            <asp:LinkButton ID="BtnEditPtf" runat="server"  commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderProjectId") %>'  CommandName="EDIT"  ToolTip="Edit">
                                                                 <i class="fa fa-pencil-square" aria-hidden="true"></i>
                                                            </asp:LinkButton>
                                                            <asp:LinkButton ID="BtnPtfDelete"  runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderProjectId") %>' onClick="DeleteProject" OnClientClick="CnfDeleteProject()" ToolTip="Delete">
                                                            <i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                                                    <%--OnClientClick = "CnfDeleteProject()"--%>
                                                             <asp:LinkButton ID="lnkBtnCopyPortfolioProject" runat="server"  commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderProjectId") %>' OnClientClick="return ShowCopyProjectDialog('');" ToolTip="Copy"><i class="fa fa-copy" aria-hidden="true"></i></asp:LinkButton>
                                                             <%--<asp:LinkButton ID="lnkBtnpfProjectData"  runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "BuilderProjectId") %>' OnClick="UpdatePfProjectData"  ToolTip="Project data"><i class="fa fa-question-circle" aria-hidden="true"></i></asp:LinkButton>--%>

                                                         </div>
                                                        <div id="divPtfUpdateCancel" runat="server" style="display: none;">
                                                            <asp:LinkButton ID="lnkBtnUpdatePortfolio" runat="server" commandname="Update" ToolTip="Update"><i class="fa fa-floppy-o" aria-hidden="true"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkBtnCancelEditPtf" runat="server" commandname="Cancel" ToolTip="Cancel"><i class="fa fa-times" aria-hidden="true"></i></asp:LinkButton>

                                                        </div>

                                            </ItemTemplate>

                                        </asp:TemplateField>

                                    </Columns>

                            </CC:GridView>
                        </div>
                    </div>
                

            </div>

            </div>
        </div>
        <div id="AddPtfProject" runat="server" class="windowPerformance" style="height: auto; display: none; width: 380px; padding-bottom: 10px;">
            <div class="pckghdgred" style="height: auto"> 
                <asp:Literal runat="server" id="ltrlPfTitle" Text="Add Project"></asp:Literal>

            </div>
            <div class="bold center" style="padding: 15px;">                
                
             
               
              <table border="0" cellpadding="2" cellspacing="2" style="padding-bottom: 10px; display: none;" runat="server" id="tblPtProjectQuestions"> 
                    <tr>
                        <td colspan="3">
                             <div style="max-height:350px; overflow-y:auto;">
                                  <asp:GridView ID="gvProjectData" runat="server" AutoGenerateColumns="false" ShowHeader="false"   EmptyDataText="" BorderWidth="0px" GridLines="None" CellPadding="2" CellSpacing="2"  Visible="true">
                                        <Columns>
                                     
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate> 
                                                    <asp:HiddenField ID="hdnpfId" runat="server" value='<%# Bind("Id") %>'></asp:HiddenField>
                                                    <asp:HiddenField ID="hdnpfQuestionId" runat="server" value='<%# Bind("QuestionId") %>'></asp:HiddenField>

                                                 <asp:Label ID="Label1" runat="server" ToolTip='<%# Bind("HintText") %>' Text='<%# Bind("QuestionLabel") %>'></asp:Label>
                                                  <a  runat="server" id="info" style="cursor:pointer;" title='<%# Bind("HintText") %>'>
                                                      <i class="fa fa-info-circle"  aria-hidden="true"></i> </a>

                                               </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:TextBox id="txtpfAnswer" runat="server" CssClass='<%# IIf(Convert.ToBoolean(Eval("IsRequired")) = True, "QAnsRequired", "QAns") %>'   Text='<%# Bind("QuestionAns") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>                                      

                                        </Columns>

                                    </asp:GridView>
                             </div>
                        </td>
                    
                    </tr>
                     <tr>
                        <td colspan="3">
                             <asp:Button ID="btnSavePtfProjectData" runat="server" cssclass="btnred" text="Save" />
                            <asp:Button ID="btnCancelPtfProjectData" runat="server" cssclass="btnred" text="Cancel" />
                            
                        </td>
                    </tr> 
             </table>
                    </div>
                         
              
          
        </div>
        
    </div>

    <div id="DivComformation" runat="server" visible="false">
        <table align="center" arial,helvetica,verdana,sans-serif;"="" border="0" cellpadding="0" cellspacing="0" font-family:="" style="line-height: 0px" width="600">
            <tr style="page-break-before: always">
                <td align="center" height="300" style="line-height: 0px;" valign="middle">
                    <table cellpadding="0" cellspacing="0" style="border: 2px solid #2f567a; background-color: rgba(255,255,255,0.5); border-radius: 10px; border-collapse: separate; line-height: 0px;" width="100%">
                        <tr style="page-break-before: always">
                        <td align="left" style="padding: 6px; line-height: 0px;" valign="top">
                            <table border="0" cellpadding="0" cellspacing="0" style="line-height: 0px" width="100%">
                                <tr style="page-break-before: always">
                                    <td align="left" style="line-height: 0px; margin: 0px; padding: 0px;" valign="top">
                                        <table border="0" cellpadding="0" cellspacing="0" style="border-bottom: 2px solid #dfdfdf; padding: 20px; background-color: #f7f7f7;" width="100%">
                                            <tr style="page-break-before: always">
                                                <td align="left" style="margin: 0px; padding: 10px 0px 0px 0px; text-align: center; font-size: 15px; line-height: 16px; color: #0e2d50; font-weight: 400;" valign="top">
                                                    <span id="SpnNameWdMonth"></span>

                                                </td>

                                            </tr>
                                            <tr style="page-break-before: always">
                                                        <td align="left" style="margin: 0px; padding: 5px 0 5px 0px; text-align: center; font-size: 15px; line-height: 16px; color: #0e2d50; font-weight: 400;" valign="top">
                                                            <span id="SpnBuilderName"></span>

                                                        </td>
                                                    </tr>
                                            <tr style="page-break-before: always">
                                                <td align="left" style="margin: 0px; padding: 0px 0px 10px 0px; text-align: center; font-size: 15px; line-height: 16px; color: #38761d; font-weight: 400;" valign="top">INVITATION ACCEPTED </td>

                                            </tr>

                                        </table>

                                    </td>

                                </tr>
                                <tr style="page-break-before: always">
                                    <td align="left" style="padding: 0px 40px; line-height: 0px;" valign="top">
                                        <table border="0" cellpadding="0" cellspacing="0" style="line-height: 0px" width="100%">
                                            <tr>
                                                <td align="left" style="line-height: 0px; padding-top: 15px;" valign="top">
                                                    <p style="margin: 0px; padding: 0px; font-size: 13px; line-height: 16px; color: #494949; font-weight: 400;">Thanks for joining the 
                                                        <span id="spnName" style="font-weight: 700;"></span>committed purchase event. 

                                                    </p>

                                                </td>

                                            </tr>
                                            <tr style="page-break-before: always">
                                                <td align="left" style="line-height: 0px; padding-top: 15px;" valign="top">
                                                <p style="margin: 0px; padding: 0px; font-size: 13px; line-height: 16px; color: #494949; font-weight: 400;">You can also change your mind and <span id="SpnOptOutLink"></span>of the committed purchase event until the enrollment deadline on <span id="SpnResponseDeadline"></span></p>

                                                </td>

                                            </tr>

                                        </table>

                                    </td>

                                </tr>

                            </table>

                        </td>

                        </tr>

                    </table>

                </td>

            </tr>

        </table>

    </div>
    <div id="divCopyProjectCounter" runat="server" class="overlay-bg" style="display: none;">
        <div class="windowPerformance committed-popup" style="height: auto; padding-bottom: 10px; border: solid 1px;">
            <table style="width: 100%;">
                <tr>
                <td style="width: 5%;"></td>
                    <td style="width: 30%;">
                        <asp:DropDownList ID="ddlCopyProjectCounter" runat="server" style="width: 50px; border-radius: 0px;"></asp:DropDownList> 
                    </td>
                    <td style="width: 30%; text-align: right;">
                        <asp:HiddenField ID="hdnSelectedProjectId" runat="server"></asp:HiddenField>
                        <asp:Button ID="btnCreateProjectCopy" runat="server" cssclass="btnred" text="COPY" />

                    </td>
                    <td style="width: 30%;">
                        <asp:Button ID="btnCloseCopyProject" runat="server" cssclass="btnred" onclientclick="return HideCopyProjectDialog();" text="CANCEL" />

                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="divCreateMultipleBlankProjCounter" runat="server" class="overlay-bg" style="display: none;">
        <div class="windowPerformance committed-popup" style="height: auto; padding-bottom: 10px; border: solid 1px;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5%;"></td>
                    <td style="width: 30%;"><asp:DropDownList ID="ddlBlankProjectCounter" runat="server" style="width: 50px; border-radius: 0px;"></asp:DropDownList> 
                    </td>
                    <td style="width: 30%; text-align: right;"><asp:HiddenField ID="hdnConstructionType" runat="server"></asp:HiddenField>
                        <asp:Button ID="btnCreateBlankProjects" runat="server" cssclass="btnred" text="CREATE" /></td>
                    <td style="width: 30%;"><asp:Button ID="btnCloseCreateBlankProject" runat="server" cssclass="btnred" onclientclick="return HideCreateBlankProjectDialog();" text="CANCEL" /></td>
                </tr>
            </table>
        </div>
    </div>

    <div id="divProjectList" runat="server" class="overlay-bg GridPopup" style="display: none;">
        <div class="windowPerformance committed-popup" style="height: auto; padding-bottom: 4px; border: solid 1px;">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <table style="width: 100%;">
                        <tr>
                            <th colspan="4" style="font-size:14px; color:#fff;">View/ Copy Projects</th>

                        </tr>
                        <tr>
                            <td>&#160;</td>
                           
        
                            <td align="right" style="padding-right:10px;">
                               <div style="float:left;"> <asp:DropDownList runat="server" id="ddlBuilderEvents" AutoPostBack="true" OnSelectedIndexChanged = "ddlBuilderEvents_OnSelectedIndexChanged"></asp:DropDownList></div>
                               <div style="float:right;">  <asp:Button ID="btnCopy" runat="server" cssclass="btnred" text="Copy"></asp:Button>
                                <asp:Button ID="btnCloseProject" runat="server" cssclass="btnred" text="CANCEL"></asp:Button>
                                   </div>
                            
                            </td> 
                          
                        </tr>
                        <tr>
                            <td colspan="3">
                                <div style="max-width:695px; overflow-x:auto;">
                                <CC:GridView ID="grdProjList" runat="server" AutoGenerateColumns="true" OnRowDataBound="grdProjList_RowDataBound" CssClass="grid-table" EmptyDataText="No Records Found." 
                                    ShowHeaderWhenEmpty="true" Visible="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="" HeaderStyle-Width="20px" >
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkProject" runat="server"></asp:CheckBox>
                                               <asp:HiddenField ID="hdnProjectId" value='<%# DataBinder.Eval(Container.DataItem, "BuilderProjectId") %>' runat="server"></asp:HiddenField>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>

                                </CC:GridView>
                                    </div>

                            </td>

                        </tr>
                       
                    </table>

                </ContentTemplate>

            </asp:UpdatePanel>

        </div>

    </div>
    <style type="text/css">


        .windowPerformance.committed-popup {
            position: absolute;
            z-index: 10;
            background-color: #e1e1e1;
            border: 2px solid #c2c2c2;
            margin-bottom: 15px;
            margin-left: -300px;
            margin-top: -150px;
            width: 250px;
            left: 90%;
            height: 294px;
            top: 50%;
        }

        .committed-popup .overlay {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            background: rgba(0, 0, 0, 0.7);
            transition: opacity 500ms;
            visibility: hidden;
            opacity: 0;
        }

            .committed-popup .overlay:target {
                visibility: visible;
                opacity: 1;
            }

        .committed-popup .table-body-wrap {
            overflow-y: auto;
            background-color: #FFF;
            padding: 2px 15px 8px;
            margin: 10px;
            height: 208px;
            font-family: montserrat;
        }

        .overlay-bg {
            background: rgba(0,0,0,0.2);
            position: fixed;
            width: 100%;
            height: 100%;
            top: 0;
            left: 0px;
            z-index: 10;
        }

        label.view-project {
            float: right;
            margin: 0 0 0 20px;
        }

        .GridIcon a {
            padding: 0 3px 0 0;
        }

        .GridPopup .windowPerformance.committed-popup {
            position: absolute;
            z-index: 10;
            background-color: #fff;
            border: 2px solid #c2c2c2;
            margin-bottom: 15px;
            margin-left: -350px;
            margin-top: -150px;
            left: 50%;
            min-height: auto;
            height: auto;
            top: 50%;
            width: 700px;
        }

            .GridPopup .windowPerformance.committed-popup th {
                /*background-color: #295f9a;*/
            }

        table.grid-table {
            width: 100%;
        }

            table.grid-table th {
                color: #FFF;
                font-size: 12px;
            }

        .grid-table tr td {
            padding: 6px 18px 4px !important;
        }

        .committed-popup table td > div {
            max-height: 284px;
            overflow-y: auto;
        }

        .committed-popup {
            font-family: montserrat;
        }
    </style>

</CT:MasterPage>