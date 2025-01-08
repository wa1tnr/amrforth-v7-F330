0 [if]   asm8051.fs   8051 Assembler
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

\ ALL20030814  }RANGE added for AJMP ACALL

[then]

in-forth

\ .( Loading asm8051.fs ) cr

IN-META

: CCONSTANT     ( n ++)
        CREATE  HOSTC,
        DOES>   ( a - n)   C@ ;

nowarn
: FSWAP   ( n1 n2 - n2 n1)   SWAP ;   ( An assembler version will be defined)

HEX
VARIABLE which

VARIABLE source
warn 

VARIABLE destination

: fresh   which OFF ; fresh
\ host' fresh !> 'FRESH   \ Resolves a forward reference in METACOMP.SEQ
\ CODE and LABEL both execute fresh to initialize things.

: (operand)   ( n - )
        1 which +!
        which @ 1 = IF   source !      EXIT   THEN
        which @ 2 = IF   destination ! EXIT   THEN
        TRUE ABORT" Too many operands" ;

: OPERAND   ( n - )   CONSTANT   DOES>   (  - ..a)  @ (operand) ;

IN-ASSEMBLER

-2 OPERAND AB
-1 OPERAND #            IN-META   -1 CONSTANT <#>       IN-ASSEMBLER
00 OPERAND A            IN-META   00 CONSTANT <A>       IN-ASSEMBLER
01 OPERAND direct       IN-META   01 CONSTANT <direct>  IN-ASSEMBLER
02 OPERAND @R0          02 OPERAND @SP
03 OPERAND @R1
04 OPERAND R0           04 OPERAND SP
05 OPERAND R1
06 OPERAND R2           06 OPERAND IH
07 OPERAND R3           07 OPERAND IL
08 OPERAND R4
09 OPERAND R5
0A OPERAND R6
0B OPERAND R7
80 OPERAND DPTR         IN-META   80 CONSTANT <DPTR>    IN-ASSEMBLER
81 OPERAND @DPTR        IN-META   81 CONSTANT <@DPTR>   IN-ASSEMBLER
82 OPERAND @A+DPTR      IN-META   82 CONSTANT <@A+DPTR> IN-ASSEMBLER
83 OPERAND @A+PC        IN-META   83 CONSTANT <@A+PC>   IN-ASSEMBLER
84 OPERAND C            IN-META   84 CONSTANT <C>       IN-ASSEMBLER
85 OPERAND BIT          IN-META   85 CONSTANT <BIT>     IN-ASSEMBLER

\ ----- Error checking.

IN-META

: ?OP-ERR   ( flag - )
        0= IF   fresh TRUE ABORT" Illegal operand combination"   THEN ;

: 0OPS?   (  - flag)   which @ 0 = ;
: 1OP?    (  - flag)   which @ 1 = ;
: 2OPS?   (  - flag)   which @ 2 = ;

: XCH?   (  - flag)
        destination @ <A> = NOT IF   FALSE EXIT   THEN
        source @ 1 12 WITHIN ;

: ADD?   (  - flag)   XCH? source @ <#> = OR ;

: XRL?   (  - flag)
        ADD? IF   TRUE EXIT   THEN
        destination @ <direct> =
        IF      source @   DUP <A> =   FSWAP <#> = OR
        ELSE    FALSE
        THEN    ;

: ANL?   (  - flag)
        XRL? IF   TRUE EXIT   THEN
        destination @ <C> =   source @ <BIT> =   AND ;


: RANGE? ( n - n)   DUP -80 80 WITHIN  NOT
   ABORT" Branch out of range." ;

: isBIT   ( n - )
        CCONSTANT
        DOES>   ( n a - bitadr)   C@ + <BIT> (OPERAND) ;

IN-ASSEMBLER

\ ----- Bit addressable SFR's common to all 80C31/80C51 family members ----
080 isBIT .P0           088 isBIT .TCON
090 isBIT .P1           098 isBIT .SCON ( Sometimes .S0CON )
0A0 isBIT .P2           0A8 isBIT .IE   ( Sometimes .IEN0  )
0B0 isBIT .P3           0B8 isBIT .IP   ( Sometimes .IP0   )
( 0C0 )                 ( 0C8 )
0D0 isBIT .PSW          ( 0D8 )
0E0 isBIT .ACC          ( 0E8 )
0F0 isBIT .B            ( 0F8 )
\ -------------------------------------------------------------------------

IN-META

: BIT?   (  - )   source @ <BIT> =   1OP? AND ;

IN-ASSEMBLER

: CLRCLR?   ( a - a)   BIT? ?OP-ERR 10 C,-T ;
\ FOLLOW WITH IF, WHILE, UNTIL)
\ Jump if bit set and clear bit, JBC.
\ e.g.   0 .P4 CLRCLR? IF   A CLR   THEN
\ means if bit 0 of port 4 is clear, then clear the accumulator, and
\ leave bit P4.0 clear in any case.

: SET?   ( a- a)   BIT? ?OP-ERR 30 C,-T ;
\ Jump if bit not set, JNB.
\ e.g.   0 .P4 SET? IF   A CLR   THEN
\ means if bit 0 of port 4 is set, then clear the accumulator.

: CLR?   ( a - a)   BIT? ?OP-ERR 20 C,-T ;
\ Jump if bit set, JB.
\ e.g.   0 .P4 CLR? IF   A CLR   THEN
\ means if bit 0 of port 4 is clear, then clear the accumulator.

IN-META

: ?0OPS   (  - )   0OPS? ?OP-ERR ;

: (i1)   ( c - )   C,-T fresh ;

: i1   ( n - )
        CCONSTANT
        DOES>   ( a - )   ?0OPS C@ (i1) ;

: (i2)   ( c - )
        source @ 0 MAX   DUP <DPTR> =
        IF      2DROP 0A3 C,-T
        ELSE    DUP >R   + C,-T   R> <direct> = IF   C,-T   THEN
        THEN    fresh ;

: (i3)   ( a - )
        destination @
        IF      destination @ <C> =
                IF      2E +
                ELSE    source @ IF   1- C,-T   ELSE   2 -   THEN
                THEN   C,-T C,-T
        ELSE    source @ 0 MAX   DUP >R + C,-T R>   2 < IF   C,-T   THEN
        THEN    fresh ;

: i3   ( n - )
        CCONSTANT
        DOES>   ( a - )   2OPS? ADD? AND ?OP-ERR C@ (i3) ;

: (i4)   ( c - )   C,-T C,-T fresh ;

IN-ASSEMBLER

: SETB   ( n - )
        1OP? source @ DUP <C> = FSWAP <BIT> = OR AND ?OP-ERR
        D2 source @ <BIT> =
        IF      C,-T
        ELSE    1+
        THEN    C,-T fresh ;

: LCALL   ( a - )   ?0OPS 12 C,-T ,-T fresh ;

: }RANGE ( n - n)  \ ALL20030814
        DUP F800 AND   HERE 2 + F800 AND = NOT
        ABORT" Branch out of range." ;

: ACALL  ( a - )  }RANGE  \ ALL20030814
        ?0OPS DUP 2/ 2/ 2/ E0 AND 11 + C,-T C,-T fresh ;

: CALL   ( a - )
        DUP F800 AND   HERE 2 + F800 AND =
        IF      ACALL
        ELSE    LCALL
        THEN ;

: LJMP   ( a - )   ?0OPS 02 C,-T ,-T fresh ;

: AJMP   ( a - )  }RANGE  \ ALL20030814
        ?0OPS DUP 2/ 2/ 2/ E0 AND 01 + C,-T C,-T fresh ;

: SJMP   ( a - )   ?0OPS $80 C,-T HERE 1+ - RANGE? C,-T fresh ;

: JUMP   ( a - )
        DUP F800 AND   HERE 2 + F800 AND =
        IF      AJMP
        ELSE    DUP HERE 2 + - -80 80 WITHIN IF
                        SJMP
                ELSE    LJMP
                THEN
        THEN ;

: XCHD   (  - )
        source @ 2 4 WITHIN   destination @ <A> =   AND
        2OPS? AND ?OP-ERR   D6 source @ 2 - + C,-T fresh ;

00 i1 NOP
22 i1 RET
32 i1 RETI

IN-META

: ?A   (  - )   1OP? source @ <A> = AND ?OP-ERR ;

IN-ASSEMBLER

: RR     (  - )   ?A 03 (i1) ;
: RRC    (  - )   ?A 13 (i1) ;
: RL     (  - )   ?A 23 (i1) ;
: RLC    (  - )   ?A 33 (i1) ;
: DA     (  - )   ?A D4 (i1) ;
: SWAP   (  - )   ?A C4 (i1) ;

IN-META

: ?AB   (  - )   1OP? source @ ( AB) -2 = AND ?OP-ERR ;

IN-ASSEMBLER

: DIV   (  - )   ?AB 84 (i1) ;
: MUL   (  - )   ?AB A4 (i1) ;

: JMP   (  - )
        1OP? source @ ( @A+DPTR) 82 = AND ?OP-ERR   73 (i1) ;

: INC   (  - )
        source @   DUP 0 12 WITHIN   FSWAP <DPTR> = OR
        1OP? AND ?OP-ERR   04 (i2) ;

: DEC   (  - )
        source @ 0 12 WITHIN   1OP? AND ?OP-ERR   14 (i2) ;

24 i3 ADD
34 i3 ADDC
94 i3 SUBB

: ORL   (  - )   2OPS? ANL? AND ?OP-ERR 44 (i3) ;
: ANL   (  - )   2OPS? ANL? AND ?OP-ERR 54 (i3) ;
: XRL   (  - )   2OPS? XRL? AND ?OP-ERR 64 (i3) ;
: XCH   (  - )   2OPS? XCH? AND ?OP-ERR C4 (i3) ;

IN-META

: ?C,/   (  - )
        destination @ <C> =
        source @ <BIT> = AND   2OPS? AND
        2OPS? AND ?OP-ERR ;

IN-ASSEMBLER

: NORL   (  - )   ?C,/ A0 (i4) ;
: NANL   (  - )   ?C,/ B0 (i4) ;

IN-META

: ?direct   (  - )   1OP? source @ <direct> = AND ?OP-ERR ;

IN-ASSEMBLER

: PUSH   (  - )   ?direct C0 (i4) ;
: POP    (  - )   ?direct D0 (i4) ;

: MOVC   (  - )
        2OPS?   destination @ <A> = AND
        source @ 82 84 WITHIN AND ?OP-ERR
        83 source @ 82 = IF   10 +   THEN   C,-T fresh ;

: MOVX   (  - )
        source @   DUP 2 4 WITHIN   FSWAP 81 = OR
        destination @ <A> = AND
        destination @   DUP 2 4 WITHIN   FSWAP 81 = OR
        source @ <A> = AND   OR 2OPS? AND   ?OP-ERR
        source @ <A> =
        IF      F0 destination
        ELSE    E0 source
        THEN
        @ DUP 81 =
        IF      DROP 0
        THEN    + C,-T fresh ;

IN-META

: ?CLR   (  - )
        source @   DUP <A> =   OVER <C> = OR
        FSWAP <BIT> = OR   1OP? AND ?OP-ERR ;

IN-ASSEMBLER

: CLR   (  | n - )
        ?CLR
        source @ <A> = IF   E4 C,-T        THEN
        source @ <C> = IF   C3 C,-T        THEN
        source @ <BIT> = IF   C2 C,-T   C,-T   THEN   fresh ;

: CPL   (  | n - )
        ?CLR
        source @ <A> = IF   F4 C,-T        THEN
        source @ <C> = IF   B3 C,-T        THEN
        source @ <BIT> = IF   B2 C,-T   C,-T   THEN   fresh ;

IN-META

: mov1   ( a - )   90 C,-T ,-T fresh ;

: mov,   ( op n1 | op 1 n2 - )
        0 MAX   DUP >R   + C,-T   R> 1 =
        IF      C,-T
        THEN ;

: mov74   (  - )   74 destination @ mov, C,-T fresh ;
: movF4   (  - )   F4 destination @ mov, fresh ;
: movE4   (  - )   E4 source @ mov, fresh ;
: mov84   ( ...? - )
        source @ <direct> = IF   FSWAP   THEN
        84 source @ mov, C,-T fresh ;
: movA4   (  - )   A4 destination @ mov, C,-T fresh ;

IN-ASSEMBLER

: MOV   (  - )
        0 ( default flag for error check)
        source @ <C> =   destination @ <BIT> = AND OR   \ MOV bit,C
        source @ <BIT> =   destination @ <C> = AND OR   \ MOV C,bit
        destination @ <DPTR> =   source @ <#> = AND OR  \ MOV DPTR,#nnnn
        source @ <#> = destination @ 0 12 WITHIN AND OR \ MOV Reg,#nn
        source @ <A> = destination @ 1 12 WITHIN AND OR \ MOV Reg,A
        destination @ <A> = source @ 1 12 WITHIN AND OR \ MOV A,Reg
        destination @ <direct> = source @ 0 12 WITHIN AND OR \ MOV direct,Reg
        source @ <direct> = destination @ 0 12 WITHIN AND OR \ MOV Reg,direct
        ?OP-ERR
        source @      84 = IF   92 C,-T C,-T fresh EXIT  THEN
        destination @ 84 = IF   A2 C,-T C,-T fresh EXIT  THEN
        destination @ 80 = IF   mov1  EXIT   THEN
        source @      -1 = IF   mov74 EXIT   THEN
        source @       0=  IF   movF4 EXIT   THEN
        destination @  0=  IF   movE4 EXIT   THEN
        destination @  1 = IF   mov84 EXIT   THEN
        source @       1 = IF   movA4 EXIT   THEN
        ;

40 CCONSTANT -C     \ Carry not set, JNC.
50 CCONSTANT +C     \ Carry set, JC.
60 CCONSTANT 0<>    \ Accumulator not zero, JNZ.
70 CCONSTANT 0=     \ Accumulator equals zero, JZ.

: -ZERO         (  - a)
\ Decrement, jump not zero: DJNZ.
\ e.g. BEGIN   R7 -ZERO UNTIL
\ means decrement R7 until it equals zero.
        source @ DUP 1 =
        IF      DROP $D5 C,-T
        ELSE    D4 +
        THEN    ;

IN-META

: CJNE?   (  - flag)
        source @ DUP <#> = FSWAP <direct> = OR   destination @ <A> = AND
        source @ <#> =   destination @ 2 12 WITHIN AND OR   2OPS? AND ;

: DJNE?   (  - flag)
        source @   DUP 4 12 WITHIN   FSWAP 1 = OR   1OP? AND ;

nowarn
: ?IF   ( c - )
\ Must be defined before = is redefined!
        0OPS?
        IF      DUP 40 =   OVER 50 = OR   OVER 60 = OR
                OVER 70 = OR   FSWAP 80 = OR   ?OP-ERR
        ELSE    DROP CJNE? DJNE? OR BIT? OR ?OP-ERR
        THEN    ;
warn

IN-ASSEMBLER

: =     (  - a)
\ Compare, jump if not equal: CJNE.
\ e.g. BEGIN   A DEC   5 # A = UNTIL
\ means loop until A equals 5.
        $B4 source @ 0 MAX   destination @ 0 MAX   OR + C,-T ;

\ Note that IF, WHILE, and UNTIL need to be preceded by a conditional,
\ e.g. +C, -C, 0<>, 0=, -ZERO, = to assemble a jump.  The sense of the
\ conditional gets reversed in the translation, so a JZ or Jump if
\ Accumulator Zero instruction is assembled by the 0<> mnemonic.  This
\ is so that the Forth will read correctly.

: BEGIN    (  - a)           romHERE ;
: IF       ( n - a)          DUP ?IF C,-T BEGIN 0 C,-T fresh ;
: WHILE    ( a1 n - a2 a3)   IF FSWAP ;
: THEN     ( a - )           romHERE OVER - 1- RANGE? FSWAP C!-T ;
: ELSE     ( a - a)          80 WHILE  THEN ;
: UNTIL    ( a n - )         DUP ?IF C,-T romHERE - 1- RANGE? C,-T fresh ;
: AGAIN    ( a - )           80 UNTIL ;
: REPEAT   ( a1 a2 - )       AGAIN THEN ;
\ Both AGAIN and ELSE assemble the SJMP or short jump instruction.

IN-META

: SFR  ( c - )
	sfr-entry cconstant
	does>   C@ <direct> (operand) ;

IN-ASSEMBLER

\ -------- SFR's common to all 80C31/80C51 family members. ----------------
\ Direct addressing of system registers, for use with PUSH, POP, =.
$00 SFR 'SP     ( $01 )         \ $02 SFR 'IH     $03 SFR 'IL
$00 SFR 'R0     $01 SFR 'R1     $02 SFR 'R2     $03 SFR 'R3
$04 SFR 'R4     $05 SFR 'R5     $06 SFR 'R6     $07 SFR 'R7

\ Bit addressable Special Function Registers.
$80 SFR P0      $88 SFR TCON    $90 SFR P1      $A0 SFR P2
$B0 SFR P3      $D0 SFR PSW     $E0 SFR ACC     $F0 SFR B

\ Not bit addressable Special Function Registers.
$81 SFR RP ( machine stack is return stack)
$82 SFR DPL     $83 SFR DPH     $87 SFR PCON    $89 SFR TMOD
$8A SFR TL0     $8B SFR TL1     $8C SFR TH0     $8D SFR TH1

\ Each chip has these four, though the names may differ.
$98 SFR SCON    $99 SFR SBUF    $A8 SFR IE      $B8 SFR IP

\ -------------------------------------------------------------------------

IN-META DECIMAL

