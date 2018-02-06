\ download-aduc.fs

0 [if]
Copyright (C) 1991-2004 by AM Research, Inc.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

For LGPL information:   http://www.gnu.org/copyleft/lesser.txt
For application information:   http://www.amresearch.com

[then]
only forth also definitions

windows [if]
	include serial-windows.fs
[else]	include serial-linux.fs
[then]	open-comm

create object-code  64 1024 * allot

: c@-t  ( a - c) object-code + c@ ;

0 value fid  \ File ID.
0 value romHERE

: read-object-code  (  - )
	s" rom.bin" r/o open-file throw to fid
	object-code [ 64 1024 * ] literal fid read-file throw
	to romHERE  fid close-file throw ;

create twirls
	char | c,
	char / c,
	char - c,
	char \ c,

0 value twirler

: twirl  (  - )
	twirler twirls + c@ emit 8 emit
	twirler 1 + 3 and to twirler ;

: untwirl  (  - ) space ;

16 CONSTANT BYTES/LINE-HEX
0 VALUE aducCHECKSUM
: +CHECKSUM   ( n - )   aducCHECKSUM + $FFFF AND TO aducCHECKSUM ;
: SPIT-HEX-BYTE   ( c - )
        DUP +CHECKSUM
        BASE @ >R HEX   0 <# # # #> DROP
        COUNT ( DUP EMIT) emit-s   C@ ( DUP EMIT) emit-s
        R> BASE ! ;
: SPIT-HEX-ADDRESS   ( a - )
        DUP $0FF AND +CHECKSUM
        DUP 0 256 UM/MOD NIP $0FF AND +CHECKSUM
        BASE @ >R HEX   0 <# # # # # #> DROP
        4 0 DO   COUNT ( DUP EMIT) emit-s   LOOP   DROP
        R> BASE ! ;
: COUNT-T   ( a1 - a2 c)   DUP 1+ SWAP C@-T ;
: SEND-LINE-HEX   ( a1 - a2)
        \ CR
        00 TO aducCHECKSUM
        [CHAR] : ( DUP EMIT) emit-s
        BYTES/LINE-HEX SPIT-HEX-BYTE
        DUP SPIT-HEX-ADDRESS
        0 SPIT-HEX-BYTE
        16 0 DO   COUNT-T SPIT-HEX-BYTE   LOOP
        aducCHECKSUM NEGATE $00FF AND SPIT-HEX-BYTE
        ;
: WAIT-FOR-ACK   (  - )
        key-s ( SPACE DUP .) 6 - ABORT" Download Error" ;
: SEND-DONE   (  - )
        CR [CHAR] ; ( DUP EMIT) emit-s
        $FF00 SPIT-HEX-ADDRESS ;
: old-download   (  - )
        0 romHERE BYTES/LINE-HEX /MOD SWAP 0= 0= 1 AND + 0 DO
                SEND-LINE-HEX WAIT-FOR-ACK
                i 7 AND 0= IF   TWIRL   THEN
        LOOP    DROP UNTWIRL
        SEND-DONE ;
: check-spit   ( c - )   dup +checksum emit-s ;	
: send-address   ( a - )
	0 check-spit	\ 16 bit address always.
	dup 8 rshift $FF and check-spit	\ High byte of address.
	$FF and check-spit		\ Low byte of address.
	;
: send-checksum   (  - )   
	aducChecksum negate $FF and emit-s
	;
: send-packet-start-ID   (  - )
	0 to aducChecksum
	$07 emit-s   $0E emit-s	\ Packet start ID.
	;
: send-program-packet   ( a1 - a2)
	send-packet-start-ID
	bytes/line-hex 4 + check-spit	\ Number of data bytes to follow.
	[char] W check-spit		\ Write code memory command
	dup send-address
	bytes/line-hex 0 do
		count-t check-spit
	loop
	send-checksum
	wait-for-ack 
	;

0 value data-flash-base
: count-d  ( a1 - a2 c)  dup 1 + swap data-flash-base + c@ ;
: send-data-packet   ( a1 - a2)
	send-packet-start-ID
	bytes/line-hex 4 + check-spit	\ Number of data bytes to follow.
	[char] E check-spit		\ Write data memory command
	dup send-address
	bytes/line-hex 0 do
		count-d check-spit
	loop
	send-checksum
	wait-for-ack 
	;
	
: erase-program-memory   (  - )
	send-packet-start-ID
	1 check-spit		\ Byte count, including command.
	[char] C check-spit	\ The erase code memory command.
	send-checksum
	wait-for-ack
	;
: erase-data&program-memory   (  - )
	send-packet-start-ID
	1 check-spit		\ Byte count, including command.
	[char] A check-spit	\ The erase code memory command.
	send-checksum
	wait-for-ack
	;
: run-program-code   (  - )
	send-packet-start-ID
	4 check-spit
	[char] U check-spit
	0 send-address
	send-checksum
	wait-for-ack
	;
: new-download   (  - )
	cr ." Erasing Program Memory" 
	erase-program-memory
	cr ." Writing Program Memory"
	0 romHERE bytes/line-hex /mod swap 0= 0= 1 and + 0 do
		send-program-packet
		i 7 and 0= if   twirl   then
	loop	drop untwirl
	run-program-code
	;
: new-full-download  (  - )
	cr ." Erasing Data and Program Memory"
	erase-data&program-memory
	cr ." Writing Data Memory"
	0 640 bytes/line-hex /mod swap 0= 0= 1 and + 0 do
		send-data-packet
		i 7 and 0= if  twirl  then
	loop  drop untwirl
	cr ." Writing Program Memory"
	0 romHERE bytes/line-hex /mod swap 0= 0= 1 and + 0 do
		send-program-packet
		i 7 and 0= if   twirl   then
	loop	drop untwirl
	run-program-code ;

\ Decide which version of the downloader is being used.
\ Aborts if chip is neither new nor old.
: new-chip?   (  - flag)
	begin	key-s dup emit [char] A = dup if
			drop key-s dup emit [char] D =
		then
	until	\ 'A' and 'D' have been received.
	9 key-s dup emit dup [char] I = over [char] u = or 0= abort" Bad downloader"
	[char] I = tuck if   drop 22   then   \ 'I' or 'u'
	0 do   key-s drop   loop   \ Discard the rest of the ID string.
	;

: download   (  - )        
	0 to twirler
        CR ." Apply power to and/or reset the target board..."
	new-chip? if new-download else old-download then ;

: go  (  - )
	['] read-object-code catch if
		cr ." Problem reading rom.bin." abort
	then
	['] download catch if
		cr ." Problem downloading object code." abort
	then
	cr ;
go
bye

