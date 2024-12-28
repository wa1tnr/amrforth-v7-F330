\ main.fs -- f330 example program
\ Fri 27 Dec 21:59:56 UTC 2024

: xdatex
  ." Sat 28 Dec 23:25:02 UTC 2024 "
;

code enbl (  - ) 0 .P2 clr  next c;
code dsbl (  - ) 0 .P2 setb next c;

\ : iterVal 14 1 + ; \ sum is 15

\ Since 8 blinks are reliable, timed against serial prints,
\ and since that is the intent, makes better sense to
\ specify iterVal as ( iterations 2 * ) - 1, where - 1 was
\ determined experimentally.
\ 8 2 * 1 -

\ : iterVal 16 1 - ; \ sum is 15
: iterVal 8 2 * 1 - ; \ outcome is 15

: bounds0< -1 max iterVal 1 - min ;

code startup
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
	$ff # P0MDIN orl   \ No analog, all digital, port 0.x
        $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
        $01 # P2MDOUT orl  \ push pull

	$01 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
\       2 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
	next c;

code ncpl 2 .P0 cpl next c; \ colon element may be here! lotta bangs today!

\ works without type - why:
: rjcpl ." test alias 23:24z Fri 27th" cr cr .s cr cr space space ncpl ; \ was: type space

: delayed
  1000 ms
  1000 ms
  1000 ms
;

variable counted
variable iterations

: gonoa
  startup
  0 counted !
  1 1 1
  begin
    key?
      if
        ncpl 50 ms
        \ key drop
        key \ .s
        emit
        counted @ 1 + counted !
        counted @ 10 > if
        0 counted !
        .s
        then
      \ .s
      \ ncpl 80 ms
      \ delayed
    then
  again
;

: gobcde
  startup
  0 counted !
  1 1 1
  begin
        ncpl 50 ms
        1100 ms
        32 emit
        43 emit \ plus
        [char] p emit
        [char] q emit
        [char] r emit
        [char] s emit
        [char] t emit
        32 emit
        counted @ 1 + counted !
        counted @ 10 > if
        0 counted !
        52 emit 52 emit
        52 emit 52 emit
        52 emit 52 emit
        \ .s
        then
  again
;

: godgca
  begin
  1 drop
  again
;

: message space ." ln: this is a message" space ;

: setup 1 iterations ! ;

: triggered space ." TRIGGERED " space ;

: resetIter iterVal iterations ! ;

\ all go-like words ignored above.  may not be
\ useful in present context.  Unexplored.

: go -99 dup 1 + dup 1 +
  3000 ms \ time to logon after power cycle
  cr xdatex cr cr
  ." go is running.. "
  resetIter
  ." greely colorado " cr
  begin
    iterations @ 1 - iterations ! iterations @
    bounds0<
    0< IF
      message
      resetIter
    THEN
    ncpl
    500 ms
  again
-;

\ end.
