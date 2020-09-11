Namespace MasterPages

    Public Interface IModule
        Property Args() As String
        ReadOnly Property EditMode() As Boolean
        Property HTMLContent() As String
        Property IsDesignMode() As Boolean
        Property Width() As Integer
    End Interface

End Namespace
