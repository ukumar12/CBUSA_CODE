Imports System.Collections.Specialized

Namespace Components

    Public Class PaymentProcessor

        Protected Output As StringDictionary

        Public Overridable Function SubmitPayment(ByVal Unique As String, ByVal TransactionType As TransactionTypeEnum, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            If TransactionType = TransactionTypeEnum.Authorization Then
                Return SubmitAuthorization(Unique, cc, addr, Amount)
            Else
                Return SubmitSale(Unique, cc, addr, Amount)
            End If
        End Function

        Public Sub New()
        End Sub

        Public Overridable Function SubmitSale(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Return False
        End Function

        Public Overridable Function SubmitAuthorization(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Return False
        End Function

        Public Overridable Function SubmitDelayedCapture(ByVal TransactionNo As String, ByVal Amount As Double) As Boolean
            Return False
        End Function

        Public Overridable Function SubmitCredit(ByVal TransactionNo As String, ByVal Amount As Double, Optional ByVal cc As CreditCardInfo = Nothing) As Boolean
            Return False
        End Function

        Public Overridable Function SubmitVoid(ByVal TransactionNo As String) As Boolean
            Return False
        End Function
        Public Overridable Function GetVerificationResponse() As String
            Return String.Empty
        End Function

        Private m_Username As String = String.Empty
        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal value As String)
                m_Username = value
            End Set
        End Property

        Private m_Password As String = String.Empty
        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal value As String)
                m_Password = value
            End Set
        End Property

        Private m_Custom1 As String = String.Empty
        Public Property Custom1() As String
            Get
                Return m_Custom1
            End Get
            Set(ByVal value As String)
                m_Custom1 = value
            End Set
        End Property

        Private m_Custom2 As String = String.Empty
        Public Property Custom2() As String
            Get
                Return m_Custom2
            End Get
            Set(ByVal value As String)
                m_Custom2 = value
            End Set
        End Property

        Private m_TestMode As Boolean = True
        Public Property TestMode() As Boolean
            Get
                Return m_TestMode
            End Get
            Set(ByVal value As Boolean)
                m_TestMode = value
            End Set
        End Property

        Private m_Timeout As Integer = 10
        Public Property Timeout() As Integer
            Get
                Return m_Timeout
            End Get
            Set(ByVal value As Integer)
                m_Timeout = value
            End Set
        End Property

        Public Overridable ReadOnly Property TransactionNo() As String
            Get
                Return "Configuration Error"
            End Get
        End Property

        Public Overridable ReadOnly Property Result() As Integer
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property ErrorMessage() As String
            Get
                Return "Configuration Error"
            End Get
        End Property

        Public Overridable ReadOnly Property FullResponse() As String
            Get
                Return "Configuration Error"
            End Get
        End Property

        Protected Function RemoveSpecialCharacters(ByVal s As String) As String
            If s = String.Empty Then Return String.Empty

            s = Replace(s, ":", "")
            s = Replace(s, "<", "")
            s = Replace(s, ">", "")
            s = Replace(s, "=", "")
            s = Replace(s, "+", "")
            s = Replace(s, "@", "")
            s = Replace(s, Chr(34), "")
            s = Replace(s, "%", "")
            s = Replace(s, "&", "")
            s = Replace(s, "/", "")
            s = Replace(s, " ", "")
            s = Replace(s, ".", "")
            s = Replace(s, "_", "")
            s = Replace(s, "-", "")
            s = Replace(s, "#", "")

            Return s
        End Function
    End Class

    Public Enum TransactionTypeEnum
        Authorization = 1
        Sale = 2
    End Enum

    Public Class AddressInfo
        Public Company As String
        Public FirstName As String
        Public LastName As String
        Public AddressLn1 As String
        Public AddressLn2 As String
        Public City As String
        Public State As String
        Public Zip As String
        Public Country As String
    End Class

    Public Class CreditCardInfo
        Public CardHolderName As String
        Public CreditCardType As String
        Public CID As String
        Public CreditCardNumber As String
        Public ExpMonth As Integer
        Public ExpYear As Integer
    End Class

End Namespace