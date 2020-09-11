Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports System.Web
Imports Newtonsoft.Json.Linq
Imports MedullusSendGridEmailLib
Imports System.Configuration.ConfigurationManager

Namespace Components

    Public Class Core

        Public Shared Function IsEmail(ByVal Email As String) As Boolean

            '********* Regex changed by Apala (Medullus) on 22.01.2018 for mGuard#1066 (to allow hyphens) *********
            Try
                Dim address As New MailAddress(Email)

                Return True

            Catch ex As Exception
                Return False
            End Try

            'Dim reg As Regex
            'reg = New Regex("^[A-Za-z0-9'#]([#_\.\-]?[a-zA-Z0-9']+)*\@([A-Za-z0-9\-]+\.)+[A-Za-z]{2,5}$")

            'Return reg.IsMatch(Email)

        End Function

		Public Shared Sub ChangeSortOrderDragDrop(ByVal DB As Database, ByVal TableName As String, ByVal SortOrderName As String, ByVal IdColumnName As String, ByVal StartId As Integer, ByVal EndId As Integer, ByVal WhereClause As String)
			TableName = ProtectParam(TableName)
			SortOrderName = ProtectParam(SortOrderName)
			IdColumnName = ProtectParam(IdColumnName)

			If StartId = EndId Then Exit Sub
			If WhereClause <> String.Empty Then
				WhereClause = " AND " & WhereClause
			End If

			Dim NewSortOrder As Integer = DB.ExecuteScalar("SELECT TOP 1 " & SortOrderName & " FROM " & TableName & " WHERE " & IdColumnName & " = " & EndId & WhereClause)
			Dim PreviousSortOrder As Integer = DB.ExecuteScalar("SELECT TOP 1 " & SortOrderName & " FROM " & TableName & " WHERE " & IdColumnName & " = " & StartId & WhereClause)
			Dim SQL As String = String.Empty

			If NewSortOrder < PreviousSortOrder Then
				SQL = "UPDATE " & TableName & " SET " & SortOrderName & " = " & SortOrderName & " + 1 WHERE " & SortOrderName & " >= (SELECT a." & SortOrderName & " FROM " & TableName & " a WHERE a." & IdColumnName & "=" & EndId & WhereClause & ") AND " & SortOrderName & " < (SELECT b." & SortOrderName & " FROM " & TableName & " b WHERE b." & IdColumnName & " = " & StartId & WhereClause & ")" & WhereClause
				DB.ExecuteSQL(SQL)

				SQL = "UPDATE " & TableName & " SET " & SortOrderName & " = " & NewSortOrder & " WHERE " & IdColumnName & " = " & StartId & WhereClause
				DB.ExecuteSQL(SQL)
			Else
				SQL = "UPDATE " & TableName & " SET " & SortOrderName & " = " & SortOrderName & " - 1 WHERE " & SortOrderName & " >= (SELECT a." & SortOrderName & " FROM " & TableName & " a WHERE a." & IdColumnName & "=" & StartId & WhereClause & ") AND " & SortOrderName & " <= (SELECT b." & SortOrderName & " FROM " & TableName & " b WHERE b." & IdColumnName & " = " & EndId & WhereClause & ")" & WhereClause
				DB.ExecuteSQL(SQL)

				SQL = "UPDATE " & TableName & " SET " & SortOrderName & " = " & NewSortOrder & " WHERE " & IdColumnName & " = " & StartId & WhereClause
				DB.ExecuteSQL(SQL)
			End If
		End Sub

        Public Shared Function GetUrl(ByVal obj As Object, ByVal Url As String, Optional ByVal UseGlobalReferer As Boolean = True) As String
            Dim GlobalRefererName As String = IIf(UseGlobalReferer, System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"), "")
            If TypeOf obj Is DataRow Then
                If Not obj.Table.Columns("CustomUrl") Is Nothing Then
                    If IsDBNull(obj("CustomUrl")) Then
                        Return GlobalRefererName & Url
                    Else
                        Return GlobalRefererName & obj("CustomUrl")
                    End If
                End If
            ElseIf TypeOf obj Is DataRowView Then
                If Not obj.DataView.Table.Columns("CustomUrl") Is Nothing Then
                    If IsDBNull(obj("CustomUrl")) Then
                        Return GlobalRefererName & Url
                    Else
                        Return GlobalRefererName & obj("CustomUrl")
                    End If
                End If
            Else
                Dim t As Type = obj.GetType
                Dim p As Reflection.PropertyInfo = t.GetProperty("customurl", System.Reflection.BindingFlags.IgnoreCase Xor System.Reflection.BindingFlags.Public Xor System.Reflection.BindingFlags.Instance)
                If Not p Is Nothing Then
                    If obj.CustomUrl = Nothing Then
                        Return GlobalRefererName & Url
                    Else
                        Return GlobalRefererName & obj.CustomUrl
                    End If
                End If
            End If
            Return GlobalRefererName & Url
        End Function

		Public Shared Function FormatCSVstring(ByVal inbuffer As Object) As String
			Dim sOutput As String, sInput As String
			If inbuffer Is Convert.DBNull Then sInput = "" Else sInput = Convert.ToString(inbuffer)

			If Not sInput <> String.Empty Then sInput = Replace(sInput, vbNewLine, " ")
			If sInput = String.Empty Then
				sOutput = """"
			Else
				sOutput = """" & Replace(sInput, """", """""") & """"
			End If
			Return sOutput
		End Function

		Public Shared Function ProtectParam(ByVal sInput As String) As String
			If sInput = String.Empty Then
				Return String.Empty
			End If
			Return Replace(sInput, ";", "")
		End Function

		Public Shared Function HTMLEncode(ByVal Input As Object) As String
			If IsDBNull(Input) Then
				Return String.Empty
			End If
			Return HttpUtility.HtmlEncode(Input)
		End Function

		Public Shared Function FileExists(ByVal FileFullPath As String) As Boolean
			Dim f As New IO.FileInfo(FileFullPath)
			Return f.Exists
		End Function

		Public Shared Function GetFileNameWithoutExtension(ByVal FileFullPath As String) As String
			Return System.IO.Path.GetFileNameWithoutExtension(FileFullPath)
		End Function

		Public Shared Function GetFileExtension(ByVal FileFullPath As String) As String
			Dim f As New IO.FileInfo(FileFullPath)
			Return f.Extension
		End Function

		Public Shared Function FolderExists(ByVal FolderPath As String) As Boolean
			Dim f As New IO.DirectoryInfo(FolderPath)
			Return f.Exists
		End Function

		Public Shared Function ChangeSortOrder(ByVal DB As Database, ByVal KeyField As String, ByVal TableName As String, ByVal SortField As String, ByVal WhereClause As String, ByVal KeyValue As Integer, ByVal Action As String) As Boolean
			Dim SQL As String

			Dim iRowsAffected As Integer
			Dim res As SqlDataReader
			Dim NEXT_SORT_ORDER As String = ""
			Dim NEXT_ID As String = ""

			SQL = "SELECT top 1 " + ProtectParam(KeyField) + "," + ProtectParam(SortField) + " FROM " + ProtectParam(TableName)
			If UCase(Action) = "UP" Then
				SQL &= " WHERE " & ProtectParam(SortField) & " < "
			Else
				SQL &= " WHERE " & ProtectParam(SortField) & " > "
			End If
			SQL &= "(SELECT " & ProtectParam(SortField) & " FROM " & ProtectParam(TableName) & " WHERE " & ProtectParam(KeyField) & "=" & DB.Quote(KeyValue) & ")"
			If Not DB.IsEmpty(WhereClause) Then
				SQL &= " AND " & WhereClause
			End If
			SQL &= " order by " & ProtectParam(SortField)

			If UCase(Action) = "UP" Then
				SQL &= " DESC "
			Else
				SQL &= " ASC "
			End If

			res = DB.GetReader(SQL)
			If res.Read() Then
				NEXT_ID = res(KeyField).ToString()
				NEXT_SORT_ORDER = res(SortField).ToString()
			End If
			res.Close()
			res = Nothing

			If DB.IsEmpty(NEXT_ID) Then
				Return False
			End If

			SQL = "UPDATE " & ProtectParam(TableName) & " SET " & ProtectParam(SortField) & "=(SELECT " & ProtectParam(SortField) & " FROM " & ProtectParam(TableName) & " WHERE " & ProtectParam(KeyField) & "=" & DB.Quote(KeyValue) & ") WHERE " & ProtectParam(KeyField) & "=" & DB.Quote(NEXT_ID)
			iRowsAffected = DB.ExecuteSQL(SQL)
			If iRowsAffected = 0 Then
				Return False
			End If

			SQL = "UPDATE " & ProtectParam(TableName) & " SET " & SortField & "=" & DB.Quote(NEXT_SORT_ORDER) & " WHERE " & ProtectParam(KeyField) & "=" & DB.Quote(KeyValue)
			iRowsAffected = DB.ExecuteSQL(SQL)
			If iRowsAffected = 0 Then
				Return False
			End If

			Return True
		End Function

		Public Shared Sub GetImageSize(ByVal sOriginalPath As String, ByRef Width As Integer, ByRef Height As Integer)
			Dim OriginalImg As Image = Image.FromFile(sOriginalPath)

			Width = OriginalImg.Width
			Height = OriginalImg.Height

			OriginalImg.Dispose()
			OriginalImg = Nothing
		End Sub

		Public Shared Sub ResizeImage(ByVal sOriginalPath As String, ByVal sNewPath As String, ByVal dWidth As Double, ByVal dHeight As Double)
			If Not System.IO.File.Exists(sOriginalPath) Then
				Exit Sub
			End If

			Dim OriginalImg As Image = Image.FromFile(sOriginalPath)
			Dim inp As IntPtr = New IntPtr
			Dim newWidth As Double, newHeight As Double
			Dim oHeight As Double, oWidth As Double

			oHeight = OriginalImg.Height
			oWidth = OriginalImg.Width

			If (dWidth < oWidth And dHeight < oHeight) Then
				newHeight = oHeight * (dWidth / oWidth)
				newWidth = dWidth
				If (dHeight < newHeight) Then
					newWidth = dWidth * (dHeight / newHeight)
					newHeight = dHeight
				End If
			ElseIf (dWidth < oWidth) Then
				newWidth = dWidth
				newHeight = oHeight * (dWidth / oWidth)
			ElseIf (dHeight < oHeight) Then
				newHeight = dHeight
				newWidth = oWidth * (dHeight / oHeight)
			Else
				newHeight = oHeight
				newWidth = oWidth
			End If

			newHeight = Math.Round(newHeight, 0)
			newWidth = Math.Round(newWidth, 0)

			Dim newPic As Bitmap = New Bitmap(Convert.ToInt32(newWidth), Convert.ToInt32(newHeight))
			Dim gr As Graphics = Graphics.FromImage(newPic)
			Dim MemStream As New MemoryStream
			Dim ImageFormat As Imaging.ImageFormat = GetImageFormat(OriginalImg)

			gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
			gr.DrawImage(OriginalImg, 0, 0, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight))

			OriginalImg.Dispose()
			gr.Dispose()

			'save tot he memory stream first
			newPic.Save(MemStream, ImageFormat)
			newPic.Dispose()

			'load from memory stream (seekable stream)
			newPic = Image.FromStream(MemStream)

			'save to file
			If sOriginalPath <> sNewPath Then
				newPic.Save(sNewPath, ImageFormat)
			Else
				System.IO.File.Delete(sNewPath)
				newPic.Save(sNewPath, ImageFormat)
			End If
		End Sub

		Public Shared Sub ResizeImageWithQuality(ByVal sOriginalPath As String, ByVal sNewPath As String, ByVal dWidth As Double, ByVal dHeight As Double, ByVal Quality As Integer)
			If Not System.IO.File.Exists(sOriginalPath) Then
				Exit Sub
			End If

			Dim OriginalImg As Image = Image.FromFile(sOriginalPath)
			Dim inp As IntPtr = New IntPtr
			Dim newWidth As Double, newHeight As Double
			Dim oHeight As Double, oWidth As Double

			oHeight = OriginalImg.Height
			oWidth = OriginalImg.Width

			If (dWidth < oWidth And dHeight < oHeight) Then
				newHeight = oHeight * (dWidth / oWidth)
				newWidth = dWidth
				If (dHeight < newHeight) Then
					newWidth = dWidth * (dHeight / newHeight)
					newHeight = dHeight
				End If
			ElseIf (dWidth < oWidth) Then
				newWidth = dWidth
				newHeight = oHeight * (dWidth / oWidth)
			ElseIf (dHeight < oHeight) Then
				newHeight = dHeight
				newWidth = oWidth * (dHeight / oHeight)
			Else
				newHeight = oHeight
				newWidth = oWidth
			End If

			newHeight = Math.Round(newHeight, 0)
			newWidth = Math.Round(newWidth, 0)

			Dim newPic As Bitmap = New Bitmap(Convert.ToInt32(newWidth), Convert.ToInt32(newHeight))
			Dim gr As Graphics = Graphics.FromImage(newPic)
			Dim MemStream As New MemoryStream
			Dim ImageFormat As Imaging.ImageFormat = GetImageFormat(OriginalImg)

			gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
			gr.DrawImage(OriginalImg, 0, 0, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight))

			OriginalImg.Dispose()
			gr.Dispose()

			'save tot he memory stream first
			newPic.Save(MemStream, ImageFormat)
			newPic.Dispose()

			'load from memory stream (seekable stream)
			newPic = Image.FromStream(MemStream)

			'save to file
			If sOriginalPath <> sNewPath Then
				SaveImage(sNewPath, newPic, Quality)
			Else
				System.IO.File.Delete(sNewPath)
				SaveImage(sNewPath, newPic, Quality)
			End If
		End Sub

		Private Shared Sub SaveImage(ByVal path As String, ByVal img As Image, ByVal Quality As Integer)
			If Quality < 0 OrElse Quality > 100 Then
				Throw New ArgumentOutOfRangeException("quality must be between 0 and 100.")
			End If
			Dim qualityParam As EncoderParameter = New EncoderParameter(Encoder.Quality, Quality)
			Dim codec As ImageCodecInfo = GetEncoderInfo(GetMimeType(img))
			Dim encoderParams As EncoderParameters = New EncoderParameters(1)
			encoderParams.Param(0) = qualityParam
			img.Save(path, codec, encoderParams)
		End Sub

		Public Shared Sub CropImage(ByVal fSource As String, ByVal dWidth As Integer, ByVal dHeight As Integer, Optional ByVal fDestination As String = Nothing, Optional ByVal bCenter As Boolean = True, Optional ByVal iAccuracy As Integer = 0, Optional ByVal Quality As Integer = 100)
			If fSource = String.Empty Then Exit Sub
			If fDestination = Nothing Then fDestination = fSource

			Dim OriginalImg As Drawing.Image = Drawing.Image.FromFile(fSource)
			Dim inp As IntPtr = New IntPtr
			Dim newWidth As Double, newHeight As Double, oRatio As Decimal, dTop As Double, dLeft As Double
			Dim oHeight As Double, oWidth As Double, dRatio As Decimal

			dRatio = dWidth / dHeight
			oHeight = OriginalImg.Height
			oWidth = OriginalImg.Width
			oRatio = oWidth / oHeight

			If oRatio >= (dRatio * (100 - iAccuracy) / 100) AndAlso oRatio <= (dRatio * (100 + iAccuracy) / 100) Then
				Exit Sub
			End If

			If oWidth < dRatio * oHeight Then
				newWidth = oWidth
				newHeight = oWidth / dRatio
			Else
				newHeight = oHeight
				newWidth = oHeight * dRatio
			End If

			If bCenter Then
				dLeft = (oWidth - newWidth) / 2
				dTop = (oHeight - newHeight) / 2
			Else
				dTop = 0
				dLeft = 0
			End If

			newHeight = Math.Round(newHeight, 0)
			newWidth = Math.Round(newWidth, 0)
			dTop = Math.Round(dTop, 0)
			dLeft = Math.Round(dLeft, 0)

			Dim newPic As Bitmap = New Bitmap(Convert.ToInt32(newWidth), Convert.ToInt32(newHeight))
			Dim gr As Graphics = Graphics.FromImage(newPic)
			Dim MemStream As New MemoryStream
			Dim destRect As Rectangle = New Rectangle(0, 0, CInt(newWidth), CInt(newHeight))
			Dim ImageFormat As Imaging.ImageFormat = GetImageFormat(OriginalImg)

			gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
			gr.DrawImage(OriginalImg, destRect, CInt(dLeft), CInt(dTop), Convert.ToInt32(newWidth), Convert.ToInt32(newHeight), GraphicsUnit.Pixel)

			OriginalImg.Dispose()
			gr.Dispose()

			'save tot he memory stream first
			newPic.Save(MemStream, ImageFormat)
			newPic.Dispose()

			'load from memory stream (seekable stream)
			newPic = Drawing.Image.FromStream(MemStream)
			If fDestination = fSource Then
				System.IO.File.Delete(fSource)
			End If

			Dim qualityParam As EncoderParameter = New EncoderParameter(Encoder.Quality, Quality)
			Dim codec As ImageCodecInfo = GetEncoderInfo(GetMimeType(newPic))
			Dim encoderParams As EncoderParameters = New EncoderParameters(1)
			encoderParams.Param(0) = qualityParam

            newPic.Save(fDestination, codec, encoderParams)
		End Sub

		Public Shared Sub SaveWithTransparency(ByVal sOriginalPath As String, ByVal sNewPath As String, ByVal TransparentColorHexCode As String)
			If Not System.IO.File.Exists(sOriginalPath) Then
				Exit Sub
			End If

			Dim OriginalImg As Image = Image.FromFile(sOriginalPath)
			Dim newPic As Bitmap = New Bitmap(OriginalImg.Width, OriginalImg.Height)
			Dim MemStream As New MemoryStream
			Dim ImageFormat As Imaging.ImageFormat = GetImageFormat(OriginalImg)

			Dim gr As Graphics = Graphics.FromImage(newPic)
			gr.DrawImage(OriginalImg, 0, 0, OriginalImg.Width, OriginalImg.Height)
			OriginalImg.Dispose()
			gr.Dispose()

			'save tot he memory stream first
			newPic.MakeTransparent(System.Drawing.ColorTranslator.FromHtml(TransparentColorHexCode))
			newPic.Save(MemStream, ImageFormat)
			newPic.Dispose()

			'load from memory stream (seekable stream)
			newPic = Image.FromStream(MemStream)

			'save to file
			If sOriginalPath <> sNewPath Then
				newPic.Save(sNewPath, ImageFormat)
			Else
				System.IO.File.Delete(sNewPath)
				newPic.Save(sNewPath, ImageFormat)
			End If
		End Sub

		Private Shared Function GetMimeType(ByVal img As Image) As String
			Dim codec As ImageCodecInfo
			For Each codec In ImageCodecInfo.GetImageDecoders()
				If codec.FormatID = img.RawFormat.Guid Then
					Return codec.MimeType
				End If
			Next
			Return "image/unknown"
		End Function

		Private Shared Function GetImageFormat(ByVal img As Image) As Imaging.ImageFormat
			Dim OutputFormats As New Hashtable
			OutputFormats.Add(ImageFormat.Gif.Guid, ImageFormat.Gif)
			OutputFormats.Add(ImageFormat.Jpeg.Guid, ImageFormat.Jpeg)
			OutputFormats.Add(ImageFormat.Bmp.Guid, ImageFormat.Bmp)
			OutputFormats.Add(ImageFormat.Tiff.Guid, ImageFormat.Tiff)
			OutputFormats.Add(ImageFormat.Png.Guid, ImageFormat.Png)
			OutputFormats.Add(ImageFormat.Emf.Guid, ImageFormat.Emf)
			OutputFormats.Add(ImageFormat.Exif.Guid, ImageFormat.Exif)
			OutputFormats.Add(ImageFormat.Icon.Guid, ImageFormat.Icon)
			OutputFormats.Add(ImageFormat.Wmf.Guid, ImageFormat.Wmf)
			Return OutputFormats(img.RawFormat.Guid)
		End Function

		Private Shared Function GetEncoderInfo(ByVal mimeType As String) As ImageCodecInfo
			Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders
			Dim i As Integer = 0
			While i < codecs.Length
				If codecs(i).MimeType = mimeType Then
					Return codecs(i)
				End If
				System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
			End While
			Return Nothing
		End Function

		Public Shared Function Text2HTML(ByVal s As String) As String
			Dim Result As String = String.Empty
			Result = Replace(s, vbLf, "<br/>")
            'Result = Replace(Result, " ", "&nbsp;")
            Result = Replace(Result, "&nbsp;", " ")
			Result = Replace(Result, "<br/><br/><br/>", "<br/><br/>")

			Return Result
		End Function

        Public Shared Sub LogEvent(ByVal Message As String, ByVal EventTypeInfo As System.Diagnostics.EventLogEntryType)

        End Sub

        Public Shared Sub SendMail(ByVal msg As MailMessage, Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")

            Dim ToEmail As String = msg.To.Item(0).Address

            'If ToEmail.Contains("customerservice@cbusa.us") Then Exit Sub
            If ToEmail.Contains("@cbusa.us") Then
                InsertEmailLog(msg.To.Item(0).Address, msg.Subject, msg.Body, "N/A")
                Exit Sub
            End If

            Dim ClientId As Int32 = 1
            Dim APIKey As String = System.Configuration.ConfigurationManager.AppSettings("ApiKey").ToString()

            If Not msg.IsBodyHtml Then
                If Not msg.Body.Contains("<body") Or Not msg.Body.Contains("<Body") Or Not msg.Body.Contains("<BODY") Then
                    Dim HtmlTemplate As String
                    '  HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))

                    If HttpContext.Current Is Nothing Then
                        HtmlTemplate = CStr(HttpRuntime.Cache.Get("HtmlTemplate"))
                    Else
                        HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))
                    End If

                    If HtmlTemplate Is Nothing Or HtmlTemplate = String.Empty Then
                        Dim GlobalRefererName As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")
                        HtmlTemplate = Core.GetRenderedHtml(GlobalRefererName & "/emailtemplate.aspx")
                        Try
                            If HttpContext.Current Is Nothing Then
                                HttpRuntime.Cache.Insert("HtmlTemplate", HtmlTemplate)
                            Else
                                HttpContext.Current.Cache.Insert("HtmlTemplate", HtmlTemplate)
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    Dim newBody As String = Replace(HtmlTemplate, "%%sBody%%", Core.Text2HTML(msg.Body))
                    msg.Body = newBody
                    msg.IsBodyHtml = True
                End If

            End If

            Try
                Dim EmailDetails As String = Newtonsoft.Json.JsonConvert.SerializeObject(New With {Key .EmailReceiverId = ToEmail})
                Dim header = Newtonsoft.Json.JsonConvert.SerializeObject(
                                New With
                                {
                                    Key .emailHeader = New With
                                    {
                                        Key .EmailSenderId = msg.From.ToString(),
                                        Key .EmailSubject = msg.Subject.ToString(),
                                        Key .EmailBody = "",
                                        Key .EmailHTMLBody = msg.Body.ToString(),
                                        Key .ClientId = ClientId,
                                        Key .EmailSenderPasswd = "",
                                        Key .APIKey = APIKey
                                }, Key .emailDetails = {Newtonsoft.Json.JsonConvert.DeserializeObject(EmailDetails)}})

                Dim Parameters As JObject = JObject.Parse("" & header)
                Dim sendMail As MedullusSendGridEmailLib.MedullusSendGridEmailLib
                sendMail = New MedullusSendGridEmailLib.MedullusSendGridEmailLib()
                Trace.WriteLine(Format("Parameters :" & Parameters.ToString()))

                Dim SendGridMsgId As String = ""

                Dim Result As String = sendMail.SendStaticEmailBySendGrid(Parameters, AttachmentList, AttachmentFilePathToRead)

                If Result.Contains("Success:") Then
                    SendGridMsgId = Result.Remove(0, 8)
                End If
                InsertEmailLog(msg.To.Item(0).Address, msg.Subject, msg.Body, SendGridMsgId)

                Trace.WriteLine(Format("Result :" & Result))

            Catch ex As Exception
                Trace.WriteLine(Format("Error :" & ex.Message))
                'Return False;
            End Try

        End Sub

        Private Shared Sub InsertEmailLog(ByVal pToEmail As String, ByVal pSubject As String, ByVal pEmailBody As String, ByVal pMessageId As String)

            Try
                Dim objDB As New Database
                objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

                Dim params(0) As SqlParameter
                params(0) = New SqlParameter("@Email", pToEmail)

                Dim dtEntity As New DataTable()
                objDB.RunProc("sp_GetEntityFromEmail", params, dtEntity)

                If dtEntity.Rows.Count > 0 Then
                    For Each row As DataRowView In dtEntity.DefaultView
                        Dim EntityType As String = row("EntityType")
                        Dim CompanyId As Integer = row("CompanyId")
                        Dim UserId As Integer = row("UserId")
                        Dim FirstName As String = row("FirstName")
                        Dim LastName As String = row("LastName")

                        Dim paramsEmailLog(7) As SqlParameter

                        paramsEmailLog(0) = New SqlParameter("@ToName", String.Concat(FirstName, " ", LastName))
                        paramsEmailLog(1) = New SqlParameter("@Email", pToEmail)
                        paramsEmailLog(2) = New SqlParameter("@MessageSubject", pSubject)
                        paramsEmailLog(3) = New SqlParameter("@MessageBody", pEmailBody)
                        paramsEmailLog(4) = New SqlParameter("@CompanyType", EntityType)
                        paramsEmailLog(5) = New SqlParameter("@CompanyID", CompanyId)
                        paramsEmailLog(6) = New SqlParameter("@UserID", UserId)
                        paramsEmailLog(7) = New SqlParameter("@MessageId", pMessageId)

                        objDB.RunProc("sp_InsertEmailLog", paramsEmailLog)
                    Next
                End If

            Catch ex As Exception
                Trace.WriteLine(Format("Error :" & ex.Message))
            End Try

        End Sub

        'Public Shared Sub SendMail(ByVal msg As MailMessage)
        '    'added code to ensure that all email use the new html template.
        '    ' Dim commonHeader As String = "<%@ Register TagPrefix=""CT"" Namespace=""MasterPages"" Assembly=""Common""%>"

        '    'System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 Or Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12

        '    If Not msg.IsBodyHtml Then
        '        If Not msg.Body.Contains("<body") Or Not msg.Body.Contains("<Body") Or Not msg.Body.Contains("<BODY") Then
        '            Dim HtmlTemplate As String
        '            '  HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))

        '            If HttpContext.Current Is Nothing Then
        '                HtmlTemplate = CStr(HttpRuntime.Cache.Get("HtmlTemplate"))
        '            Else
        '                HtmlTemplate = CStr(HttpContext.Current.Cache.Get("HtmlTemplate"))
        '            End If

        '            If HtmlTemplate Is Nothing Or HtmlTemplate = String.Empty Then
        '                Dim GlobalRefererName As String = System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")
        '                HtmlTemplate = Core.GetRenderedHtml(GlobalRefererName & "/emailtemplate.aspx")
        '                Try
        '                    If HttpContext.Current Is Nothing Then
        '                        HttpRuntime.Cache.Insert("HtmlTemplate", HtmlTemplate)
        '                    Else
        '                        HttpContext.Current.Cache.Insert("HtmlTemplate", HtmlTemplate)
        '                    End If
        '                Catch ex As Exception
        '                End Try
        '            End If
        '            Dim newBody As String = Replace(HtmlTemplate, "%%sBody%%", Core.Text2HTML(msg.Body))
        '            msg.Body = newBody
        '            msg.IsBodyHtml = True
        '        End If

        '    End If

        '    Dim Client As SmtpClient
        '    Client = New SmtpClient()
        '    Client.Host = System.Configuration.ConfigurationManager.AppSettings("MailServer")
        '    Client.Port = CInt(System.Configuration.ConfigurationManager.AppSettings("SmtpPort"))

        '    Dim NetworkCred As New System.Net.NetworkCredential()
        '    NetworkCred.UserName = ConfigurationManager.AppSettings("SmtpUsername")
        '    NetworkCred.Password = ConfigurationManager.AppSettings("SmtpPassword")

        '    Client.UseDefaultCredentials = False
        '    Client.EnableSsl = True
        '    Client.Credentials = NetworkCred
        '    Client.Timeout = System.Configuration.ConfigurationManager.AppSettings("MailServerTimeout")

        '    'Send from MailServer first and if any error occurs then then try to send from MailServerBackup
        '    Try
        '        Client.Send(msg)
        '    Catch ex As Exception
        '        'Client.Host = System.Configuration.ConfigurationManager.AppSettings("MailServerBackup")
        '        'Client.Send(msg)
        '    End Try
        'End Sub

        Public Shared Sub SendSimpleMail(ByVal FromEmail As String, ByVal FromName As String, ByVal ToEmail As String, ByVal ToName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")
            Dim msgFrom As MailAddress = New MailAddress(FromEmail, FromName)
            Dim msgTo As MailAddress = New MailAddress(ToEmail, ToName)
            Dim msg As MailMessage = New MailMessage(msgFrom, msgTo)
            msg.IsBodyHtml = False
            msg.Subject = Subject
            msg.Body = Body
            'msg.Attachments.Add(New Attachment(AttachmentFilePathToRead))
            SendMail(msg, AttachmentList, AttachmentFilePathToRead)
        End Sub

        Public Shared Sub SendSimpleMailToMultipleRecipient(ByVal FromEmail As String, ByVal FromName As String, ByVal ToEmail As String, ByVal ToName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")
            Dim msgFrom As MailAddress = New MailAddress(FromEmail, FromName)
            Dim msgTo As MailAddress = New MailAddress(ToEmail, ToName)
            Dim msg As MailMessage = New MailMessage(msgFrom, msgTo)
            msg.IsBodyHtml = False
            msg.Subject = Subject
            msg.Body = Body
            If Not CC = "" Then
                msg.CC.Add(CC)
            End If
            If Not BCC = "" Then
                msg.Bcc.Add(BCC)
            End If
            'msg.Attachments.Add(New Attachment(AttachmentFilePathToRead))
            SendMail(msg, AttachmentList, AttachmentFilePathToRead)
        End Sub

        Public Shared Sub SendHTMLMail(ByVal FromEmail As String, ByVal FromName As String, ByVal ToEmail As String, ByVal ToName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")
            Dim msgFrom As MailAddress = New MailAddress(FromEmail, FromName)
            Dim msgTo As MailAddress = New MailAddress(ToEmail, ToName)
            Dim msg As MailMessage = New MailMessage(msgFrom, msgTo)
            msg.IsBodyHtml = True
            msg.Subject = Subject
            msg.Body = Body
            If Not CC = "" Then
                msg.CC.Add(CC)
            End If
            If Not BCC = "" Then
                msg.Bcc.Add(BCC)
            End If
            SendMail(msg)
        End Sub


        Public Shared Sub SendHTMLMailToMultipleRecipient(ByVal FromEmail As String, ByVal FromName As String, ByVal ToEmail As String, ByVal ToName As String, ByVal Subject As String, ByVal Body As String, Optional ByVal CC As String = "", Optional ByVal BCC As String = "", Optional ByVal AttachmentList As String = "", Optional ByVal AttachmentFilePathToRead As String = "")
            Dim msgFrom As MailAddress = New MailAddress(FromEmail, FromName)
            'Dim msgTo As MailAddress = New MailAddress(ToEmail, ToName)
            Dim msg As MailMessage = New MailMessage()
            msg.From = msgFrom
            msg.To.Add(ToEmail)
            msg.IsBodyHtml = True
            msg.Subject = Subject
            msg.Body = Body
            If Not CC = "" Then
                msg.CC.Add(CC)
            End If
            If Not BCC = "" Then
                msg.Bcc.Add(BCC)
            End If
            SendMail(msg)
        End Sub

        Public Shared Function StripDblQuote(ByVal sInput As String) As String
			If sInput = String.Empty Then Return String.Empty

			If Left(sInput, 1) = """" And Right(sInput, 1) = """" Then
				Return Mid(sInput, 2, Len(sInput) - 2)
			Else
				Return sInput
			End If
		End Function

		Public Shared Function DblQuote(ByVal s As String) As String
			Dim t

			If s = String.Empty Then
				Return ""
			Else
				t = Replace(s, """", """""")
				t = Replace(t, vbCrLf, " ")
				t = Trim(t)
				Return """" & t & """"
			End If
		End Function

		Public Shared Function QuoteCSV(ByVal sInput As String) As String
			Dim bDblQuote As Boolean = False

			If InStr(sInput, ",") > 0 Then bDblQuote = True
			If InStr(sInput, """") > 0 Then bDblQuote = True
			If InStr(sInput, vbCrLf) > 0 Then bDblQuote = True

			If bDblQuote Then
				Return DblQuote(sInput)
			Else
				Return sInput
			End If
		End Function

		Public Shared Function GenerateFileID() As String
			Dim sResult

			sResult = System.Guid.NewGuid().ToString()
			sResult = Replace(sResult, "{", "")
			sResult = Replace(sResult, "}", "")
			sResult = Replace(sResult, "-", "")

			Return Left(sResult, 32)
		End Function

		' Strips the HTML tags from strHTML
		Public Shared Function StripHTML(ByVal sInput As String) As String
			Dim r As Regex = New Regex("<(.|\n)+?>", RegexOptions.IgnoreCase)
			Dim Result As String = String.Empty

			'Replace all HTML tag matches with the empty string
			Result = r.Replace(sInput, " ")

			'Replace all < and > with &lt; and &gt;
			Result = Replace(Result, "<", "&lt;")
			Result = Replace(Result, ">", "&gt;")
			Result = Replace(Result, "&#160;", " ")

			Return Result
		End Function

		Public Shared Function NullToZero(ByVal val As Object) As Double
			If val.Equals(DBNull.Value) Then
				Return 0
			Else
				Return Convert.ToDouble(val)
			End If
		End Function

		Public Shared Function Escape(ByVal s As String) As String
			Dim t As String

			If s = String.Empty Then
				Return "''"
			Else
				t = Replace(s, "'", "\'")
				t = Trim(t)
				Return "'" & t & "'"
			End If
		End Function

		Public Shared Function RemoveSpecialCharacters(ByVal sInput As String) As String
			If sInput = String.Empty Then
				Return String.Empty
			End If

			sInput = Replace(sInput, ":", "")
			sInput = Replace(sInput, "<", "")
			sInput = Replace(sInput, ">", "")
			sInput = Replace(sInput, "=", "")
			sInput = Replace(sInput, "+", "")
			sInput = Replace(sInput, "@", "")
			sInput = Replace(sInput, Chr(34), "")
			sInput = Replace(sInput, "%", "")
			sInput = Replace(sInput, "&", "")
			sInput = Replace(sInput, "/", "")
			sInput = Replace(sInput, " ", "")
			sInput = Replace(sInput, ".", "")
			sInput = Replace(sInput, "_", "")
			sInput = Replace(sInput, "-", "")
			sInput = Replace(sInput, "#", "")

			Return sInput
		End Function

		Public Shared Function SplitSearchOR(ByVal Search As String) As String
			Dim Result As String, Result1 As String = String.Empty, Result2 As String = String.Empty, iLoop As Integer
			Dim aWords() As String, ConnStr1 As String = String.Empty, ConnStr2 As String = String.Empty

			aWords = Split(Trim(Search), " "c)

			For iLoop = LBound(aWords) To UBound(aWords)
				Result1 &= ConnStr1 & DblQuote(aWords(iLoop))
				ConnStr1 = " or "

				Result2 &= ConnStr2 & DblQuote(aWords(iLoop))
				ConnStr2 = " and "
			Next

			If aWords.Length > 1 Then
				Result = "(" & Result1 & ") or (" & Result2 & ") or " & DblQuote(Search)
			Else
				Result = Result1
			End If
			Return Result
		End Function

		Public Shared Function SplitSearchAND(ByVal Search As String) As String
			Dim Result As String = String.Empty, iLoop As Integer
			Dim aWords() As String, ConnStr As String = String.Empty

			aWords = Split(Trim(Search), " "c)

			For iLoop = LBound(aWords) To UBound(aWords)
				Result &= ConnStr & DblQuote(aWords(iLoop))
				ConnStr = " and "
			Next

			If aWords.Length > 1 Then
				Result = "(" & Result & ") or " & DblQuote(Search)
			End If
			Return Result
		End Function

		Public Shared Function BuildFullName(ByVal FirstName As String, ByVal MiddleInitial As String, ByVal LastName As String) As String
			Dim Result As String = String.Empty

			Result = Trim(FirstName & " " & MiddleInitial)
			Result = Trim(Result & " " & LastName)

			Return Result
		End Function

        Public Shared Function GetRenderedHtml(ByVal Url As String) As String

            '*********** Added by Apala (Medullus) to overcome Certificate Host Name issue on staging environments
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf GetResult

            System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12

            Dim Results As String = String.Empty
            Dim req As Net.HttpWebRequest = Net.WebRequest.Create(Url)
            Dim myCache As New System.Net.CredentialCache()
            myCache.Add(New Uri(Url), "Basic", New System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("WindowsAuthLogin"), System.Configuration.ConfigurationManager.AppSettings("WindowsAuthPassword")))
            req.Credentials = myCache
            Dim sr As StreamReader = Nothing
            Try
                sr = New StreamReader(req.GetResponse().GetResponseStream())
                Results = sr.ReadToEnd
            Catch ex As Net.WebException
                sr = New StreamReader(ex.Response.GetResponseStream)
                Results = sr.ReadToEnd
            Catch ex As Exception
            Finally
                If Not sr Is Nothing Then sr.Dispose()
            End Try
            Return Results
        End Function

        Public Shared Function IsDangerousString(ByVal val As String) As Boolean
			Dim Pattern As String = String.Empty

			Pattern = "(<\s*(script|object|applet|embed|form)\s*>)"	  ' <  script xxx >
			Pattern = Pattern & "|" & "(<.*>)"
			' >xxxxx<  warning includes hyperlinks and stuff between > and <
			Pattern = Pattern & "|" & "(&.{1,5};)"	 ' &xxxx;
			Pattern = Pattern & "|" & "eval\s*\("	 ' eval  ( 
			Pattern = Pattern & "|" & "(event\s*=)"	 ' event  =

			'Now lets check for encoding
			Pattern = Replace(Pattern, "<", "(<|%60|<)")
			Pattern = Replace(Pattern, ">", "(>|%62|>)")

			Dim Matches As MatchCollection = Regex.Matches(val, Pattern, RegexOptions.Multiline And RegexOptions.IgnoreCase)
			Return (Matches.Count > 0)
		End Function

		Public Shared Function GetURLOnly(ByVal Url As String) As String
			If Url.IndexOf("?") > -1 Then Url = Url.Substring(0, Url.IndexOf("?"))
			Return Url
		End Function

		Public Shared Function ParseCSVLine(ByVal lineIn As String) As String()
			Dim csvRegEx As Regex = New Regex(",(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))")
			Dim aLine As String() = csvRegEx.Split(lineIn)
			For iLoop As Integer = 0 To UBound(aLine)
				If Len(aLine(iLoop)) > 2 Then
					If Left(aLine(iLoop), 1) = """" And Right(aLine(iLoop), 1) = """" Then
						aLine(iLoop) = Right(aLine(iLoop), Len(aLine(iLoop)) - 1) ' Remove left quote
						aLine(iLoop) = Left(aLine(iLoop), Len(aLine(iLoop)) - 1) ' Remove right quote
					End If
				ElseIf aLine(iLoop) = """" Then
					aLine(iLoop) = ""
				End If
				If Len(aLine(iLoop)) > 0 Then aLine(iLoop) = Replace(aLine(iLoop), """""", """")
			Next
			Return aLine
		End Function

		Public Shared Sub PermanentRedirect(ByVal url As String)
			If HttpContext.Current Is Nothing Then
				Exit Sub
			End If
			Dim Response As HttpResponse = HttpContext.Current.Response
			Response.Clear()
			Response.StatusCode = 301
			Response.AppendHeader("location", url)
			Response.End()
		End Sub

        Public Shared Function GetString(ByVal sObject As Object) As String
            If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToString(sObject)
        End Function
        Public Shared Function GetBoolean(ByVal sObject As Object) As Boolean
            If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToBoolean(sObject)
        End Function
        Public Shared Function GetDate(ByVal sObject As Object) As DateTime
            If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToDateTime(sObject)
        End Function
        Public Shared Function GetDouble(ByVal sObject As Object) As Double
            If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToDouble(sObject)
        End Function
        Public Shared Function GetMoney(ByVal sObject As Object) As Double
            Return GetDouble(sObject)
        End Function
        Public Shared Function GetInt(ByVal sObject As Object) As Integer
            If IsDBNull(sObject) Then Return Nothing Else Return Convert.ToInt32(sObject)
        End Function

        '*********** Added by Apala (Medullus) to overcome Certificate Host Name issue **********************
        Public Shared Function GetResult() As Boolean
            Return True
        End Function

        Public Shared Sub DataLog(ByVal _ModuleName As String, ByVal _PageURL As String, ByVal _CurrentUserId As String, ByVal _OperationType As String, ByVal _ColumnName As String, ByVal _OldValue As String, ByVal _NewValue As String, ByVal FileName As String, ByVal UserName As String, Optional ByVal PartitionKeyParam As String = "CBUSA_Legacy Application")
            Dim _AuditTrailID As Integer = 0
            Dim _ProjectName As String = "CBUSA_Legacy Application"
            Dim _OperationDate As DateTime = DateAndTime.Now()
            'Dim _ModuleName As String = ""
            'Dim _PageURL As String = ""
            'Dim _CurrentUserId As String = "" 'logged in user id
            'Dim _OperationType As String = ""
            'Dim _ColumnName As String = ""
            'Dim _OldValue As String = ""
            'Dim _NewValue As String = ""
            Dim StorageAccount As String = "UseDevelopmentStorage = True;"
            'Dim FileName As String = ""
            'Dim UserName As String = ""  'first name n last name
            Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName, PartitionKeyParam)
            Dim Logdata As String = Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName, PartitionKeyParam)
            'Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName, PartitionKeyParam)
        End Sub

        '*********** Added by Apala (Medullus) to log all emails sent out from the system **********************
        Public Shared Sub LogEmail(ByVal pEmail As String)



        End Sub

    End Class

    Public Class GenericCollection(Of T)
        Inherits CollectionBase

        Public Sub New()
        End Sub

        Public Overridable Sub Add(ByVal item As T)
            Me.List.Add(item)
        End Sub

        Public Function Contains(ByVal item As T) As Boolean
            Return Me.List.Contains(item)
        End Function

        Public Function IndexOf(ByVal item As T) As Integer
            Return Me.List.IndexOf(item)
        End Function

        Public Sub Insert(ByVal Index As Integer, ByVal item As T)
            Me.List.Insert(Index, item)
        End Sub

        Default Public Property Item(ByVal Index As Integer) As T
            Get
                Return CType(Me.List.Item(Index), T)
            End Get

            Set(ByVal Value As T)
                Me.List(Index) = Value
            End Set
        End Property

        Public Sub Remove(ByVal item As T)
            Me.List.Remove(item)
        End Sub
    End Class

    <Serializable()> _
    Public Class GenericSerializableCollection(Of ItemType)
        Inherits CollectionBase

        Public Sub New()
        End Sub

        Public Sub Add(ByVal item As ItemType)
            Me.List.Add(item)
        End Sub

        Public Function Contains(ByVal item As ItemType) As Boolean
            Return Me.List.Contains(item)
        End Function

        Public Function IndexOf(ByVal item As ItemType) As Integer
            Return Me.List.IndexOf(item)
        End Function

        Public Sub Insert(ByVal Index As Integer, ByVal item As ItemType)
            Me.List.Insert(Index, item)
        End Sub

        Default Public Property Item(ByVal Index As Integer) As ItemType
            Get
                Return CType(Me.List.Item(Index), ItemType)
            End Get

            Set(ByVal Value As ItemType)
                Me.List(Index) = Value
            End Set
        End Property

        Public Sub Remove(ByVal item As ItemType)
            Me.List.Remove(item)
        End Sub
    End Class
End Namespace
