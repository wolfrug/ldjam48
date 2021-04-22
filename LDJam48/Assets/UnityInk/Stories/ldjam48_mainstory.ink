INCLUDE ldjam48_functions.ink
INCLUDE ldjam48_journal.ink

// Default main story object - always have a stitch named ==start

==start
This is the start of the story.

Do we have test1? {CheckItem(test1, 1): Yep, we have {print_num(checkItem)} amount! | Nope}

->Say("This is a dialogue", "MyCharacter")->

+ {CheckItem(test1, 1)} [We have one of item1, let's eat it]
{Consume(test1, 1)}
Om nom.
->DONE
+ [Open the journal] ->OpenJournalInt->

- And that's all for now.

+ [Close writer.] ->DONE

==inspectTest
It's a thingie. Yay!
->DONE