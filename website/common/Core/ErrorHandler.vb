Imports System.Web
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Net.Mail
Imports System.Xml
Imports System.Web.Caching
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports log4net

Namespace Components

    Public Class ErrorHandler
        Private ErrorMapCollection As StringDictionary
        Private DB As Database

        Private Sub LoadErrMapCollection()
            Dim Filename As String

            'RETRIEVE FROM CACHE
            ErrorMapCollection = CType(HttpContext.Current.Cache("ErrMapCollection"), StringDictionary)
            If ErrorMapCollection Is Nothing Then

                ErrorMapCollection = New StringDictionary

                Filename = System.Configuration.ConfigurationManager.AppSettings("ErrorMapFile").ToString
                Dim xmlReader As XmlTextReader = Nothing
                Try
                    xmlReader = New XmlTextReader(HttpContext.Current.Server.MapPath(Filename))
                    While (xmlReader.Read())
                        If xmlReader.NodeType = XmlNodeType.Element Then
                            If xmlReader.HasAttributes Then
                                While xmlReader.MoveToNextAttribute()
                                    Dim original As String = xmlReader.Value
                                    If xmlReader.MoveToNextAttribute() Then
                                        ErrorMapCollection.Add(original, xmlReader.Value)
                                    End If
                                End While
                            End If
                        End If
                    End While
                Finally
                    If Not xmlReader Is Nothing Then xmlReader.Close()
                End Try

                'SAVE IN THE CACHE    
                HttpContext.Current.Cache.Insert("ErrMapCollection", ErrorMapCollection, New CacheDependency(HttpContext.Current.Server.MapPath(Filename)))
            End If
        End Sub

        Public Sub New(ByVal DB As Database)
            Me.DB = DB
            LoadErrMapCollection()
        End Sub

        Public ReadOnly Property ErrorText(ByVal ex As Exception) As String
            Get
                Dim ErrorDesc As String = String.Empty
                If Left(ex.Message, 7) = "CUSTOM:" Then
                    ErrorDesc = Right(ex.Message, Len(ex.Message) - 7)
                Else
                    For Each entry As DictionaryEntry In ErrorMapCollection
                        If InStr(UCase(ex.Message), UCase(CStr(entry.Key))) > 0 Then
                            ErrorDesc = CStr(entry.Value)
                            Exit For
                        End If
                    Next
                End If

                If ErrorDesc = String.Empty Then
                    Logger.Auto(Logger.GetErrorMessage(ex))
                    Return "A System Error has occured. Our systems administrators have been notified and are working to fix the problem. If the problem persists, please exit your Web browser and try again. Please contact Customer Service at <a href=""mailto:customerservice@cbusa.us"">customerservice@cbusa.us</a> if you need further assistance."
                End If
                Return ErrorDesc
            End Get
        End Property
    End Class

End Namespace
