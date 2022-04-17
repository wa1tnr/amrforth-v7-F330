0 [if]   compile.fs    Load file for amrForth for Linux, 8051 family.
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

\ ALL20030814   ROM used also in hex.

[then]

only forth also definitions

( *) constant romming
true constant extending

: has  (  - flag)  BL word find nip 0<> ; immediate
: $=ngfv6  ( addr1 len1 addr2 len2 - flag) compare 0= ;

: warn  warnings on ;
: nowarn  warnings off ;

include vtags.fs
include ~+/amrfconf.fs	\ ~+/ means current working directory to Gforth.
include metacomp.fs
include asm8051.fs
sfr-file included
include bootloader.fs
include kernel8051.fs
include debug.fs
\ cr .( See compile.fs for choice of division operators. )
\ include mathunsigned.fs  cr .( Unsigned division loaded )
\ include mathfloored.fs   cr .( Floored division loaded )
include mathfloored.fs   cr .( Floored division loaded )
\ include mathsymmetric.fs cr .( Symmetric division loaded )
include ~+/job.fs
include end8051.fs
in-forth

include bin2hex.fs

: in-full-pages  ( n1 - n2)
	512 /mod swap if  1 +  then  512 * ;

\ The 552 and friends start at $8000 when developing, 0 when romming.
0
has bootloader-installed 0= romming 0= and [if] $8000 or [then]
constant rom-offset
: save-object-code  (  - )
	s" rom.bin" w/o create-file throw >r
	rom-offset there romHERE rom-offset -
	in-full-pages r@ write-file throw
	r> close-file throw
	save-rom.hex ;

' save-object-code catch [if]
	.( Problem saving object code.) cr
[then]

\ Disable target compiler, only compile once.
warnings off
in-forth  : _c ." disabled " ; : c _c ; : r _c ; in-meta
warnings on

cr .( Host Stack = ) .s
cr .( ROM used = ) romHERE dup u. hex.  \ ALL20030814i
iram.log close-file drop
s" immed.log" immed make-vocabulary.log
close-log-files
in-meta

\ QUIT here strands Gforth and Tclpip83 in Windows.  Must execute BYE.
\ quit  \ Avoid the Gforth prompt.

