\ thepins.fs -- f300 simple program

code startup
        \ XBR2 bit 7 WEAKPUD bit 6 XBARE
        \ bit 7 reset: weak pull-ups enabled.
        \ bit 6   set: crossbar enabled.
	$40 # XBR2 mov  \ Enable crossbar and weak pull-ups. TODO
	next c;

\ end.
