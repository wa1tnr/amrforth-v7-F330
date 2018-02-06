\ download-cygnal.fs

0 [if]
Copyright (C) 2004 by AM Research, Inc.

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

0 value fid

: open-hex-object-file  (  - ) s" rom.hex" r/o open-file throw to fid ;

: close-hex-object-file  (  - ) fid close-file throw 0 to fid ;

: autobaud  (  - ) [char] U emit-s key-s emit cr ;

: send-line  ( addr len - )
	dup >r 0 ?do  count emit-s  loop  drop
	-3 begin  1 + key-s 10 = until  r> - if
		." Downloader error" abort
	then ;

: download  (  - )
	." Atmel Downloader." cr
	." Downloading rom.hex via RS232" cr
	clear-sbuf autobaud
	begin	pad 128 fid read-line throw while
		pad swap send-line [char] . emit
	repeat  drop
	;

: go  (  - )
	['] open-hex-object-file catch if
		." Problem opening rom.hex" abort
	then  cr
	."    To download to the target," cr
	." insert the jumper at JP3 and RESET the target board," cr
	." then press the SPACE key..." cr
	key drop
	['] download catch if
		." Problem downloading rom.hex" abort
	then
	close-hex-object-file ;

go
bye

