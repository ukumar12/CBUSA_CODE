var ND="display:''";
function X(n){var a=document.getElementById('A'+n).style;var i=document.getElementById('I'+n);var b=a.display=='none';a.display=b?'block':'none';i.src=b?i.src.replace(/collapsed/i,'expanded'):i.src.replace(/expanded/i,'collapsed');return false;}

