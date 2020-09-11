<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailingSteps.ascx.vb" Inherits="MailingSteps" %>


<% if CurrentStep = 1 Then %>
<table cellpadding="0" cellspacing="0">
<tr>
<td><div><a href="" onclick="return false;" class="L3">1: Define Layout</a></div></td>
<%  If Step1 Then%>
<td><div><a href="target.aspx?MessageId=<%=MessageId%>" class="L1">2: Target Audience</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">2: Target Audience</a></div></td>
<% end if %>
<%  If Step2 Then%>
<td><div><a href="review.aspx?MessageId=<%=MessageId%>" class="L1">3: Test/Preview</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">3: Test/Preview</a></div></td>
<% end if %>
<%  If Step3 Then%>
<td><div><a href="send.aspx?MessageId=<%=MessageId%>" class="L1">4: Send/Schedule</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">4: Send/Schedule</a></div></td>
<% end if %>
</tr>
</table>

<%  ElseIf CurrentStep = 2 Then%>

<table cellpadding="0" cellspacing="0">
<tr>
<td><div><a href="layout.aspx?MessageId=<%=MessageId%>" class="L1">1: Define Layout</a></div></td>
<td><div><a href="" onclick="return false;" class="L3">2: Target Audience</a></div></td>
<% if STEP2 then %>
<td><div><a href="review.aspx?MessageId=<%=MessageId%>" class="L1">3: Test/Preview</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">3: Test/Preview</a></div></td>
<% end if %>
<% if STEP3 then %>
<td><div><a href="send.aspx?MessageId=<%=MessageId%>" class="L1">4: Send/Schedule</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">4: Send/Schedule</a></div></td>
<% end if %>
</tr>
</table>

<%  ElseIf CurrentStep = 3 Then%>

<table cellpadding="0" cellspacing="0">
<tr>
<td><div><a href="layout.aspx?MessageId=<%=MessageId%>" class="L1">1: Define Layout</a></div></td>
<%  If Step1 Then%>
<td><div><a href="target.aspx?MessageId=<%=MessageId%>" class="L1">2: Target Audience</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">2: Target Audience</a></div></td>
<% end if %>
<td><div><a href="" onclick="return false;" class="L3">3: Test/Preview</a></div></td>
<%  If Step3 Then%>
<td><div><a href="send.aspx?MessageId=<%=MessageId%>" class="L1">4: Send/Schedule</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">4: Send/Schedule</a></div></td>
<% end if %>
</tr>
</table>

<%  ElseIf CurrentStep = 4 Then%>

<table cellpadding=0 cellspacing=0>
<tr>
<% if STEP1 then %>
<td><div><a href="layout.aspx?MessageId=<%=MessageId%>" class="L1">1: Define Layout</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">1: Define Layout</a></div></td>
<% end if %>
<% if STEP2 then %>
<td><div><a href="target.asp?MessageId=<%=MessageId%>" class="L1">2: Target Audience</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">2: Target Audience</a></div></td>
<% end if %>
<% if STEP3 then %>
<td><div><a href="review.aspx?MessageId=<%=MessageId%>" class="L1">3: Test/Preview</a></div></td>
<% else %>
<td><div><a href="" onclick="return false;" class="L2">3: Test/Preview</a></div></td>
<% end if %>
<td><div><a href="" onclick="return false;" class="L3">4: Send/Schedule</a></div></td>
</tr>
</table>

<%  End if %>
