\ metacomp.fs
0 [if]   metacomp.fs   The amr8051 Forth metacompiler.
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

\ ALL20030706 hex nums also to iram.log

[then]

only forth also definitions

\ .( loading metacomp.fs ) cr

\ Gforth has a 'feature' which allows an expression like  'A  to return
\ the ascii code for A, 65.  Unfortunately the same feature also causes
\ 'anyword to return a number.  The characters $, %, &, and ' cause the
\ base to be temporarily changed to hex, binary, decimal, and 256 for
\ number conversion.  There is a table called bases that has the numbers
\ 16, 2, 10, 256 in order to implement this 'feature'.  The 256 is a
\ trick that allows 'A to return 65.  Having spent many hours debugging
\ broken code where a misspelled word like 'delax for 'delay returns
\ a number instead of aborting, I disable the ' part below by patching
\ the bases table.
0 bases 3 cells + !

\ Words we're used to from the DOS days and FPC.

: 2+   2 + ;

: flip   ( n1 - n2)  \ swap two lower bytes
        $FFFF and   dup $FF and 256 *
        swap $FF00 and 256 /   or ;

: not   ( n - flag)   0= ;

: exec:   ( n - )   cells r> + @ execute ;

\ Thanks to Wil Baden and Neil Bawd's ugly website.
: \s   (  - )   begin	-1 parse 2drop refill 0= until ;

: escape-quotes  ( addr len - addr' len')
	pad c! pad count 0 do
		over c@ dup [char] " = if
			over [char] \ swap c! 1 under+
			pad c@ 1 + pad c!
		then
		over c! 1 + 1 under+
	loop	2drop pad count ;

\ This can be used to print a vocabulary to a file.
: make-vocabulary.log  ( addr len - )
    w/o create-file throw >r
    context @ wordlist-id 
    BEGIN
        @ dup  WHILE 
        dup name>string r@ write-file throw
        BL r@ emit-file throw
    REPEAT 
    r> close-file throw drop ;

\ All the vocabularies and ways to get at them.

root definitions

vocabulary meta		\ Defining words in transition from host to target.
vocabulary target	\ Words whose code resides on the target.
vocabulary asm		\ The target assembler.
vocabulary immed	\ Control structure words, similar to immediate words.

\ These need to be available even if forth is not in the search order.
: also   also ;
: definitions   definitions ;

only forth also definitions

: to-meta      (  - )   only forth also meta also definitions ;
: in-meta      (  - )   only forth also definitions meta also ;
: in-compiler  (  - )   in-meta ;

: in-forth     (  - )   only forth also definitions ;
: in-host      (  - )   in-forth ;
: in-assembler (  - )   only forth also meta also asm also definitions ;
: in-target (  - )   only forth also meta also target also definitions ;
: in-immed  (  - )   only forth also meta also immed also definitions ;

\ Add a word to the meta vocabulary.
: m:   (  - )   to-meta : ;
: ;m   (  - )   postpone ; in-meta ; immediate

\ Add a word to the forth vocabulary.
: f:   (  - )   in-meta : ;
' ;m alias ;f immediate

\ Add a word to the assembler vocabulary, an assembler macro.
: a:   (  - )   in-assembler : ;
' ;m alias ;a immediate

\ Add a word to the immed vocabulary, probably a compiler directive.
: i:   (  - )   in-immed use-tags : no-tags ;
' ;m alias ;i immediate

variable romDP

variable cpuDP   8 cpuDP !
has interrupts-kernel [if]
    $20 cpuDP !  \ Skip input buffer.
[then]

: write-crlf  ( fid - )
    >r 13 r@ emit-file throw 10 r> emit-file throw ;

: (.)  ( n - ) 0 <# #s #> ;

\ Assumes the fileid is for an open file.
: makelogger  ( variable fileid - )
    create  , ,
    does>
        dup @ >r  \ fid
        last @ name>string r@ write-file throw
        9 r@ emit-file throw
        \ cell+ @ @ 0 <# #s #> r@ write-file throw
        cell+ @ @ dup (.) r@ write-file throw
        ( -- aIRAM )    \ ALL20030706+
        base @ swap hex
        9 r@ emit-file throw
        0 <# # # [char] $ hold #> r@ write-file throw
        ( ai -- ) base !  \ ALL20030706-
        r@ write-crlf
        r> drop ;

create dash ," -"
create aspace ,"  "
create acolon ," :"
: (##)  ( n - ) 0 <# # # #> ;
: get-system-date  ( fid - )
    >r time&date (.) pad place dash count pad +place
    (.) pad +place dash count pad +place (.) pad +place
    aspace count pad +place (##) pad +place acolon count pad +place
    (##) pad +place acolon count pad +place (##) pad +place
    pad count r@ write-file throw  r> write-crlf ;

: createlog  ( addr len - )
    create
        w/o create-file throw dup >r ,
        r> get-system-date
    does>  (  - fid)
        @ ;

s" iram.log" createlog iram.log
cpuDP iram.log makelogger logIRAM

\ 8051 has three memory spaces, so do some other chips.
: romHERE   (  - a)   romDP @ ;
: cpuHERE   (  - a)   cpuDP @ ;

3 constant bytes/target-record  \ Helps with single step source.
64 1024 * bytes/target-record * constant target-markers-size
create target-markers   target-markers-size allot
target-markers target-markers-size erase

: save-target-markers  (  - )
	s" target-markers.bin" w/o create-file throw >r
	target-markers target-markers-size r@ write-file throw
	r> close-file throw ;

: column#  ( a1 - a2) bytes/target-record * target-markers + ;
: line#  ( a1 - a2) column# 1+ ;

: erase-target-byte-record  ( a - ) column# bytes/target-record erase ;
: erase-target-word-record  ( a - ) column# bytes/target-record 2* erase ;f

\ Building the symbol tables.
variable symbols-file  0 symbols-file !
variable hidden-symbols-file  0 hidden-symbols-file !
variable exits-file    0 exits-file !
variable labels-file   0 labels-file !
variable sfrs-file   0 sfrs-file !
variable commas-file   0 commas-file !
variable branches-file   0 branches-file !
variable sources-file   0 sources-file !

: create-log  ( addr len a - n)
	dup @ if  @ nip nip exit  then
	>r w/o create-file throw dup r> ! ;

: sources-id  (  - n) s" sources.log" sources-file create-log ;

: branches-id  (  - n) s" branches.log" branches-file create-log ;

: commas-id  (  - n) s" commas.log" commas-file create-log ;

: sfrs-id  (  - n) s" sfrs.log" sfrs-file create-log ;

: labels-id  (  - n) s" labels.log" labels-file create-log ;

: symbols-id  (  - n)
	s" symbols.log" symbols-file create-log ;

: hidden-symbols-id  (  - n)
	s" hidden-symbols.log" hidden-symbols-file create-log ;

: exits-id  (  - n)
	s" exits.log" exits-file create-log ;

: make-end-label  (  - )
	labels-id >r
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-label  (  - )
	labels-id >r
	s" : .label  ( a - flag) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" 0 swap " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-label

: make-end-commas  (  - )
	commas-id >r
	( tab) 9 r@ emit-file throw
	s" 0 swap" r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-commas  (  - )
	commas-id >r
	s" : comma?  ( a - flag) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-commas

: make-end-exits  (  - )
	exits-id >r
	( tab) 9 r@ emit-file throw
	s" 0 swap" r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-exits  (  - )
	exits-id >r
	s" : exit-point?  ( a - flag) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-exits

: make-end-branches  (  - )
	branches-id >r
	( tab) 9 r@ emit-file throw
	s" 0 swap" r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-branches  (  - )
	branches-id >r
	s" : branch-point?  ( a - flag) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-branches

: make-end-sfrs  (  - )
	sfrs-id >r
	( tab) 9 r@ emit-file throw
	s" dup .XX" r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-sfrs  (  - )
	sfrs-id >r
	s" : .sfr  ( a - flag) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-sfrs

: make-end-sources  (  - )
	sources-id >r
	( tab) 9 r@ emit-file throw
	s" endcase ; " r@ write-file throw
	r> drop ;

: make-start-sources  (  - )
	sources-id >r
	s" : source-filename$  ( a1 - a2 len) " r@ write-file throw
	newline r@ write-file throw
	( tab) 9 r@ emit-file throw
	s" case " r@ write-file throw
	newline r@ write-file throw
	r> drop ;
make-start-sources

: close-log-files  (  - )
	symbols-file @ close-file throw
	hidden-symbols-file @ close-file throw
	make-end-label labels-file @ close-file throw
	make-end-sfrs sfrs-file @ close-file throw
	make-end-commas commas-file @ close-file throw
	make-end-exits exits-file @ close-file throw
	make-end-branches branches-file @ close-file throw
	make-end-sources sources-file @ close-file throw
	save-target-markers
	;

: flag-entry  ( addr id - )
	>r
	( tab) 9 r@ emit-file throw
	0 <# #s #> r@ write-file throw
	BL r@ emit-file throw
	s" of  -1  endof" r@ write-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: exits-entry  ( addr - ) exits-id flag-entry ;
: branches-entry  ( addr - ) branches-id flag-entry ;
: (commas-entry)  ( addr - ) commas-id flag-entry ;

: sources-entry  (  - )
	sources-id >r
	( tab) 9 r@ emit-file throw
	romHERE base @ >r decimal 0 <# #s #> r> base !
	r@ write-file throw
	BL r@ emit-file throw
	s" of  s" r@ write-file throw
	[char] " r@ emit-file throw
	BL r@ emit-file throw
	sourcefilename r@ write-file throw
	[char] " r@ emit-file throw
	s"   endof" r@ write-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: C_commas-entry  (  - ) romHERE (commas-entry) ;

: commas-entry  (  - )
	romHERE (commas-entry) romHERE 1 + (commas-entry) ;

: sfr-entry  ( n - n)
	sfrs-id >r
	( tab) 9 r@ emit-file throw
	dup 0 
	base @ >r decimal <# #s #> r> base !
	r@ write-file throw
	BL r@ emit-file throw
	s" of  ." r@ write-file throw
	[char] " r@ emit-file throw
	BL r@ emit-file throw
	>in @ BL word count r@ write-file throw >in !
	[char] " r@ emit-file throw
	s"   endof" r@ write-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: labels-entry  (  - )
	labels-id >r
	( tab) 9 r@ emit-file throw
	romHERE 0 <# #s #> r@ write-file throw
	BL r@ emit-file throw
	s" of  .\" r@ write-file throw
	[char] " r@ emit-file throw
	BL r@ emit-file throw
	last @ name>string escape-quotes r@ write-file throw
	BL r@ emit-file throw
	[char] " r@ emit-file throw
	s"  -1 or  endof" r@ write-file throw
	newline r@ write-file throw
	r> flush-file throw ;
	
: symbols-entry  (  - )
	labels-entry
	symbols-id >r
	[char] : r@ emit-file throw
	BL r@ emit-file throw
	last @ name>string r@ write-file throw
	( tab) 9 r@ emit-file throw
	base @ decimal
	romHERE 0 <# #s #> r@ write-file throw
	base !
	BL r@ emit-file throw
	[char] ; r@ emit-file throw
	newline r@ write-file throw
	r> flush-file throw ;

: hidden-symbols-entry  (  - )
	labels-entry
	hidden-symbols-id >r
	[char] : r@ emit-file throw
	BL r@ emit-file throw
	last @ name>string r@ write-file throw
	( BL) 9 r@ emit-file throw
	base @ decimal
	romHERE 0 <# #s #> r@ write-file throw
	base !
	BL r@ emit-file throw
	[char] ; r@ emit-file throw
	newline r@ write-file throw
	r> flush-file throw ;

0 value last-sourceline#
0 value last->in

\ --- Create a target image and ways to read and write it.

$10000 constant target-size

create target-image   target-size allot

\ Fill with all bits set, like an erased rom.
target-image target-size $ff fill

\ Gforth is 32 bits, target forth is 16 bits.
: w!   ( n a - )   2dup c!   >r 256 / r> 1+ c! ;
nowarn
: w@   ( a - n)   dup c@ swap 1+ c@ 256 * or ;
warn
: sw@   ( a - n)   w@ dup $8000 and if   $FFFF0000 or   then ;

target-image target-size erase

: org   ( n - )   romDP ! ;
: romALLOT   ( n - )   romDP +! ;

nowarn
128 value rp0   \ sort of a forward reference, resolved in begin.seq.
128 value sp0
warn

\ The 8051 family keeps bit variables right in the middle of the
\ byte variables.  Special care needs to be taken to allocate
\ byte variables around them.  Use   0 bit-variables   to declare
\ that you won't be using them and allow byte variables free reign.

create #bit-variables 1 c,

m: bit-variables   ( b  - )   #bit-variables c! ;m

f: overlapped?   ( a1 n1 a2 n2 - flag)
        over + >r >r   over +   r> > not
        swap r> < not   or not ;f

f: bit-collision?   ( n - flag)
        #bit-variables c@ 0= if   drop false exit   then
        cpudp @ swap $20 #bit-variables c@ overlapped? ;f

f: skip-bit-variables   (  - )
        $20 #bit-variables c@ + cpudp ! ;f

m: cpuhere    (  - a)   cpudp @ ;m

f: ?skip-bits   ( n - )
	bit-collision? if   skip-bit-variables   then ;f

m: cpuALLOT   ( n - )
	dup bit-collision?
	ABORT" Attempted to ALLOT into bit variables."
	cpuDP @ + 
	dup 128 < not ABORT" Variables out of range"
	DUP RP0 >
	ABORT" Variables and Return Stack have collided."
	cpuDP ! ;M

m: allot  ( n - )  cpuALLOT ;m

f: there  ( a1 - a2)  target-image + ;f

f: c@-t  ( a - c)  there c@ ;f
f: c!-t  ( c a - )  there c! ;f

f: @-t  ( a - n)  dup 1+ c@-t   swap c@-t   8 lshift or ;f
f: !-t  ( n a - )  >r dup 8 rshift r@ c!-t r> 1+ c!-t ;f
	
f: c,-t  ( c - )  romHERE c!-t   1 romALLOT ;f
f: ,-t  ( n - )  romHERE !-t   2 romALLOT ;f

\ Sometimes we need to store things in the host.
f: hostc,  ( c - )  c, ;f
f: host,   ( n - )  ,  ;f

\ By default now we store things in the target.
m: c,  ( c - ) C_commas-entry c,-t ;m
m: ,  ( n - ) commas-entry ,-t ;m

\ Generic assembly language words, the same for any processor family.

m: label   (  - )   romHERE constant hidden-symbols-entry in-assembler ;m
a: end-code   (  - )   in-meta ;a
a: c;   (  - )   in-meta ;a

\ Store a string in the target.
f: s,-t   ( a n - )   0 ?do   count c,-t   loop   drop ;m

nowarn
\ Optimization, suppress edge of word or control structure.
variable 'edge
: hide  (  - )  -1 'edge ! ; hide
: hint  (  - )  romHERE 'edge ! ;
: edge  (  - a)  'edge @ ;
warn

in-forth

\ A list of the names of target words, to allow the debugger to
\ get a name string from a target address, not otherwise available.
CREATE TARGET-NAMES   0 ,

: CFA-T   ( a - a' flag)
        TARGET-NAMES
        BEGIN   @ ?DUP WHILE
                2DUP CELL+ @ = IF
                        NIP 2 CELLS + @ TRUE EXIT
                THEN
        REPEAT  FALSE ;

nowarn
\ name>string is a Gforth specific word.	
: .id   ( a - )   name>string type space ;
warn

: .ID-T   ( a - )
        CFA-T IF
                .ID
        ELSE    DROP ." ?"
        THEN    ;

\ ***** FORGET is not yet implemented, either in Gforth or in amrForth. 
\ In order to implement FORGET you will have to prune this linked list
\ so as not to contain any names that have been forgotten.   *****
: REMEMBER-NAME   ( a-host a-target - )
        HERE TARGET-NAMES
        DUP @ , !   , , cpuHERE ,
        ;

: .target-names   (  - )
        base @ hex
        target-names
        begin   @ ?dup while
                cr
                dup 3 cells + @ >r
                dup cell+ @ u.
                dup 2 cells + @ dup u. r> u. .id
        repeat  base ! ;

: save-labels  (  - )
	target-names
	begin
		@ ?dup while
		cr
		dup cell+ @ u.
		dup 2 cells + @ .id
	repeat ;

in-meta

\ Create a header for a target word.
f: tcreate   (  - )
	use-tags
	in-target romHERE dup constant symbols-entry hide
	last @ swap remember-name
	in-meta no-tags ;f

\ Create a header, don't add it to the symbol table file.
f: -tcreate  (  - )
	use-tags
	in-target romHERE dup constant hidden-symbols-entry hide
	last @ swap remember-name
	in-meta no-tags ;f

f: precreate   (  - )   >in @ tcreate >in ! ;f

\ In order to be able to use target constants at compile time,
\ say for address calculations, we also define those constants
\ in the host.
f: mcreate   ( n - )  in-meta constant ;f

m: code   (  - )  tcreate in-assembler ;m
m: -code  (  - )  -tcreate in-assembler ;m

\ Sometimes we want the host version.
f: host-here   (  - a)   here ;f

\ Mostly we want the target version.
m: here   (  - a)   romHERE ;m

in-forth

: target-variable  ( n - )
	create  ,  does>  (  - a)  @ ;

in-meta

f: vcreate      ( n  - )   in-meta target-variable ;m

\ This is a search order trick to help search a single vocabulary,
\ without also looking in forth or root.
f: exclusively   ( a - cfa -1 | a 0)
    dup count context @ search-wordlist
    dup if   rot drop   then ;m

\ Is counted string a target immediate word?
f: IMMEDIATE?    ( a - cfa ? | a 0)
        DUP C@
        IF      ONLY PREVIOUS IMMED FIND IN-TARGET
        ELSE    0
        THEN ;m

\ Is counted string a normal target word?
f: TARGET?   ( a - cfa ? | a 0)
        ALSO TARGET EXCLUSIVELY PREVIOUS ;m

\ Is counted string a meta word?
f: META?  ( a - cfa ? | a 0)
        DUP C@
        IF      IN-META FIND IN-TARGET
        ELSE    0
        THEN ;m

m: ?missing   ( flag - )   abort" is undefined" ;

\ To look up the address of a host word.
f: host'   (  - a)   ' ;m

\ Get the target address of a target word.
m: '   (  - a)   bl word target? 0= ?missing execute ;m

\ Compile a target address literal into the host, not the target image.
m: [t']   (  - a)   ' [compile] literal ;m immediate

IN-META

\ Building strings on the target.
M: STRING   ( c - )   WORD COUNT DUP C,-T S,-T ;M
f: HOST,"   (  - )   ," ;M
M: ,"       (  - )   [CHAR] " STRING ;M
M: ,'       (  - )   [CHAR] ' STRING ;M

f: double?   (  - flag)   dpl @ 0< not ;

