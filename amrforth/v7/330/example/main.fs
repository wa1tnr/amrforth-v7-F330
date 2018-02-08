\ main.fs -- f330 example program

\ main.fs

code startup
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
\ ----- initialization code goes here, before MAIN.

	$ff # P0MDIN orl   \ No analog, all digital.
      \ $04 # P0MDOUT orl  \ P0.2 output, push pull.
        $04 # P0MDOUT orl  \ P0.2 output, push pull.
      \ $0e # P0MDOUT orl  \ P0.2 output, push pull.  \ P0.1 P0.3 also outputs.

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

