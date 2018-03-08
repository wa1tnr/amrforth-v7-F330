\ testf330.fs
in-meta decimal

code init  (  - )
	$de # WDTCN mov  $ad # WDTCN mov  \ Disable WDT
	$04 # XBR0 mov  \ Enable UART only.
	$40 # XBR2 mov  \ Enable crossbar and weak pull-ups.
	$04 # PRT0CF orl \ Set P0.2 output state to push-pull.
	next c;

code blink  (  - ) 2 .P0 cpl  next c;

code dark  (  - ) 2 .P0 setb  next c;

code light  (  - ) 2 .P0 clr   next c;


\ : delay  (  - ) -1 for  next ; \ v7 seems to choke -- did I fix everything yet?
\ : delay  (  - ) 254 for  next ; \ safer maybe

: delay  (  - )  $6543
  for  next ; \ v7 probably fine.  I have two separate initialize

\ code blocks.  Feh.
\ old: seems to choke -- did I fix everything yet?
\ ans: nope.  You missed it.


code use-crystal  (  - )
	$67 # OSCXCN mov
	10 # R7 mov
	begin	100 # R6 mov
		begin	R6 -zero until
	R7 -zero until
	begin	OSCXCN A mov
	7 .ACC set? until
	$08 # OSCICN mov  \ External clock.
	next c;

code initialize
	$de # WDTCN mov  $ad # WDTCN mov  \ Disable Watchdog.
\ ----- initialization code goes here, before MAIN.
	$04 # XBR0 mov  \ Enable UART only.
	$40 # XBR2 mov  \ Enable crossbar and weak pull-ups.
	$67 # OSCXCN mov  \ 11MHz crystal, or faster.
	10 # R7 mov  \ 10 times outer loop.
	begin	100 # R6 mov
		begin  R6 -zero until  \ 200 cycle inner loop.
	R7 -zero until  \ Total 1ms delay.
	begin
		OSCXCN A mov
	7 .ACC set? until  \ Wait for crystal to stabilize.
\	$08 # OSCICN orl  \ Enable external clock.
\	$04 invert # OSCICN anl  \ Disable internal clock.
	$08 # OSCICN mov  \ External clock.
\ Setup serial port.
	$52 # SCON mov
	$20 # TMOD mov
	$f3 # TH1 mov  \ 9600 baud when crystal = 24MHz.
	6 .TCON setb
\	$7f # PCON anl  \ SMOD=0.
	$80 # PCON orl  \ SMOD=1.
	next c;

\ 0 [if]

\ cannot use Apop from v6 anymore.  -cwh April 2012

\ code erase-page-1  (  - )
	\ $89 # FLSCL mov
	\ 3 # PSCTL mov
	\ $200 # DPTR mov
	\ A @DPTR movx
	\ next c;

\ code write  ( c addr - )
	\ $89 # FLSCL mov
	\ 1 # PSCTL mov  \ Enable flash writes.
	\ Apop  A DPH mov  Apop  A DPL mov
	\ SP inc  Apop  A @DPTR movx
	\ next c;

\ [ then ]

\ : go  (  - ) initialize 44 quit ;

: 11fordelay (  - ) 11 for delay next ;


: blinkpattern (  - ) 
 2 for
   11fordelay
   11fordelay
   12   for blink delay 
            3 for delay
              next
        next 
    next ;

: bigfinish (  - )
  light delay dark
  light delay dark   delay delay delay
  light delay dark ;

: initgo (  - )
  \ initialize
  init
  use-crystal ;

: pseudogo  (  - )
  bigfinish
  blinkpattern
  bigfinish
  \ 44 77 22 55
  ;
	
\ : go  (  - )  init use-crystal  begin  blink delay  again -;

\ initgo pseudogo

: sdelay ( n - ) \ sizeable delay
  for 220 for delay next next ;

: go (  - )
  initgo
  begin
      pseudogo
      dark \ insurance
      3 sdelay
  again -;


romHERE ( *)  0 org
label cold-start  $200 ljmp c;
( *) org

