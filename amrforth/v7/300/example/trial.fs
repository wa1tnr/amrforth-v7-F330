\ job.fs

include ustimer-cwh.fs

code startup
    $40 invert # PCA0MD anl
    $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
    $40 # XBR2 mov  \ Enable crossbar and weak pull-ups.
    next c;

code !2p0 2 .P0 cpl next c;

: grumatic !2p0 ; \ alias proof of programmed MCU

\ code set2p0 2 .P0 setb next c;

code 2p0setb 2 .P0 setb next c;
code 2p0clr  2 .P0 clr  next c;

: runzzq
  startup
  cr
  ." we begin here 21 April 2022." cr

  ." 2 .P0 blinks for 128 loop iterations." cr

  2 for
      ." this is setb - dark: " cr
      2p0setb
      100 ms
      ." this is clr - bright: " cr
      2p0clr
      100 ms
  next

  3000 ms 2 for
  2p0setb \ dark
      6 for
          ."   !2p0  "
          !2p0
          1000 ms
      next
  next

  ." beginners luck wait 4 seconds " 4000 ms
  ." have waited four seconds back quiet." cr ;

: run runzzq ;

0 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  include thepins.fs
  include main.fs
  \ include little.fs
[then]
