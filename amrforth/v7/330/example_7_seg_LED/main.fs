\ main.fs -- f330 example program

\ Thu Mar  8 01:52:50 UTC 2018

\ 7-segment display - ignore references to older 14-segment display.

\ 09 Feb - quick mod for 14-segment alphanumeric common cathode

\ Sat Feb 10 02:14:44 UTC 2018
\ Fri Feb  9 19:47:02 UTC 2018
\ Fri Feb  9 17:40:24 UTC 2018

\ PROBLEM: after reset all pins source Vcc 3.3 which would
\ load the heck out of Vcc/GND with very low current-limiting
\ resistors (should be 500 ohms anyway).

\ +3.3 Vcc also turns on the darlingtons that sink the LED array common cathodes.

\ So, the default after power-on reset is to turn the entire display on!



\ 37 and 42 are cathodes

\ 35 is lowest pin number on breadboard.
\ 43 is highest pin number on breadboard.


\ V.

\     35      36      37      38      39      40      41      42      43
\   P0.7    P1.2       x    P1.3    P1.4    P1.0    P0.0       x    P0.1

\   P0.6    P1.5       n    P1.6    P1.7    P1.1    P0.3            P0.2
\     35      36      37      38      39      40      41      42      43


\ main.fs

code startup
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
\ ----- initialization code goes here, before MAIN.

	$ff # P0MDIN orl   \ No analog, all digital, port 0.x
      \ $04 # P0MDOUT orl  \ P0.2 output, push pull.
      \ $04 # P0MDOUT orl  \ P0.2 output, push pull.
      \ $0e # P0MDOUT orl  \ P0.2 output, push pull.  \ P0.1 P0.3 also outputs.
        $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5

        $ff # P1MDIN orl   \ No analog, all digital - port 1.x
        $ff # P1MDOUT orl  \ all pins output, push pull.

        \ does not exist - P2MDIN $01 # P2MDIN orl   \ No analog, all digital - port 1.x
        $01 # P2MDOUT orl  \ push pull

  \ 7654 3210
  \ 11-- 1111 $nf  $cf

  \ 7654 3210
  \ 0000 1110

	$01 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5.

\	$09 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5, also sysclk on P0.0

	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
\ Setup serial port.
	\ $c3 # OSCICN mov  \ Full speed internal, 24.5 MHz.
	\ $00 # CKCON mov  \ T1 uses SYSCLK/12.
	\ $12 # SCON0 mov  \ 8 bit UART mode, TX ready.
	\ $20 # TMOD mov  \ Mode 2, 8 bit auto-reload.
	\ $96 # TH1 mov  \ 9600 baud, at 24.5MHz.
	\ 6 .TCON setb  \ Enable Timer 1.

        2 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.

        0 .P0 clr
        1 .P0 clr
     \  2 .P0 clr
        3 .P0 clr
        6 .P0 clr
        7 .P0 clr
        0 .P1 clr
        1 .P1 clr
        2 .P1 setb
        3 .P1 setb
        4 .P1 setb
        5 .P1 setb
        6 .P1 clr
        7 .P1 clr

\ 1. The ULN2804A Darlington sinks current to ground.
\ 2. Bring the Darlington's input to Vcc, to enable its output,
\    which will light the common-cathode LED array.

\ 3. The PN2222A NPN transistor provides the signal to the ULN2804A input.
\ 4. Bring the PN2222A's input to Vcc to *disable* the Darlington; doing
\    so provides a ground on the Darlington transistor's input lead,
\    disabling its output.  Note that if there is no connection to the
\    darlington's input, it will not turn on; you have to raise its
\    input to Vcc to get it to conduct.
\  
\    Whereas, with the PN2222A NPN transistor, if you leave its input
\    floating, its output will rise to Vcc (thus switching on the
\    Darlington's input!) because the output of the NPN is effectively
\    on a pullup to Vcc.
\ 
\    So, in this case, if there's a broken wire between the port pin
\    and the input to the NPN, the display will come on, unintentionally.
\  
\    Not exactly fail-safe during prototyping, but better than not
\    doing even this much for prevention.



     \  0 .P2 clr \ NPN driving UNL2804A input.
                  \ Bring P2.0 LOW to enable the darlington's input
                  \ (NPN drives the darlington input positive)
                  \ which will in turn, ground the common cathodes
                  \  and light the display.

	next c;

code allrlow (  - ) 

        0 .P0 clr
        1 .P0 clr
        2 .P0 clr
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
     \  0 .P2 clr \ NPN driver.  Bring P2.0 LOW to ground the common cathodes and light the display.
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
        \ 0 .P2 setb \ NPN driver.  Bring P2.0 HIGH to disable the display.
	next c;


code elA (  - )
        0 .P0 setb
	next c;


code elB (  - )
        1 .P0 setb
	next c;


code elC (  - )
        2 .P0 setb
	next c;


code elD (  - )
        3 .P0 setb
	next c;


code elE (  - )
        6 .P0 setb
	next c;


code elF (  - )
        7 .P0 setb
	next c;


code elG (  - )
        0 .P1 setb
	next c;


code elDP (  - )
        1 .P1 setb
	next c;


\ hjk  lmn


code clrdg0-3 (  - )
        2 .P1 setb
        3 .P1 setb
        4 .P1 setb
        5 .P1 setb
	next c;

: clrdgts clrdg0-3 ;

code dg0c (  - )
        2 .P1 clr
	next c;

: dg0 (  - )
        clrdgts
        dg0c ;


code dg1c (  - )
        3 .P1 clr
	next c;

: dg1 (  - )
        clrdgts
        dg1c ;


code dg2c (  - )
        4 .P1 clr
	next c;

: dg2 (  - )
        clrdgts
        dg2c ;


code dg3c (  - )
        5 .P1 clr
	next c;

: dg3 (  - )
        clrdgts
        dg3c ;

code elM (  - )
        6 .P1 setb
	next c;


code elN (  - )
        7 .P1 setb
	next c;


code wink  (  - ) 2 .P0 cpl  next c;
code dark  (  - ) 2 .P0 clr   next c;
code light (  - ) 2 .P0 setb  next c;

code pins123 (  - )  1 .P0 setb  \ make LEDs attached to them bright; output is an NPN feeding a PNP transistor
                     2 .P0 setb
                     3 .P0 setb next c;

code !pins123 (  - ) 1 .P0  clr  \ make the attached LEDs dark
                     2 .P0  clr
                     3 .P0  clr next c;

\ : delay  (  - ) $1111 for next ;
: delay  (  - ) 40 ms ;

: ldelay  (  - ) \ sizeable delay
  400 ms ;

: wink-all (  - ) \ blink P0.1 P0.2 P0.3
  pins123 ldelay !pins123 ldelay ;

: wink-them (  - ) \ repeat blink P0.1 P0.2 P0.3
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all wink-all wink-all
  wink-all ;

: ssdelay (  - ) \ short delay
  50 ms 

  \ intent is maybe 10 ms but let's expand that wildly:


;

: blink  (  - ) wink delay delay delay wink ;

: mowink (  - ) light ssdelay dark 600 ms ;

: kanufb (  - ) dark ldelay light ldelay ;
: kanuf  (  - ) kanufb  kanufb  kanufb  kanufb  kanufb  kanufb  ;

: pialgo (  - ) kanuf kanuf kanuf ;






: blank (  - ) 
  allrlow ;

: hblank (  - )
  2 ms dsbl blank 55 us \ 1 ms blank time formerly
  ;

\ abcEFGlGR
: paintA (  - ) 
  enbl elA hblank
  enbl elB hblank
  enbl elC hblank
\ enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;


\ hjk  lmn





\ AEFGl
: paintF (  - ) 
  enbl elA hblank
\ enbl elB hblank
\ enbl elC hblank
\ enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

\ pF, D
: paintE (  - )
  paintF
  enbl elD hblank
  ;



\ pE, KN
: paintB (  - ) 
\ enbl elA hblank
\ enbl elB hblank
  enbl elC hblank
  enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

: paintEIGHT (  - ) 
  enbl elA hblank
  enbl elB hblank
  enbl elC hblank
  enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

: paint5 (  - ) 
  enbl elA hblank
  enbl elF hblank
  enbl elG hblank
  enbl elC hblank
  enbl elD hblank
\ enbl elB hblank
\ enbl elE hblank
  ;

: paint6 (  - ) 
  enbl elA hblank
\ enbl elB hblank
  enbl elC hblank
  enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

: paint9 (  - ) 
  enbl elA hblank
  enbl elB hblank
  enbl elC hblank
\ enbl elD hblank
\ enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

: paint4 (  - ) 
\ enbl elA hblank
  enbl elB hblank
  enbl elC hblank
\ enbl elD hblank
\ enbl elE hblank
  enbl elF hblank
  enbl elG hblank
  ;

: paint3 (  - ) 
  enbl elA hblank
  enbl elB hblank
  enbl elC hblank
  enbl elD hblank
\ enbl elE hblank
\ enbl elF hblank
  enbl elG hblank
  ;

\ abcdefGlGR
: paint2 (  - ) 
  enbl elA hblank
  enbl elB hblank
  enbl elG hblank
  enbl elE hblank
  enbl elD hblank
\ enbl elC hblank
\ enbl elF hblank
  ;

\ abcdefGlGR
: paint8 (  - ) 
  enbl elC hblank
  enbl elF hblank
  paint2
  ;

\ ABCDJM
: paintD (  - )
\ enbl elA hblank
  enbl elB hblank
  enbl elC hblank
  enbl elD hblank
  enbl elE hblank
\ enbl elF hblank
  enbl elG hblank
  ;

\ ADEF
: paintC (  - ) 
  enbl elA hblank
  enbl elD hblank
  enbl elE hblank
  enbl elF hblank
  ;

\ ABCDEF
: paint0 (  - ) 
  paintC
\ enbl elA hblank
  enbl elB hblank
  enbl elC hblank
\ enbl elD hblank
\ enbl elE hblank
\ enbl elF hblank
\ enbl elG hblank
  ;

\ BC
: paint1 (  - ) 
\ enbl elA hblank
  enbl elB hblank
  enbl elC hblank
\ enbl elD hblank
\ enbl elE hblank
\ enbl elF hblank
\ enbl elG hblank
  ;

\ ABC
: paint7 (  - )
  enbl elA hblank
  paint1
;

: paint (  - ) paint7 ;

: delcount 400 ;

: painta_F 1 begin paintF
      1 + dup delcount = if drop exit then again ;

: painta_D 1 begin paintD
      1 + dup delcount = if drop exit then again ;

: painta_C 1 begin paintC
      1 + dup delcount = if drop exit then again ;


: painta_A 1 begin paintA
      1 + dup delcount = if drop exit then again ;

: painta_B 1 begin paintB
      1 + dup delcount = if drop exit then again ;

: painta_E 1 begin paintE
      1 + dup delcount = if drop exit then again ;

: painta_0 1 begin paint0
      1 + dup delcount = if drop exit then again ;

: painta_1 1 begin paint1
      1 + dup delcount = if drop exit then again ;

: painta_2 1 begin paint2
      1 + dup delcount = if drop exit then again ;

: painta_3 1 begin paint3
      1 + dup delcount = if drop exit then again ;

: painta_4 1 begin paint4
      1 + dup delcount = if drop exit then again ;

: painta_5 1 begin paint5
      1 + dup delcount = if drop exit then again ;

: painta_6 1 begin paint6
      1 + dup delcount = if drop exit then again ;

: painta_7 1 begin paint7
      1 + dup delcount = if drop exit then again ;

: painta_8 1 begin paint8
      1 + dup delcount = if drop exit then again ;

: painta_9 1 begin paint9
      1 + dup delcount = if drop exit then again ;


: lxdelay ldelay ldelay ldelay ldelay ldelay ;

\ : iter vipe3 lxdelay ;

: iterA painta_A lxdelay ;
: iterB painta_B lxdelay ;
: iterC painta_C lxdelay ;
: iterD painta_D lxdelay ;
: iterE painta_E lxdelay ;
: iterF painta_F lxdelay ;
: iter0 painta_0 lxdelay ;
: iter1 painta_1 lxdelay ;
: iter2 painta_2 lxdelay ;
: iter3 painta_3 lxdelay ;
: iter4 painta_4 lxdelay ;
: iter5 painta_5 lxdelay ;
: iter6 painta_6 lxdelay ;
: iter7 painta_7 lxdelay ;
: iter8 painta_8 lxdelay ;
: iter9 painta_9 lxdelay ;

: test startup
  iter0 iter1 iter2 iter3 
  iter4 iter5 iter6 iter7
  iter8 iter9 iterA iterB
  iterC iterD iterE iterF ;

\ grand test
: gtest test test test test test
  test test test test test
  test test test test test ;

\ : testaa 1 begin again ;




: vipew (  - ) paint ;

  
: vipe2 1 begin
      vipew
      1 + dup
      200 = if
        \  cr .s cr drop exit
          drop exit
      then again ;


: vipe3 1 begin
      vipe2
      1 + dup
      12 = if
          \ cr .s cr drop exit
          drop exit
      then again ;


: vipe4 1 begin
      vipe3
      1 + dup
      18  = if
          \ cr .s cr drop exit
          drop exit
      then again ;

: vipe (  - )
  \ 100 1 do vipew loop
  vipew
  vipew
  vipew
  vipew
  vipew
  vipew
;

: planes  ( n - ) \ TOS holds the number of planes (blink iterations)

  \ ldelay ldelay ldelay ldelay ldelay \ delay 2 seconds - ldelay is 400 ms

  0 swap  ( planes - 0 planes )

  dark

  for
    1 +
    dup
    for
         mowink
    next
    1800 ms
  \ .s cr
  next
  drop
  ;

: go  (  - ) startup 
  \ 65 emit 
  begin
  dark
  77 planes
  30 for 2000 ms next
  30 for 2000 ms next
  30 for 2000 ms next
  again \ quit
  -;

