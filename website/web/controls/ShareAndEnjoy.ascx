<%@ Control Language="VB" EnableViewState="False" AutoEventWireup="false" CodeFile="ShareAndEnjoy.ascx.vb" Inherits="ShareAndEnjoy" %>
<a href="http://digg.com/submit?phase=2&amp;url=<%=url%>&amp;title=<%=Server.UrlEncode(title)%>" title="Digg"><img src="/images/share/digg.png" title="Digg" alt="Digg" /></a>
<a href="http://del.icio.us/post?url=<%=url%>&amp;title=<%=Server.UrlEncode(title)%>" title="del.icio.us"><img src="/images/share/delicious.png" title="del.icio.us" alt="del.icio.us" /></a>
<a href="http://reddit.com/submit?url=<%=url%>&amp;title=<%=Server.UrlEncode(title)%>" title="Reddit"><img src="/images/share/reddit.png" title="Reddit" alt="Reddit" /></a>
<a href="http://www.newsvine.com/_tools/seed&amp;save?u=<%=url%>&amp;h=<%=Server.UrlEncode(title)%>" title="NewsVine"><img src="/images/share/newsvine.png" title="NewsVine" alt="NewsVine" /></a>
<a href="http://www.netvouz.com/action/submitBookmark?url=<%=url%>&amp;title=<%=Server.UrlEncode(title)%>;popup=no" title="Netvouz"><img src="/images/share/netvouz.png" title="Netvouz" alt="Netvouz" /></a>
<a href="http://www.dzone.com/links/add.html?url=<%=url%>&amp;title=<%=Server.UrlEncode(title)%>" title="DZone"><img src="/images/share/dzone.png" title="DZone" alt="DZone" /></a>
<a href="http://www.thisnext.com/pick/new/submit/sociable/?url=<%=url%>&amp;name=<%=Server.UrlEncode(title)%>" title="ThisNext"><img src="/images/share/thisnext.png" title="ThisNext" alt="ThisNext" /></a>
<a href="http://myweb2.search.yahoo.com/myresults/bookmarklet?u=<%=url%>&amp;t=<%=Server.UrlEncode(title)%>" title="YahooMyWeb"><img src="/images/share/yahoomyweb.png" title="YahooMyWeb" alt="YahooMyWeb" /></a>
<a href="http://slashdot.org/bookmark.pl?title=<%=Server.UrlEncode(title)%>&amp;url=<%=url%>" title="Slashdot"><img src="/images/share/slashdot.png" title="Slashdot" alt="Slashdot" /></a>
