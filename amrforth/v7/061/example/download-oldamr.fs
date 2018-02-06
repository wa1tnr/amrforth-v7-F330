\ download-oldamr.fs

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

: sputter  ( n - ) dup emit-s 8 rshift emit-s ;

: download-all  (  - )
	clear-sbuf cr
	4 emit-s $8000 sputter romHERE $7fff and dup sputter
	0 do  i c@-t emit-s i 511 and 0= if  ." ."  then  loop
	2 emit-s $8000 sputter
	;

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

