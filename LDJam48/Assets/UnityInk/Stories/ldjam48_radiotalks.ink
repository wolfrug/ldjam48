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
{Talk>1:

{Stevie} Hello..?

{Stevie} Nothing.

{Max} But me.

{Stevie} Yeah.

{not InteractableLevel3.Complete1 && InteractableLevel3.Interact1:
{Max} Any luck finding that keycard yet?
{CheckItem(item_keycard, 1):
{Stevie} I have it right here.

{Max} Off to see if it fits then.
- else:
{Stevie} Still looking.

{Max} Good luck. Don't let the Rift-monsters bite.
}
- else:
{Max} Getting cold feet? You can always come back topside.
}
->finishRadio
}

{Stevie} Hello? Is anyone there?

{Voice} ...*static*...

{Max} Told you, there’s nothing down here.

{Stevie} Shut up, Max.

{Stevie} Hello…? <i>Am I crazy? Maybe this is crazy, I--

{Voice} *whine skrik* You c…*skrr*... Hello.

{Max} Holy shit.

{Stevie} Hi! Wow, yes, hello there! My name is Stevie, I’m with the Zone 56 maintenance flyers.

{Stevie} Just tell me where you are and I’ll come and get you.

{Voice} *skkrik*... down… *skrr*

{Stevie} Down, on level 4?

{Voice} *schhhrrr* … no….*skrr*... deeper. Follow the sig…*static*.

{Max} Damn, looks like the signal has faded out again.

{Stevie} But you heard her! You saw it, clear as day!

{Max} Yeah...yeah I...I saw it. The signal was live, I could see it on the plane console. But, Stevie.

{Stevie} What?

{Max} It was deep. Like...really deep. We’re already having communication problems. I don’t want to lose you down there.

{Stevie} You won’t lose me. Have I ever let you down before?

{Max} No, but. Be careful would you?

{Stevie} Always.

{Stevie} I’m going to look for a computer terminal, to see if we can jimmy open Level 4.

{Max} Let me know when you’re ready.
->finishRadio

==RadioLevel4
{Max} Hmm...
->finishRadio
=Talk
{Talk<2:
{Stevie} All these mushrooms are giving me the creeps.

{Voice} *skrrr* Don’t worry, they are harmless. *skrr*.

{Stevie} Oh shit, hi again! You’re so much clearer than before.

{Voice} Yes…but I don’t know how long it will hold.

{Stevie} True. Where are you?

{Voice} *skrrrrr* I’m.... on the lowest level.

{Stevie} Are you stuck there? Can you find a way up?

{Voice} No ...*skrik*... way up. Only ...*skrr*... down.
 
{Stevie} But if you’re on the last level then there’s nothing else...down. Just the ocean,

{Voice} You’ll see soon enough.

{Stevie} Okay, look I’m coming to you, just hold on.

{Voice} The levels below you are flooded. Use the…*skriii whine*...

{Stevie} Ah shit, the signal is going again. What did you say?

{Voice} Use the….*skrr*... valves. *zzt*

{Stevie} Damn, she’s gone

{Stevie} Anyone there? Voice…? Max…?
- else:
{Stevie} Max?

{Stevie} Man, I miss your voice...
}
->finishRadio

==RadioLevel5
->finishRadio
=Talk
{RadioLevel5<2:
{Stevie} Is this thing working?

{Stevie} Are you there?

{Voice} It’s working.

{Stevie} Nice to finally hear you - properly I mean.

{Voice} You’re almost here.

{Stevie} Yes, almost. Though I’m still not sure why. You asked for my help in the beginning, but I get the feeling there’s more to this.

{Voice} Isn’t there always… But I didn’t lie, if that’s what is troubling you. I do need your help, very badly.

{Voice} I’ve been holding on for so long now, but it won’t be long now.

{Voice} Come down when you’re ready, and I’ll explain. But first, a favour?

{Stevie} Sure, why not.

{Voice} There is a dead man on the other side of the level you are on. Don’t be alarmed! He’d been dead for a long time now, and there’s nothing to fear. But he has something, in his coat pocket…

{Voice} Bring it to me?

{Stevie} I… I’ll see what I can do.
- else:
{Stevie} Hello?

{Stevie} ...too much to ask I guess.
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
