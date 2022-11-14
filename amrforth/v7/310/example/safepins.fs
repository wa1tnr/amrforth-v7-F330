\ thepins.fs -- f310 example program
\ Mon 14 Nov 13:20:06 UTC 2022
\ Unverified - commit as-is 14 Nov 2022

0 [if]
[then]

code safestart
    $40 invert # PCA0MD anl   \ p. 216  WDTE watchdog disabled
    $ff # P0MDIN orl          \ p. 136  No analog, all digital, port 0.x
    $ff # P1MDIN orl          \ p. 138  No analog, all digital - port 1.x
    \ $cf # P0MDOUT orl       \ p. 137  push-pull  for pins 7 6 3 2 1 and 0
    $cf invert # P0MDOUT anl  \ p. 137  open drain for pins 7 6 3 2 1 and 0

    \ high impedance - set each port pin bit when mode is open drain:
    0 .P0 setb 1 .P0 setb 2 .P0 setb 3 .P0 setb
    6 .P0 setb 7 .P0 setb

    \ $ff # P1MDOUT orl       \ p. 139  all pins output, push pull.
    $ff invert # P1MDOUT anl  \ p. 139  all pins open drain
    0 .P1 setb 1 .P1 setb 2 .P1 setb 3 .P1 setb
    4 .P1 setb 5 .P1 setb 6 .P1 setb 7 .P1 setb

    $01 # XBR0 mov            \ p. 134  Route UART to P0.4 and P0.5
    $40 # XBR1 mov            \ p. 135  Enable crossbar and weak pull-ups.
    \ 2 .P0 setb 
next c;

: init 1 drop cr cr ." this is init." cr cr ;

