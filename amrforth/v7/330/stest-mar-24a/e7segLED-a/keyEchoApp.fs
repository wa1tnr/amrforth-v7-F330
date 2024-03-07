\ keyEchoApp.fs
\ Thu  7 Mar 23:46:10 UTC 2024

: ctrl [char] ^ emit ;

: bspace 8 emit space 8 emit ;
: cr_payl cr ; \ debug: space space ." CR seen" cr ;

: handler
	dup 127 = if  drop  bspace   exit  then  \ control-h, bs
	dup  31 > if  emit           exit  then  \ most printable chars
	dup   3 = if  drop           exit  then  \ control-c
	dup  13 = if  drop  cr_payl  exit  then  \ cr
                      drop ;                     \ fall-thru

: keyapp_init 99 negate dup 1 + dup 1 + ;

: mandelay 1800 ms ;

: go
  keyapp_init
  mandelay
  cr ." signon notice keyEchoApp.fs" cr
  begin
      key handler
  again -;

\ end.

