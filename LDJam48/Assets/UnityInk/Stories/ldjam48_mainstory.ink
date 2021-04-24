INCLUDE ldjam48_functions.ink
INCLUDE ldjam48_journal.ink

// Default main story object - always have a stitch named ==start

{debug:
->start
}

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

==finishRadio
->DONE

==inspectTest
It's a thingie. Yay!
->DONE

==inspect_level1_test1
Looks like everything is fine here.
->finishRadio

==inspect_level1_test2
Yep, that is a placeholder.
->finishRadio

==inspect_level2_test1
I made the background sprite a little darker, did you notice?.
->finishRadio

==inspect_level2_test2
There's nothing else to do now.
->finishRadio