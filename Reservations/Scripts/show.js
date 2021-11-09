// This function gets called when the end-user clicks on some date.

function selected(cal, date) {
  cal.sel.value = date; // just update the date in the input field.
  cal.callCloseHandler();
}


// And this gets called when the end-user clicks on the _selected_ date,
// or clicks on the "Close" button.  It just hides the calendar without
// destroying it.

function closeHandler(cal) {
  cal.hide();                        // hide the calendar
}



// This function shows the calendar under the element having the given id.
// It takes care of catching "mousedown" signals on document and hiding the
// calendar if the click was outside.

function showCalendar(id)
{
  var el = document.getElementById(id);
  //*
  if(calendar != null)
  { // we already have some calendar created
    calendar.hide();                 // so we hide it first.
  }
  else
  {
    // first-time call, create the calendar.
    var cal = new Calendar(true, null, selected, closeHandler);
    // cal.weekNumbers = false;
    calendar = cal;                  // remember it in the global var
    cal.setRange(1900, 2070);        // min/max year allowed.
    cal.create();
  }
  calendar.setDateFormat("dd.mm.y");    // set the specified date format
  calendar.parseDate(el.value);      // try to parse the text in field
  calendar.sel = el;                 // inform it what input field we use
  calendar.showAtElement(el);        // show the calendar below it
  //*/

  return false;
}



