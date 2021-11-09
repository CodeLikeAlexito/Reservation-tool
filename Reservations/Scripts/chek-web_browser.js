
/* If Web Browser is IE11 Redirect to IEException */
function checkIfItIsIE() {
    if (!sessionStorage.alreadyClicked) {
        //console.log("Here");
        if (!!window.MSInputMethodContext && !!document.documentMode) //IF IE > 10
        {
            //console.log("IE");
            window.location.href = '/Error/IEException/';
            //var myUrl = '<%= Url.Action("IEException", "Error") %>';
            //var url = '@Url.Action("IEException", "Error")';
            //window.location.href = url; 
            console.log('IE');
        }
        sessionStorage.alreadyClicked = "true";
    }
    
}

$(document).ready(function () {
    checkIfItIsIE();
});


