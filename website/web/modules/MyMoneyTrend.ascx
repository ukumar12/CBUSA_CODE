<%@ Control EnableViewstate="False" Language="VB" AutoEventWireup="false" CodeFile="MyMoneyTrend.ascx.vb" Inherits="_default" %>
<div class="mmheader">
    <p class="mmtitle">Money Trends</p>
</div>

<div>
    <OBJECT classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0" width="500" height="600" name="LineChart">
        <param name="allowScriptAccess" value="always" />
        <param name="movie" value="/FusionCharts/FCF_Line.swf" />
        <param name="FlashVars" value="&chartWidth=500&chartHeight=500&debugMode=0&dataXML=<%=LineChartXML() %>" />
        <param name="quality" value="high" />
        <param name="wmode" value="transparent" />
    <embed src="/FusionCharts/FCF_Line.swf" flashVars="&chartWidth=500&chartHeight=500&debugMode=0&dataXML=<%=LineChartXML() %>" wmode="transparent" quality="high" width="500" height="600" name="LineChart" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
    </object>    
</div>
<br />                