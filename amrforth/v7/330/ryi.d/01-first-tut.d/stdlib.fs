\ stdlib.fs
\ Thu  9 Jan 23:39:24 UTC 2025

cr
5 spaces .( loading stdlib.fs) cr

include ustimer-cwh.fs
\ include gpio-lib-a.fs
include gpio-lib-abb.fs \ not checked if 'standard' version or modified lately
\ include gpio-init-a.fs
include gpio-init-abb.fs \ again was not checked against a 'standard' version

: stkpad -99 dup 1 + dup 1 + ;
: 2r 2^raised ;

\ end.
