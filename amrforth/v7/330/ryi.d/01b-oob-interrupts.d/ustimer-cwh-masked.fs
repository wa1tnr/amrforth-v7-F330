\ ustimer-cwh.fs
\ adapted from: main.fs  JTAG driver for F300
in-meta decimal

0 bit-variables

a: 1us-mask  5 # R7 mov  begin  R7 -zero until   ;a

\ Maximum of 255 us allowed, since it uses 8 bits.
\ The high byte of the count is ignored.
code us-mask  ( c - )
	begin  1us-mask  ACC -zero until
	' drop jump c;

: ms-mask  ( n - )
	dup 0= if  drop exit  then
	1 - 250 us-mask 250 us-mask 250 us-mask
        250 us-mask  ms-mask ;

\ patch-headers

