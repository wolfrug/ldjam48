// For level inspect stuff

==endInspect
->DONE

//// THIS IS THE TEMPLATE START - NOTE THIS IS LEVEL0, WE DON'T NEED IT
==InspectLevel0
{Stevie} Hmmm...
->endInspect
=Item1
{Item1<2:
{Stevie} Curious.
- else:
{Stevie} I've seen it already.
}
->endInspect
=Item2
{Item2<2:
{Stevie} Huh.
- else:
{Stevie} Still there.
}
->endInspect

=Item3
{Item2<2:
{Stevie} Ah, interactable!.
- else:
{Stevie} Still an interactable..
}
->endInspect
/// THIS IS THE TEMPLATE END


==InspectLevel1
{Stevie} Hmmm...
->endInspect
=Item1
{Item1<2:
{Stevie} Curious.
- else:
{Stevie} I've seen it already.
}
->endInspect
=Item2
{Item2<2:
{Stevie} Huh.
- else:
{Stevie} Still there.
}
->endInspect

=Item3
{Item2<2:
{Stevie} Ah, interactable!.
- else:
{Stevie} Still an interactable..
}
->endInspect
