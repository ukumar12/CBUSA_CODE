Option Strict Off

Imports Components
Imports DataLayer

Partial Class _default
    Inherits ModuleControl

    Protected EntityID As Integer = 1

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        'Dim SQL As String = String.Empty
        'Dim dt As DataTable

        'SQL = RepeaterSQL()

        'dt = DB.GetDataTable(SQL)

        'Me.rptMessagesAlerts.DataSource = dt
        'Me.rptMessagesAlerts.DataBind()

    End Sub

    'Private Function RepeaterSQL() As String
    '    Return BuilderRepeaterSQL()
    'End Function

    'Private Function BuilderRepeaterSQL() As String

    '    Dim SQL As String = String.Empty

    '    SQL = "SELECT TOP 10" & vbCrLf
    '    SQL &= "  am.AutomaticMessageID," & vbCrLf
    '    SQL &= "  am.Subject," & vbCrLf
    '    SQL &= "  LEFT(CAST(am.Message as varchar(100)), 100) Message," & vbCrLf
    '    SQL &= "  am.Title," & vbCrLf
    '    SQL &= "  ambr.Created" & vbCrLf
    '    SQL &= "FROM" & vbCrLf
    '    SQL &= "  AutomaticMessageBuilderRecipient ambr " & vbCrLf
    '    SQL &= "  JOIN AutomaticMessages am ON ambr.AutomaticMessageID = am.AutomaticMessageID" & vbCrLf
    '    SQL &= "WHERE" & vbCrLf
    '    SQL &= "  am.IsMessage = 1" & vbCrLf
    '    SQL &= "  AND ambr.BuilderID = " & EntityID.ToString & vbCrLf

    '    Return SQL

    'End Function

    'Private Function VendorRepeaterSQL() As String
    '    Return String.Empty
    'End Function

    'Private Function PIQRepeaterSQL() As String
    '    Return String.Empty
    'End Function

End Class
