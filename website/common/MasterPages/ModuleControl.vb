Imports System.Web.UI
Imports System.web.Caching
Imports MasterPages

Namespace Components

    Public MustInherit Class ModuleControl
        Inherits BaseControl
        Implements IModule

        Private m_IsDesignMode As Boolean
        Private m_HTMLContent As String
        Private m_Args As String
        Private m_Width As Integer

        Public Sub New()
        End Sub

        Public Overridable ReadOnly Property EditMode() As Boolean Implements IModule.EditMode
            Get
                Return False
            End Get
        End Property

        Public Overridable Property IsDesignMode() As Boolean Implements IModule.IsDesignMode
            Get
                Return m_IsDesignMode
            End Get
            Set(ByVal Value As Boolean)
                m_IsDesignMode = Value
            End Set
        End Property

        Public Overridable Property Args() As String Implements IModule.Args
            Get
                Return m_Args
            End Get
            Set(ByVal Value As String)
                m_Args = Value
            End Set
        End Property

        Public Overridable Property Width() As Integer Implements IModule.Width
            Get
                Return m_Width
            End Get
            Set(ByVal Value As Integer)
                m_Width = Value
            End Set
        End Property

        Public Property HTMLContent() As String Implements MasterPages.IModule.HTMLContent
            Get
                Return m_HTMLContent
            End Get
            Set(ByVal Value As String)
                m_HTMLContent = Value
            End Set
        End Property

    End Class
End Namespace