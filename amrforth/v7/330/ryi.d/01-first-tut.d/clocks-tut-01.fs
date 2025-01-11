\ clocks-tut-01.fs -- ryi tutorial 01 - clocks - f330 example program
\ Sat 11 Jan 21:05:18 UTC 2025

5 spaces .( loading clocks-tut-01.fs) cr
10 spaces .( Conditional compilations: )

variable pb
variable LEDState

: sayClocksID
  ." Sat 11 Jan 21:05:18 UTC 2025 "
  $20 emit
  ." WORKING clock scaling /1 /2 /4 and /8"
  cr
;

: initDevice (  - )
  init
  sayClocksID \ disable of oscicn/1 is not in effect
;

\ sfr-f330.fs:75:$b1  sfr OSCXCN
\ sfr-f330.fs:76:$b2  sfr OSCICN
\ sfr-f330.fs:77:$b3  sfr OSCICL
\ sfr-f330.fs:117:$e3 sfr OSCLCN

\ OSCICN
\ Bit7: 0:   IOSCEN Internal H-F Osc Dsbl
\ Bit7: 1:   IOSCEN Internal H-F Osc Enbl
\ Bits 1-0:  IFCN1-0: Internal H-F Oscillator Frequency Control Bits

\ 00: SYSCLK H-F Osc / 8
\ 01: SYSCLK H-F Osc / 4
\ 10: SYSCLK H-F Osc / 2
\ 11: SYSCLK H-F Osc / 1

code oscicn/8 (  - ) $80 # OSCICN mov next c;
code oscicn/4 (  - ) $81 # OSCICN mov next c;
code oscicn/2 (  - ) $82 # OSCICN mov next c;
code oscicn/1 (  - ) $83 # OSCICN mov next c;

: defaultClock oscicn/1 ; \ use when printing text to terminal

: setInitPins
  0 .P1 setb
  1 .P1 clr
  2 .P1 setb
;

: mainFcn (  - )
  initDevice
  setInitPins
  defaultClock
;

: resetPBCtr    (  - )   0 pb ! ;
: resetLEDState (  - )   0 LEDState ! ;
: setLEDState   (  - )   -1 LEDState ! ;
: getLEDState   (  - s ) ledState @ ;
: getPbSwCount  (  - n ) pb @ ;
: ledsDisplay01 (  - )   0 .P1 setb 1 .P1 clr  ;
: ledsDisplay10 (  - )   0 .P1 clr  1 .P1 setb ;
: duty50Timing  (  - )   340 ms ;

: toggleLEDs
  getLEDState
  dup 0= IF
    setLEDState
    ledsDisplay01
    duty50Timing
    drop
    exit
  THEN
  drop
  resetLEDState
  ledsDisplay10
  duty50Timing
;

-1 [if] .( TRUE )
: dispatch ( n - n )
  dup -3 - 0= IF \ ." saw -3" cr
    oscicn/1
    exit THEN
  dup -2 - 0= IF \ ." saw -2" cr
    oscicn/2
    exit THEN
  dup -1 - 0= IF \ ." saw -1" cr
    oscicn/4
    exit THEN
  dup  0 - 0= IF \ ." saw  0" cr 
    oscicn/8
    exit THEN
  \ ." UNTESTED CASE drops: " .s cr
;
[then]

0 [if] .( FALSE) cr
: dispatch ( n - n )
  dup -3 - 0= IF ." saw -3" cr exit THEN
  dup -2 - 0= IF ." saw -2" cr exit THEN
  dup -1 - 0= IF ." saw -1" cr exit THEN
  dup 0 - 0= IF  ." saw  0" cr exit THEN
  ." UNTESTED CASE drops: " .s cr
;
[then]

-1 [if] .( TRUE )
: simPBSw (  - )
  pb @ 1 +
  dup
  pb !
  20 ms
  4 -
  \ ." simPBSw  20 ms 4 -   result before dispatch, TOS: " dup .
  dispatch
  \ ." after dispatch .s: " .s cr
  0< IF
    exit
  THEN
  resetPBCtr
;
[then]


0 [if] .( FALSE) cr
: simPBSw (  - )
  pb @ 1 +
  dup
  pb !
  20 ms
  4 -
  ." simPBSw  20 ms 4 -   result before dispatch, TOS: " dup .
  dispatch
  ." after dispatch .s: " .s cr
  0< IF
    exit
  THEN
  resetPBCtr
;
[then]

: iterLEDs 12 ; \ 32

-1 [if] .( TRUE )
: go (  - )
  clear stkpad
  mainFcn
  resetLEDState
  resetPBCtr
  ." pb: 0" $20 emit
  begin
    simPBSw
    iterLEDs for
      toggleLEDs
    next
    \ ." did 8 for next toggle leds stack: " .s cr
    \ ." bottom of loop, stack: " .s cr
    getPbSwCount
    oscicn/1 $20 emit [char] . emit $20 emit $20 emit ." pb: " .
  again
-;
[then]

cr \ final cr during compile

0 [if] .( FALSE) cr
: go (  - )
  clear stkpad
  mainFcn
  resetLEDState
  resetPBCtr
  begin
    simPBSw
    iterLEDs for
      toggleLEDs
    next
    ." did 8 for next toggle leds stack: " .s cr
    ." bottom of loop, stack: " .s cr
  again
-;
[then]

\ end.

\ defaultClock