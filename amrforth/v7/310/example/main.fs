\ Tue 19 Apr 10:48:49 UTC 2022

\ NICE CODE BUT .. unvetted GPIO shorting consequences expected.

\ lost an F310 target; suspicious of GPIO set to push-pull, set low, then externally shorted.

\ original code follows, but commented out Tue 19 Apr 10:48z 2022



0 [if] \ a zero here will disable this code block
include tms-tdo-tck-tdi-aa.fs

: help    ." C2 INTERFACE did this upload 14:52z" cr cr
          ." J3 is the JTAG IDE socket and the RJ45's there directly correspond - they are not crossed." cr
          ." J4 is the longest IDE socket.  J2 is the middle one (14-ish pin targets)." cr
          cr
          cr ."   tms  tck  tdi  tdo   go init help" cr cr
;

: init startup ;

: gobb init
  begin
      cr ." . " cr
      tms cr
      tck cr
      tdi cr
      tdo cr
      2900 ms
  again ;

\ : go begin 1 drop again -;

[then]
