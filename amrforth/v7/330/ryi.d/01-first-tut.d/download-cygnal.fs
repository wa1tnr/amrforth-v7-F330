\ download-cygnal.fs
\ Sun  5 Jan 18:18:01 UTC 2025 - matched set timestamp
\ REVISED - KLUDGE - Wed  1 Jan 17:22:50 UTC 2025
\ download word is having issues with present hardware reset circuit
\ Best to press and hold RESET then invoke ./d from bash shell
\ or use the download word in forth (same exec path after).

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

: download-page  ( a1 - a2)
	$200 0 do  dup c@-t emit-s 1 +  loop ;

: .spcInstrux ( n - )
   14 spaces ;

: download-all  (  - )
	clear-sbuf cr
        ." download-cygnal.fs:" cr
        ." Silabs CP2104 is a suitable interface for uploading." cr cr
	."     To download to the target," cr cr
	."         press and hold RESET on the target board," cr
	."         then," cr
	."         run the download word," cr
	."         and then," cr
	cr
	\ key drop
	."         release the RESET button on the target board." cr cr
	."         You may also try pressing and releasing the" cr
	."         RESET button NOW if no target response was" cr
	."         seen during the above invocation." cr cr
	."         You can also try simply pressing and releasing RESET," cr
	."         now, with no further planned action preceding that. ;)" cr cr
	."         Ctrl + \  to abort   (frees /dev/ttyUSB0 CP2104)." cr
	begin	$a5 emit-s 10 ms
		key?-s if
			key-s $5a =  \ Target has responded.
		else
			false
		then
	until
	\ Shake hands with the target.
	[char] a emit-s [char] m emit-s [char] r emit-s
	begin  key-s $a5 = until
	cr ." 512 byte pages: "
	romHERE $200 - $200 /mod swap 0<> 1 and +  \ # pages.
	dup emit-s  key-s $5a - abort" Problem bootloading"
	$200 swap 0 do
		download-page key-s .
	loop
	drop 0 emit-s ;

: go  (  - )
	['] read-object-code catch if
		cr ." Problem reading rom.bin." abort
	then
	['] download-all catch if
		cr ." Problem downloading object code." abort
	then
	cr ;
go
bye

