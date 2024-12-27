\ main.fs -- f330 example program

code startup
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
	$ff # P0MDIN orl   \ No analog, all digital, port 0.x
        $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
	$01 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
\       2 .P0 clr \ initialize the LED's GPIO pin so that the LED is not lit.
	next c;
\ short

code ncpl 2 .P0 cpl next c;

: delayed
  1000 ms
  1000 ms
  1000 ms
;

variable counted

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
-;
\ end.
