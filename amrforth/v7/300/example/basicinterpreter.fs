\ basicinterpreter.fs

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

include amrfconf.fs
windows [if]
	include serial-windows.fs
[else]	include serial-linux.fs
[then]	open-comm

warnings off

include colorscheme.fs

\ This is a search order trick to help search a single vocabulary,
\ without also looking in forth or root.
: exclusively   ( a - cfa -1 | a 0)
	dup count context @ search-wordlist
	dup if   rot drop   then ;

: file-exists?  ( addr len - flag)
	r/o open-file 0= dup if  over close-file throw  then  nip ;

: ?include  (  - )
	BL word count 2dup file-exists? if
		2dup included
	then	2drop ;

only forth also root definitions
	vocabulary forthsymbols
	vocabulary hostbasic
	vocabulary basicsymbols
	vocabulary operator
	vocabulary bytevariable 
	vocabulary wordvariable
	vocabulary sfrreader
	vocabulary sfrwriter
	vocabulary aliases
	vocabulary bitio
only forth also forthsymbols definitions also forth
	?include symbols.log
only forth also basicsymbols definitions also forth
	?include basicsymbols.txt
only forth also operator definitions also forth
	?include operators.txt
only forth also bytevariable definitions also forth
	?include bytevariables.txt
only forth also wordvariable definitions also forth
	?include wordvariables.txt
only forth also sfrreader definitions also forth
	?include readsfrs.txt
only forth also sfrwriter definitions also forth
	?include writesfrs.txt
only forth also aliases definitions also forth
	?include aliases.txt
only forth also bitio definitions also forth
	?include bitio.txt
only forth also definitions

: membership  ( addr - )
	create  ,
	does> @ also execute exclusively previous ;
   ' hostbasic membership hostbasic?
' basicsymbols membership basicsymbol?
' bytevariable membership bytevariable?
' wordvariable membership wordvariable?
    ' operator membership operator?
   ' sfrreader membership sfrreader?
   ' sfrwriter membership sfrwriter?
     ' aliases membership alias?	
       ' bitio membership bitio?

: shows  ( addr - )
	create  ,
	does> @ also execute words previous ;
   ' hostbasic shows hostbasic-words
' basicsymbols shows basicsymbol-words
' bytevariable shows bytevariable-words
' wordvariable shows wordvariable-words
    ' operator shows operator-words
   ' sfrwriter shows sfrwriter-words
     ' aliases shows alias-words	
       ' bitio shows bitio-words

0 value timer

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

\ Is counted, BL delimited string a literal?
: literal?  ( a - n flag)
	['] number catch if  false dup exit  then
	drop true ;

: fetch-bytevariable  ( addr - )
	execute >put [ also forthsymbols ] c@ [ previous ] >execute ;
	
: fetch-wordvariable  ( addr - )
	execute >put [ also forthsymbols ] @ [ previous ] >execute ;

: fetch-sfr  ( addr - ) execute >execute ;

: fetch-bitio  ( addr - )
	execute >put
	[ also forthsymbols ] p0bit@ [ previous ] >execute
	;

: execute-operator  ( addr - )
	operator? if  execute >execute exit  then
	." Not a valid operator " abort ;

: execute-operand  ( addr - )
	bytevariable? if  fetch-bytevariable exit  then
	wordvariable? if  fetch-wordvariable exit  then
	sfrreader? if  fetch-sfr exit  then
	bitio? if  fetch-bitio exit  then
	literal? if  >put exit  then
	." Not a valid operand " abort ;

: target-.s  (  - )
	[ also forthsymbols ] .s [ previous ] >execute ;

: infix  (  - )
	BL word execute-operand
	begin
		BL word operator? 0= if  drop exit  then
		BL word execute-operand
		( operator) execute >execute
	again ;

\ Make this check for the equal sign!
: read-equals  (  - )
	BL word count s" =" compare if
		." Assignment requires = sign " abort
	then ;

: assign-bytevariable  ( addr - )
	execute read-equals infix >put
	[ also forthsymbols ] c! [ previous ] >execute ;

: assign-wordvariable  (  - )
	execute read-equals infix >put
	[ also forthsymbols ] ! [ previous ] >execute ;

: assign-sfr  (  - )
	execute read-equals infix >execute ;

: assign-bitio  (  - )
	execute read-equals infix >put
	[ also forthsymbols ] p0bit! [ previous ] >execute ;

only forth also hostbasic definitions also forth
: PRINT  (  - )
	infix
	[ also forthsymbols ] . [ previous ] >execute ;

: LET  (  - )
	BL word
	bytevariable? if  assign-bytevariable exit  then
	wordvariable? if  assign-wordvariable exit  then
	sfrwriter? if  assign-sfr exit  then
	bitio? if  assign-bitio exit  then
	." Not a valid LET expression " abort ;

: GOSUB  (  - )
	;

: PAUSE  (  - )
	infix [ also forthsymbols ] ms [ previous ] >execute ;

: WAIT  (  - )
	infix [ also forthsymbols ] wait [ previous ] >execute ;

: HIGH  (  - )
	infix [ also forthsymbols ] high [ previous ] >execute ;

: LOW  (  - )
	infix [ also forthsymbols ] low [ previous ] >execute ;

: words  (  - )
	cr
	basic-colors ." Host BASIC words: " label-colors hostbasic-words cr
	basic-colors ." BASIC Symbols: " label-colors basicsymbol-words cr
	basic-colors ." Byte Variables: " label-colors bytevariable-words cr
	basic-colors ." Word Variables: " label-colors wordvariable-words cr
	basic-colors ." Operators: " label-colors operator-words cr
	basic-colors ." SFR Writers: " label-colors sfrwriter-words cr
	basic-colors ." Bit I/O: " label-colors bitio-words cr
	basic-colors ." Aliases: " label-colors alias-words cr
	;

: bye  (  - ) cr text_normal bye ;

: timing  (  - ) timer 0= to timer ."  timing=" timer . ;


only forth also definitions

: interpret-word  ( a - )
	alias? if  exit  then
	basicsymbol? if  execute >execute exit  then
	bytevariable? if  assign-bytevariable exit  then
	wordvariable? if  assign-wordvariable exit  then
	sfrwriter? if  assign-sfr exit  then
	bitio? if  assign-bitio exit  then
	hostbasic? if  execute exit  then
	." Not a valid amrBASIC expression" abort ;

: please  (  - )
	begin	bl word dup c@ while
		interpret-word
	repeat drop ;

: interpret  (  - )
	pad 128 blank  s" please " pad place
	pad count + 80 accept >r  pad count r> +
	utime ( **) 2>r
	evaluate interpreter-colors ."  ok   "
	utime 2r> ( **) d-
	timer 0= if  2drop exit  then
	<# # # # # # # [char] . hold #s #> 3 - type
	[char] s emit ;

: go  (  - )
	cr interpreter-colors ." ok" cr
	begin  ['] interpret catch drop .s cr  again ;

