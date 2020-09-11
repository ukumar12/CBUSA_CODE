Imports Components
Imports Utility
Imports System.Data
Imports DataLayer

Partial Class MemberAddress
    Inherits BaseControl

    Private m_Address As MemberAddressRow
    Public Property Address() As MemberAddressRow
        Get
            Return m_Address
        End Get
        Set(ByVal value As MemberAddressRow)
            m_Address = value
        End Set
    End Property

End Class
