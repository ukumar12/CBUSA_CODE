Option Strict Off

Imports Components
Imports DataLayer

Partial Class Callout
    Inherits ModuleControl

    Protected m_Type As String = String.Empty
    Protected m_Index As Integer = 0

    Public Property Type() As String
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpType.SelectedValue = "" Then
                    Return 0
                Else
                    Return CType(drpType.SelectedValue, String)
                End If
            Else
                Return m_Type
            End If
        End Get
        Set(ByVal value As String)
            If IsAdminDisplay And IsDesignMode Then
                drpType.SelectedValue = value
            Else
                m_Type = value
            End If
        End Set
    End Property

    Public Property Index() As Integer
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpIndex.SelectedValue = "" Then
                    Return 0
                Else
                    Return CType(drpIndex.SelectedValue, Integer)
                End If
            Else
                Return m_Index
            End If
        End Get
        Set(ByVal value As Integer)
            If IsAdminDisplay And IsDesignMode Then
                drpIndex.SelectedValue = value
            Else
                m_Index = value
            End If
        End Set
    End Property

    Public Overrides Property Args() As String
        Get
            Return "Index=" & Index & "&Type=" & Type
        End Get
        Set(ByVal Value As String)
            If Value = String.Empty Then Exit Property
            Dim Pairs() As String = Value.Split("&"c)

            If Pairs.Length >= 1 Then
                Dim IndexValues() As String = Pairs(0).Split("="c)
                If IndexValues.Length > 0 Then
                    Index = CType(IndexValues(1), Integer)
                End If
            End If

            If Pairs.Length >= 2 Then
                Dim TypeValues() As String = Pairs(1).Split("="c)
                If TypeValues.Length > 0 Then
                    Type = CType(TypeValues(1), String)
                End If
            End If
            
        End Set
    End Property


    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        divDesigner.Visible = IsDesignMode

        If IsDesignMode Then
            If IsPostBack Then
                CheckPostData(Controls)
            End If
            hdnField.Value = Args
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Not IsAdminDisplay Then
                If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                    m_Type = "Builder"
                ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                    m_Type = "Vendor"
                ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                    m_Type = "PIQ"
                End If
            End If
            
            If IsAdminDisplay And IsDesignMode Then

                drpIndex.Items.Insert(0, New ListItem("1", "1"))
                drpIndex.Items.Insert(0, New ListItem("2", "2"))

                drpType.Items.Insert(0, New ListItem("Builder", "Builder"))
                drpType.Items.Insert(0, New ListItem("Vendor", "Vendor"))
                drpType.Items.Insert(0, New ListItem("PIQ", "PIQ"))

            End If

            LoadData()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub LoadData()

        Dim SQL As String = String.Empty
        Dim CallOut As String

        Try

            'SQL = "SELECT CallOut" & Index.ToString & " FROM " & Type & "CallOut"
            SQL = "SELECT CallOut1 FROM " & Type & "CallOut"

            If Index <> 0 And Type <> String.Empty Then
                CallOut = CType(Me.DB.ExecuteScalar(SQL), String)
                If CallOut <> String.Empty Then
                    Me.divContent.InnerHtml = "<div class=""splitcollft"">" & CallOut & "<br /></div>"
                End If
            End If

            SQL = "SELECT CallOut2 FROM " & Type & "CallOut"

            If Index <> 0 And Type <> String.Empty Then
                CallOut = CType(Me.DB.ExecuteScalar(SQL), String)
                If CallOut <> String.Empty Then
                    Me.divContent.InnerHtml &= "<div class=""splitcolrt"">" & CallOut & "<br /></div>"
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub

End Class
