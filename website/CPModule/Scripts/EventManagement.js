$(document).ready(function () {
    BindControls();
    BindMultiselect();
    ViewSelectedAdditionalContact();
    $("#hdnScheduledCanclationCheck").val('');

    $("#hdnEventTabChanged").val("False");
    $("#hdnInvitationTabChanged").val("False");
    $("#hdnReminderTabChanged").val("False");
    $("#hdnProjectDataTabChanged").val("False");    
    $("#hdnDistributionTabChanged").val("False");

    ///////////////// Tab Fixed
    var tab = $("#hdnSelectedTab").val();

    $("a.top-tab").removeClass("active");
    $("div.top-level-tab-pane").removeClass("active");
    $("div.top-level-tab-pane").removeClass("show");

    if (tab == "Invitation") {
        $("#navInvitationTab").addClass("active");
        $("#navInvitation").addClass("active");
        $("#navInvitation").addClass("show");
    } else {
        if (tab == "Reminder") {
            $("#navRemindersTab").addClass("active");
            $("#navReminders").addClass("active");
            $("#navReminders").addClass("show");
        } else {
            if (tab == "ProjectData") {
                $("#navProjectDataTab").addClass("active");
                $("#navProjectData").addClass("active");
                $("#navProjectData").addClass("show");
            }
            else {
                if (tab == "Distribution") {
                    $("#navDistributionTab").addClass("active");
                    $("#navDistribution").addClass("active");
                    $("#navDistribution").addClass("show");
                }
                else {
                    $("#navEventTab").addClass("active");
                    $("#navEvent").addClass("active");
                    $("#navEvent").addClass("show");
                }
            }
        }
    }
    /// Disable all Button
    function DisableButtons() {
        var inputs = document.getElementsByTagName("INPUT");
        for (var i in inputs) {
            if (inputs[i].type == "button" || inputs[i].type == "submit") {
                inputs[i].disabled = true;
            }
        }
    }
    window.onbeforeunload = DisableButtons;

});


function ViewSelectedAdditionalContact() {
    //alert($('#hdnSelectedAdmin').val());
    var EmailList = $('#hdnSelectedAdmin').val().split(',');
    var Emails = [];
    for (var i = 0; i < EmailList.length; i++) {
        Emails.push(EmailList[i]);
    }
    $("#txtNotify").data("kendoMultiSelect").value(Emails);
}
function BindMultiselect() {
    $("#txtNotify").kendoMultiSelect({
        placeholder: "",
        dataTextField: "FirstName",
        dataValueField: "Email",
        autoBind: false,
        autoClose: false,
        dataSource: {
            type: "json",
            serverFiltering: false,
            transport: {
                read: {
                    url: "./BindPrimaryContactMultiselect.ashx"
                    //success: function (data) {
                    //    console.log(data);
                    //}
                }
            }
        },
        change: onChange
        //value: [
        //    { ProductName: "Chang", ProductID: 2 },
        //    { ProductName: "Uncle Bob's Organic Dried Pears", ProductID: 7 }
        //]
    });
}
function onChange() {
    var multiselect = $("#txtNotify").data("kendoMultiSelect");
    var dataItem = multiselect.dataItems();
    console.log(dataItem);
    var EmailList = '';
    for (var i = 0; i < dataItem.length; i++) {
        EmailList += dataItem[i].Email + ',';
    }
    
    $('#hdnSelectedAdmin').val(EmailList);
    console.log($('#hdnSelectedAdmin').val());
}








function EventTabChanged() {
    $("#hdnEventTabChanged").val("True");
}
function InvitationTabChanged() {
    $("#hdnInvitationTabChanged").val("True");
}
function ReminderTabChanged() {
    $("#hdnReminderTabChanged").val("True");
}
function DistributionTabChanged() {
    $("#hdnDistributionTabChanged").val("True");
}

function ProjectDataTabChanged() {
    $("#hdnProjectDataTabChanged").val("True");
}
////Page Load Funtion
function BindControls() {
    //Local Variable
    var today = new Date();
    var PlaceholderData = [
        { ID: "0", Name: "Insert Merge Field" },
        { ID: "{{%EventTitle%}}", Name: "EventTitle" },
        { ID: "{{%EventStart%}}", Name: "EventStart" },
        { ID: "{{%EventEnd%}}", Name: "EventEnd" },
        { ID: "{{%EventDescription%}}", Name: "EventDescription" },
        { ID: "{{%ResponseDeadline%}}", Name: "ResponseDeadline" },
        { ID: "{{%ResponseDeadlineFull%}}", Name: "ResponseDeadlineFull" },
        { ID: "{{%ResponseDeadlineDay%}}", Name: "ResponseDeadlineDay" },
        { ID: "{{%ResponseDeadlineFullDay%}}", Name: "ResponseDeadlineFullDay" },
        { ID: "{{%BuilderName%}}", Name: "BuilderName" },
        { ID: "{{%RecipientFirstName%}}", Name: "RecipientFirstName" },
        { ID: "{{%RecipientLastName%}}", Name: "RecipientLastName" },
        { ID: "{{%RecipientFullName%}}", Name: "RecipientFullName" },
        { ID: "{{%ContactName%}}", Name: "ContactName" },
        { ID: "{{%ContactEmail%}}", Name: "ContactE-Mail" },
        { ID: "{{%ContactPhone%}}", Name: "ContactPhone" },
        { ID: "{{%OptIn%}}", Name: "OptIn" },
        { ID: "{{%OptOut%}}", Name: "OptOut" },
        { ID: "{{%ReminderBlock%}}", Name: "ReminderBlock" }
    ];

    setTimeout(function () { $("#MessageDiv").hide(); }, 10000);


    //********************************Event Tab
    $("#editorEventDescription").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            // "fontSize",
        ]
    });
    $("#dtPickerResponseDeadline").kendoDatePicker(
        {
            min: new Date(),
        });
    //********************************Invitation Tab
    $("#editorInvitationMsgBody").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            //"fontSize",
            {
                template: $("#insertSymbol-template-editorInvitationMsgBody").html()
            }
        ]
    });
    $("#editorInvitationMergeField").kendoDropDownList({
        //filter: 'contains',
        dataSource: PlaceholderData,
        dataTextField: "Name",
        dataValueField: "ID",
        index: 0,
        change: function (e) {
            //var inputValue = e.sender._inputValue();
            var dataItem = e.sender.dataItem();
            // alert(dataItem.id);
            // alert(dataItem.name);
            if (dataItem.ID != '0') {

                $('#editorInvitationMsgBody').data('kendoEditor').exec("insertHtml", { html: dataItem.ID });
            }
        }

    });

    //********************************Reminders Tab
    //Reminder 1
    $("#timepickerReminder1Time").kendoTimePicker({
        //dateInput: true,
        interval: 15,
    });
    $("#editorReminder1EnrollMessage").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            //"fontSize",
            {
                template: $("#insertSymbol-template-editorReminder1EnrollMessage").html()
            }
        ]
    });
    $("#editorReminder1EnrollMessageMergeField").kendoDropDownList({
        //filter: 'contains',
        dataSource: PlaceholderData,
        dataTextField: "Name",
        dataValueField: "ID",
        index: 0,
        change: function (e) {
            //var inputValue = e.sender._inputValue();
            var dataItem = e.sender.dataItem();
            // alert(dataItem.id);
            // alert(dataItem.name);
            if (dataItem.ID != '0') {

                $('#editorReminder1EnrollMessage').data('kendoEditor').exec("insertHtml", { html: dataItem.ID });
            }
        }

    });

    $("#editorReminder1ProjectsMessage").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            //"fontSize",
            {
                template: $("#insertSymbol-template-editorReminder1ProjectsMessage").html()
            }
        ]
    });
    $("#editorReminder1ProjectsMessageMergeField").kendoDropDownList({
        //filter: 'contains',
        dataSource: PlaceholderData,
        dataTextField: "Name",
        dataValueField: "ID",
        index: 0,
        change: function (e) {
            //var inputValue = e.sender._inputValue();
            var dataItem = e.sender.dataItem();
            // alert(dataItem.id);
            // alert(dataItem.name);
            if (dataItem.ID != '0') {

                $('#editorReminder1ProjectsMessage').data('kendoEditor').exec("insertHtml", { html: dataItem.ID });
            }
        }

    });

    //Reminder 2
    $("#timepickerReminder2Time").kendoTimePicker({
        // dateInput: true,
        interval: 15
    });

    $("#editorReminder2EnrollMessage").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            //"fontSize",
            {
                template: $("#insertSymbol-template-editorReminder2EnrollMessage").html()
            }
        ]
    });
    $("#editorReminder2EnrollMessageMergeField").kendoDropDownList({
        //filter: 'contains',
        dataSource: PlaceholderData,
        dataTextField: "Name",
        dataValueField: "ID",
        index: 0,
        change: function (e) {
            //var inputValue = e.sender._inputValue();
            var dataItem = e.sender.dataItem();
            // alert(dataItem.id);
            // alert(dataItem.name);
            if (dataItem.ID != '0') {

                $('#editorReminder2EnrollMessage').data('kendoEditor').exec("insertHtml", { html: dataItem.ID });
            }
        }

    });

    $("#editorReminder2ProjectsMessage").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",
            "strikethrough",
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",
            "indent",
            "outdent",
            "createLink",
            "unlink",
            "viewHtml",
            "formatting",
            //"fontName",
            //"fontSize",
            {
                template: $("#insertSymbol-template-editorReminder2ProjectsMessage").html()
            }
        ]
    });
    $("#editorReminder2ProjectsMessageMergeField").kendoDropDownList({
        //filter: 'contains',
        dataSource: PlaceholderData,
        dataTextField: "Name",
        dataValueField: "ID",
        index: 0,
        change: function (e) {
            //var inputValue = e.sender._inputValue();
            var dataItem = e.sender.dataItem();
            // alert(dataItem.id);
            // alert(dataItem.name);
            if (dataItem.ID != '0') {

                $('#editorReminder2ProjectsMessage').data('kendoEditor').exec("insertHtml", { html: dataItem.ID });
            }
        }

    });


    //********************************Distribution Tab
    $("#dtPickerDistribution").kendoDatePicker(
        {
            min: new Date(),
        });
    $("#timepickerDistributionTime").kendoTimePicker({
        //dateInput: true,
        interval: 15
    });

    //For Click Event Set to All Tab Control

    $("#navEventTab").on('click', function (e) {
        if ($("#hdnInvitationTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnReminderTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnProjectDataTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnDistributionTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
             
        else {
            $("#hdnSelectedTab").val("Event");
            $("#MessageDiv").hide();
        }
    });

    $("#navInvitationTab").on('click', function (e) {
        if ($("#hdnEventTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }

        else if ($("#hdnReminderTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnProjectDataTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnDistributionTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
      
        else {
            $("#hdnSelectedTab").val("Invitation");
            $("#MessageDiv").hide();
        }

    });

    $("#navRemindersTab").on('click', function (e) {
        if ($("#hdnEventTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnInvitationTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnProjectDataTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnDistributionTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
    
        else {
            $("#hdnSelectedTab").val("Reminder");
            $("#MessageDiv").hide();
        }
    });

    $("#navDistributionTab").on('click', function (e) {
        if ($("#hdnEventTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnInvitationTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnReminderTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnProjectDataTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnDistributionTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
     

        else {
            $("#hdnSelectedTab").val("Distribution");
            $("#MessageDiv").hide();
        }
    });

    $("#navProjectDataTab").on('click', function (e) {
        if ($("#hdnEventTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnInvitationTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnReminderTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnDistributionTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }
        else if ($("#hdnProjectDataTabChanged").val() == "True") {

            alert("You have unsaved data in this tab. Please Save or Cancel the changes before switching tabs.");

            e.preventDefault();
            return false;
        }

        else {
            $("#hdnSelectedTab").val("ProjectData");
            $("#MessageDiv").hide();
        }
    });
}

////Call Funtion
function CancelButton() {
    location.reload();
}
Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}
function GetCanculateCalenderData(element) {
    debugger;
    var elementID = element.id;
    //alert(elementID)
    var ResponseDeadline = $("#lblResponseDeadline").text();
    //alert(ResponseDeadline);
    if (ResponseDeadline != '') {

        var lblResponseDeadline = new Date($("#lblResponseDeadline").text());
        //alert(lblResponseDeadline)
        var ddlValue = parseInt($('#' + elementID).val().trim());
        //alert(ddlValue)

        var CalDate = lblResponseDeadline.addDays(-ddlValue);

        //var CalDate = new Date();
        //var test = lblResponseDeadline.getDate();
        //CalDate.setDate(lblResponseDeadline.getDate() - ddlValue);

        //alert(CalDate);
        var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        var dayName = days[CalDate.getDay()];
        //alert(dayName);
        var monthNames = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];
        var monthName = monthNames[CalDate.getMonth()]
        //alert(monthName);
        var FinalResult = dayName + ',' + monthName + '  ' + CalDate.getDate()
        //alert(FinalResult);

        if (elementID == "ddlReminder1DaysBefore") {
            $("#lblReminder1ScheduledUTC").text(FinalResult);
        }
        else if (elementID == "ddlReminder2DaysBefore") {
            $("#lblReminder2ScheduledUTC").text(FinalResult);
        }

    }
    else {
        //alert('else');
        if (elementID == "ddlReminder1DaysBefore") {
            $("#lblReminder1ScheduledUTC").text('');
        }
        else if (elementID == "ddlReminder2DaysBefore") {
            $("#lblReminder2ScheduledUTC").text('');
        }
    }

}
function OnchangeInvitationSendType() {
    var InvitationSendType = $("input[name='rblInvitationSendType']:checked").val();
    if (InvitationSendType == '1') {

        $('#dtPickerDistribution').data('kendoDatePicker').enable(false);
        $('#timepickerDistributionTime').data('kendoTimePicker').enable(false);
        $("#ddlDistributionTimeZone").prop('disabled', true);

        $("#dvCancelSchedule").hide();

        var InvitationStatus = $("#lblInvitationStatus").text();
        // alert('InvitationSendType=' + InvitationSendType + '  ' + 'InvitationStatus=' + InvitationStatus); //****Check

        if (InvitationStatus != "" && InvitationStatus == "1") {

            $("#btnDistributionScheduled").text('RESEND');
            $("#lblDistributionStatus").text('SENT');
            $("#lblDistributionStatus").addClass('alert alert-success');

        }
        else if (InvitationStatus == '') {

            $("#btnDistributionScheduled").text('SEND');
            $("#lblDistributionStatus").text("");
            $("#lblDistributionStatus").removeClass();
        }
    }
    else if (InvitationSendType == '2') {
        $('#dtPickerDistribution').data('kendoDatePicker').enable(true);
        $('#timepickerDistributionTime').data('kendoTimePicker').enable(true);
        $("#ddlDistributionTimeZone").prop('disabled', false);

        var InvitationStatus = $("#lblInvitationStatus").text();
        //alert('InvitationSendType=' + InvitationSendType + '  ' + 'InvitationStatus=' + InvitationStatus); //****Check

        if (InvitationStatus != "" && InvitationStatus == "2") {
            $("#btnDistributionScheduled").text('CANCEL');
            $("#lblDistributionStatus").text('SCHEDULED');
            $("#lblDistributionStatus").addClass('alert alert-info');

            $('#dtPickerDistribution').data('kendoDatePicker').enable(false);
            $('#timepickerDistributionTime').data('kendoTimePicker').enable(false);
            $("#ddlDistributionTimeZone").prop('disabled', true);

            $("#dvCancelSchedule").show();
        }
        else if (InvitationStatus == '') {
            $("#btnDistributionScheduled").text('SCHEDULE');
            $("#lblDistributionStatus").text("NOT SCHEDULED");
            $("#lblDistributionStatus").addClass('alert alert-danger');

            $("#dvCancelSchedule").hide();

        }
    }
}
function ChangeDistibutionSchedule() {
    var ButtonText = $("#btnDistributionScheduled").text();
    //alert(ButtonText);
    if (ButtonText == "CANCEL") {
        if (confirm('Are you sure you want to cancel sending the invitation? You will need to do this if you want to cancel the schedule completely.')) {
            $("#btnDistributionScheduled").text('CHANGE TO SCHEDULE');
            $("#lblDistributionStatus").text("NOT SCHEDULED");
            $("#lblDistributionStatus").addClass('alert alert-danger');

            $('#dtPickerDistribution').data('kendoDatePicker').enable(true);
            $('#timepickerDistributionTime').data('kendoTimePicker').enable(true);
            $("#ddlDistributionTimeZone").prop('disabled', false);

            //("#rblInvitationSendType").prop('disabled', false);
            //$("input[type=radio]").attr('disabled', false); 
            //$("#<%=rblInvitationSendType.ClientID %>").find('input').prop('disabled', 'false');

            $("#hdnScheduledCanclationCheck").val('CancledScheduled');
            //alert($("#hdnScheduledCanclationCheck").val());

            $('#rblInvitationSendType').find('*').each(function () {
                $(this).attr("disabled", false);
            });


        }
    }
}
function EnableDisableDateTimePicker(Status) {
    //1 -Enable,2 -Disable
    //alert('EnableDisableDateTimePicker=' + Status);
    if (Status == "1") {
        $('#dtPickerDistribution').data('kendoDatePicker').enable(true);
        $('#timepickerDistributionTime').data('kendoTimePicker').enable(true);

    }
    else if (Status == "2") {
        //alert('2');
        $('#dtPickerDistribution').data('kendoDatePicker').enable(false);
        $('#timepickerDistributionTime').data('kendoTimePicker').enable(false);
    }

}

function Confirm() {
    var confirm_value = document.createElement("INPUT");
    confirm_value.type = "hidden";
    confirm_value.name = "confirm_value";

    var InvitationSendType = $("input[name='rblInvitationSendType']:checked").val();
    var SendStatus = $('#lblSendInfo').text();
    var marketList = $("#lblMarketList").text();
    var alertSmg = "";
    var FixedSmg = "Are you sure want to send the invitation to all builders in the " + marketList + " market? ";
    if (SendStatus != '') {
        alertSmg = "This invitation was sent previously. " + FixedSmg + "You might do this if you need to provide updated information. ";
    }
    else {
        alertSmg = FixedSmg + "Once the message is sent, it's sent for good. ";
    }
    if (InvitationSendType == '1') {
        if (confirm(alertSmg)) {
            confirm_value.value = "Yes";
        } else {
            confirm_value.value = "No";
        }
    }
    else {
        confirm_value.value = "Yes";
    }

    document.forms[0].appendChild(confirm_value);

}



