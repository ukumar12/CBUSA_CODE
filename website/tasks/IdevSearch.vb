Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Components
Imports System.Net.Mail
Imports DataLayer
Imports System.Web
Imports System.Net
Imports System.Data.SqlClient
Imports System.Collections.Specialized
Imports System.Linq

Public Class IdevSearch

    Private Shared IndexDirectoryRoot As String = AppSettings("SearchIndexDirectory")
    Private Shared IndexDirectory As String = AppSettings("SearchIndexDirectory") & "queue\"
    Private Shared CurrentDate As Date = Date.Today
    Private Shared ConnectionString As String
    Private Shared FileName As String = AppSettings("SearchIndexname")

    Public Shared Sub Run(ByVal DB As Database)
        Console.WriteLine("Running IdevSearch ... ")

        Dim Sql As String = String.Empty
        Dim Xml As StreamWriter = Nothing
        Dim dbTaskLog As TaskLogRow = Nothing
        Try



            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "IdevSearch"
            dbTaskLog.Status = "Started"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = ""
            dbTaskLog.Insert()

            Dim TempName As String = FileName
            If Core.FileExists(TempName & ".xml") Then File.Delete(TempName & ".xml")

            Dim x As New Xml.XmlTextWriter(TempName & ".xml", System.Text.Encoding.UTF8)
            x.WriteProcessingInstruction("xml", "version=""1.0"" encoding=""utf-8""")
            x.WriteStartElement("data")

            'always clear index
            x.WriteElementString("clearindex", "true")

            Dim dtSupplyPhases As DataTable = SupplyPhaseRow.GetList(DB)

            Sql = " SELECT p.*, m.Manufacturer, u.UnitOfMeasure, t.ProductType "
            Sql &= " FROM Product p "
            Sql &= "     INNER JOIN ProductType t ON p.ProductTypeID=t.ProductTypeID"
            Sql &= "     LEFT OUTER JOIN Manufacturer m ON p.ManufacturerID=m.ManufacturerID"
            Sql &= "     LEFT OUTER JOIN UnitOfMeasure u ON p.SizeUnitOfMeasureID=u.UnitOfMeasureID "
            Sql &= " WHERE p.IsActive = 1 "
            Sql &= " AND EXISTS (SELECT sp.SupplyPhaseID from SupplyPhase sp INNER JOIN SupplyPhaseProduct sps ON sp.SupplyPhaseID=sps.SupplyPhaseID WHERE sps.ProductID=p.ProductID)"

            Dim dt As DataTable = DB.GetDataTable(Sql)

            Dim hashNames As New Hashtable
            Dim hashParents As New Hashtable
            For Each row As DataRow In dtSupplyPhases.Rows
                hashParents.Add(row("SupplyPhaseID").ToString, New SupplyPhaseItem(row("SupplyPhaseID"), row("SupplyPhase"), IIf(IsDBNull(row("ParentSupplyPhaseID")), -1, row("ParentSupplyPhaseID"))))
            Next

            Dim dtDepartments As DataTable = DB.GetDataTable("select sp.SupplyPhaseID, spp.ProductID from SupplyPhase sp inner join SupplyPhaseProduct spp on sp.SupplyPhaseID=spp.SupplyPhaseID ")
            'standard depts
            Dim hashDepartments As New Hashtable
            For Each row As DataRow In dtDepartments.Rows
                If hashDepartments(row("ProductID").ToString) Is Nothing Then
                    hashDepartments(row("ProductID").ToString) = New ArrayList
                End If
                Dim al As ArrayList = hashDepartments(row("ProductID").ToString)
                al.Add(row("SupplyPhaseID"))
            Next

            Dim cCount As Integer = 0

            For Each dr As DataRow In dt.Rows
                'add 1 to EndDate since dates stored @ 12:00:00AM

                cCount += 1
                Console.WriteLine("Processing row " & cCount & " of " & dt.Rows.Count & " (" & Now() & ")")

                x.WriteStartElement("product")
                x.WriteElementString("ProductID", dr("ProductID"))
                x.WriteElementString("ProductIDTemp", dr("ProductID"))
                x.WriteElementString("ProductName", dr("Product"))
                x.WriteElementString("ProductText", dr("Product"))
                x.WriteElementString("SKU", dr("SKU"))
                x.WriteElementString("ProductType", dr("ProductType"))
                x.WriteElementString("ProductTypeText", dr("ProductType"))
                If Not IsDBNull(dr("Description")) Then
                    x.WriteElementString("Description", dr("Description"))
                End If
                If Not IsDBNull(dr("Manufacturer")) Then
                    x.WriteElementString("Manufacturer", dr("Manufacturer"))
                    x.WriteElementString("ManufacturerText", dr("Manufacturer"))
                End If
                If Not IsDBNull(dr("UnitOfMeasure")) Then
                    x.WriteElementString("SizeUnitOfMeasure", dr("UnitOfMeasure"))
                    x.WriteElementString("SizeUnitOfMeasureText", dr("UnitOfMeasure"))
                End If
                If Not IsDBNull(dr("Grade")) Then
                    x.WriteElementString("Grade", dr("Grade"))
                    x.WriteElementString("GradeText", dr("Grade"))
                End If

                If Not IsDBNull(dr("Length")) Then
                    x.WriteElementString("Length", SearchIndex.GetNumber(Regex.Replace(dr("Length"), "^[\D]*([\d]+).*", "$1")))
                End If
                If Not IsDBNull(dr("Width")) Then
                    x.WriteElementString("Width", SearchIndex.GetNumber(Regex.Replace(dr("Width"), "^[\D]*([\d]+).*", "$1")))
                End If
                If Not IsDBNull(dr("Thickness")) Then
                    x.WriteElementString("Thickness", SearchIndex.GetNumber(Regex.Replace(dr("Thickness"), "^[\D]*([\d]+).*", "$1")))
                End If

                For Each id As Integer In hashDepartments(dr("ProductID").ToString)
                    Dim current As SupplyPhaseItem = hashParents(id.ToString)
                    'While current.Parent > 0
                    While current IsNot Nothing
                        If current.Parent = -1 Then
                            x.WriteElementString("SupplyPhase", "ROOT")
                        End If
                        x.WriteElementString("SupplyPhaseID", current.ID)
                        x.WriteElementString("SupplyPhase", current.ID & "|" & current.Name)
                        x.WriteElementString("SupplyPhaseText", current.Name)
                        current = hashParents(current.Parent.ToString)
                    End While
                Next

                x.WriteEndElement()
            Next

            '</data>'
            x.WriteEndElement()
            x.Close()

            'remove live file
            Try
                If Core.FileExists(IndexDirectory & TempName & ".xml") Then File.Delete(IndexDirectory & TempName & ".xml")
            Catch ex As Exception

            End Try
            Try
                File.Copy(TempName & ".xml", IndexDirectory & TempName & ".xml", True)
            Catch ex As Exception

            End Try
            'Try
            '    File.Delete(TempName & ".xml")
            'Catch ex As Exception

            'End Try

            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "IdevSearch"
            dbTaskLog.Status = "Completed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = IndexDirectory & TempName & ".xml (" & cCount & " rows processed)"
            dbTaskLog.Insert()

        Catch ex As Exception
            Dim Body As String = ex.Source & vbCrLf & vbCrLf & ex.Message & vbCrLf & ex.StackTrace & vbCrLf
            Logger.Error(Logger.GetErrorMessage(ex))
            SendErrorNotification(Body)
            dbTaskLog = New TaskLogRow(DB)
            dbTaskLog.TaskName = "IdevSearch"
            dbTaskLog.Status = "Failed"
            dbTaskLog.LogDate = Now()
            dbTaskLog.Msg = Body
            dbTaskLog.Insert()
        Finally
            If Not Xml Is Nothing Then Xml.Close()
        End Try
    End Sub

    Private Shared Function CleanHTML(ByVal html As String) As String
        Dim content As String = html
        content = Regex.Replace(content, "(<SCRIPT[^>]*>)([\s\S]*?(</SCRIPT[^>]*>))?", "", RegexOptions.IgnoreCase)
        content = Core.StripHTML(content)
        content = Replace(content, "[Skip To Content]", String.Empty)
        content = Replace(content, vbCrLf & " ", vbCrLf)
        content = Replace(content, vbCrLf & vbTab, vbCrLf)
        content = Regex.Replace(content, "\s{2,}", " ", RegexOptions.IgnoreCase)
        content = Regex.Replace(content, "^\s*", "", RegexOptions.IgnoreCase)

        Return content
    End Function

    Private Shared Sub SendErrorNotification(ByVal sBody As String)
        Dim Sender As String = AppSettings("ErrorLogEmailFrom")
        Dim Recipients As String = AppSettings("ErrorLogEmailRecipients")
        Dim Subject As String = AppSettings("ErrorLogEmailSubject")

        Dim SmtpMail As SmtpClient = New SmtpClient(AppSettings("MailServer"))
        SmtpMail.Send(Sender, Recipients, Subject, sBody)
    End Sub

    Private Class SupplyPhaseItem
        Public Name As String
        Public Parent As Integer
        Public ID As Integer
        Public Sub New(ByVal ID As Integer, ByVal Name As String, ByVal Parent As Integer)
            Me.ID = ID
            Me.Name = Name
            Me.Parent = Parent
        End Sub
    End Class
End Class