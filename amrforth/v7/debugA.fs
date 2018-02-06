\ debugA.fs
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

\ ALL20030807 .sh added
\ ALL20030706 space .. spaces -> all-lib.fs
\ ALL20030701 dump modified, dumpi dumpc ... added

An alternate set of debug tools.  More extensive and readable memory
dumps.

[then]

: ***debugA.fs*** ;

0 [IF]
in-meta

\ .( Loading debug.fs) cr

\ : space  (  - )  $20 emit ;

: cr  (  - )  13 emit  10 emit ;

code (.hex)  ( digit - )
    SP inc  Apop
    $90 # A add  A DA
    $40 # A addc  A DA
    Apush  A clr  Apush  next c;
: .hex  ( digit - ) (.hex) emit ;

: h.  ( n - )  3 for  16 /mod  next 4 for  .hex  next  space ;

: hb.  ( c - )  $ff and 16 /mod .hex .hex space ;

: dot  ( u base - )
	push -1 swap
	begin  i /mod dup 0= until  pop 2drop
	begin  dup 10 < not 7 and + [char] 0 + emit dup 0< until
	drop ;

: u.  ( u - )  10 dot space ;

\ Only decimal numbers show a sign.
: .  ( n - ) dup 0< if  abs [char] - emit  then  u. ;

: spaces ( n - ) 1 max for  space  next ;		\ ALL20030701+
[THEN]

 1 [IF]
cvariable %dumpa
create vdump 	\ dumpi  dumpc     dumpx
		\ iRAM   cRAM      xRAM
		' c@i ,  ' c@x ,   ' c@xd ,
: dump@   %dumpa c@ vdump + @x execute ;
: (dump)  ( addr len - )				\ ALL20030701
	cr 5 SPACES 				\ print fancy header
	16 for 16 i - dup HB. $0f and $07 = if   SPACE   then next
	over $000f and + swap $0fff0 and swap 	\ adjust to 16/line
	for	( -- a )	
		dup $000f and
		dup $08 = if   SPACE   then
		    $00 = if   cr dup H.  then  
		dup 1 + swap dump@ HB.	( -- a' )
	next  drop ( a -- ) ;
: dump  ( addr len ) 4 %dumpa c!   (dump) ;	\ Xmem
: dumpi ( addr len ) 0 %dumpa c!   (dump) ;	\ Imem   
: dumpc ( addr len ) 2 %dumpa c!   (dump) ;	\ Cmem

: .IRAM 0 $100 dumpi ;					\ ALL20030701-
 [THEN]

: .s  (  - )
	[char] < emit depth 10 dot [char] > emit space
	depth 0= if  exit  then
	[ SP0 1 - ] literal depth 1 - for
		dup @ . 2 -
	next  drop ;
: .sh (  - )
	[char] < emit depth 10 dot [char] > emit space
	depth 0= if  exit  then
	[ SP0 1 - ] literal depth 1 - for
		dup @ h. 2 -
	next  drop ;

\ : type  ( addr u - )  1 max for  count emit  next  drop ;

