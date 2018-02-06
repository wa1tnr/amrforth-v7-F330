\ interpret.fs

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

include amrfconf.fs
windows [if]
	include serial-windows.fs
[else]	include serial-linux.fs
[then]	open-comm

warnings off

0 value alone?  \ By default, not standalone.

include colorscheme.fs

create argument 256 allot
\ create the-word  256 allot
: see  (  - )
	hello-decompiler
	BL word count escape-quotes argument place
	gforth-string
	windows-string count pad +place
	s\" ./dis8051.fs -e \"see " pad +place
	argument count pad +place
	s\"  bye\"" pad +place
	pad count system cr
	hello-forth-interpreter
	;

only forth also root definitions
vocabulary symbols
only forth also symbols definitions also forth
include ./symbols.log  \ Read the symbol table.
only forth also definitions

: handle-key  ( c - )
	dup  1 = if  key  emit-s exit  then
	dup  2 = if  key? emit-s exit  then
	dup  7 = if  r> 2drop    exit  then  \ Ok.
	dup 13 = if  drop        exit  then  \ Linux EOL.
	emit ;

: -listen  (  - ) begin  key-s handle-key  again ;
: listen  (  - ) target-colors -listen ;

\ Host sends byte commands to target.
: sputter  ( n - )   dup emit-s 8 rshift emit-s ;

: >put  ( n - )
	clear-sbuf 1 emit-s sputter listen interpreter-colors ;

: >execute  ( a - )
	clear-sbuf 2 emit-s sputter listen interpreter-colors ;

\ Get a 16 bit number off the stack.  This is for single-stepping.
: >get  (  - c)
	clear-sbuf
	2 emit-s s" symbols dup forth" evaluate sputter listen
	2 emit-s s" symbols flip forth" evaluate sputter listen
	2 emit-s s" symbols emit forth" evaluate sputter key-s 8 lshift listen
	2 emit-s s" symbols emit forth" evaluate sputter key-s + listen
	interpreter-colors ;

include singlestep.fs

\ This is a search order trick to help search a single vocabulary,
\ without also looking in forth or root.
: exclusively   ( a - cfa -1 | a 0)
	dup count context @ search-wordlist
	dup if   rot drop   then ;
	
\ Is counted string a symbol?
: symbol?  ( a - cfa ? | a 0)
        also symbols exclusively previous ;

: show-symbols  (  - ) also symbols words previous ;

\ Is counted, BL delimited string a literal?
: literal?  ( a - n flag)
	['] number catch if  false dup exit  then
	drop true ;

: bye?  ( a - a flag)
	dup count s" bye" compare 0= ;

: words?  ( a - a flag)
	dup count s" words" compare 0= ;

: step?  ( a - a flag)
	dup count s" step" compare 0= ;

: see?  ( a - a flag)
	dup count s" see" compare 0= ;

0 value timing

: timing?  ( a - a flag)
	dup count s" timing" compare 0= ;

: toggle-timing  (  - )
	timing 0= to timing
	."  timing=" timing . ;

\ Note that the symbol? test comes first.  This allows symbol names like
\ 'words' and 'bye' and any other special words added in the future, to
\ take precedence over the special words, just like in most forths.
: interpret-word  ( a - )
	symbol? if  execute >execute exit  then
	step? if  drop step exit  then
	see? if  drop see exit  then
	words? if  drop show-symbols exit  then
	bye? if  cr text_normal bye  then
	timing? if  drop toggle-timing exit  then
	literal? if  >put exit  then
	." ?" abort ;

: please  (  - )
	begin	bl word dup c@ while
		interpret-word
	repeat drop ;

: interpret  (  - )
	pad 128 blank  s" please " pad place
	pad count + 80 accept >r  pad count r> +
	utime ( **) 2>r
	evaluate interpreter-colors ." ok   "
	utime 2r> ( **) d-
	timing 0= if  2drop exit  then
	<# # # # # # # [char] . hold #s #> 3 - type
	[char] s emit ;

: go  (  - )
	cr interpreter-colors ." ok" cr
	begin  ['] interpret catch drop cr  again ;

