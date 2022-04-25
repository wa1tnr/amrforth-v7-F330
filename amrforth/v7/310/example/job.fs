\ job.fs

include ustimer-cwh.fs

code starting
    $40 invert # PCA0MD anl   \ p. 216  WDTE watchdog disabled
    $ff # P0MDIN orl          \ p. 136  No analog, all digital, port 0.x
    $04 # P0MDOUT orl         \ p. 137  push-pull  for pin 2

next c;

code !2p0 2 .P0 cpl  next c;
code _2p0 2 .P0 clr  next c;
code  2p0 2 .P0 setb next c;

0 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  \ include safepins.fs
  \ include thepins.fs
  \ include main.fs
[then]


: blink ( -- )
  !2p0 2000 ms !2p0 2000 ms
;

: blinks ( n -- )
  for blink next
;

: run
  starting
  2p0 \ turns off LED by setting this pin HIGH
  3 blinks
;

\ : go 1 drop -;
