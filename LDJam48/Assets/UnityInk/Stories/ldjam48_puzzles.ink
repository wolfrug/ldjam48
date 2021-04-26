// For nitty gritty mechanics

=debugPuzzles
Debug which puzzle?
+ [Level0Interactable 1]
->InteractableLevel0.Interact1
+ [Level0Interactable 2]
->InteractableLevel0.Interact2


==stopInteract
{debug: ->debugPuzzles}
->DONE

/// TEMPLATE
===InteractableLevel0
{Stevie} What was I doing?
->stopInteract
=Interact1
{Complete1<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete1
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete1
{Stevie} Easy peasy.
->CompleteAll

=Interact2
{Complete2<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete2
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete2
{Stevie} Easy peasy.
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Stevie} Solved! On to the next level.
{AddElevatorLevel(Level1)}
- else:
{Stevie} Still stuff to be done.
}
->stopInteract

// END TEMPLATE

===InteractableLevel1
{Stevie} What was I doing?
->stopInteract
=Interact1
{Complete1<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete1
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete1
{Stevie} Easy peasy.
->CompleteAll

=Interact2
{Complete2<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete2
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete2
{Stevie} Easy peasy.
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Stevie} Solved! On to the next level.
{AddElevatorLevel(Level2)}
- else:
{Stevie} Still stuff to be done.
}
->stopInteract