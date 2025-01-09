\ gpio-lib-a.fs
\ Thu  2 Jan 19:24:04 UTC 2025

\ : testLshift ( n - p )
\  0 max 15 min ;

: lshift ( bitmask places - n )
  \ testLshift
  dup 0= if
    drop
    exit
  then \ handles special case 2^0=1
  for 2* next
;

: 2^raised ( n - p )
  1 swap
  lshift ( mask places - shifted_val )
;

: .P0 ( portPin - )
  2^raised $80
;

: .P1 ( portPin - )
  2^raised $90
;

: setb ( bitmask PORT - ) \ 3 .P0 setb
  dup
  $80 - 0= if
    drop
    [ in-assembler A P0 orl ]
    drop
    exit
  then
  $90 - 0= if
    [ in-assembler A P1 orl ]
    drop
    exit
  then
;

: clr  ( bitmask PORT - ) \ 3 .P0 clr
  dup
  $80 - 0= if
    drop
    invert
    [ in-assembler A P0 anl ]
    drop
    exit
  then
  $90 - 0= if
    invert
    [ in-assembler A P1 anl ]
    drop
    exit
  then
;

\ end.
