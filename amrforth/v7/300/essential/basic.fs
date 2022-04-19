\ basic.fs
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

only forth also root definitions
nowarn

' noop IS dobacktrace  \ Turn off the backtrace after abort.

vocabulary basic-keywords
vocabulary basic-variables
vocabulary basic-operators
vocabulary basic-symbols
vocabulary basic-labels

in-forth

sfr-file s" sfr-f300.fs" compare 0= [if]
	true constant has-f300?
	false constant has-f000?
[then]
sfr-file s" sfr-f000.fs" compare 0= [if]
	true constant has-f000?
	false constant has-f300?
[then]

\ Making symbol tables for Tcl/Tk.
variable basic-symbols-file   0 basic-symbols-file !
variable byte-variables-file  0 byte-variables-file !
variable word-variables-file  0 word-variables-file !
variable operators-file       0 operators-file !
variable read-sfr-file        0 read-sfr-file !
variable write-sfr-file       0 write-sfr-file !
variable bit-io-file          0 bit-io-file !
variable aliases-file         0 aliases-file !

: basic-symbols-id  (  - n)
	basic-symbols-file @ ?dup if  exit  then
	s" basicsymbols.txt" w/o create-file throw
	dup basic-symbols-file ! ;

: byte-variables-id  (  - n)
	byte-variables-file @ ?dup if  exit  then
	s" bytevariables.txt" w/o create-file throw
	dup byte-variables-file ! ;

: word-variables-id  (  - n)
	word-variables-file @ ?dup if  exit  then
	s" wordvariables.txt" w/o create-file throw
	dup word-variables-file ! ;

: operators-id  (  - n)
	operators-file @ ?dup if  exit  then
	s" operators.txt" w/o create-file throw
	dup operators-file ! ;

: read-sfr-id  (  - n)
	read-sfr-file @ ?dup if  exit  then
	s" readsfrs.txt" w/o create-file throw
	dup read-sfr-file ! ;

: write-sfr-id  (  - n)
	write-sfr-file @ ?dup if  exit  then
	s" writesfrs.txt" w/o create-file throw
	dup write-sfr-file ! ;

: bit-io-id  (  - n)
	bit-io-file @ ?dup if  exit  then
	s" bitio.txt" w/o create-file throw
	dup bit-io-file ! ;

: aliases-id  (  - n)
	aliases-file @ ?dup if  exit  then
	s" aliases.txt" w/o create-file throw
	dup aliases-file ! ;

: symbol-table-entry  ( n fid - )
	>r
	[char] : r@ emit-file throw
	BL r@ emit-file throw
	>in @ BL word count r@ write-file throw >in !
	BL r@ emit-file throw
	base @ swap decimal 0 <# #s #> r@ write-file throw
	base !
	BL r@ emit-file throw
	[char] ; r@ emit-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: alias-entry  (  - )
	aliases-id >r
	[char] : r@ emit-file throw
	BL r@ emit-file throw
	>in @ BL word count r@ write-file throw >in !
	BL r@ emit-file throw
	>in @ BL word drop BL word drop BL word count
	r@ write-file throw >in !
	BL r@ emit-file throw
	[char] ; r@ emit-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: subroutine-entry  (  - )
	romHERE basic-symbols-id symbol-table-entry ;

: byte-variable-entry  (  - )
	cpuHERE byte-variables-id symbol-table-entry ;

: word-variable-entry  (  - )
	cpuHERE word-variables-id symbol-table-entry ;

: operators-entry  ( a - )
	operators-id symbol-table-entry
	BL word drop ;

: read-sfr-entry  (  - )
	>in @ >r
	BL word drop  \ BASIC name.
	BL word drop  \ storer.
	BL word target? not abort" Bad SFR entry" execute  \ fetcher.
	r> >in ! read-sfr-id symbol-table-entry ;
	
: write-sfr-entry  (  - )
	>in @ >r
	BL word drop  \ BASIC name.
	BL word target? not abort" Bad SFR entry" execute  \ storer.
	r> >in ! write-sfr-id symbol-table-entry ;

: bit-io-entry  ( n - )
	bit-io-id symbol-table-entry ;

: operators  (  - )  also basic-operators words previous ;
: keywords   (  - )  also basic-keywords  words previous ;
: variables  (  - )  also basic-variables words previous ;
: labels     (  - )  also basic-labels    words previous ;
: symbols    (  - )  also basic-symbols   words previous ;

1001 constant not-a-keyword
1002 constant not-an-equal
1003 constant not-a-variable
1004 constant not-a-label
1005 constant i/o-not-implemented
1006 constant not-an-operator
1007 constant not-an-operand
1008 constant not-keyword-nor-variable
1009 constant not-variable-nor-number
1010 constant not-a-number
1011 constant bad-and
1012 constant bad-or
1013 constant bad-to
1014 constant bad-step
1015 constant bad-next
1016 constant not-compiling
1017 constant i/o-not-allowed
1018 constant not-forth

: show-error  ( n - )
	cr ." *** amrBASIC Error ***> "
	dup not-a-keyword = if
		drop ." Expected one of the following keywords: " keywords exit
	then
	dup not-an-equal = if
		drop ." Expected an = sign" drop exit
	then
	dup not-a-variable = if
		drop ." Expected one of the following variables: " variables exit
	then
	dup not-a-label = if
		drop ." Expected one of the following labels: " labels exit
	then
	dup i/o-not-implemented = if
		drop ." I/O operation not implemented." exit
	then
	dup not-an-operator = if
		drop ." Expected one of the following operators: " operators exit
	then
	dup not-an-operand = if
		drop ." Expected a number or one of the following operands:"
		cr ." Variables:" variables cr ." Symbols:" symbols exit
	then
	dup not-keyword-nor-variable = if
		drop ." Expected one of the following keywords or variables:"
		cr ." Keywords:" keywords cr ." Variables:" variables exit
	then
	dup not-variable-nor-number = if
		drop ." Expected a number or one of the following variables:"
		variables exit
	then
	dup not-a-number = if
		drop ." Expected a number." exit
	then
	dup bad-and = if
		drop ." AND must come between IF and THEN." exit
	then
	dup bad-or = if
		drop ." OR must come between IF and THEN." exit
	then
	dup bad-to = if
		drop ." TO must be followed by a number." exit
	then
	dup bad-step = if
		drop ." STEP must be followed by a number." exit
	then
	dup bad-next = if
		drop ." NEXT must be followed by a variable." exit
	then
	dup not-compiling = if
		drop ." This command must be compiled, not interpreted." exit
	then
	dup i/o-not-allowed = if
		drop ." I/O variables not allowed as FOR NEXT counters." exit
	then
	dup not-forth = if
		drop ." Not a Forth word." exit
	then
	drop ." Unknown error." ;

: basic-error  ( n - )  ?dup if  show-error cr abort  then ;

: compile-only  (  - ) state @ 0= if  not-compiling throw  then ;

: quote  (  - a)
	[ also meta ]] postpone [t'] [[ previous ] ; immediate

: keyword?  ( a - cfa flag | a 0)
	also basic-keywords exclusively previous ;

: symbol?  ( a - cfa flag | a 0)
	also basic-symbols exclusively previous ;

: operator?  ( a - cfa flag | a 0)
	also basic-operators exclusively previous ;

: variable?  ( a - cfa flag | a 0)
	also basic-variables exclusively previous ;

: label?  ( a - cfa flag | a 0)
	also basic-labels exclusively previous ;

: keyword:  (  - )
	also basic-keywords definitions : previous definitions ;

: special:  (  - )
	also basic-variables definitions : previous definitions ;

\ This is specific to Gforth I think.
: number?  ( a - a 0 | n -1) number? dup 0> if  nip  then  0= not ;

\ Remove leading blanks from input stream.
: -leading  begin  tib >in @ + c@ BL = while  1 >in +!  repeat ;

keyword: SYMBOL 
	alias-entry
	>in @ >r BL word drop BL word drop BL word r> >in !
	variable? if
		also basic-variables definitions  create  previous
		BL word drop  -1 c,  BL word drop
	else
		also basic-symbols definitions  create  previous
		BL word drop  0 c,
		drop BL word number? not if
			not-variable-nor-number throw
		then
	then  ,
	does>
		count if  @ execute  else  @ literal-t  then ;

variable for-next
: for-next?  (  - flag)  for-next @ ;
: use-for-next  (  - )  true for-next ! ;
: no-for-next  (  - )  false for-next ! ;

variable side
: left-side?  (  - flag)  side @ ;
: left-side  (  - )  true side ! ;
: right-side  (  - )  false side ! ;

variable using=
: use=  (  - )  true using= ! ;
: no=  (  - )  false using= ! ;
: use=?  (  - flag)  using= @ ;

: display  ( a - a)  dup count ." ***" type ." ***" cr ;

: operand  ( a - )
	variable? if  execute exit  then
	symbol? if  execute exit  then
	number? if  literal-t exit  then
	not-an-operand throw ;

: operator  ( a - a) operator? not if  not-an-operator throw  then ;

: infix  (  - )
	right-side BL word operand
	begin
		>in @ BL word
		dup count s" :" compare 0= if  2drop exit  then
		dup count s" ," compare 0= if  2drop exit  then
		dup count s" "  compare 0= if  2drop exit  then
		dup count s" '" compare 0= >r
		dup count s" REM" compare 0= >r
		dup count s" rem" compare 0= r> or r> or if 
			postpone \  2drop exit
		then
		dup count s" to" compare 0= if
			drop >in ! exit
		then
		nip operator ( *)
		BL word operand
		( *) execute token,
	again ;
	
: assignment  (  - )
	BL word count s" =" compare if  not-an-equal throw  then  infix ;

in-compiler

: **  um* drop ;
: /  0 swap um/mod nip ;
: // 0 swap um/mod drop ;
: &/ 0= and ;
: ^/ 0= xor ;
: |/ 0= or  ;

: <= > not ;
: >= < not ;
: <> = not ;

\ Used by BASIC version of IF.
code ?skip  ( n - )
    DPH pop  DPL pop
    R2 A orl  0= if  DPTR inc  DPTR inc  DPTR inc  then
    |drop  DPL push  DPH push  ret c;

m: make-operator  ( n - )
	dup operators-entry
	create , in-meta
	does>  @ ;

only forth also target also basic-operators also definitions

+   constant +		+   operators-entry +
-   constant -		-   operators-entry -
*   constant *		*   operators-entry *
**  constant **		**  operators-entry **
/   constant /		/   operators-entry /
//  constant //		//  operators-entry //
and constant & 		&   operators-entry &
xor constant ^		^   operators-entry ^
or  constant |		|   operators-entry |
&/  constant &/		&/  operators-entry &/
^/  constant ^/		^/  operators-entry ^/
|/  constant |/		|/  operators-entry |/
max constant max	max operators-entry max
min constant min	min operators-entry min
=   constant =		=   operators-entry =
<   constant <		<   operators-entry <
>   constant >		>   operators-entry >
<=  constant <=		<=  operators-entry <=
>=  constant >=		>=  operators-entry >=
<>  constant <>		<>  operators-entry <>

in-forth

\ ----- Byte sized variables in internal RAM ----- /

in-meta

: byte-next  ( addr limit step variable - )
	>r dup r@ c@i + dup r> c!i  ( addr limit step current)
	swap  ( addr limit current step)
	0< if  <=  else  >=  then if  r> drop execute  then ;

: word-next  ( addr limit step variable - )
	>r dup r@ @i + dup r> !i  ( addr limit step current)
	swap  ( addr limit current step)
	0< if  <=  else  >=  then if  r> drop execute  then ;

code interrupts-disabled  (  - )  7 .IE clr   next c;
code interrupts-enabled   (  - )  7 .IE setb  next c;

in-forth

: BYTE-VARIABLE  (  - )
	byte-variable-entry
	also basic-variables definitions create
	previous definitions cpuHERE c, 1 cpuDP +!
	does>
		c@
		for-next? if
			compile-only
			literal-t quote byte-next token,
			no-for-next exit
		then
		left-side? if
			use=? if
				>r assignment r>
			then
			literal-t quote c!i token,
			right-side exit
		then
		literal-t quote c@i token, ;

: WORD-VARIABLE  (  - )
	word-variable-entry
	also basic-variables definitions create 
	previous definitions cpuHERE c,
	does>
		c@
		for-next? if
			compile-only
			literal-t quote word-next token,
			no-for-next exit
		then
		left-side? if
			use=? if
				>r assignment r>
			then
			literal-t quote !i token,
			right-side exit
		then
		literal-t quote @i token, ;

word-variable w0  byte-variable b0  byte-variable b1
word-variable w1  byte-variable b2  byte-variable b3
word-variable w2  byte-variable b4  byte-variable b5
word-variable w3  byte-variable b6  byte-variable b7
word-variable w4  byte-variable b8  byte-variable b9

\ ----- SFR's ----- /

: I/O-VARIABLE 
\ e.g. I/O-VARIABLE P0 P0! P0@
\ The BASIC word is P0. Uses P0! for storage and P0@ for fetching.
	read-sfr-entry write-sfr-entry
	also basic-variables definitions  create  previous definitions
	[ also meta ]]  ' host,  ' host,  [[ previous ]
	does>
		for-next? if  i/o-not-allowed throw  then
		left-side? if
			use=? if
				>r assignment r>
			then
			right-side
		else
			4 +
		then
		@ dup quote noop = if  i/o-not-implemented throw  then
		token, ;

in-meta

code P0!  ( c - ) A P0 mov  |drop  next c;

code P0@  (  - c) |dup  P0 A mov  0 # R2 mov  next c;

has-f000? [if]

code PRT0CF!  ( c - ) A PRT0CF mov  |drop  next c;

code PRT0CF@  (  - c) |dup  PRT0CF A mov  0 # R2 mov  next c;

[then] \ has-f000?

has-f300? [if]

\ Assumes no ADC being used for now.
code P0MDOUT!  ( c - )
	$ff # P0MDIN mov  \ All digital, no analog.
    A P0MDOUT mov  |drop  next c;

code P0MDOUT@  (  - c)
    |dup  P0MDOUT A mov  0 # R2 mov  next c;

[then] \ has-f300?

in-forth

I/O-VARIABLE pins P0! P0@
has-f000? [if]  I/O-VARIABLE dirs PRT0CF! PRT0CF@  [then]
has-f300? [if]  I/O-VARIABLE dirs P0MDOUT! P0MDOUT@  [then]

\ ----- Bits on the I/O port ----- /

in-compiler

code P0and!  ( c - ) A P0 anl  |drop  next c;
code P0or!  ( c - ) A P0 orl  |drop  next c;

: P0bit!  ( 0|1 bit-mask - )
	swap if  P0or!  else  invert P0and!  then ;

: P0bit@  ( bit-mask - 0|1) P0@ and 0= not 1 and ;

create '2** 1 c, 2 c, 4 c, 8 c, $10 c, $20 c, $40 c, $80 c,
: 2**  0 max 7 min '2** + c@ ;

: high  ( bit# - )  2** P0or! ;
: low   ( bit# - )  2** invert P0and! ;

in-forth

keyword: HIGH  infix quote high token, ;

keyword: LOW  infix quote low token, ;

: I/O-BIT-VARIABLE 
\ e.g. $10 I/O-BIT-VARIABLE pin4
	dup bit-io-entry
	also basic-variables definitions  create  previous definitions
	host,
	does>
		for-next? if  i/o-not-allowed throw  then
		left-side? if
			use=? if
				>r assignment r>
			then
			@ literal-t quote P0bit! token,
			right-side
		else
			@ literal-t quote P0bit@ token,
		then ;

$01 I/O-BIT-VARIABLE pin0
$02 I/O-BIT-VARIABLE pin1
$04 I/O-BIT-VARIABLE pin2
$08 I/O-BIT-VARIABLE pin3
$10 I/O-BIT-VARIABLE pin4
$20 I/O-BIT-VARIABLE pin5
$40 I/O-BIT-VARIABLE pin6
$80 I/O-BIT-VARIABLE pin7

special: STACK  left-side? if  use= assignment  then ;

\ ----- Bit sized variables, overlayed on byte variables ----- /


\ Labels can be forward referenced.

\ Create a new label on the fly, forward referenced.
\ The first word of data is the label's address, the following word is a
\ flag telling whether the reference has been resolved yet.
: (forward)  (  - )
	create  0 , 0 c,  in-forth
	does>  (  - a)
		dup 4 + c@ if  @ exit  then
		here 0 ,  swap 2dup @ swap ! !  \ Linked list.
		romHERE 1 + , 0 ;
: forward  (  - )
	also basic-labels definitions (forward) ;

\ Given the head of a list of forward references, follow the list and
\ resolve each reference to the actual address.
: resolve  ( a - )
	>body
	begin
		@ ?dup while  \ Follow the list of references.
		dup 4 + @ romHERE swap !-t
	repeat ;

\ Forward references are recognized by their code field addresses.
\ This is not very portable. Here's how it works in Gforth.
(forward) this ' this >code-address constant #forward#
 
: forward?  ( a - ?)  >code-address #forward# = ;

\ This is a simple label that is not forward referenced.
: (label)  (  - )
	nowarn create warn romHERE ,
	does>  (  - a)  @ ;

\ Declare a label, possibly one that needs to be resolved.
: :label  (  - )
	subroutine-entry
	>in @  BL word label? over forward? and if
		dup resolve  true over >body 4 + c!  \ Mark as resolved.
		romHERE swap >body !  drop \ >in !
	else
		drop >in !
		also basic-labels definitions
		(label)  \ Create a simple label.
		previous definitions
	then ;

\ Either create a forward referenced label, or get the address of an
\ already resolved label.
: >label  (  - a)
	>in @ >r  BL word label? not if
		drop  r@ >in ! forward
		r@ >in ! BL word label? drop
	then
	execute r> drop ;

\ The BASIC interpreter checks each potential keyword first to see if it
\ could be a label declaration. If so the label is made this way.
: make-label?  ( a - flag)  count + 1 - c@ [char] : = ;

: make-label  ( a - )
	s" :label " pad place  count 1 - pad +place
	pad count evaluate put-tags-entry ;

: compile-one-keyword  (  - )
	begin
		\ Remove leading spaces.
		-leading
		\ Check for need to refill buffer.
		BL word count pocket place  pocket dup c@ 0=
	while
		drop refill 0= if  exit  then
	repeat
	\ If the word ends in : make it a label.
	dup make-label? if  make-label exit  then
	keyword? if  execute exit  then
	left-side variable? if  execute exit  then
	not-keyword-nor-variable throw ;

variable if-then

\ This is the compiler.
: BASIC  (  - )
	in-forth  state on  use=  false if-then !  no-for-next 
	begin
		state @ while
		['] compile-one-keyword catch basic-error
	repeat ;

\ Maybe this installs the last word defined as GO.
keyword: END  (  - ) state off ;

keyword: GOTO 
	compile-only >label [ also asm ] ljmp [ previous ] ;

keyword: GOSUB 
	>label [ also asm ] lcall [ previous ] ;

keyword: RETURN 
	compile-only [ also asm ] ret [ previous ] ;

keyword: '  compile-only postpone \ ; immediate

keyword: REM  compile-only postpone \ ; immediate

\ If we decide to make spaces optional in assignments, LET will probably
\ parse the whole assignment statement. For now it is not needed.
keyword: LET 
	;

keyword: IF 
	true if-then !
	compile-only right-side BL word operand
	begin
		>in @ >r
		BL word dup c@ 0= if  r> 2drop exit  then
		dup count s" THEN" drop capscomp 0= if  r> >in ! drop exit  then
		dup count s" AND" drop capscomp 0= if  r> >in ! drop exit  then
		dup count s" OR" drop capscomp 0= if  r> >in ! drop exit  then
		r> drop operator BL word dup c@ 0= if  2drop exit  then operand
		execute token,
	again ;

keyword: AND 
	compile-only if-then @ not if  bad-and throw  then  quote and token,
	[ also basic-keywords ] IF [ previous ] ;

keyword: OR 
	compile-only if-then @ not if  bad-or throw  then  quote or token,  
	[ also basic-keywords ] IF [ previous ] ;

keyword: THEN 
	compile-only false if-then !  quote ?skip token,
	[ also basic-keywords ] GOTO [ previous ] ;

\ The assignment following FOR is self-executing.
keyword: FOR 
	;

\ Establish the limit of the loop on data stack, also tentative step.
keyword: TO  (  - addr limit step)
	compile-only  romHERE literal-t
	BL word number? not if  bad-to throw  then  literal-t 1 ;

\ Change step from the default value of 1
keyword: STEP  ( limit step - limit step')
	compile-only
	drop BL word number? not if  bad-step throw  then ;

keyword: NEXT  ( step - ) 
	literal-t
	BL word variable? not if  not-a-variable throw  then
	use-for-next execute
	quote drop token, ;

keyword: BYE  bye ;

keyword: SERIN 
	begin
		BL word dup c@ 0=
		over count 1 = swap c@ [char] : = and or if
			drop exit
		then
		variable? not if  not-a-variable throw  then
		quote key token, left-side no= execute use=
	again ;

keyword: SEROUT 
	begin
		BL word dup c@ 0=
		over count 1 = swap c@ [char] : = and or if
			drop exit
		then
		operand  quote emit token,
	again ;

\ ----- MS timer module ----- /

\ Use last channel of the PCA to keep an MS timer.

in-forth decimal

create '2** 1 c, 2 c, 4 c, 8 c, $10 c, $20 c, $40 c, $80 c,
: 2**  0 max 7 min '2** + c@ ;

in-meta

\ Be sure this doesn't conflict with b0.
variable ms-counter

: atomic-@  ( a - n)
	[ in-assembler  IE push  7 .IE clr  ]
	@i
	[ in-assembler  IE pop  ]
	;

code add-timer  ( n - )
    IE push  7 .IE clr
    ms-counter 1 + direct A add  A ms-counter 1 + direct mov
    ms-counter direct A mov  R2 A addc  A ms-counter direct mov
    IE pop  |drop  next c;

in-forth

: TIMER-VARIABLE  (  - )
	word-variable-entry
	also basic-variables definitions create 
	previous definitions ms-counter c,
	does>
		c@
		for-next? abort" For,next can't use TIMER"
		left-side? abort" TIMER can't be assigned"
		literal-t
		quote atomic-@ token, ;

TIMER-VARIABLE TIMER

in-meta

has-f000? [if]

frequency @ 1000 um*  12000000 um/mod nip constant cycles/ms
cycles/ms $ff and constant ms-reload-low
cycles/ms 8 rshift $ff and constant ms-reload-high

\ Counts down to zero by MS ticks.
label ms-timer-interrupt
	PSW push  ACC push
	PCA0CN A mov  4 .ACC clr? if  \ Maybe another PCA channel?
		ACC pop  PSW pop  reti
	then
	$10 invert # PCA0CN anl  \ Clear interrupt bit.
	ms-counter 1 + direct A mov  ms-counter direct A orl 0= if
		1 invert # PCA0CPM4 anl  \ Disable this interrupt.
		ACC pop  PSW pop  reti
	then
	ms-reload-low # A mov  PCA0CPL4 A add  A PCA0CPL4 mov
	ms-reload-high # A mov  PCA0CPH4 A addc  A PCA0CPH4 mov
	ms-counter 1 + direct A mov  0= if
		ms-counter direct dec
	then
	ms-counter 1 + direct dec
	ACC pop  PSW pop  reti c;
ms-timer-interrupt $4b int!

\ Configure system to use channel 4 of PCA.
code set-mstimer  ( n - )
	1 invert # PCA0CPM4 anl  \ Disable the interrupt, in case.
    R2 ms-counter direct mov  \ Load ms counter.
    A ms-counter 1 + direct mov
    |drop
	$00 # PCA0MD mov  \ System clock / 12.
	$40 # PCA0CN mov  \ Run the PCA timer.
	%01001001 # PCA0CPM4 mov  \ Comparator,match,interrupt.
	$08 # EIE1 orl  \ Enable PCA interrupts.
	7 .IE setb  \ Enable global interrupts.
	next c;

[then] \ has-f000?

has-f300? [if]

\ F300 seems to clock the PCA twice as fast as the F000.
frequency @ 1000 um*  ( 6000000) 12000000 um/mod nip constant cycles/ms
cycles/ms $ff and constant ms-reload-low
cycles/ms 8 rshift $ff and constant ms-reload-high

\ Counts down to zero by MS ticks.
label ms-timer-interrupt
	PSW push  ACC push
	PCA0CN A mov  2 .ACC clr? if  \ Maybe another PCA channel?
		ACC pop  PSW pop  reti
	then
	$04 invert # PCA0CN anl  \ Clear interrupt bit.
	ms-counter 1 + direct A mov  ms-counter direct A orl 0= if
		1 invert # PCA0CPM2 anl  \ Disable this interrupt.
		ACC pop  PSW pop  reti
	then
	ms-reload-low # A mov  PCA0CPL2 A add  A PCA0CPL2 mov
	ms-reload-high # A mov  PCA0CPH2 A addc  A PCA0CPH2 mov
	ms-counter 1 + direct A mov  0= if
		ms-counter direct dec
	then
	ms-counter 1 + direct dec
	ACC pop  PSW pop  reti c;
ms-timer-interrupt $4b int!

\ Configure system to use channel 2 of PCA.
code set-mstimer  ( n - )
	1 invert # PCA0CPM2 anl  \ Disable the interrupt, in case.
    R2 ms-counter direct mov
    A ms-counter 1 + direct mov
    |drop
	$00 # PCA0MD mov  \ System clock / 12.
	$40 # PCA0CN mov  \ Run the PCA timer.
	%01001001 # PCA0CPM2 mov  \ Comparator,match,interrupt.
	$08 # EIE1 orl  \ Enable PCA interrupts.
	7 .IE setb  \ Enable global interrupts.
	next c;

[then] \ has-f300?

cycles/ms 100 / constant cycles/10us

: scale-to-clock  ( n1 - n2)  cycles/10us * ;

code ms-wait  (  - )
	A R1 mov
	begin
		ms-counter direct A mov
		ms-counter 1 + direct A orl
	0= until
	R1 A mov
	next c;

code ms-timeout?  (  - flag)
    |dup  ms-counter direct A mov  ms-counter 1+ direct A orl
    $ff # A add  ACC A subb  A cpl  A R2 mov  next c;

: ms  ( n - )  set-mstimer ms-wait ;

code set-tone-timer  ( n - )
    PCA0L A add  A PCA0CPL0 mov
    R2 A mov  PCA0H A addc  A PCA0CPH0 mov
    |drop  next c;

code enable-tones  (  - )  %01001100 # PCA0CPM0 mov  next c;

code tone-timeout?  (  - flag)
    |dup  0 .PCA0CN C mov  ACC A subb  A R2 mov
    next c;

code clear-tone-timeout  (  - ) 0 .PCA0CN clr  next c;

code disable-tones  (  - )  0 # PCA0CPM0 mov  next c;

code toggle-P0  ( mask - ) A P0 xrl  |drop  next c;

\ n = 10us intervals.
: pulseout  ( mask n - )
	scale-to-clock
	1000 set-mstimer \ pin2-output
	set-tone-timer enable-tones
	dup toggle-P0  \ dup P0@ xor P0!
	begin  tone-timeout? until
	clear-tone-timeout disable-tones
	toggle-P0  \ P0@ xor P0!
	1 set-mstimer ;

: wait  ( n - )
	scale-to-clock
	1000 set-mstimer \ pin2-output
	set-tone-timer enable-tones
	begin  tone-timeout? until
	clear-tone-timeout disable-tones
	1 set-mstimer ;

\ Watch for : at end for each of the following. ***

in-forth

\ Wait n milliseconds.
keyword: PAUSE  infix quote ms token, ;

keyword: TIMING  infix quote set-mstimer token, ;

\ Allow SYMBOL for pin number.  10 us units.
keyword: PULSEOUT 
	BL word symbol? if
		execute  quote 2** token,
	else
		number? not if  not-a-number throw  then  2** literal-t
	then
	BL word number? not if  not-a-number throw  then
	frequency @ um* 1200000 um/mod nip literal-t
	quote pulseout token, ;

\ 10 us units.
keyword: WAIT  infix quote wait token, ;

keyword: RUN 
	BL word label? not if  not-a-label throw  then
	execute state off in-meta
	s" code go  ljmp " evaluate in-forth  state on ;

in-compiler

: jump  ( a - ) r>drop execute ;

: 2dup  over over ;

: umax  ( n1 n2 - n3)  2dup u< not if  swap  then  drop ;

: basic-branch  ( index limit - )
	umax dup 2* + r> flip + jump ;

in-forth

variable counter

\ e.g. BRANCH b0 zero one two three
\ where zero, one, two, and three are subroutine labels.
keyword: BRANCH 
	compile-only  0 counter !
	BL word variable? not if  not-a-variable throw  then
	right-side execute
	quote LIT token,  romHERE  0 ,-t  \ Limit.
	quote basic-branch token,
	begin
		\ Compile long jumps to following labels.
		BL word dup c@ 0= if
			drop counter @ swap !-t exit
		then
		label? not if  not-a-label throw  then
		execute [ also asm ] ljmp [ previous ]
		\ Count the labels and insert number before basic-branch above,
		\ to bounds check the variable.
		1 counter +!
	again ;

in-compiler

: basic-lookup  ( index address limit original - value|original)
	>r swap >r  ( index limit)
	over >r u< not if
		r> r> 2drop r> exit  \ Leave n1 unchanged.
	then
	r> 2* r> + @ r> drop ;

in-forth

keyword: LOOKUP 
	compile-only  0 counter !
	BL word variable? not if  not-a-variable throw  then
	right-side execute
	romHERE 1 + [ also asm ] 0 ljmp [ previous ]  \ Jump over list.
	begin
		BL word variable? if
			swap dup romHERE swap !-t 2 + literal-t
			counter @ literal-t
			dup right-side execute
			quote basic-lookup token,
			left-side no= execute use=
			exit
		then
		number? not if  not-a-number throw  then  ,-t
		1 counter +!
	again ;

in-compiler

: basic-lookdown  ( target address limit original - index|original)
	>r >r 0 >r
	begin
		2dup @ = if  2drop r> r> r> 2drop exit  then  \ Found it.
		r> 1 + r> 2dup = if  2drop 2drop r> exit  then  \ Original value.
		>r >r 2 +  \ Next address.
	again ;
	
in-forth

keyword: LOOKDOWN 
	compile-only  0 counter !
	BL word variable? not if  not-a-variable throw  then
	right-side execute
	romHERE 1 + [ also asm ] 0 ljmp [ previous ]  \ Jump over list.
	begin
		BL word variable? if
			swap dup romHERE swap !-t 2 + literal-t
			counter @ literal-t
			dup right-side execute
			quote basic-lookdown token,
			left-side no= execute use=
			exit
		then
		number? not if  not-a-number throw  then  ,-t
		1 counter +!
	again ;
	

\ ----- Interactive debugging. ----- /

\ To call a word previously defined in Forth, including passing
\ parameters on the data stack.
keyword: FORTH 
	begin
		BL word dup c@ while
		target? if
			execute token,
		else
			number? not if  not-forth throw  then  literal-t
		then
	repeat  drop ;

: colon?  ( a - flag)
	count 1 = swap c@ [char] : = and ;

\ Variables only.
keyword: PRINT 
	BL word variable? not if  not-a-variable throw  then
	begin
		right-side execute quote . token,
		>in @ BL word dup c@ 0= if  2drop exit  then
		dup count s" '" compare 0= >r
		dup count s" REM" compare 0= >r
		dup count s" rem" compare 0= r> or r> or if 
			postpone \  2drop exit
		then
		colon? if  drop exit  then
		>in ! BL word variable? not if  not-a-variable throw  then
	again ;

keyword: ?  [ also basic-keywords ] PRINT [ previous ] ;

keyword: .S 
	state @ if  quote .S token, quote cr token, exit  then ;

keyword: add-timer 
	infix quote add-timer token, ;

