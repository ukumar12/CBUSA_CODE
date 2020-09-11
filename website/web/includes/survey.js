function ChoiceSelected(m_QuestionId, m_ChoiceId){
    var aryChoices = document.getElementsByTagName('INPUT');
    for (u=0;u<aryChoices.length;u++){
        if(aryChoices[u].getAttribute('QuestionId') == m_QuestionId && aryChoices[u].getAttribute('ChoiceId') != m_ChoiceId){
            aryChoices[u].checked = false;
        }        
    }
}
