Imports System.Web.UI
Imports System.web.Caching
Imports System.Web

Namespace Components

	Public Class BaseSurveyControl
		Inherits BaseControl

		Private m_SurveyId As Integer
		Private m_PageId As Integer
		Private m_QuestionId As Integer
		Private m_ResponseId As Integer

		Public Property SurveyId() As Integer
			Get
				Return m_SurveyId
			End Get
			Set(ByVal value As Integer)
				m_SurveyId = value
			End Set
		End Property

		Public Property PageId() As Integer
			Get
				Return m_PageId
			End Get
			Set(ByVal value As Integer)
				m_PageId = value
			End Set
		End Property

		Public Property QuestionId() As Integer
			Get
				Return m_QuestionId
			End Get
			Set(ByVal value As Integer)
				m_QuestionId = value
			End Set
		End Property

		Public Property ResponseId() As Integer
			Get
				Return m_ResponseId
			End Get
			Set(ByVal value As Integer)
				m_ResponseId = value
			End Set
		End Property

		Public Sub New()
		End Sub

	End Class

End Namespace