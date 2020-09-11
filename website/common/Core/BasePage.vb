Imports Microsoft.VisualBasic
Imports Controls
Imports System.Web
Imports System.Web.UI
Imports System.Collections.Specialized
Imports System.Configuration.ConfigurationManager
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Web.UI.HtmlControls

Namespace Components

    Public Class BasePage
        Inherits System.Web.UI.Page

        Private Const DefaultViewStateTimeout As Integer = 120

        Private m_DB As Database
        Private m_ErrHandler As ErrorHandler

        Private m_ViewStateConnectionString As String
        Private m_ViewStateTimeout As TimeSpan
        Private m_ConnectionString As String

        Protected SQL As String

        Protected Sub RefreshValueOnPostback(ByVal ctrl As Control)
            If IsPostBack Then
                Dim hnd As IPostBackDataHandler = CType(ctrl, IPostBackDataHandler)
                hnd.LoadPostData(ctrl.UniqueID, Request.Form)
            End If
        End Sub

        Protected Sub EnsureSSL()
            Dim SecureURL As String = System.Configuration.ConfigurationManager.AppSettings("GlobalSecureName")
            If Not SecureURL = String.Empty Then
                If Not Request.IsSecureConnection Then
                    SecureURL = SecureURL & Request.Url.PathAndQuery
                    Response.Redirect(SecureURL)
                End If
            End If
        End Sub

        Public ReadOnly Property DB() As Database
            Get
                If m_DB Is Nothing Then
                    'open database connection
                    m_DB = New Database
                    m_DB.Open(m_ConnectionString)
                End If
                Return m_DB
            End Get
        End Property

        Public ReadOnly Property ErrHandler() As ErrorHandler
            Get
                If m_ErrHandler Is Nothing Then m_ErrHandler = New ErrorHandler(DB)
                Return m_ErrHandler
            End Get
        End Property

        Public Sub New()
        End Sub

        Public ReadOnly Property GetPageParams() As String
            Get
                Dim Params As New FilterFields
                Return Params.ToString(Components.FilterFieldType.All, "", ViewState)
            End Get
        End Property

        Public ReadOnly Property GetPageParams(ByVal preserve As FilterFieldType) As String
            Get
                Dim Params As New FilterFields
                Return Params.ToString(preserve, "", ViewState)
            End Get
        End Property

        Public ReadOnly Property GetPageParams(ByVal preserve As FilterFieldType, ByVal removeList As String) As String
            Get
                Dim Params As New FilterFields
                Return Params.ToString(preserve, removeList, ViewState)
            End Get
        End Property

        Private Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
            m_ConnectionString = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))

            'If viewstate is saved in the database
            If ViewStateInSQL Then
                m_ViewStateConnectionString = DBConnectionString.GetConnectionString(AppSettings("ViewStateConnectionString"), AppSettings("ViewStateConnectionStringUsername"), AppSettings("ViewStateConnectionStringPassword"))
                Try
                    m_ViewStateTimeout = TimeSpan.FromMinutes(Convert.ToDouble(AppSettings("ViewStateTimeout")))
                Catch
                    m_ViewStateTimeout = TimeSpan.FromMinutes(DefaultViewStateTimeout)
                End Try
            End If
        End Sub

        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
            If Not m_DB Is Nothing Then
                If Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                m_DB.Dispose()
                m_DB = Nothing
            End If
        End Sub

        Public Sub AddError(ByVal ErrorMessage As String)
            'Find Error Placeholder and update with error (only for front end)
            Dim ph As MasterPages.ErrorMessage = ErrorPlaceHolder
            If Not ph Is Nothing Then ph.AddError(ErrorMessage)
        End Sub

        Public Sub ClearErrors()
            Dim ph As MasterPages.ErrorMessage = ErrorPlaceHolder
            If Not ph Is Nothing Then ph.ClearErrors()
        End Sub

        Public ReadOnly Property ErrorPlaceHolder() As MasterPages.ErrorMessage
            Get
                Dim eph As MasterPages.ErrorMessage = FindControl("ErrorPlaceHolder")

                'If this is admin section, then find the ErrorPlaceHolder in the master page
                If eph Is Nothing AndAlso Not Master Is Nothing Then
                    eph = Master.FindControl("ErrorPlaceHolder")
                End If
                Return eph
            End Get
        End Property

        Public Overrides Sub Validate()
            MyBase.Validate()

            Dim ph As MasterPages.ErrorMessage = ErrorPlaceHolder
            If Not ph Is Nothing Then ph.UpdateSummary()
        End Sub

        Public Overrides Sub Validate(ByVal validationGroup As String)
            MyBase.Validate(validationGroup)
            Dim ph As MasterPages.ErrorMessage = ErrorPlaceHolder
            If Not ph Is Nothing Then ph.UpdateSummary(validationGroup)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            Dim ph As MasterPages.ErrorMessage = ErrorPlaceHolder
            If Not ph Is Nothing Then ph.UpdateVisibility()

            MyBase.OnPreRender(e)
        End Sub

        Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Error
            Logger.Auto(Logger.GetErrorMessage(Server.GetLastError()))

            Dim CustomErrors As CustomErrorsSection = ConfigurationManager.GetSection("system.web/customErrors")
            If CustomErrors.Mode = CustomErrorsMode.Off Then Exit Sub
            If CustomErrors.Mode = CustomErrorsMode.RemoteOnly And Request.IsLocal Then Exit Sub

            If Not CustomErrors.Errors("500").Redirect = String.Empty Then
                Server.ClearError()
                Server.Transfer("/" & CustomErrors.Errors("500").Redirect)
            End If
        End Sub

        Public ReadOnly Property ViewStateInSQL() As Boolean
            Get
                Try
                    If AppSettings("ViewStateInSQL") <> String.Empty Then
                        Return CBool(AppSettings("ViewStateInSQL"))
                    Else
                        Return False
                    End If
                Catch
                    Return False
                End Try
            End Get
        End Property

        Public Property ViewStateTimeout() As TimeSpan
            Get
                Return m_ViewStateTimeout
            End Get
            Set(ByVal Value As TimeSpan)
                m_ViewStateTimeout = Value
            End Set
        End Property

        Private Function GetMacKeyModifier() As String
            Dim value As Long = CLng(Me.TemplateSourceDirectory.GetHashCode) + CLng(Me.GetType.Name.GetHashCode)
            If Not ViewStateUserKey Is Nothing Then
                Return String.Concat(value.ToString(NumberFormatInfo.InvariantInfo), ViewStateUserKey)
            End If
            Return value.ToString(NumberFormatInfo.InvariantInfo)
        End Function

        Private Function GetLosFormatter() As LosFormatter
            If EnableViewStateMac Then
                Return New LosFormatter(True, Me.GetMacKeyModifier())
            End If
            Return New LosFormatter()
        End Function

        Private Function GetViewStateGuid() As Guid
            Dim viewStateKey As String = Request.Form("__VIEWSTATEGUID")
            If viewStateKey Is Nothing OrElse viewStateKey.Length < 1 Then
                viewStateKey = Me.Request.QueryString("__VIEWSTATEGUID")
                If viewStateKey Is Nothing OrElse viewStateKey.Length < 1 Then
                    Return Guid.NewGuid()
                End If
            End If
            Try
                Return New Guid(viewStateKey)
            Catch
                Return Guid.NewGuid()
            End Try
        End Function

        Protected Overrides Function LoadPageStateFromPersistenceMedium() As Object
            If Not ViewStateInSQL Then Return MyBase.LoadPageStateFromPersistenceMedium()

            Dim ViewStateGuid As Guid = GetViewStateGuid()
            Dim rawData() As Byte = Nothing

            Dim connection As SqlConnection = New SqlConnection(m_ViewStateConnectionString)
            Try
                Dim command As SqlCommand = New SqlCommand("GetViewState", connection)
                Try
                    command.CommandType = CommandType.StoredProcedure
                    command.Parameters.Add("@returnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                    command.Parameters.Add("@viewStateId", SqlDbType.UniqueIdentifier).Value = ViewStateGuid
                    connection.Open()
                    Dim reader As SqlDataReader = command.ExecuteReader()
                    Try
                        If reader.Read() Then
                            rawData = CType(Array.CreateInstance(GetType(Byte), reader.GetInt32(0)), Byte())
                        End If
                        If reader.NextResult AndAlso reader.Read() Then
                            reader.GetBytes(0, 0, rawData, 0, rawData.Length)
                        End If
                    Finally
                        reader.Dispose()
                    End Try
                Finally
                    command.Dispose()
                End Try
            Finally
                connection.Dispose()
            End Try

            If rawData Is Nothing Then
                Return Nothing
            End If
            Dim stream As MemoryStream = New MemoryStream(rawData)
            Try
                Return GetLosFormatter().Deserialize(stream)
            Finally
                stream.Dispose()
            End Try
        End Function

        Protected Overrides Sub SavePageStateToPersistenceMedium(ByVal ViewState As Object)
            If Not ViewStateInSQL Then
                MyBase.SavePageStateToPersistenceMedium(ViewState)
                Exit Sub
            End If

            Dim ViewStateGuid As Guid = GetViewStateGuid()
            Dim stream As MemoryStream = New MemoryStream()
            Try
                Me.GetLosFormatter().Serialize(stream, ViewState)

                Dim connection As SqlConnection = New SqlConnection(m_ViewStateConnectionString)
                Try
                    Dim command As SqlCommand = New SqlCommand("SetViewState", connection)
                    Try
                        command.CommandType = CommandType.StoredProcedure
                        command.Parameters.Add("@returnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                        command.Parameters.Add("@viewStateId", SqlDbType.UniqueIdentifier).Value = ViewStateGuid
                        command.Parameters.Add("@value", SqlDbType.Image).Value = stream.ToArray()
                        command.Parameters.Add("@timeout", SqlDbType.Int).Value = ViewStateTimeout.TotalMinutes
                        connection.Open()
                        command.ExecuteNonQuery()
                    Finally
                        command.Dispose()
                    End Try
                Finally
                    connection.Dispose()
                End Try
            Finally
                stream.Dispose()
            End Try

            Dim Ctrl As HtmlInputHidden = FindControl("__VIEWSTATEGUID")
            If Ctrl Is Nothing Then
                ClientScript.RegisterHiddenField("__VIEWSTATEGUID", ViewStateGuid.ToString())
            Else
                Ctrl.Value = ViewStateGuid.ToString()
            End If
        End Sub
    End Class

End Namespace
