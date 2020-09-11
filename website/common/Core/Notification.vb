Imports System.Net.Mail

Namespace Components

	Public Class Notification
		Public Message As MailMessage
		Public Recipients As New MailAddressCollection

		Private Sub Send()
			If Message Is Nothing OrElse Recipients.Count = 0 OrElse Message.Body = String.Empty OrElse Message.From Is Nothing Then Exit Sub

			For Each Recipient As MailAddress In Recipients
				Try
					If Message.IsBodyHtml Then
						Core.SendHTMLMail(Message.From.Address, Message.From.DisplayName, Recipient.Address, Recipient.DisplayName, Message.Subject, Message.Body)
					Else
						Core.SendSimpleMail(Message.From.Address, Message.From.DisplayName, Recipient.Address, Recipient.DisplayName, Message.Subject, Message.Body)
					End If
				Catch ex As Exception
				End Try
			Next
		End Sub

		Private Shared Sub SendNotification(ByVal Notify As Object)
			CType(Notify, Notification).Send()
		End Sub

		Public Sub Queue()
			System.Threading.ThreadPool.QueueUserWorkItem(New System.Threading.WaitCallback(AddressOf Notification.SendNotification), Me)
		End Sub
	End Class

End Namespace