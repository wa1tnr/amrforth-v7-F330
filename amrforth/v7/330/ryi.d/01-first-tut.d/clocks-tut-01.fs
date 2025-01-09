\ clocks-tut-01.fs -- ryi tutorial 01 - clocks - f330 example program
\ Thu  9 Jan 19:19:12 UTC 2025

5 spaces .( loading clocks-tut-01.fs) cr
5 spaces .(         not yet interesting .. more study ahead) cr

: sayClocksID
  ." Thu  9 Jan 19:19:12 UTC 2025 "
  cr
;

: initDevice (  - )
  init
  sayClocksID
;

\  sfr-f330.fs:75:$b1 sfr OSCXCN
\  sfr-f330.fs:76:$b2 sfr OSCICN
\  sfr-f330.fs:77:$b3 sfr OSCICL
\  sfr-f330.fs:117:$e3 sfr OSCLCN

\ OSCICN
\ Bit7: 0: IOSCEN Internal H-F Osc Dsbl
\ Bit7: 1: IOSCEN Internal H-F Osc Enbl
\ Bits 1-0:  IFCN1-0: Internal H-F Oscillator Frequency Control Bits

\   00: SYSCLK H-F Osc / 8
\   01: SYSCLK H-F Osc / 4
\   10: SYSCLK H-F Osc / 2
\   11: SYSCLK H-F Osc / 1

code oscicn/8 (  - ) $80 # OSCICN mov next c;
code oscicn/4 (  - ) $81 # OSCICN mov next c;
code oscicn/2 (  - ) $82 # OSCICN mov next c;
code oscicn/1 (  - ) $83 # OSCICN mov next c;

\ breaks serial : defaultClock oscicn/8 ;
: defaultClock oscicn/1 ; \ trial - not expected to break serial comm P0.4 (TX) and P0.5 (RX)

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

variable pb

: resetPBSwCounter
  0 pb !
;

variable LEDState

: resetLEDState (  - ) 0 LEDState !  ;
: setLEDState (  - ) -1 LEDState !  ;
: getLEDState (  - s ) ledState @ ;

: ledsDisplay01 (  - ) 0 .P1 setb 1 .P1 clr  ;
: ledsDisplay10 (  - ) 0 .P1 clr  1 .P1 setb ;

: toggleLEDs
  getLEDState
  dup 0= IF
    setLEDState
    ledsDisplay01
    drop
    exit
  THEN
  drop
  resetLEDState
  ledsDisplay10
;

: simPBSw (  - )
  cr ." every simPBSw iteration marker" cr
  pb @
  cr ." pb @ stack: " .s
  1 +
  dup
  pb !
  43 emit
  20 ms
  5 -
  0< IF
    exit
  THEN
  resetPBSwCounter
  toggleLEDs
;

: go (  - )
  clear stkpad
  mainFcn
  resetLEDState
  resetPBSwCounter
  begin
    simPBSw
  again
-;

\ end.
