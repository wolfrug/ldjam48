INCLUDE default_functions.ink
INCLUDE default_journal.ink

// Default main story object - always have a stitch named ==start

==start
This is the start of the story.

->Say("This is a dialogue", "MyCharacter")->

+ [Open the journal] ->OpenJournalInt->

- And that's all for now.

+ [Close writer.] ->DONE