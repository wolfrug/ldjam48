VAR isJournalOpen = false
VAR journal_last_open_tab = ->Journal.journal_1

==OpenJournalExt==
// This is called externally, and ends with a ->DONE
// We close it if it is already open
{not isJournalOpen:
#openJournal
~isJournalOpen = true
<i></i>
->Journal->
#closeJournal
~isJournalOpen = false
<i></i>
->DONE
- else:
#closeJournal
~isJournalOpen = false
<i></i>
->DONE
}


==OpenJournalInt==
// This is called internally, and ends with a tunnel
#openJournal
~isJournalOpen = true
<i></i>
->Journal->
#closeJournal
~isJournalOpen = false
<i></i>
->->

==CloseJournal(continue)
#closeJournal
~isJournalOpen = false
<i></i>
{continue:
->->
- else:
->DONE
}

==Journal
// OPENING THE JOURNAL TO LAST SAVED SPOT
->journal_last_open_tab

=journal_1
<-navigation
{UseText("journal_description")}Tab one
+ [Entry 1]
<-navigation
Tab one text
++[Back]->journal_1
// FINAL ENTRY
- ->journal_1

=journal_2
<-navigation
{UseText("journal_description")}Tab two
+ [Entry 1]
<-navigation
Tab two text
++[Back]->journal_2
// FINAL ENTRY
- ->journal_2

=journal_3
<-navigation
{UseText("journal_description")}Tab three
+ [Entry 1]
<-navigation
Tab three text
++[Back]->journal_3
// FINAL ENTRY
- ->journal_3

=navigation
+ [{UseButton("journal_1")}Tab 1]
~journal_last_open_tab = ->Journal.journal_1
->journal_1
+ [{UseButton("journal_2")}Tab 2]
~journal_last_open_tab = ->Journal.journal_2
->journal_2
+ [{UseButton("journal_3")}Tab 3]
~journal_last_open_tab = ->Journal.journal_3
->journal_3
+ [{UseButton("journal_quit")}X]
->closeJournal

=closeJournal
{UseText("journal_description")} <br>
->->