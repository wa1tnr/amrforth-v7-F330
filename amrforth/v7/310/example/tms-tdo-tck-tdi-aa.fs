\ unvetted code after GPIO failure experienced F310

\ This was used to exercise the pins - keep on hand,
\ but reverify what it does Tue 19 Apr 10:48z 2022

code wink  (  - ) 2 .P0 cpl  next c;

0 [if]

code wynk  (  - )
  7 .P1 cpl
  5 .P1 cpl
  3 .P1 cpl
  1 .P1 cpl
next c;

code 7p1 7 .P1 cpl next c;
code 5p1 5 .P1 cpl next c;
code 3p1 3 .P1 cpl next c;
code 1p1 1 .P1 cpl next c;

code 1p1clr 1 .P1 clr next c;

: tms 7p1 ." P1.7 is on J4 IDE fem socket board edge side." cr
          ." Skip 8 pins to get to it." cr

          ." Pad is closest to F310 stamp - skip 7 pins." cr
          ." On J3, TMS is mapped two in (skip 1) and on the board edge side of J3." cr
;
: tck 5p1 ." P1.5 is on J4 IDE fem socket board edge side." cr
          ." Skip 7 pins to get to it." cr

          ." Pad is closest to F310 stamp - skip 6 pins." cr
          ." On J3, TCK is mapped three in (skip 2) and on the prototype area side of J3." cr
;

: tdi 3p1 ." P1.3 is on J4 IDE fem socket board edge side." cr
          ." Skip 6 pins to get to it." cr

          ." Pad is closest to F310 stamp - skip 5 pins." cr
          ." On J3, TDI is mapped four in (skip 3) and on the prototype area side of J3." cr
;

: tdo 1p1 ." P1.1 is on J4 IDE fem socket board edge side." cr
          ." skip 5 pins to get to it." cr

          ." Pad is closest to F310 stamp - skip 4 pins." cr
          ." On J3, TDO is mapped three in (skip 2) and on the board edge side of J3." cr
;

[then]
