\ dis8051.fs

0 [if]   dis8051.fs   Disassembler for amrForth for 8051.
Copyright (C) 2001-2004 by AM Research, Inc.

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
include colorscheme.fs

only forth also root definitions
vocabulary symbols

only forth also symbols definitions also forth
include ./symbols.log  \ Read the symbol tables.
include ./hidden-symbols.log
only forth also definitions

include commas.log

: nowarn  warnings off ;
: warn    warnings on ;
nowarn

create object-code  64 1024 * allot

: c@-t  ( a - c) object-code + c@ ;
: @-t  ( a - n) object-code + count 256 * swap c@ + ;

0 value fid  \ File ID.
0 value romHERE

: read-object-code  (  - )
	s" rom.bin" r/o open-file throw to fid
	object-code [ 64 1024 * ] literal fid read-file throw
	to romHERE  fid close-file throw ;

include labels.log  \ Defines .label
: .self  ( a flag - )
	>r label-colors .label r> and if  ." ; "  then
	( interpreter-colors) opcode-colors ;

: +byte  ( a1 - a2 c) dup 1 + swap c@-t ;

: +word  ( a - a+2 n) +byte 256 * >r +byte r> + ;

: .literal  ( n - ) base @ >r hex 16 spaces u. r> base ! ;

: .string  ( a1 - a2) 16 spaces +byte 0 do  +byte emit  loop ;

\ ( *)HERE 1+ ," R0R1R0R1R2R3R4R5R6R7"
create regnames  ," R0R1R0R1R2R3R4R5R6R7"
: .Rn   ( op - )
	$0f and 6 - dup 2 < if   ." @"   then
	2* regnames + 1+ 2 type ;

: .XX   ( n - )
	base @ >r   hex 0 <# # # #> type   r> base ! ;

include ./sfrs.log  \ For names of the SFRs.

: .XXXX   ( n - )
	base @ >r   hex 0 <# # # # # #> type   r> base ! ;

: .address  ( a - )
	dup label-colors .label if  cr  then
	( default-colors) disasm-colors 3 spaces .XXXX ;

: .byte   ( a1 - a2) +byte .xx ;

: .rel-adr   ( a1 - a2)
	+byte dup .xx ." ("   dup $80 and if   $ff00 or   then
	over + .xxxx   ." )" ;

: .word   ( a - a+2 n flag)
	>r dup @-t r> .self .byte .byte ;

: (ready) ( a op - a' op) dup $0f and ;

: ready   ( a op - a' op) (ready) 6 min ;

: absolute   ( a op - a' n)
	$00e0 and 2* 2* 2* >r   +byte r> + ;

: .opcode  ( a op table - a')
	opcode-colors >r cells r> + @ execute disasm-colors ;

: (bytes)  ( a n - )
	>r 1 - r>
	dup 1 = if  drop .byte drop 7 spaces exit  then
	dup 2 = if  drop .byte space .byte drop 4 spaces exit  then
	drop .byte space .byte space .byte space drop ;
: bytes  ( a op n - a op)
	( default-colors) disasm-colors
	>r over r> (bytes) opcode-colors ;

: handle-literal  ( a1 a2 - a3)
	dup s" symbols lit forth" evaluate = if
		drop dup cr .address +word .literal exit
	then
	dup s" symbols ?branch forth" evaluate = if
		drop dup cr .address +word .literal exit
	then
	dup s" symbols (next) forth" evaluate = if
		drop dup cr .address +word .literal exit
	then
	dup s" symbols (string) forth" evaluate = if
		drop dup cr .address .string exit
	then
	drop ;

\ Disassember 0X
: .NOP  ( a op - a) 1 bytes drop ."   nop" ;
: .AJMP  ( a op - a+1)
	2 bytes absolute ."  ajmp " dup >r over $f800 and or
	dup -1 .self r> .XXXX ." (" .XXXX ." )" ;
: .LJMP  ( a op - a+2) 3 bytes drop ."  ljmp " -1 .word ;
: .RR  ( op - ) 1 bytes drop ."    rr A" ;
: .INC(A)  ( op - )  1 bytes drop ."   inc A" ;
: .INC(direct)  ( a op - a+1)
	2 bytes drop ."   inc " ( .byte) +byte .sfr ;
: .INC(Rn)  ( op - ) 1 bytes ."   inc " .Rn ;
create table0x
	' .NOP , ' .AJMP , ' .LJMP , ' .RR ,
	' .INC(A) , ' .INC(direct) , ' .INC(Rn) ,
: .0x  ( a op - a') ready table0x .opcode ;


\ Disassember 1X
: .JBC  ( a op - a+3)
	3 bytes drop ."   jbc " .byte ." ," .rel-adr ;
: .ACALL  ( a op - a+1)
	2 bytes absolute ." acall " dup >r over $f800 and or dup >r
	dup 0 .self r> r> .XXXX ." (" .XXXX ." )"
	handle-literal ;
: .LCALL  ( a op - a+3)
	3 bytes drop ." lcall " dup >R 0 .word R> @-t handle-literal ;
: .RRC  ( op - ) 1 bytes drop ."   rrc A" ;
: .DEC(A)  ( op - ) 1 bytes drop ."   dec A" ;
: .DEC(direct)  ( a op - a+1)
	2 bytes drop ."   dec " ( .byte) +byte .sfr ;
: .DEC(Rn)  ( op - ) 1 bytes ."   dec " .Rn ;
create table1x
        ' .JBC , ' .ACALL , ' .LCALL , ' .RRC ,
	' .DEC(A) , ' .DEC(direct) , ' .DEC(Rn) ,
: .1x  ( a op - a') ready table1x .opcode ;


\ Disassember 2X
: .JB  ( a op - a+3) 3 bytes drop ."    jb " .byte ." ," .rel-adr ;
: .RET  ( op)
	1 bytes drop ."   ret " label-colors [char] ; emit ;
: .RL  ( op) 1 bytes drop ."    rl A" ;
: .ADD(#) ( a op - a+2) 2 bytes drop ."   add A,#" .byte ;
: .ADD(direct)  ( a op - a+1)
	2 bytes drop ."   add A," ( .byte) +byte .sfr ;
: .ADD(Rn)  ( op) 1 bytes ."   add A," .Rn ;
create table2x
        ' .JB , ' .AJMP , ' .RET , ' .RL ,
	' .ADD(#) , ' .ADD(direct) , ' .ADD(Rn) ,
: .2x  ( a op - a') ready table2x .opcode ;


\ Disassember 3X
: .JNB  ( a op - a+3)
	3 bytes drop ."   jnb " .byte ." ," .rel-adr ;
: .RETI  ( op - ) 1 bytes drop ."  reti" ;
: .RLC  ( op - ) 1 bytes drop ."   rlc A" ;
: .ADDC(#) ( a op - a+1) 2 bytes drop ."  addc A,#" .byte ;
: .ADDC(direct)  ( a op - a+1)
	2 bytes drop ."  addc A," ( .byte) +byte .sfr ;
: .ADDC(Rn)  ( op - ) 1 bytes ."  addc A," .Rn ;
create table3x
        ' .JNB , ' .ACALL , ' .RETI , ' .RLC ,
	' .ADDC(#) , ' .ADDC(direct) , ' .ADDC(Rn) ,
: .3x  ( a op - a') ready table3x .opcode ;


\ Disassember 4X
: .JC  ( a op - a+3) 3 bytes drop ."    jc " .rel-adr ;
: .ORL(direct,A)  ( a op - a+1)
	2 bytes drop ."   orl " ( .byte) +byte .sfr ." ,A" ;
: .ORL(direct,#)  ( a op - a+2)
	3 bytes drop ."   orl " ( .byte) +byte .sfr ." ,#" .byte ;
: .ORL(A,#)  ( a op - a+2) 2 bytes drop ."   orl A,#" .byte ;
: .ORL(A,direct)  ( a op - a+2)
	2 bytes drop ."   orl A," ( .byte) +byte .sfr ;
: .ORL(A,Rn)  ( op - ) 1 bytes ."   orl A," .Rn ;
create table4x
        ' .JC , ' .AJMP , ' .ORL(direct,A) , ' .ORL(direct,#) ,
	' .ORL(A,#) , ' .ORL(A,direct) , ' .ORL(A,Rn) ,
: .4x  ( a op - a') ready table4x .opcode ;


\ Disassember 5X
: .JNC  ( a op - a+3) 3 bytes drop ."   jnc " .rel-adr ;
: .ANL(direct,A)  ( a op - a+1)
	2 bytes drop ."   anl " ( .byte) +byte .sfr ." ,A" ;
: .ANL(direct,#)  ( a op - a+2)
	3 bytes drop ."   anl " ( .byte) +byte .sfr ." ,#" .byte ;
: .ANL(A,#)  ( a op - a+2) 2 bytes drop ."   anl A,#" .byte ;
: .ANL(A,direct)  ( a op - a+2)
	2 bytes drop ."   anl A," ( .byte) +byte .sfr ;
: .ANL(A,Rn)  ( op - ) 1 bytes ."   anl A," .Rn ;
create table5x
        ' .JNC , ' .ACALL , ' .ANL(direct,A) , ' .ANL(direct,#) ,
	' .ANL(A,#) , ' .ANL(A,direct) , ' .ANL(A,Rn) ,
: .5x  ( a op - a') ready table5x .opcode ;


\ Disassember 6X
: .JZ  ( a op - a+3) 3 bytes drop ."    jz " .rel-adr ;
: .XRL(direct,A)  ( a op - a+1)
	2 bytes drop ."   xrl " ( .byte) +byte .sfr ." ,A" ;
: .XRL(direct,#)  ( a op - a+2)
	3 bytes drop ."   xrl " ( .byte) +byte .sfr ." ,#" .byte ;
: .XRL(A,#)  ( a op - a+2) 2 bytes drop ."   xrl A,#" .byte ;
: .XRL(A,direct)  ( a op - a+2)
	2 bytes drop ."   xrl A," ( .byte) +byte .sfr ;
: .XRL(A,Rn)  ( op - ) 1 bytes ."   xrl A," .rN ;
create table6x
        ' .JZ , ' .AJMP , ' .XRL(direct,A) , ' .XRL(direct,#) ,
	' .XRL(A,#) , ' .XRL(A,direct) , ' .XRL(A,Rn) ,
: .6x  ( a op - a') ready table6x .opcode ;


\ Disassember 7X
: .JNZ ( a op - a+3) 3 bytes drop ."   jnz " .rel-adr ;
: .ORL(C,bit)  ( a op - a+2) 2 bytes drop ."   orl C," .byte ;
: .JMP(@A+DPTR)  ( op - ) 1 bytes drop ."   jmp @A+DPTR" ;
: .MOV(A,#)  ( a op - a+2) 2 bytes drop ."   mov A,#" .byte ;
: .MOV(direct,#)  ( a op - a+2)
	3 bytes drop ."   mov " ( .byte) +byte .sfr ." ,#" .byte ;
: .MOV(Rn,#)  ( a op - a+2) 2 bytes ."   mov " .Rn ." ,#" .byte ;
create table7x
        ' .JNZ , ' .ACALL , ' .ORL(C,bit) , ' .JMP(@A+DPTR) ,
	' .MOV(A,#) , ' .MOV(direct,#) , ' .MOV(Rn,#) ,
: .7x  ( a op - a') ready table7x .opcode ;


\ Disassember 8X
: .SJMP  ( a op - a+2) 2 bytes drop ."  sjmp " .rel-adr ;
: .ANL(C,bit)  ( a op - a+2) 2 bytes drop ."   anl C," .byte ;
: .MOVC(A,@A+PC)  ( op - ) 1 bytes drop ."  movc A,@A+PC" ;
: .DIV  ( op - ) 1 bytes drop ."   div AB" ;
: .MOV(direct,direct)  ( a op - a+3)
	3 bytes drop ."   mov " +byte >r +byte ( .xx) .sfr ." ,"
	r> ( .xx) .sfr ;
: .MOV(direct,Rn)  ( a op - a)
	2 bytes ."   mov " >r ( .byte) +byte .sfr ." ," r> .Rn ;
create table8x
        ' .SJMP , ' .AJMP , ' .ANL(C,bit) , ' .MOVC(A,@A+PC) ,
	' .DIV , ' .MOV(direct,direct) , ' .MOV(direct,Rn) ,
: .8x  ( a op - a') ready table8x .opcode ;


\ Disassember 9X
: .MOV(DPTR,#)  ( a op - a+2) 2 bytes drop ."   mov dptr,#" 0 .word ;
: .MOV(bit,C)  ( a op - a+2) 2 bytes drop ."   mov " .byte ." ,C" ;
: .MOVC(A,@A+DPTR)  ( op - ) 1 bytes drop ."  movc A,@A+DPTR" ;
: .SUBB(A,#)  ( a op - a+2) 2 bytes drop ."  subb A,#" .byte ;
: .SUBB(A,direct)  ( a op - a+2)
	2 bytes drop ."  subb A," ( .byte) +byte .sfr ;
: .SUBB(A,Rn)  ( op - ) 1 bytes ."  subb A," .Rn ;
create table9x
        ' .MOV(DPTR,#) , ' .ACALL , ' .MOV(bit,C) , ' .MOVC(A,@A+DPTR) ,
        ' .SUBB(A,#) , ' .SUBB(A,direct) , ' .SUBB(A,Rn) ,
: .9x  ( a op - a') ready table9x .opcode ;


\ Disassember AX
: .ORL(C,/bit)  ( a op - a+2) 2 bytes drop ."   orl C,/" .byte ;
: .MOV(C,bit)  ( a op - a+2) 2 bytes drop ."   mov C," .byte ;
: .INC(DPTR)  ( op - ) 1 bytes drop ."   inc DPTR" ;
: .MUL  ( op - ) 1 bytes drop ."   mul AB" ;
: .reserved  ( op - ) 1 bytes drop ." reserved" ;
: .MOV(Rn,direct)  ( a op - a)
	2 bytes ."   mov " .Rn ." ," ( .byte) +byte .sfr ;
create tableAx
        ' .ORL(C,/bit) , ' .AJMP , ' .MOV(C,bit) , ' .INC(DPTR) ,
        ' .MUL , ' .reserved , ' .MOV(Rn,direct) ,
: .Ax  ( a op - a') ready tableAx .opcode ;


\ Disassember BX
: .ANL(C,/bit)  ( a op - a+2) 2 bytes drop ."   anl C,/" .byte ;
: .CPL(bit)  ( a op - a+2) 2 bytes drop ."   cpl " .byte ;
: .CPL(C)  ( op - ) 2 bytes drop ."   cpl C" ;
: .CJNE(A,#,rel)  ( a op - a+3) 3 bytes drop ."  cjne A,#" .byte ." ," .rel-adr ;
: .CJNE(A,direct,rel)  ( a op - a+3)
	3 bytes drop ."  cjne A," ( .byte) +byte .sfr ." ," .rel-adr ;
: .CJNE(Rn)  ( a op - a+2) 3 bytes ."  cjne " .Rn   ." ,#" .byte ." ," .rel-adr ;
create tableBx
        ' .ANL(C,/bit) , ' .ACALL , ' .CPL(bit) , ' .CPL(C) ,
        ' .CJNE(A,#,rel) , ' .CJNE(A,direct,rel) , ' .CJNE(Rn) ,
: .Bx  ( a op - a') ready tableBx .opcode ;


\ Disassember CX
: .PUSH  ( a op - a+2) 2 bytes drop ."  push " ( .byte) +byte .sfr ;
: .CLR(bit)  ( a op - a+2) 2 bytes drop ."   clr " .byte ;
: .CLR(C)  ( op - ) 1 bytes drop ."   clr C" ;
: .SWAP  ( op - ) 1 bytes drop ."  swap A" ;
: .XCH(A,direct)  ( a op - a+2)
	2 bytes drop ."   xch A," ( .byte) +byte .sfr ;
: .XCH(Rn)  ( op - ) 1 bytes ."   xch A," .Rn ;
create tableCx
        ' .PUSH , ' .AJMP , ' .CLR(bit) , ' .CLR(C) , ' .SWAP ,
        ' .XCH(A,direct) , ' .XCH(Rn) ,
: .Cx  ( a op - a') ready tableCx .opcode ;


\ Disassember DX
: .POP  ( a op - a+1) 1 bytes drop ."   pop " ( .byte) +byte .sfr ;
: .SETB(bit)  ( a op - a+1) 2 bytes drop ."  setb " .byte ;
: .SETB(C)  ( op - ) 1 bytes drop ."  setb C" ;
: .DA  ( op - ) 1 bytes drop ."    da A" ;
: .DJNZ(direct,rel)  ( a op - a+2)
        3 bytes drop ."  djnz " +byte .sfr ." ," .rel-adr ;
: .XCHD(Rn)  ( op - ) 1 bytes ."  xchd A,@R" 1 and . ;
: .DJNZ(Rn,rel)  ( op - ) 2 bytes ."  djnz " .Rn ." ," .rel-adr ;
create tableDx
        ' .POP , ' .ACALL , ' .SETB(bit) , ' .SETB(C) , ' .DA ,
        ' .DJNZ(direct,rel) , ' .XCHD(Rn) , ' .XCHD(Rn) ,
	' .DJNZ(Rn,rel) ,
: .Dx  ( a op - a') (ready) 8 min tableDx .opcode ;


\ Disassember EX
: .MOVX(A,@DPTR)  ( op - ) 1 bytes drop ."  movx A,@DPTR" ;
: .MOVX(A,Rn)  ( op - ) 1 bytes ."  movx A," 1 and 6 + .rN ;
: .CLR(A)  ( op - ) 1 bytes drop ."   clr A" ;
: .MOV(A,direct)  ( a op - a+1)
	2 bytes drop ."   mov A," ( .byte) +byte .sfr ;
: .MOV(A,Rn)  ( op - ) 1 bytes ."   mov A," .Rn ;
create tableEx
        ' .MOVX(A,@DPTR) , ' .AJMP , ' .MOVX(A,Rn) , ' .MOVX(A,Rn) ,
        ' .CLR(A) , ' .MOV(A,direct) , ' .MOV(A,Rn) ,
: .Ex  ( a op - a') ready tableEx .opcode ;


\ Disassember FX
: .MOVX(@DPTR,A)  ( op - ) 1 bytes drop ."  movx @DPTR,A" ;
: .MOVX(Rn,A)  ( op - ) 1 bytes ."  movx " 1 and 6 + .Rn ." ,A" ;
: .CPL(A)  ( op - ) 1 bytes drop ."   cpl A" ;
: .MOV(direct,A)  ( a op - a+1)
	2 bytes drop ."   mov " ( .byte) +byte .sfr ." ,A" ;
: .MOV(Rn,A)  ( op - ) 1 bytes ."   mov " .Rn ." ,A" ;
create tableFx
        ' .MOVX(@DPTR,A) , ' .ACALL , ' .MOVX(Rn,A) , ' .MOVX(Rn,A) ,
        ' .CPL(A) , ' .MOV(direct,A) , ' .MOV(Rn,A) ,
: .Fx  ( a op - a') ready tableFx .opcode ;


create instruction-types
	' .0x , ' .1x , ' .2x , ' .3x , ' .4x , ' .5x , ' .6x , ' .7x ,
	' .8x , ' .9x , ' .Ax , ' .Bx , ' .Cx , ' .Dx , ' .Ex , ' .Fx ,
: .instruction  ( a - a')
	dup .address space
	dup comma? if  .byte cr exit  then
	+byte dup 2/ 2/ 2/ 2/ cells
	instruction-types + @ execute cr ;

: disasm  ( a - )
	( cr) disasm-colors
	begin  .instruction key [char] q = until  drop
	disasm-colors ;

: see  (  - ) symbols ' execute disasm ;

: words  (  - ) also symbols words previous ;

read-object-code
disasm-colors
\ quit
