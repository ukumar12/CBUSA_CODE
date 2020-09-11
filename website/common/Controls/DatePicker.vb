Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web
Imports System.Text

Namespace Controls

    ''' <summary>
    ''' Provides a popup control functionality for selecting a date via a calendar mechanism.
    ''' </summary>
    ''' <remarks>The <see cref="DatePicker.Value" /> property returns a <see cref="DateTime" /> reference, and the 
    ''' <see cref="DatePicker.Text" /> property returns the date in text form.
    ''' <seealso cref="RequiredDateValidator" />
    ''' <seealso cref="RequiredDateValidatorFront" />
    ''' <seealso cref="DateValidator" />
    ''' <seealso cref="DateValidatorFront" /></remarks>
    Public Class DatePicker
        Inherits Label
        Implements INamingContainer
        Implements IPostBackDataHandler

#Region "Properties"

        Private m_DateType As String = "mm/dd/yyyy"

        ''' <summary>
        ''' Gets or sets the value of the <see cref="DatePicker" /> control as a <see cref="String" /> object.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> that represents the value of the control.</value>
        ''' <remarks>This property implicitly converts the <see cref="String" /> that it represents into
        ''' a <see cref="String" /> for the <see cref="Text" /> property.</remarks>
        Public Property Value() As DateTime
            Get
                If Text = String.Empty Then Return Nothing
                Return Text
            End Get
            Set(ByVal value As DateTime)
                If value = Nothing Then
                    Text = String.Empty
                Else
                    Text = value
                End If
            End Set
        End Property

        Public Property onChangeMonthYear() As Boolean
            Get
                If ViewState("onChangeMonthYear") Is Nothing Then ViewState("onChangeMonthYear") = False
                Return ViewState("onChangeMonthYear")
            End Get
            Set(ByVal value As Boolean)
                
                ViewState("onChangeMonthYear") = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the <see cref="String" /> which represents the date currently selected by the control.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the current value of the control.</value>
        ''' <remarks>If the child controls have not been created yet, this property is stored in 
        ''' <see cref="ViewState" />.  If they have, conversely, it becomes the <see cref="TextBox.Text" />
        ''' property of the internal <see cref="TextBox" /> which contains the currently selected date.</remarks>
        Public Overrides Property Text() As String
            Get
                If (Me.Controls.Count = 0) Then
                    If ViewState("Text") <> Nothing Then
                        Return ViewState("Text")
                    Else
                        Return ""
                    End If
                End If
                Return CType(FindControl("txtDatePicker"), System.Web.UI.WebControls.TextBox).Text
            End Get
            Set(ByVal Value As String)
                If Value = "12:00:00 AM" Then Value = String.Empty

                If Not (Me.Controls.Count = 0) Then
                    CType(FindControl("txtDatePicker"), System.Web.UI.WebControls.TextBox).Text = Value
                    ViewState("Text") = ""
                Else
                    ViewState("Text") = Value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the CSS class of the <see cref="TextBox" /> which contains the currently selected date.
        ''' </summary>
        ''' <value>A <see cref="String" /> which references a CSS class for the <see cref="TextBox" /> which
        ''' contains the currently selected date.</value>
        ''' <remarks>If the child controls have not been created yet, this property is stored in 
        ''' <see cref="ViewState" />.  If they have, conversely, it becomes the <see cref="TextBox.CssClass" />
        ''' property of the internal <see cref="TextBox" /> which contains the currently selected date.</remarks>
        Public Overrides Property CssClass() As String
            Get
                If (Me.Controls.Count = 0) Then
                    Return ""
                End If
                Return CType(FindControl("txtDatePicker"), System.Web.UI.WebControls.TextBox).Attributes("class")
            End Get
            Set(ByVal Value As String)
                If Not (Me.Controls.Count = 0) Then
                    CType(FindControl("txtDatePicker"), System.Web.UI.WebControls.TextBox).Attributes("class") = Value & " datepicker-control"
                    ViewState("CSS") = ""
                Else
                    ViewState("CSS") = Value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the format string for the control's <see cref="Text" /> property.
        ''' </summary>
        ''' <value>A format string which tells the control how to parse its text input and render its text
        ''' output.</value>
        ''' <remarks><para>In parsing the date, it splits the pieces of the format string by the characters 
        ''' " ", "/", ".", and "-".  At this point there should be three tokens.  Then it looks through the 
        ''' pieces and determines what they represent.  "m" and "mm" are a numeric month. "d" and "dd" are a
        ''' numeric day.  "yyyy" is the year.  "mmm" is the name of the month.</para>
        ''' <para>In rendering the date, "m" represents a numeric month, "mm" represents a 0-padded numeric
        ''' month, "d" represents a numeric day, "dd" represents a 0-padded numeric day, "mmm" represents the
        ''' name of the month, and "yyyy" represents the year.</para></remarks>
        Public Property DateType() As String
            Get
                Return m_DateType
            End Get
            Set(ByVal Value As String)
                m_DateType = Value
            End Set
        End Property

#End Region

        ''' <summary>
        ''' Constructs the HTML of the control from the various properties which control the appearance of 
        ''' the control.
        ''' </summary>
        ''' <remarks>It may be interesting to note that of all of the images that you can affect about this
        ''' control, you cannot change the URL of the image that you click on in order to pop up the calendar.
        ''' It will remain always at "/images/calendar/picker.gif".</remarks>
        Protected Overrides Sub CreateChildControls()
            Dim txtDatePicker As New System.Web.UI.WebControls.TextBox()
            txtDatePicker.ID = "txtDatePicker"
            txtDatePicker.Style.Add("width", "80px")

            If Not (ViewState("Text") = "") Then txtDatePicker.Text = ViewState("Text")

            txtDatePicker.Attributes("class") = ViewState("CSS") & " datepicker-control"

            Me.Controls.Add(txtDatePicker)
        End Sub

        ''' <summary>
        ''' Registers JavaScript and Postback functionality, then triggers the Init event.
        ''' </summary>
        ''' <param name="e">An <see cref="EventArgs" /> object that contains the event data.</param>
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Dim sScript As String = String.Empty

            If onChangeMonthYear Then
                sScript = "$('input.datepicker-control').datepicker({changeMonth: true, changeYear: true, showButtonPanel: true, showOn: 'both', buttonImage: '/images/calendar/picker.gif',onChangeMonthYear: function (year, month, datePicker) {var $t = $(this); var day = $t.data('preferred-day') || new Date().getDate(); var newDate = new Date(month + '/' + day + '/' + year); while (day > 28) { if (newDate.getDate() == day && newDate.getMonth() + 1 == month && newDate.getFullYear() == year) { break;} else {day -= 1; newDate = new Date(month + '/' + day + '/' + year);}} $t.datepicker('setDate', newDate);},beforeShow: function (dateText, datePicker) {$(this).data('preferred-day', ($(this).datepicker('getDate') || new Date()).getDate());}, buttonImageOnly: true, onSelect: function() { CalcDate(); }});"
            Else
                sScript = "$('input.datepicker-control').datepicker({changeMonth: true, changeYear: true, showButtonPanel: true, showOn: 'both', buttonImage: '/images/calendar/picker.gif', buttonImageOnly: true, onSelect: function() { }});"
            End If

           
            Dim jQueryUITodayButtonHack As String = "var _gotoToday = $.datepicker._gotoToday; $.datepicker._gotoToday = function(id) { $(id).val(new Date().format(""MM/dd/yyyy"")); this._hideDatepicker(); };"

            ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType, "DatePicker", sScript, True)
            ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType, "DatePickerJQueryHack", jQueryUITodayButtonHack, True)
            Page.RegisterRequiresPostBack(Me)
            MyBase.OnInit(e)
        End Sub

        ''' <summary>
        ''' Persists the <see cref="TextBox.Text" /> property of the internal <see cref="TextBox" /> into the
        ''' <see cref="Text" /> property of the control.
        ''' </summary>
        ''' <param name="postDataKey">Not used by this implementation.</param>
        ''' <param name="postCollection">Not used by this implementation.</param>
        ''' <returns><see langword="False" />.</returns>
        ''' <remarks>This method completely ignores the values of <paramref name="postDataKey" /> and
        ''' <paramref name="postCollection" /> in its implementation.</remarks>
        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Text = CType(FindControl("txtDatePicker"), System.Web.UI.WebControls.TextBox).Text
            Return False
        End Function

        ''' <summary>
        ''' This method is required by the <see cref="IPostBackDataHandler" /> interface.  It currently does
        ''' nothing.
        ''' </summary>
        ''' <remarks>This method is not used by this implementation of this interface.</remarks>
        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

    End Class
End Namespace