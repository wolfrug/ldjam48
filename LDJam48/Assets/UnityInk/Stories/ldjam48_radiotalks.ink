// Spooky radio signals

==RadioLevel0
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Max} Hey Stevie.
{Stevie} Yo.
- else:
{Max} Whatcha doing back here?
{Stevie} Not much.
}
->finishRadio

==RadioLevel1
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:

{Max} Stevie? Do you copy?

{Stevie} There you are!

{Stevie} Finally!

{Max} Everything okay down there? I could barely hear you earlier.

{Stevie} Yeah I’m fine, the signal just crapped out. I had to wander around for a bit to find a good reception.

{Max} Make sense. These old research stations are built like tanks. The hull is pretty thick, so that’s probably our culprit. 

{Stevie} Probably right, I’ll try not to wander too far.

{Stevie} So remind me...what am I looking for down here.

{Max} We need to establish direct access to the Station mainframe. There should be a console in the back somewhere. Take a look.

{Max} If that’s a bust though, we’ll have to use the elevator and try the second floor.

{Max} And if you need anything, I’m just a call away. Max out!

{Stevie} Hey, we agreed! No more stupid sign-off phrases.

{Max} I can’t hear youuuuuu.

{Stevie} Such a child.

-else:
{InteractableLevel1.Complete1:
->InteractableLevel1.Interact2
}
}
{Stevie} What was I doing again?

{InteractableLevel1.Complete1 && InteractableLevel1.Complete2 && not InteractableLevel2.CompleteAll:
{Max} Going down to level two to get me plugged in?

{Stevie} Right! On it.
}
{accessLevels?Level0:
{Max} Coming back topside? Come on, Stevie.
}

->finishRadio

==RadioLevel2
{Max} Hmm...
->finishRadio
=Talk
{Talk>1:
{Stevie} Hello?

{Max} What?

{Stevie} Oh. Nothing...

{not InteractableLevel2.Complete1 && not InteractableLevel2.Complete2:
{Max} Forgot what you were supposed to do? Find me that computer, Stevie.

{Stevie} On it.
->finishRadio
}
{not InteractableLevel2.Complete2:
{Max} The breaker, Stevie.

{Stevie} On it.
->finishRadio
}
{InteractableLevel2.Complete1 && InteractableLevel2.Complete2:
{Max} There's no voice, Stevie. Just come up.

{Stevie} ...
}
->finishRadio
}

{Voice}  *skrr* … *skrrr* … <i>Hello?</i>

{Stevie} What...

{Stevie} Gah, stupid radio is fritzing again. Max! Can you hear me?

{Voice} I *skrrrr*... you *skr*.

{Voice} Please... *skrrr*... help.

{Stevie} Max, this isn’t funny...

{Voice} We’re still down here ....*skrrrr* … Come and find us. <zzt>

{Stevie} What the…

{Stevie} Uhhh, Max? Max, come in.

{Max} *static* What’s that, Stevie? Stupid *skrr* signal.

{Max} Did you say you needed help with something?

{Stevie} No. Not me.

{Stevie} Someone else. Someone...lower down.

{Max} What? No. No way! There’s no one down there, hasn’t been for years.

{Stevie} And yet we both heard it!

{Max} No, I don’t know <i>what</i> I heard. Look just come back up, the system is going through a diagnostic. There’s nothing more we can do.

{Max} I was probably just a phantom recording, an old beacon from the war. 

{Stevie} I guess... 

{Stevie} But it sounded so real. She ...sounded so real.

{Stevie} Shouldn’t we check though, just in case?

{Max} I don't think...

{Stevie} That level access head office granted, how far does it go?

{Max} Just down to 2.

{InteractableLevel2.Complete1:

{Stevie} But you’re in the system now. You can open up the others, can’t you?

{Max} Not a chance. Besides, they locked these places up for a reason.

}

{Max} Let's finish what we came here to do. It's not like we can even go further down right now anyway.

{Max} We can talk more afterwards.

{Stevie} Fine. Flipping a switch then?

{Max} Like always.

->finishRadio

==RadioLevel3
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Voice} Hey Stevie again.
{Stevie} Wooow again.
- else:
{Voice} Still here.
{Stevie} Neat. Cool.
}
->finishRadio

==RadioLevel4
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Voice} Hey Stevie again.
{Stevie} Wooow again.
- else:
{Voice} Still here.
{Stevie} Neat. Cool.
}
->finishRadio

==RadioLevel5
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Voice} Hey Stevie again.
{Stevie} Wooow again.
- else:
{Voice} Still here.
{Stevie} Neat. Cool.
}
->finishRadio

==RadioLevel6
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Voice} Hey Stevie again.
{Stevie} Wooow again.
- else:
{Voice} Still here.
{Stevie} Neat. Cool.
}
->finishRadio
