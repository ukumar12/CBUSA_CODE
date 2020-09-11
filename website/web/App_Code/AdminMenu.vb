Namespace Components

	Public Class AdminMenu
		Private Output As String
		Private TREE_MENU_iRootCount As Integer = 1

		Public Sub New()
			Output = String.Empty
		End Sub

		Public Shared Function MenuTopEmptyRoot(ByVal sUrl As String, ByVal sRoot As String) As String
			Return "<nobr><a href=""" & sUrl & """ target=""main"" class=""L1""><img src=""/images/spacer.gif"" width=""2"" height=""1"" border=""0"">" & sRoot & "</a></nobr>"
		End Function

		Public Sub WriteEmptyRoot(ByVal sUrl As String, ByVal sRoot As String)
			Output += "<div><a href=""" & sUrl & """ target=""main"" class=""L1""><img src=""/images/spacer.gif"" class=""I1"">" & sRoot & "</a></div>"
		End Sub

		Public Sub WriteRoot(ByVal sRoot As String)
			Output += "<div><a href=""#"" onclick=""return X(" & TREE_MENU_iRootCount & ");"" class=""L1""><img id=""I" & TREE_MENU_iRootCount & """ src=""/images/collapsed.gif"" class=""I1"">" & sRoot & "</a></div>"
			Output += "<span id=""A" & TREE_MENU_iRootCount & """ class=""S1"" style=""display:none;"">"

			TREE_MENU_iRootCount = TREE_MENU_iRootCount + 1
		End Sub

		Public Sub WriteBranch(ByVal sUrl As String, ByVal sLink As String)
			Output += "<div><a href=""" & sUrl & """ target=""main"" class=""L2"">" & sLink & "</a></div>"
		End Sub

		Sub WriteEmptyBranch(ByVal sLink As String)
			Output += "<div class=""L2"">" & sLink & "</div>"
		End Sub

		Sub WriteLeaf(ByVal sUrl As String, ByVal sLink As String, ByVal bLastLeaf As Boolean)
			Output += "<div><a href=""" & sUrl & """ target=""main"" class=""L3"">" & sLink & "</a></div>"
			If bLastLeaf Then
				Output += "</span>"
			End If
		End Sub

		Sub WriteHTML(ByVal sHTML As String)
			Output += sHTML
		End Sub

		Public ReadOnly Property Menu() As String
			Get
				Return Output
			End Get
		End Property

	End Class

End Namespace
