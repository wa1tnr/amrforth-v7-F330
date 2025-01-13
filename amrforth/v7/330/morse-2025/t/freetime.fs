\ freetime.fs
\ Targeting the gadgets, g300, g310, g017, g061.
\ All of the above plus the 552 and 537 have timer2 with a
\ 16 bit auto-reload mode.  Because of the auto-reload feature
\ there is no interrupt latency effect on the timing.

\ The g300, g310, and g061 all run without an external crystal.
\ If you find that the cycles/period calculated is not accurate
\ enough for your application, you can define your own 'cycles/period'
\ and calibrate it with an oscilloscope.  Note that '1000 us/period'
\ means there are 1000 microseconds per interrupt period, that is, the
\ period is one millisecond.

in-host

: us/period  frequency @ 12000000 */ ;
1000 us/period constant ticks/period
ticks/period          $ff and constant reload-low
ticks/period 8 rshift $ff and constant reload-high

in-compiler

\ Let the 300 family names be the default.
\ Alias the others to mimic these as follows.
\ 'PENDING' is more generic.
has C8051F000 [if]
.( Has c8051f000)
a: SYSCLK/12  $08 invert # CKCON anl ;a
a: PENDING  7 .T2CON ;a
a: TMR2H    TH2 ;a
a: TMR2L    TL2 ;a
a: TMR2RLH  RCAP2H ;a
a: TMR2RLL  RCAP2L ;a
a: TMR2CN   T2CON ;a
[then]

has C8051F061 [if]
.( Has c8051f061)
a: SYSCLK/12  $08 invert # CKCON anl ;a
a: PENDING  7 .TMR2CN ;a
a: TMR2RLH  RCAP2H ;a
a: TMR2RLL  RCAP2L ;a
[then]

has C8051F300 [if]
.( Has c8051f300)
a: SYSCLK/12  $08 invert # CKCON anl ;a
a: PENDING  7 .TMR2CN ;a
[then]

has C8051F310 [if]
.( Has c8051f310)
a: SYSCLK/12  $08 invert # CKCON anl ;a
a: PENDING  7 .TMR2CN ;a
[then]

code init-ms
    SYSCLK/12
    reload-high invert # TMR2RLH mov
    reload-low  invert # TMR2RLL mov
    $04 # TMR2CN mov  \ Timer2 on.
    $a0 # IE orl  \ Enable timer2 and global interrupts.
    next c;

variable ticks

label ms-interrupt
    ACC push
    ticks 1 + direct inc  ticks 1 + direct A mov
    0= if  ticks direct inc  then
    ACC pop
    PENDING clr
\    -1 # P2 xrl  \ Toggle all pins at P2.
    reti c;
ms-interrupt $2b int!

code ticks@  (  - n)
    |dup  5 .IE clr  \ Disable the timer interrupt.
    ticks 1 + direct A mov  ticks direct R2 mov
\    ticks 1 + direct A mov  Apush
\    ticks direct A mov
    5 .IE setb  \ Apush
    next c;

: timeout  ( n a - ) swap ticks@ + swap ! ;

: elapsed?  ( a - flag) @ ticks@ > 0= ;

variable milliseconds

: ms  ( n - )
    milliseconds timeout
    begin  milliseconds elapsed? until ;

