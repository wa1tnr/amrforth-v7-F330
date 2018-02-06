0 [if]   decompiler.fs   Disassembler and decompiler for amr8051 Forth.
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

\ Disassembler.
IN-FORTH

: .self  ( a - ) cfa-t if  .id space  else  drop  then ;

defer +byte
: h-+byte  dup 1 + swap c@-t ;
' h-+byte is +byte

: +WORD   ( a - a+2 n)   +BYTE FLIP >R +BYTE R> + ;

in-meta

f: .literal  ( n - ) base @ >r hex cr 12 spaces u. r> base ! ;f

nowarn
f: .string  ( a1 - a2) cr 12 spaces +byte 0 do  +byte emit  loop ;
warn

f: handle-literal  ( a1 a2 - a3)
	dup [t'] clit = if  drop +byte .literal exit  then
	dup c8 = if  drop +byte .literal exit  then
	dup [t'] lit = if  drop +word .literal exit  then
	dup c16 = if  drop +word .literal exit  then
\ ***	dup [t'] branch = if  drop +word .literal exit  then
\ ***	dup [t'] ?branch = if  drop +word .literal exit  then
\ ***	dup [t'] (next) = if  drop +word .literal exit  then
\	dup [t'] (.") = if  drop .string exit   then
	dup [t'] (s") = if  drop .string exit   then
\ other literals too?
	drop ;f

in-forth	

\ ( *)HERE 1+ ," R0R1R0R1R2R3R4R5R6R7"
create "regs  ," R0R1R0R1R2R3R4R5R6R7"
: .Rn   ( op - )
        $0F AND 6 - DUP 2 < IF   ." @"   THEN
\        2* [ ( *)DUP ] LITERAL + 2 TYPE ; DROP
	2* "regs + 1+ 2 type ;

: .XX   ( n - )
        BASE @ >R   HEX 0 <# # # #> TYPE   R> BASE ! ;

: .XXXX   ( n - )
        BASE @ >R   HEX 0 <# # # # # #> TYPE   R> BASE ! ;

: .BYTE   ( a1 - a2)   +BYTE .XX ;

: .REL-ADR   ( a1 - a2)
        +BYTE DUP .XX ." ("   DUP $80 AND IF   $FF00 OR   THEN
        OVER + .XXXX   ." )" ;

: .WORD   ( a - a+2 n)   dup @-t .self .BYTE .BYTE ;

: (READY)   ( a op - a' op)   DUP $0F AND ;

: READY   ( a op - a' op)   (READY) 6 MIN ;

: ABSOLUTE   ( a op - a' n)
        $00E0 AND 2* 2* 2* >R   +BYTE R> + ;

\ Disassember 0X

: .NOP   ( op - )   DROP ."   NOP" ;

: .AJMP   ( a op - a+1)
	absolute ." AJMP " dup >r over $f800 and or
	dup .self r> .XXXX ." (" .XXXX ." )" ;
\        ABSOLUTE ."  AJMP " DUP .XXXX
\        ." (" OVER $F800 AND OR dup .self .XXXX ." )" ;

: .LJMP   ( a op - a+2)   DROP ."  LJMP " .WORD ;

: .RR   ( op - )   DROP ."    RR A" ;

: .INC(A)   ( op - )   DROP ."   INC A" ;

: .INC(direct)   ( a op - a+1)   DROP ."   INC " .BYTE ;

: .INC(Rn)   ( op - )   ."   INC " .Rn ;

: .0X   ( a op - a')
        READY EXEC:
        .NOP .AJMP .LJMP .RR .INC(A) .INC(direct) .INC(Rn) ;

\ Disassember 1X

: .JBC   ( a op - a+3)
        DROP ."   JBC " .BYTE ." ," .REL-ADR ;

: .ACALL   ( a op - a+1)
	absolute ." ACALL " dup >r over $f800 and or dup >r
	dup .self r> r> .XXXX ." (" .XXXX ." )"
	handle-literal ;

: .LCALL   ( a op - a+1)
	DROP ." LCALL " dup >r .WORD r> @-t handle-literal ;

: .RRC   ( op - )   DROP ."   RRC A" ;

: .DEC(A)   ( op - )   DROP ."   DEC A" ;

: .DEC(direct)   ( a op - a+1)   DROP ."   DEC " .BYTE ;

: .DEC(Rn)   ( op - )   ."   DEC " .Rn ;

: .1X   ( a - a')
        READY EXEC:
        .JBC .ACALL .LCALL .RRC .DEC(A) .DEC(direct) .DEC(Rn) ;

\ Disassember 2X

: .JB   ( a op - a+3)   DROP ."    JB " .BYTE ." ," .REL-ADR ;

: .RET   ( op)   DROP ."   RET" ;

: .RL   ( op)   DROP ."    RL A" ;

: .ADD(#)  ( a op - a+1)   DROP ."   ADD A,#" .BYTE ;

: .ADD(direct)   ( a op - a+1)   DROP ."   ADD A," .BYTE ;

: .ADD(Rn)   ( op)   ."   ADD A," .Rn ;

: .2X   ( a - a')
        READY EXEC:
        .JB .AJMP .RET .RL .ADD(#) .ADD(direct) .ADD(Rn) ;

\ Disassember 3X

: .JNB   ( a op - a+3)
        DROP ."   JNB " .BYTE ." ," .REL-ADR ;

: .RETI   ( op - )   DROP ."  RETI" ;

: .RLC   ( op - )   DROP ."   RLC A" ;

: .ADDC(#)  ( a op - a+1)   DROP ."  ADDC A,#" .BYTE ;

: .ADDC(direct)   ( a op - a+1)   DROP ."  ADDC A," .BYTE ;

: .ADDC(Rn)   ( op - )   ."  ADDC A," .Rn ;

: .3X   ( a - a')
        READY EXEC:
        .JNB .ACALL .RETI .RLC .ADDC(#) .ADDC(direct) .ADDC(Rn) ;

\ Disassember 4X

: .JC   ( a op - a+3)   DROP ."    JC " .REL-ADR ;

: .ORL(direct,A)   ( a op - a+1)
        DROP ."   ORL " .BYTE ." ,A" ;

: .ORL(direct,#)   ( a op - a+2)
        DROP ."   ORL " .BYTE ." ,#" .BYTE ;

: .ORL(A,#)   ( a op - a+1)   DROP ."   ORL A,#" .BYTE ;

: .ORL(A,direct)   ( a op - a+1)   DROP ."   ORL A," .BYTE ;

: .ORL(A,Rn)   ( op - )   ."   ORL A," .Rn ;

: .4X   ( a - a')   READY EXEC:
        .JC .AJMP .ORL(direct,A) .ORL(direct,#) .ORL(A,#)
        .ORL(A,direct) .ORL(A,Rn) ;

\ Disassember 5X

: .JNC   ( a op - a+3)   DROP ."   JNC " .REL-ADR ;

: .ANL(direct,A)   ( a op - a+1)
        DROP ."   ANL " .BYTE ." ,A" ;

: .ANL(direct,#)   ( a op - a+2)
        DROP ."   ANL " .BYTE ." ,#" .BYTE ;

: .ANL(A,#)   ( a op - a+1)   DROP ."   ANL A,#" .BYTE ;

: .ANL(A,direct)   ( a op - a+1)   DROP ."   ANL A," .BYTE ;

: .ANL(A,Rn)   ( op - )   ."   ANL A," .Rn ;

: .5X   ( a - a')
        READY EXEC:
        .JNC .ACALL .ANL(direct,A) .ANL(direct,#) .ANL(A,#)
        .ANL(A,direct) .ANL(A,Rn) ;

\ Disassember 6X

: .JZ   ( a op - a+3)   DROP ."    JZ " .REL-ADR ;

: .XRL(direct,A)   ( a op - a+1)
        DROP ."   XRL " .BYTE ." ,A" ;

: .XRL(direct,#)   ( a op - a+2)
        DROP ."   XRL " .BYTE ." ,#" .BYTE ;

: .XRL(A,#)   ( a op - a+1)   DROP ."   XRL A,#" .BYTE ;

: .XRL(A,direct)   ( a op - a+1)   DROP ."   XRL A," .BYTE ;

: .XRL(A,Rn)   ( op - )   ."   XRL A," .Rn ;

: .6X   ( a - a')
        READY EXEC:
        .JZ .AJMP .XRL(direct,A) .XRL(direct,#) .XRL(A,#)
        .XRL(A,direct) .XRL(A,Rn) ;

\ Disassember 7X

: .JNZ   ( a op - a+3)   DROP ."   JNZ " .REL-ADR ;

: .ORL(C,bit)   ( a op - a+1)   DROP ."   ORL C," .BYTE ;

: .JMP(@A+DPTR)   ( op - )   DROP ."   JMP @A+DPTR" ;

: .MOV(A,#)   ( a op - a+1)   DROP ."   MOV A,#" .BYTE ;

: .MOV(direct,#)   ( a op - a+2)
        DROP ."   MOV " .BYTE ." ,#" .BYTE ;

: .MOV(Rn,#)   ( a op - a+1)
        ."   MOV " .Rn ." ,#" .BYTE ;

: .7X   ( a - a')
        READY EXEC:
        .JNZ .ACALL .ORL(C,bit) .JMP(@A+DPTR) .MOV(A,#)
        .MOV(direct,#) .MOV(Rn,#) ;

\ Disassember 8X

: .SJMP   ( a op - a+1)   DROP ."  SJMP " .REL-ADR ;

: .ANL(C,bit)   ( a op - a+1)   DROP ."   ANL C," .BYTE ;

: .MOVC(A,@A+PC)   ( op - )   DROP ."  MOVC A,@A+PC" ;

: .DIV   ( op - )   DROP ."   DIV AB" ;

: .MOV(direct,direct)   ( a op - a+2)
        DROP ."   MOV " +BYTE >R +BYTE .XX ." ," R> .XX ;
\        .BYTE ." ," .BYTE ;

: .MOV(direct,Rn)   ( a op - a)
        ."   MOV " >R .BYTE ." ," R> .Rn ;

: .8X   ( a - a')
        READY EXEC:
        .SJMP .AJMP .ANL(C,bit) .MOVC(A,@A+PC) .DIV
        .MOV(direct,direct) .MOV(direct,Rn) ;

\ Disassember 9X

: .MOV(DPTR,#)   ( a op - a+2)   DROP ."   MOV DPTR,#" .WORD ;

: .MOV(bit,C)   ( a op - a+1)   DROP ."   MOV " .BYTE ." ,C" ;

: .MOVC(A,@A+DPTR)   ( op - )   DROP ."  MOVC A,@A+DPTR" ;

: .SUBB(A,#)   ( a op - a+1)   DROP ."  SUBB A,#" .BYTE ;

: .SUBB(A,direct)   ( a op - a+1)   DROP ."  SUBB A," .BYTE ;

: .SUBB(A,Rn)   ( op - )   ."  SUBB A," .Rn ;

: .9X   ( a - a')
        READY EXEC:
        .MOV(DPTR,#) .ACALL .MOV(bit,C) .MOVC(A,@A+DPTR)
        .SUBB(A,#) .SUBB(A,direct) .SUBB(A,Rn) ;

\ Disassember AX

: .ORL(C,/bit)   ( a op - a+1)   DROP ."   ORL C,/" .BYTE ;

: .MOV(C,bit)   ( a op - a+1)   DROP ."   MOV C," .BYTE ;

: .INC(DPTR)   ( op - )   DROP ."   INC DPTR" ;

: .MUL   ( op - )   DROP ."   MUL AB" ;

: .reserved   ( op - )   DROP ." reserved" ;

: .MOV(Rn,direct)   ( a op - a)   ."   MOV " .Rn ." ," .BYTE ;

: .AX   ( a - a')   READY EXEC:
        .ORL(C,/bit) .AJMP .MOV(C,bit) .INC(DPTR)
        .MUL .reserved .MOV(Rn,direct) ;

\ Disassember BX

: .ANL(C,/bit)   ( a op - a+1)   DROP ."   ANL C,/" .BYTE ;

: .CPL(bit)   ( a op - a+1)   DROP ."   CPL " .BYTE ;

: .CPL(C)   ( op - )   DROP ."   CPL C" ;

: .CJNE(A,#,rel)   ( a op - a+2)
        DROP ."  CJNE A,#" .BYTE ." ," .REL-ADR ;

: .CJNE(A,direct,rel)   ( a op - a+2)
        DROP ."  CJNE A," .BYTE ." ," .REL-ADR ;

: .CJNE(Rn)   ( op - )   ( a op - a+2)
        ."  CJNE " .Rn   ." ,#" .BYTE ." ," .REL-ADR ;

: .BX   ( a - a')
        READY EXEC:
        .ANL(C,/bit) .ACALL .CPL(bit) .CPL(C)
        .CJNE(A,#,rel) .CJNE(A,direct,rel) .CJNE(Rn) ;

\ Disassember CX

: .PUSH   ( a op - a+1)   DROP ."  PUSH " .BYTE ;

: .CLR(bit)   ( a op - a+1)   DROP ."   CLR " .BYTE ;

: .CLR(C)   ( op - )   DROP ."   CLR C" ;

: .SWAP   ( op - )   DROP ."  SWAP A" ;

: .XCH(A,direct)   ( a op - a+1)   DROP ."   XCH A," .BYTE ;

: .XCH(Rn)   ( op - )   ."   XCH A," .Rn ;

: .CX   ( a - a')   READY EXEC:
        .PUSH .AJMP .CLR(bit) .CLR(C) .SWAP
        .XCH(A,direct) .XCH(Rn) ;

\ Disassember DX

: .POP   ( a op - a+1)   DROP ."   POP " .BYTE ;

: .SETB(bit)   ( a op - a+1)   DROP ."  SETB " .BYTE ;

: .SETB(C)   ( op - )   DROP ."  SETB C" ;

: .DA   ( op - )   DROP ."    DA A" ;

: .DJNZ(direct,rel)   ( a op - a+2)
        DROP ."  DJNZ " .BYTE ." ," .REL-ADR ;

: .XCHD(Rn)   ( op - )   ."  XCHD A,@R" 1 AND . ;

: .DJNZ(Rn,rel)   ( op - )   ."  DJNZ " .Rn ." ," .REL-ADR ;

: .DX   ( a - a')
        (READY) 8 MIN EXEC:
        .POP .ACALL .SETB(bit) .SETB(C) .DA
        .DJNZ(direct,rel) .XCHD(Rn) .XCHD(Rn) .DJNZ(Rn,rel) ;

\ Disassember EX

: .MOVX(A,@DPTR)   ( op - )   DROP ."  MOVX A,@DPTR" ;

: .MOVX(A,Rn)   ( op - )   ."  MOVX A," 1 AND 6 + .Rn ;

: .CLR(A)   ( op - )   DROP ."   CLR A" ;

: .MOV(A,direct)   ( a op - a+1)   DROP ."   MOV A," .BYTE ;

: .MOV(A,Rn)   ( op - )   ."   MOV A," .Rn ;

: .EX   ( a - a')
        READY EXEC:
        .MOVX(A,@DPTR) .AJMP .MOVX(A,Rn) .MOVX(A,Rn)
        .CLR(A) .MOV(A,direct) .MOV(A,Rn) ;

\ Disassember FX

: .MOVX(@DPTR,A)   ( op - )   DROP ."  MOVX @DPTR,A" ;

: .MOVX(Rn,A)   ( op - )   ."  MOVX " 1 AND 6 + .Rn ." ,A" ;

: .CPL(A)   ( op - )   DROP ."   CPL A" ;

: .MOV(direct,A)   ( a op - a+1)
        DROP ."   MOV " .BYTE ." ,A" ;

: .MOV(Rn,A)   ( op - )   ."   MOV " .Rn ." ,A" ;

: .FX   ( a - a')
        READY EXEC:
        .MOVX(@DPTR,A) .ACALL .MOVX(Rn,A) .MOVX(Rn,A)
        .CPL(A) .MOV(direct,A) .MOV(Rn,A) ;

: .INSTRUCTION   ( a - a')
	dup cfa-t if  cr ( over .XXXX) ." ----  LABEL " .id  else  drop  then
	CR DUP .XXXX 2 SPACES  +BYTE DUP
\   U2/ U2/ U2/ U2/ EXEC:
   2/ 2/ 2/ 2/ exec:
   .0X .1X .2X .3X .4X .5X .6X .7X
   .8X .9X .AX .BX .CX .DX .EX .FX ;

: DISASSEMBLE   ( a - )
        CR
." Press <ESCAPE> to quit disassembling, any other key to continue."
        BEGIN   .INSTRUCTION KEY 27 =
        UNTIL   DROP ;

nowarn
: decode  ( addr1 addr2 - )
    cr >r
    begin  .instruction  dup r@ < while repeat  r> 2drop ;
warn

: decode-all  (  - ) rom-start romHERE decode ;

: tdump  ( a n - )  swap there  swap dump ;

in-meta
0 [if]

F: .ID-T_BYTE   ( flag - )
        IF      IP-T @ C@-T .XX   1 IP-T +!   THEN ;F

F: .ID-T_CELL   ( flag - )
        IF      IP-T @ @-T .XXXX   2 IP-T +!   THEN ;F

F: .ID-T_STRING   ( flag - )
        IF      IP-T @ THERE 1+ IP-T @ C@-T type \ TYPEL
                IP-T @ C@-T 1+ IP-T +!
        THEN ;F

F: .ID-T_SPECIAL   (  - flag)
        IP-T @ DUP CR .XXXX 4 SPACES @-T   2 IP-T +!
        DUP .ID-T
        DUP [T'] CLIT       = .ID-T_BYTE
        DUP [T'] LIT        = .ID-T_CELL
        DUP [T'] BRANCH     = .ID-T_CELL
        DUP [T'] ?BRANCH    = .ID-T_CELL
\        DUP [T'] (next)     = .ID-T_CELL
\ 		DUP [T'] (do)       = .ID-T_CELL
\		DUP [T'] (loop)     = .ID-T_CELL
\		DUP [T'] (+loop)    = .ID-T_CELL
\		DUP [T'] (.")       = .ID-T_STRING
        DROP SPACE   \ IP-T @ 1- COLUMN# C@ $FE =
        ;F
[then]

m: see  ' disassemble ;m
\ t: see  see ;t

\s *****

F: DECOMPILE      ( a - )
        DUP DUP C@-T <JUMP> =   SWAP 1+ @-T NEST =   AND NOT
        ABORT" Not a colon word, can't be decompiled." CR
        ." Press <ESCAPE> to quit decompiling, any other key to continue."
        3 + IP-T !
        BEGIN   .ID-T_SPECIAL \ IF   EXIT   THEN
                KEY 27 =
        UNTIL   ;F

WARNINGS OFF
F: (SEE)   ( a - )
        [[ TALKER ]] HOSTING
        \ Read from the Target image, not the actual target.
        DUP CR ." Code Field Address: " .XXXX
        DUP +BYTE #LJMP# = NOT IF   DROP DISASSEMBLE EXIT   THEN
        +WORD
        DUP  c8 = IF   DROP NIP .CCONSTANT EXIT   THEN
        DUP c16 = IF   DROP NIP .CONSTANT  EXIT   THEN
        DUP pfa = IF   DROP NIP .CREATE    EXIT   THEN
        2DROP DECOMPILE ;F
WARNINGS ON

M: SEE      (  - )   ' (SEE) ;M
T: SEE      (  - )   SEE ;T

