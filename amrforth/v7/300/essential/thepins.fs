\ thepins.fs -- f300 example program

0 [if] \ a zero here will disable this code block

code 0p0 0 .P0 cpl next c;
code 1p0 1 .P0 cpl next c;
code 2p0 2 .P0 cpl next c;
code 3p0 3 .P0 cpl next c;
code 6p0 6 .P0 cpl next c;
code 7p0 7 .P0 cpl next c;

code !0p0 0 .P0 setb next c;
code !1p0 1 .P0 setb next c;
code !2p0 2 .P0 setb next c;
code !3p0 3 .P0 setb next c;
code !6p0 6 .P0 setb next c;
code !7p0 7 .P0 setb next c;

: allpinsset
    !0p0 !1p0 !2p0 !3p0 !6p0 !7p0
;
code startup
    $40 invert # PCA0MD anl
    \ default \ $ff # P0MDIN orl   \ No analog, all digital, port 0.x
    $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
    \ $30 # XBR0 mov \ skip pins 4 and 5
    \ $03 # XBR1 mov \ route P0.4 and P0.5 to USART
    $40 # XBR2 mov  \ Enable crossbar and weak pull-ups.
    next c;

: init startup allpinsset ;

: test init
  begin
      0p0 1p0 2p0 3p0
      6p0 7p0 600 ms
  again ;

\ : go 1 drop begin 1 drop again -;

[then]
\ end.
