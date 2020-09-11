Imports Components
Imports System.Web
Imports System.Configuration.ConfigurationManager

Namespace Utility
    Public Class GlobalDB
        Public Shared ReadOnly Property DB() As Database
            Get
                'AddHandler HttpContext.Current.ApplicationInstance.EndRequest, AddressOf OnEndRequset
                'try to keep one DB instance per Context
                If HttpContext.Current IsNot Nothing Then
                    If HttpContext.Current.Items("GlobalDB") Is Nothing Then
                        HttpContext.Current.Items.Add("GlobalDB", GetDatabase())
                    End If
                    Return HttpContext.Current.Items("GlobalDB")
                Else
                    'if no conext, return new instance
                    Return GetDatabase()
                End If
            End Get
        End Property

        Public Shared Sub OnEndRequset(ByVal sender As Object, ByVal e As System.EventArgs)
            If HttpContext.Current.Items("GlobalDB") IsNot Nothing Then
                Dim db As Database = HttpContext.Current.Items("GlobalDB")
                db.Close()
                HttpContext.Current.Items("GlobalDB") = Nothing
            End If
        End Sub

        Private Shared Function GetDatabase() As Database
            Dim cs As String = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))
            Dim db As New Database()
            db.Open(cs)
            Return db
        End Function
    End Class
End Namespace
