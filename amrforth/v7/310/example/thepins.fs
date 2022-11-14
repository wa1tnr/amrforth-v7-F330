\ thepins.fs -- f310 example program
\ Mon 14 Nov 13:20:06 UTC 2022
\ Unverified - commit as-is 14 Nov 2022

\ thepins.fs is much bigger than safepins.fs is.

\ older (than 14 Nov 2022):
\ 13 April 2022

0 [if]
[then]

\ main.fs

code startup
	$40 invert # PCA0MD anl  \ Fri 15 Apr 13:20z CHECKED datasheet GOOD reset bit 6 only
	$ff # P0MDIN orl   \ No analog, all digital, port 0.x
        $ff # P1MDIN orl   \ No analog, all digital - port 1.x
        $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
        $ff # P1MDOUT orl  \ all pins output, push pull.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
        7 .P1 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        5 .P1 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        3 .P1 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        1 .P1 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        \ 0 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        \ 1 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        \ 2 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        \ 3 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
	next c;


0 [if]
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
        $ff # P1MDIN orl   \ No analog, all digital - port 1.x
        $ff # P1MDOUT orl  \ all pins output, push pull.
	$01 # XBR0 mov  \ Route TX and RX on P0.4, P0.5.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
        2 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
        0 .P0 clr
        1 .P0 clr
     \  2 .P0 clr
        3 .P0 clr
        6 .P0 clr
        7 .P0 clr
        0 .P1 clr
        1 .P1 clr
        2 .P1 clr
        3 .P1 clr
        4 .P1 clr
        5 .P1 clr
        6 .P1 clr
        7 .P1 clr
[then]


	\ next c;

0 [if]
code allrlow (  - ) 

        0 .P0 clr
        1 .P0 clr
        2 .P0 clr
        3 .P0 clr
        6 .P0 clr
        7 .P0 clr
        0 .P1 clr
        1 .P1 clr
        6 .P1 clr
        7 .P1 clr
	next c;

code enbl (  - ) 0 .P2 clr  next c;
code dsbl (  - ) 0 .P2 setb next c;

code allrhi (  - )
        0 .P0 setb
        1 .P0 setb
        2 .P0 setb
        3 .P0 setb
        6 .P0 setb
        7 .P0 setb
        0 .P1 setb
        1 .P1 setb
        2 .P1 setb
        3 .P1 setb
        4 .P1 setb
        5 .P1 setb
        6 .P1 setb
        7 .P1 setb
	next c;


code elA (  - )
        7 .P0 setb
	next c;


code elB (  - )
        1 .P1 setb
	next c;


code elC (  - )
        1 .P0 setb
	next c;

code elD (  - )
        3 .P0 setb
	next c;

[then]
