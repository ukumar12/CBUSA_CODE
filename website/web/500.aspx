<%
   Response.Status = "500 Internal Server Error"
%> 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
    <head>
		<title>Error</title>
        <meta name="robots" content="noindex, nofollow" />
        <style type="text/css">
        <!--
       	body {font-family:helvetica,arial,sans-serif; font-size:80%; color:#000; background-color:#fff;}
        p {font-size:1em; color:#000; margin:0 0 8px 0;}
        td {font-size:1em; color:#636363;}
        a {color:#059;text-decoration:underline;}
        a:link {color: #059;}
        a:visited {color: #059;}
        a:hover {color: #666;}
        a:active {color: #059;}
        -->
        </style>
    </head>
    <body>


            <table width="600" cellpadding="15" cellspacing="0" style="border:1px solid #059; margin:100px auto;">
                <tr>
                    <td>

                        <p style="text-align:center;"><img src="/images/global/cbusa-320x120.gif" style="width:320px; height:120px; border-style:none;" alt="CBUSA" /><br /></p>

						<p style="font-weight:bold; font-size:1.1em; text-align:center;">Sorry, an error has occurred on the page you are trying to access.</p>

						<p>It is possible that the page has been moved, or you typed the address incorrectly. Please click the link below to go to the CBUSA Portal home page.</p>

						<p style="text-align:center; font-size:1.2em; font-weight:bold;"><a href="http://<%=Request.ServerVariables("SERVER_NAME")%>">CBUSA Portal</a></p>


                    </td>
                </tr>
            </table>
        </div>
    </body>
</html>