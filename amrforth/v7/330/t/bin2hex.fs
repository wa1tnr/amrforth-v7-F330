\ bin2hex.fs

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

0 value program-size
0 value rom.hex
0 value rom.bin

16 constant raw-bytes/line

raw-bytes/line 2* 13 + constant bytes/ihr   \ ihr= Intel Hex Record.

create IHR   $0d c,   $0a c,    \ carriage return, line feed.

Bytes/ihr 2 - allot

ihr 2 + constant _IHR

: write-ihr   ( n - ) IHR swap ROM.HEX write-file throw ;

: -write-ihr   ( n - ) IHR 2 + swap 2 - ROM.HEX write-file throw ;

: 2digits   ( c - a l) base @ >r   hex 0 <# # # #>   r> base ! ;

: 4digits   ( n - a l) base @ >r   hex 0 <# # # # # #>   r> base ! ;

variable checksum

: build-ihr-record   ( a1 - a2)
        [char] : _IHR c!
        raw-bytes/line ( bytes/record ) checksum !
        raw-bytes/line 2digits      _IHR 1+  swap cmove
        dup $100 /mod + checksum +!
        dup 4digits         _IHR 3 + swap cmove
        0 2digits       _IHR 7 + swap cmove
        raw-bytes/line 0
        do      dup c@-t 2digits _IHR 9 + i 2* + swap cmove
                dup c@-t checksum +!   1+
        loop
        checksum c@ negate
        2digits _IHR bytes/ihr 4 - + swap cmove
        ;

\ Tells rom-burner to use segment 0,
\ just in case that makes a difference.
: build-ihr-begin   (  - )
        [char] :        _ihr c!
        2 2digits       _ihr 1+  swap cmove
        0 4digits       _ihr 3 + swap cmove
        2 2digits       _ihr 7 + swap cmove
        0 4digits       _ihr 9 + swap cmove
        $FC 2digits     _ihr 13 + swap cmove
        ;

: build-ihr-end   (  - )
        [char] :        _IHR c!
        0 2digits       _IHR 1+  swap cmove
        0 4digits       _IHR 3 + swap cmove
        01 2digits      _IHR 7 + swap cmove
        $FF 2digits     _IHR 9 + swap cmove
        13 _IHR $0b + c!   \ carriage return
        10 _IHR $0c + c!   \ line feed
        ;

: ?write-ihr  ( addr len flag - )
	if write-ihr exit then -write-ihr ;

: write-rom.hex   (  - )
        0 program-size 0 raw-bytes/line um/mod swap if   1+   then
        0 do    build-ihr-record bytes/ihr
			    i ?write-ihr  \ Suppress leading crlf.
        loop    drop
        build-ihr-end 15 write-ihr ;

: save-rom.hex   (  - )
	s" rom.bin" r/o open-file throw to rom.bin
	rom.bin file-size throw drop to program-size
	target-image program-size rom.bin read-file throw drop
	rom.bin close-file throw
        s" rom.hex" w/o create-file throw to rom.hex
	write-rom.hex
        rom.hex close-file throw ;

