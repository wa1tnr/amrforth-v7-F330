\ stdlib.fs
\ Thu  9 Jan 23:53:54 UTC 2025

cr
5 spaces .( loading stdlib.fs) cr

include ustimer-cwh.fs
include gpio-lib-a.fs \ substantially the same as earlier version
\ include gpio-init-a.fs \ rescinded a more complete version
include gpio-init-abb.fs \ NOT the standard version - pared down some

: stkpad -99 dup 1 + dup 1 + ;
: 2r 2^raised ;

\ end.
