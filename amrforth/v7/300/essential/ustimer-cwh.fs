\ ustimer-cwh.fs
\ adapted from: main.fs  JTAG driver for F300
in-meta decimal

0 bit-variables

a: 1us  5 # R7 mov  begin  R7 -zero until   ;a

\ Maximum of 255 us allowed, since it uses 8 bits.
\ The high byte of the count is ignored.
code us  ( c - )
	begin  1us  ACC -zero until
	' drop jump c;

: ms  ( n - )
	dup 0= if  drop exit  then
	1 - 250 us 250 us 250 us 250 us  ms ;

\ patch-headers

