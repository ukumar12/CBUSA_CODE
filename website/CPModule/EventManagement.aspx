<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EventManagement.aspx.cs" Inherits="EventManagement" EnableEventValidation="false" ClientIDMode="Static" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Event Management</title>
    <%--Local Work--%>
    <%--  <link href="~/Content/css/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/css/font-awesome.min.css" rel="stylesheet" />
    <link href="~/Content/css/global.css" rel="stylesheet" />
    <link href="~/Content/css/style.css" rel="stylesheet" />
    <link href="~/Content/css/responsive.css" rel="stylesheet" />--%>
    <%--Live Work--%>
    <link href="Content/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Content/css/global.css" rel="stylesheet" />
    <link href="Content/css/style.css" rel="stylesheet" />
    <link href="Content/css/responsive.css" rel="stylesheet" />


    <%--Editor--%>
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.common.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.uniform.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.uniform.mobile.min.css" />
    <%--DatePicker--%>
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.common-bootstrap.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.bootstrap.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2018.3.911/styles/kendo.bootstrap.mobile.min.css" />


    <%--<script src="Scripts/jquery-1.10.2.min.js"></script>--%>

    <script src="Scripts/jquery-3.3.1.min.js"></script>

    <script src="Scripts/bootstrap.min.js"></script>


    <script src="https://kendo.cdn.telerik.com/2018.3.911/js/jquery.min.js"></script>
    <script src="https://kendo.cdn.telerik.com/2018.3.911/js/kendo.all.min.js"></script>
    <script src="Scripts/EventManagement.js"></script>


    <%--<script type="text/javascript" src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>--%>



    <script>
        $(document).ready(function () {

            $("[id*=list1] option").prop("selected", false);
            $("[id*=list2] option").prop("selected", false);

            var Flag = $("[id*=chkOptionSendToTheseVendors]").is(":Checked");
            var FlagVal = Flag == true ? 1 : 0;
            $("[id*=list1]").prop('disabled', !Flag);
            $("[id*=list2]").prop('disabled', !Flag);
            $("[id*=btnAdd]").prop('disabled', !Flag);
            $("[id*=btnAddAll]").prop('disabled', !Flag);
            $("[id*=btnRemove]").prop('disabled', !Flag);
            $("[id*=btnRemoveAll]").prop('disabled', !Flag);


            var gridHeader = $('#<%=gvProjectData.ClientID%>').clone(true);
                $(gridHeader).find("tr:gt(0)").remove();
                $('#<%=gvProjectData.ClientID%> tr th').each(function (i) {
                switch (i) {
                    case 0:
                        $("th:nth-child(" + (i + 1) + ")", gridHeader).css('width', "10%");
                        break;
                    case 1:
                        $("th:nth-child(" + (i + 1) + ")", gridHeader).css('width', "25%");
                        break;
                    case 2:
                        $("th:nth-child(" + (i + 1) + ")", gridHeader).css('width', "25%");
                        break;
                    case 3:
                        $("th:nth-child(" + (i + 1) + ")", gridHeader).css('width', "40%");
                        break;
                    case 1:
                        break;
                }

            });
            $("#GHead").append(gridHeader);
            $('#GHead').css('position', 'fixed');
            $('#GHead').css('width', '71%');


            function ReArrangeRows() {
                var sort = 0;
                $("#<%= gvProjectData.ClientID %> tr").each(function () {
                    var $checkBox = $(this).find("input[type='checkbox']");
                    var $hdnSortOrder = $(this).find("input[id='hdnSortOrder']");
                    if ($checkBox.is(':checked')) {
                        sort++;
                        $hdnSortOrder.val(sort);
                        //if ($textBox.val().length === 0) {
                        //    alert("You must specify the amount needed");
                        //    return false;
                        //}
                        //else {
                        //    return true;
                        //}
                    }
                });
            }

            $("#<%= gvProjectData.ClientID %> input[type='checkbox']").change(function () {
                ProjectDataTabChanged();
                ReArrangeRows();
            })

            //Sort Grid
            //$(function () {
            //    $("[id*=gvProjectData]").sortable({
            //        items: 'tr:not(tr:first-child)',
            //        cursor: 'pointer',
            //        axis: 'y',
            //        dropOnEmpty: false,
            //        start: function (e, ui) {
            //            ui.item.addClass("selected");
            //            ui.item.find("td:first-child").find("#btnSort").removeClass("nosort").addClass("sort");
            //        },
            //        stop: function (e, ui) {
            //            ui.item.removeClass("selected");
            //            ui.item.find("td:first-child").find("#btnSort").removeClass("sort").addClass("nosort");
            //            ReArrangeRows();
            //        },
            //        receive: function (e, ui) {
            //            $(this).find("tbody").append(ui.item);

            //        }
            //    });
            //});

            //Edit event handler.
            $("body").on("click", "[id*=gvProjectData] .Edit", function () {
                var row = $(this).closest("tr");
                $("td", row).each(function () {
                    if ($(this).find("input[type='text']").length > 0) {
                        $(this).find("input[type='text']").show();
                        $(this).find("span").hide();
                    }
                    $(this).find("input[type='checkbox']").hide();
                });
                row.find(".Update").show();
                row.find(".Cancel").show();
                $(this).hide();
                return false;
            });

            //Update event handler.
            $("body").on("click", "[id*=gvProjectData] .Update", function () {
                ProjectDataTabChanged();
                var row = $(this).closest("tr");
                $("td", row).each(function () {
                    if ($(this).find("input[type='text']").length > 0) {
                        var span = $(this).find("span");
                        var input = $(this).find("input[type='text']");
                        span.html(input.val());
                        span.show();
                        input.hide();
                        var hdn = $(this).find("input[type='hidden']");
                        if (hdn != null || hdn != undefined) {
                            hdn.val(input.val());
                        }
                    }
                    $(this).find("input[type='checkbox']").show();
                });
                row.find(".Edit").show();
                row.find(".Cancel").hide();
                $(this).hide();
                return false;
            });

            //Cancel event handler.
            $("body").on("click", "[id*=gvProjectData] .Cancel", function () {
                var row = $(this).closest("tr");
                $("td", row).each(function () {
                    if ($(this).find("input[type='text']").length > 0) {
                        var span = $(this).find("span");
                        var input = $(this).find("input[type='text']");
                        input.val(span.html());
                        span.show();
                        input.hide();
                    }
                    $(this).find("input[type='checkbox']").show();
                });
                row.find(".Edit").show();
                row.find(".Update").hide();
                $(this).hide();
                return false;
            });

            $('#btnAdd').click(function (e) {
                $('#list1 > option:selected').appendTo('#list2');
                e.preventDefault();
            });

            $('#btnAddAll').click(function (e) {
                $('#list1 > option').appendTo('#list2');
                e.preventDefault();
            });

            $('#btnRemove').click(function (e) {
                $('#list2 > option:selected').appendTo('#list1');
                e.preventDefault();
            });

            $('#btnRemoveAll').click(function (e) {
                $('#list2 > option').appendTo('#list1');
                e.preventDefault();
            });

            $("[id*=btnSaveDistribution]").click(function () {
                Confirm();
                debugger;
                $("[id*=list1] option").prop("selected", "selected");
                $("[id*=list2] option").prop("selected", "selected");
            });

            $("[id*=chkOptionSendToTheseVendors]").change(function () {
                var Flag = $(this).is(":Checked");
                var FlagVal = Flag == true ? 1 : 0;
                $("[id*=list1]").prop('disabled', !Flag);
                $("[id*=list2]").prop('disabled', !Flag);
                $("[id*=btnAdd]").prop('disabled', !Flag);
                $("[id*=btnAddAll]").prop('disabled', !Flag);
                $("[id*=btnRemove]").prop('disabled', !Flag);
                $("[id*=btnRemoveAll]").prop('disabled', !Flag);
                PageMethods.UpdateVendorEventNotificationOption(FlagVal, OnSuccess);
            })
            function OnSuccess(response, userContext, methodName) {
                if (response != 0) {
                    alert('An error occurred during data update.')
                }
            }


        });

    </script>


</head>

<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        <input type="hidden" id="hdnTwoPriceCampaignId" runat="server" value="" />
        <input type="hidden" id="hdnEventTabChanged" runat="server" value="False" />
        <input type="hidden" id="hdnInvitationTabChanged" runat="server" value="False" />
        <input type="hidden" id="hdnReminderTabChanged" runat="server" value="False" />
        <input type="hidden" id="hdnDistributionTabChanged" runat="server" value="False" />
        <input type="hidden" id="hdnProjectDataTabChanged" runat="server" value="False" />
        <input type="hidden" id="hdnSelectedTab" runat="server" value="Event" />

        <div class="container-fluid evnt-management">

            <h2>
                <label class="heading">Invite Builders to Committed Purchase Event</label>
                <label class="sub-heading">
                    <a href="<%=ConfigurationManager.AppSettings["CPAppHosting"].ToString()%>/admin/twoprice/campaigns/default.aspx">Return to Event Management</a> <%--devLink--%>
                </label>
            </h2>

            <div class="tab-toparea">
                <label class="sub-heading">
                    <asp:Label ID="lblNameWdMonth" runat="server" Text=""></asp:Label></label>
                <div class="tab-toparea-right">
                    <ul>
                        <li><strong>
                            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></strong></li>
                        <li>
                            <asp:Label ID="lblStartToEndDate" runat="server" Text=""></asp:Label></li>
                        <li>
                            <asp:Label ID="lblActiveStatus" runat="server" Text=""></asp:Label></li>

                    </ul>
                </div>
            </div>
            <div class="alart-area">
                <div id="MessageDiv" runat="server" visible="false">
                    <asp:Label ID="lblErrorMsg" runat="server" Text="NA"></asp:Label>
                </div>
            </div>
            <div id="dvRequiredHiddenData" style="display: none">
                <asp:Label ID="lblResponseDeadline" runat="server" Text=""></asp:Label>
                <asp:Label ID="lblMarketList" runat="server" Text=""></asp:Label>

            </div>
            <div class="management-tab">
                <nav class="tab-menu">
                    <div class="nav nav-tabs" id="nav-tab" role="tablist">
                        <a class="nav-item nav-link top-tab active" id="navEventTab" data-toggle="tab" href="#navEvent" role="tab" aria-controls="nav-home" aria-selected="true">Event</a>
                        <a class="nav-item nav-link top-tab" id="navInvitationTab" data-toggle="tab" href="#navInvitation" role="tab" aria-controls="nav-profile" aria-selected="false">Invitation</a>
                        <a class="nav-item nav-link top-tab" id="navRemindersTab" data-toggle="tab" href="#navReminders" role="tab" aria-controls="nav-contact" aria-selected="false">Reminders</a>
                        <a class="nav-item nav-link top-tab" runat="server" id="navProjectDataTab" data-toggle="tab" href="#navProjectData" role="tab" aria-controls="nav-Project" aria-selected="false">Project Data</a>
                        <a class="nav-item nav-link top-tab" runat="server" id="navDistributionTab" data-toggle="tab" href="#navDistribution" role="tab" aria-controls="nav-contact1" aria-selected="false">Distribution</a>

                    </div>
                </nav>
                <div style="float: right; border: solid 1px; padding: 10px">
                    Copy Content From:
                    <asp:DropDownList ID="ddlCopyFromTPCampaignList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCopyFromTPCampaignList_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="tab-content" id="nav-tabContent">
                    <%--Start Event--%>
                    <div class="tab-pane fade show active top-level-tab-pane" id="navEvent" role="tabpanel" aria-labelledby="nav-home-tab">
                        <div class="reset-save">
                            <ul>
                                <li>
                                    <asp:Button ID="btnSaveEvent" runat="server" Text="SAVE" CssClass="btn" OnClick="btnSaveEvent_Click" />
                                </li>
                                <li>
                                    <a class="btn" onclick="CancelButton();">CANCEL</a>
                                </li>
                            </ul>
                        </div>
                        <div class="tab-content-body">
                            <label class="margin-btm">Event Description</label>
                            <div class="auto-flex-height">
                                <textarea id="editorEventDescription" rows="10" cols="30" runat="server" onchange="EventTabChanged();"></textarea>
                            </div>
                            <div class="row date-calender-bottom">
                                <div class="col-md-2">
                                    <label style="display: block">Projection Deadline</label>
                                    <input id="dtPickerResponseDeadline" title="datepicker" runat="server" onkeydown="return false;" onchange="EventTabChanged();" />
                                </div>
                                <div class="col-md-2">
                                    <label style="display: block">Primary Contact</label>
                                    <asp:DropDownList ID="ddlPrimaryContact" runat="server" onchange="EventTabChanged();"></asp:DropDownList>
                                </div>
                                <div class="col-md-4">
                                    <label style="display: block">Also Notify</label>
                                    <div id="txtNotify" class="txtNotify">
                                        <%--<select ></select>--%>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--Start Invitation--%>
                    <div class="tab-pane fade top-level-tab-pane" id="navInvitation" role="tabpanel" aria-labelledby="nav-profile-tab">
                        <div class="reset-save">
                            <ul>
                                <li>
                                    <asp:Button ID="btnSaveInvitation" runat="server" Text="SAVE" CssClass="btn" OnClick="btnSaveInvitation_Click" />
                                </li>
                                <li>
                                    <a class="btn" onclick="CancelButton();">CANCEL</a>
                                </li>
                            </ul>
                        </div>
                        <div class="tab-content-body invitation">
                            <label class="margin-btm">Invitation Subject Line</label>
                            <div style="margin-bottom: 10px;">
                                <asp:TextBox ID="txtInvitationSubLine" runat="server" Width="100%" onchange="InvitationTabChanged();"></asp:TextBox>
                            </div>
                            <h2>
                                <label class="heading">Message Body</label>
                            </h2>
                            <div class="invitation-editor">
                                <textarea id="editorInvitationMsgBody" rows="10" cols="30" runat="server" onchange="InvitationTabChanged();"></textarea>
                                <script type="text/x-kendo-template" id="insertSymbol-template-editorInvitationMsgBody"> 
                                  <select id='editorInvitationMergeField'></select>
                                </script>
                            </div>
                            <div class="clearfix"></div>
                            <div class="row col-md-12">
                                <div class="col-md-6">
                                    <label style="display: block" class="margin-btm">Opt-In Link Text</label>
                                    <asp:TextBox ID="txtOptInLinkText" runat="server" Width="100%" onchange="InvitationTabChanged();"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label style="display: block" class="margin-btm">Opt-out Link Text</label>
                                    <asp:TextBox ID="txtOptOutLinkText" runat="server" Width="100%" onchange="InvitationTabChanged();"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--Start Reminders--%>
                    <div class="tab-pane fade top-level-tab-pane" id="navReminders" role="tabpanel" aria-labelledby="nav-contact-tab">
                        <div class="reset-save">
                            <ul>
                                <li>
                                    <asp:Button ID="btnSaveReminders" runat="server" Text="SAVE" CssClass="btn" OnClick="btnSaveReminders_Click" />
                                </li>
                                <li>
                                    <a class="btn" onclick="CancelButton();">CANCEL</a>
                                </li>
                                <li>
                                    <asp:Button ID="btnCancelationReminder" runat="server" Text="CANCEL REMINDERS" CssClass="btn reminderbtn" OnClick="btnCancelationReminder_Click" />
                                </li>

                            </ul>
                        </div>
                        <div class="tab-content-body Reminder-1">

                            <div id="dvReminder1" runat="server">
                                <label>Reminder 1</label>
                                <div class="reminder-body">
                                    <label>Send</label>

                                    <ul class="style-modify">
                                        <li>
                                            <asp:DropDownList ID="ddlReminder1DaysBefore" CssClass="width-60" runat="server" onchange="GetCanculateCalenderData(this);ReminderTabChanged();"></asp:DropDownList></li>
                                        <li><span class="reminder-text-area">days before the deadline at</span></li>
                                        <li>
                                            <input id="timepickerReminder1Time" title="timepicker" value="02:30 AM" runat="server" onchange="ReminderTabChanged();" onkeydown="return false;" />
                                            <asp:DropDownList ID="ddlReminder1TimeZone" runat="server" onchange="ReminderTabChanged();">
                                                <asp:ListItem Text="Eastern" Value="Eastern"></asp:ListItem>
                                                <asp:ListItem Text="Central" Value="Central"></asp:ListItem>
                                                <asp:ListItem Text="Mountain" Value="Mountain"></asp:ListItem>
                                                <asp:ListItem Text="Pacific" Value="Pacific"></asp:ListItem>
                                            </asp:DropDownList>
                                        </li>
                                        <li><span class="reminder-text-area">on</span></li>
                                        <li>
                                            <asp:Label ID="lblReminder1ScheduledUTC" runat="server" Text="" CssClass="reminder-text-area"></asp:Label>
                                        </li>
                                        <li class="rem-alert">
                                            <asp:Label ID="lblReminderNotScheduled" runat="server" Text="NOT SCHEDULED" CssClass="alert alert-danger" Visible="false"></asp:Label>
                                        </li>
                                    </ul>

                                    <div>
                                        <div class="management-tab">
                                            <nav class="tab-menu">
                                                <div class="nav nav-tabs" id="nav-tab-reminder1" role="tablist">
                                                    <a class="nav-item nav-link active" id="navReminder1SignUpTab" data-toggle="tab" href="#navReminder1SignUp" role="tab" aria-controls="nav-home" aria-selected="true">Sign-Up</a>
                                                    <a class="nav-item nav-link" id="navReminder1ProjectStartsTab" data-toggle="tab" href="#navReminder1ProjectStarts" role="tab" aria-controls="nav-profile" aria-selected="false">Projected Starts</a>
                                                </div>
                                            </nav>
                                            <div class="tab-content" id="nav-tabContent-reminder1">
                                                <div class="tab-pane fade active show" id="navReminder1SignUp" role="tabpanel" aria-labelledby="nav-contact1-tab">
                                                    <div class="tab-content-body">
                                                        <label>Reminder Subject Line</label>
                                                        <div>
                                                            <asp:TextBox ID="txtReminder1EnrollSubject" runat="server" Width="100%" onchange="ReminderTabChanged();"></asp:TextBox>
                                                        </div>

                                                        <label>Reminder Block Text</label>

                                                        <div class="invitation-editor">
                                                            <textarea id="editorReminder1EnrollMessage" rows="5" cols="30" runat="server" onchange="ReminderTabChanged();"></textarea>
                                                            <script type="text/x-kendo-template" id="insertSymbol-template-editorReminder1EnrollMessage"> 
                                                                <select id='editorReminder1EnrollMessageMergeField'></select>
                                                            </script>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="navReminder1ProjectStarts" role="tabpanel" aria-labelledby="nav-contact1-tab">
                                                    <div class="tab-content-body">
                                                        <label>Reminder Subject Line</label>
                                                        <div>
                                                            <asp:TextBox ID="txtReminder1ProjectsSubject" runat="server" Width="100%" onchange="ReminderTabChanged();"></asp:TextBox>
                                                        </div>

                                                        <label>Reminder Block Text</label>
                                                        <div class="invitation-editor">
                                                            <textarea id="editorReminder1ProjectsMessage" rows="5" cols="30" runat="server" onchange="ReminderTabChanged();"></textarea>
                                                            <script type="text/x-kendo-template" id="insertSymbol-template-editorReminder1ProjectsMessage"> 
                                                                 <select id='editorReminder1ProjectsMessageMergeField'></select>
                                                            </script>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="dvReminder2" runat="server">
                                <label>Reminder 2</label>
                                <div class="reminder-body">
                                    <label>Send</label>
                                    <ul class="style-modify">
                                        <li>
                                            <asp:DropDownList ID="ddlReminder2DaysBefore" CssClass="width-60" runat="server" onchange="GetCanculateCalenderData(this);ReminderTabChanged();"></asp:DropDownList></li>
                                        <li><span class="reminder-text-area">days before the deadline at</span></li>
                                        <li>
                                            <input id="timepickerReminder2Time" title="timepicker" value="02:30 AM" runat="server" onchange="ReminderTabChanged();" onkeydown="return false;" />
                                            <asp:DropDownList ID="ddlReminder2TimeZone" runat="server" onchange="ReminderTabChanged();">
                                                <asp:ListItem Text="Eastern" Value="Eastern"></asp:ListItem>
                                                <asp:ListItem Text="Central" Value="Central"></asp:ListItem>
                                                <asp:ListItem Text="Mountain" Value="Mountain"></asp:ListItem>
                                                <asp:ListItem Text="Pacific" Value="Pacific"></asp:ListItem>
                                            </asp:DropDownList>
                                        </li>
                                        <li><span class="reminder-text-area">on</span></li>
                                        <li>
                                            <asp:Label ID="lblReminder2ScheduledUTC" runat="server" Text="" CssClass="reminder-text-area"></asp:Label>
                                        </li>
                                    </ul>
                                    <div class="management-tab">
                                        <nav class="tab-menu">
                                            <div class="nav nav-tabs" id="nav-tab-reminder2" role="tablist">
                                                <a class="nav-item nav-link active" id="navReminder2SignUpTab" data-toggle="tab" href="#navReminder2SignUp" role="tab" aria-controls="nav-home" aria-selected="true">Sign-Up</a>
                                                <a class="nav-item nav-link" id="navReminder2ProjectStartsTab" data-toggle="tab" href="#navReminder2ProjectStarts" role="tab" aria-controls="nav-profile" aria-selected="false">Projected Starts</a>
                                            </div>
                                        </nav>
                                        <div class="tab-content" id="nav-tabContent-reminder2">
                                            <div class="tab-pane fade active show" id="navReminder2SignUp" role="tabpanel" aria-labelledby="nav-contact1-tab">
                                                <div class="tab-content-body">
                                                    <label>Reminder Subject Line</label>
                                                    <div>
                                                        <asp:TextBox ID="txtReminder2EnrollSubject" runat="server" Width="100%" onchange="ReminderTabChanged();"></asp:TextBox>
                                                    </div>

                                                    <label>Reminder Block Text</label>

                                                    <div class="invitation-editor">
                                                        <textarea id="editorReminder2EnrollMessage" rows="5" cols="30" runat="server" onchange="ReminderTabChanged();"></textarea>
                                                        <script type="text/x-kendo-template" id="insertSymbol-template-editorReminder2EnrollMessage"> 
                                                                <select id='editorReminder2EnrollMessageMergeField'></select>
                                                        </script>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="tab-pane fade" id="navReminder2ProjectStarts" role="tabpanel" aria-labelledby="nav-contact1-tab">
                                                <div class="tab-content-body">
                                                    <label>Reminder Subject Line</label>
                                                    <div>
                                                        <asp:TextBox ID="txtReminder2ProjectsSubject" runat="server" Width="100%" onchange="ReminderTabChanged();"></asp:TextBox>
                                                    </div>

                                                    <label>Reminder Block Text</label>
                                                    <div class="invitation-editor">
                                                        <textarea id="editorReminder2ProjectsMessage" rows="5" cols="30" runat="server" onchange="ReminderTabChanged();"></textarea>
                                                        <script type="text/x-kendo-template" id="insertSymbol-template-editorReminder2ProjectsMessage"> 
                                                                 <select id='editorReminder2ProjectsMessageMergeField'></select>
                                                        </script>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                    <%--Start Distribution--%>
                    <div class="tab-pane fade top-level-tab-pane" id="navDistribution" role="tabpanel" aria-labelledby="nav-contact1-tab">
                        <div class="reset-save">
                            <ul>
                                <li>
                                    <asp:Label ID="lblSendInfo" runat="server" Text="" class="label-text"></asp:Label>
                                </li>
                                <li>
                                    <asp:Button ID="btnSaveDistribution" runat="server" Text="SAVE" CssClass="btn" OnClick="btnSaveDistribution_Click" />
                                </li>
                                <li>
                                    <a class="btn" onclick="CancelButton();">CANCEL</a>
                                </li>
                            </ul>
                        </div>
                        <div class="tab-content-body distribution-tab">
                            <label>Send the Invitation</label>
                            <asp:RadioButtonList ID="rblInvitationSendType" runat="server" onchange="OnchangeInvitationSendType(); DistributionTabChanged();">
                                <asp:ListItem Text="Immediately" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Scheduled Date and Time" Value="2" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                            <div id="dvDistributionTimeZone">
                                <ul class="style-modify">
                                    <li>
                                        <input id="dtPickerDistribution" title="datepicker" runat="server" onkeydown="return false;" onchange=" DistributionTabChanged();" />
                                    </li>
                                    <li><span class="at">at</span></li>
                                    <li>
                                        <input id="timepickerDistributionTime" title="timepicker" value="02:30 AM" runat="server" onchange=" DistributionTabChanged();" onkeydown="return false;" />
                                        <asp:DropDownList ID="ddlDistributionTimeZone" runat="server" onchange=" DistributionTabChanged();">
                                            <asp:ListItem Text="Eastern" Value="Eastern"></asp:ListItem>
                                            <asp:ListItem Text="Central" Value="Central"></asp:ListItem>
                                            <asp:ListItem Text="Mountain" Value="Mountain"></asp:ListItem>
                                            <asp:ListItem Text="Pacific" Value="Pacific"></asp:ListItem>
                                        </asp:DropDownList>
                                    </li>
                                    <li>
                                        <div id="dvCancelSchedule" style="display: none" runat="server">
                                            <a onclick="ChangeDistibutionSchedule()" class="icon-text"><i class="fa fa-times-circle-o" aria-hidden="true" title="Cancel schedule"></i></a>
                                            <asp:HiddenField ID="hdnScheduledCanclationCheck" runat="server" />
                                        </div>
                                    </li>
                                </ul>
                            </div>
                            <div id="dvbutton">
                                <asp:HiddenField ID="hdnSelectedAdmin" runat="server" Value="" />
                                <asp:Label ID="lblDistributionStatus" runat="server" Text="NOT SCHEDULED"></asp:Label>
                                <%--<asp:Button ID="btnDistributionScheduled" runat="server" Text="SCHEDULE" CssClass="btn" />--%>
                                <div style="display: none">
                                    <a class="btn" id="btnDistributionScheduled" runat="server" onclick="ChangeDistibutionSchedule()">SCHEDULE</a>
                                </div>
                                <div id="dvdidendata" runat="server" style="display: none">
                                    <asp:Label ID="lblInvitationStatus" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="option-area">
                                <label class="option-heading">
                                    Option
                                </label>
                                <asp:CheckBox ID="chkOptionSendToNewBuilder" runat="server" Text="&nbsp; Automatically send to newly added Builders" Checked="true" onchange=" DistributionTabChanged();" />
                            </div>

                            <div class="option-area">

                                <asp:CheckBox ID="chkOptionSendToTheseVendors" runat="server" Text="&nbsp; Also send notification to these vendors" Checked="true" onchange=" DistributionTabChanged();" />
                            </div>

                            <table cellpadding="4" cellspacing="4" width="100%" align="center">
                                <tr>
                                    <td height="10px"></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:ListBox ID="list1" runat="server" Width="350px" Height="200px" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                    <td align="center">
                                        <input type="button" id="btnAdd" class="active" value=">" style="width: 50px; margin: 3px;" /><br />
                                        <input type="button" id="btnAddAll" class="active" value=">>" style="width: 50px; margin: 3px;" /><br />
                                        <input type="button" id="btnRemove" class="active" value="<" style="width: 50px; margin: 3px;" /><br />
                                        <input type="button" id="btnRemoveAll" class="active" value="<<" style="width: 50px; margin: 3px;" />
                                    </td>
                                    <td align="center">
                                        <asp:ListBox ID="list2" runat="server" Width="350px" Height="200px" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="10px">
                                        <asp:Literal ID="ltrlList1" runat="server"></asp:Literal>
                                        <br />
                                        <asp:Literal ID="ltrlList2" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <%--Start Project Data Section--%>
                    <div class="tab-pane fade top-level-tab-pane" id="navProjectData" role="tabpanel" aria-labelledby="nav-Project">
                        <div class="reset-save">
                            <ul>
                                <li>
                                    <asp:Button ID="btnSaveProjectData" runat="server" Text="SAVE" OnClick="btnSaveProjectData_Click" CssClass="btn" />
                                </li>
                                <li>
                                    <a class="btn" onclick="CancelButton();">CANCEL</a>
                                </li>
                            </ul>
                        </div>
                        <div class="tab-content-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <fieldset id="fieldset">
                                        <legend>Participation Options</legend>
                                        <asp:CheckBoxList runat="server" ID="chkParticipationOption" CellPadding="5" CellSpacing="5" RepeatDirection="Vertical" RepeatColumns="1">
                                            <asp:ListItem Value="0" Text="Yes, With specific Projects"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Yes, if projects arise"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="No"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </fieldset>

                                </div>
                                <div class="col-md-9">
                                    <div id="GHead" style="background-color: #fff;"></div>
                                    <div style="height: 400px; overflow: auto">
                                        <asp:GridView ID="gvProjectData" runat="server" AutoGenerateColumns="false" GridLines="None">
                                            <Columns>

                                                <asp:TemplateField HeaderText="&nbsp;" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <%-- <asp:ImageButton ID="btnSort" ImageUrl="Content/images/sort.jpg" AlternateText="Sort" runat="server" CssClass="nosort" />--%>
                                                        &nbsp;<asp:LinkButton ToolTip="Edit" runat="server" CssClass="Edit"><i class="fa fa-pencil-square fa-lg" aria-hidden="true"></i></asp:LinkButton>
                                                        <asp:LinkButton ToolTip="Update" runat="server" CssClass="Update" Style="display: none"><i class="fa fa-floppy-o  fa-lg" aria-hidden="true"></i></asp:LinkButton>
                                                        &nbsp;<asp:LinkButton ToolTip="Cancel" runat="server" CssClass="Cancel" Style="display: none"><i class="fa fa-times  fa-lg" aria-hidden="true"></i></asp:LinkButton>
                                                        <asp:CheckBox runat="server" ID="chkId" Checked='<%# Convert.ToInt32( Eval("TwoPriceCampaignId"))>0 %>' />
                                                    </ItemTemplate>

                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Question" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="25%">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnQuestionId" Value='<%# Eval("QuestionId") %>' runat="server" />
                                                        <asp:HiddenField ID="hdnQuestion" Value='<%# Eval("Question") %>' runat="server" />
                                                        <asp:Label ID="lblQuestion" Text='<%# Eval("Question") %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Label" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="25%">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnQuestionLabel" Value='<%# Eval("QuestionLabel") %>' runat="server" />
                                                        <asp:Label ID="lblQuestionLabel" Text='<%# Eval("QuestionLabel") %>' runat="server" />
                                                        <asp:TextBox Text='<%# Eval("QuestionLabel") %>' runat="server" Style="width: 95%; display: none" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Hint Text" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40%">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnSortOrder" Value='<%# Eval("SortOrder") %>' runat="server" />
                                                        <asp:HiddenField ID="hdnHintText" Value='<%# Eval("HintText") %>' runat="server" />
                                                        <asp:Label ID="lblHintText" Text='<%# Eval("HintText") %>' runat="server" />
                                                        <asp:TextBox Text='<%# Eval("HintText") %>' runat="server" Style="width: 95%; display: none" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Required" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="2%">
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="chkRequired" Checked='<%# Convert.ToBoolean( Eval("IsRequired")) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </div>
                            </div>

                        </div>
                    </div>


                    <%--End Project Data Section--%>
                </div>
            </div>
        </div>



    </form>

</body>



</html>

<style>
    #fieldset {
        border: solid #808080 1px;
    }

        #fieldset legend {
            width: auto;
            font-size: 13px;
            font-weight: bold;
            margin-left: 6px;
        }

    #chkParticipationOption label {
        margin-left: 5px;
    }

    #gvProjectData {
        width: 100%;
        border: 0px;
    }

        #gvProjectData tbody th {
            border-bottom: 1px solid #808080;
            margin-left: 20px;
        }

        #gvProjectData td, #gvProjectData th {
            padding: 6px;
        }

        #gvProjectData tr:hover {
            background-color: #f3f3f3;
            cursor: pointer;
        }

    .sort {
        display: inline-block;
    }

    .nosort {
        display: none;
    }
</style>
