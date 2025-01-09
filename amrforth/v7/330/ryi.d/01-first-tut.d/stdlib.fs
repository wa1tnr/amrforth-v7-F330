\ stdlib.fs
\ Thu  9 Jan 17:17:01 UTC 2025

cr
5 spaces .( loading stdlib.fs) cr

include ustimer-cwh.fs
include gpio-lib-a.fs
include gpio-init-a.fs

: stkpad -99 dup 1 + dup 1 + ;
: 2r 2^raised ;

\ end.
