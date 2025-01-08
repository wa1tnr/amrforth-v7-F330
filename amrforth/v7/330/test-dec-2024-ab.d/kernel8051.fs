\  kernel8051.fs  -  DO NOT PROPAGATE - test - f330 changes 2012 cwh

0 [if]   kernel8051.fs
    Most primitive part of the amrForth virtual machine.
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

0 [if]
This Forth was developed by :

    Albert Lee Mitchell and
    Charles Shattuck

Register Bank Zero is used by amrFORTH as follows:

0   SP 8 bit stack pointer.
1   R1 Scratch register or 8 bit external ram return stack pointer
2   R2 High byte of Top of Stack.
3   R3 Scratch register.
4   R4 Scratch register.
5   R5 Scratch register.
6   R6 Scratch register.
7   R7 Scratch register.

DPTR is also a scratch register.

PSW.5 is used by the serial interrupt routine.

The 8051 machine stack is used as the Forth Return Stack, RP, and at
this point the stacks are always in internal RAM.

Variables start at address 8 or at $20 (with interrupt driven UART)  \ ALL
Address $20 is reserved for 8 bit addressable bit variables, unless the
command   0 bit-variables   is executed.

A low level Forth word must preserve or knowingly and carefully change
A, B, SP and RP.  All other registers may be changed freely and need not
be restored.  In the same vein, none of those registers may contain
static data as any register may be used and modified by any other Forth
word.  Keep static data in variables.

This is a call-threaded forth, with a few words compiled as inline code
without a call.  When a call is immediately followed by an exit the
call is changed to a jump and the return is eliminated.  This is
efficient tail recursion, but also works as a goto.

The 8051 seems to prefer to have the low byte in a 16 bit value at the
higher address, and the high byte at the lower address.  This is seen in
PUSH and POP as well as  # DPTR mov  so we followed that convention in
variables and on the return stack.  On the data stack the bytes are
reversed in order to simplify the coding of the arithmetic and logic
words.  This won't matter to the application programmer for the most
part.

The Top of Stack is now cached in the R2:A register pair.  A contains   \ ALL
the low byte, and R2 contains the high byte.  For code words the macros
|dup and |drop move data into and out of the TOS.  This will matter to
the application programmer in code words that get data from the data
stack.

[then]

in-forth

nowarn
\ We're about to define Target versions, keep alias's of the old ones.
: [[  (  - ) [compile] [ ; immediate
: ]]  (  - ) ] ;
warn

\ not defined here anymore:
\ : $=  ( a1 l1 a2 l2 - flag)  compare 0= ;

\ defined in compile.fs and used here:
\ compile.fs
\ 31 : $=ngfv6  ( addr1 len1 addr2 len2 - flag) compare 0= ;

sfr-file s" sfr-816.fs"  $=ngfv6 [if]  create ADuC816  [then]
sfr-file s" sfr-812.fs"  $=ngfv6 [if]  create ADuC812  [then]
sfr-file s" sfr-f000.fs" $=ngfv6 [if]  create C8051F000  [then]
sfr-file s" sfr-f300.fs" $=ngfv6 [if]  create C8051F300  [then]
sfr-file s" sfr-f310.fs" $=ngfv6 [if]  create C8051F310  [then]
sfr-file s" sfr-f330.fs" $=ngfv6 [if]  create C8051F330  [then]
sfr-file s" sfr-f061.fs" $=ngfv6 [if]  create C8051F061  [then]
sfr-file s" sfr-552.fs"  $=ngfv6 [if]  create 80c552   [then]
sfr-file s" sfr-537.fs"  $=ngfv6 [if]  create 80c537   [then]
sfr-file s" sfr-31.fs"   $=ngfv6 [if]  create 8051   [then]
sfr-file s" sfr-32.fs"   $=ngfv6 [if]  create 8052   [then]
sfr-file s" sfr-C51RC2.fs"   $=ngfv6 [if]  create C51RC2   [then]

has C8051F000
has C8051F300 or
has C8051F310 or
has C8051F330 or
has C8051F061 or [if]  create cygnal-chip  [then]

has ADuC816
has ADuC812 or [if]  create aduc-chip  [then]

has C51RC2 [if]  create atmel-chip  [then]

has cygnal-chip
has aduc-chip or [if]  create bootloader-installed  [then]

in-compiler

nowarn
f: call,  ( a - ) hint [[ also asm ]] call [[ previous ]] ;f
f: lcall, ( a - ) [[ also asm ]] lcall [[ previous ]] ;f
f: jump,  ( a - ) [[ also asm ]] jump [[ previous ]] ;f
f: ljmp,  ( a - )  [[ also asm ]] ljmp [[ previous ]] ;f

false value table?

f: mark-token  (  - )
	romHERE 1+ erase-target-byte-record  \ Erase following record.
	\ sourceline# romHERE line# w!ngfv7 >in @ romHERE column# c! ;f
	sourceline# romHERE line# w! >in @ romHERE column# c! ;f

\ Tokens need to be called, except in jump tables.
f: token,  ( a - )
	mark-token
	table? if  romHERE exits-entry ljmp, exit then  call, ;f

\ In a call threaded or subroutine threaded system there is no inner
\ interpreter for colon type words. They are just subroutines in the
\ native machine language. We alias NEXT to assemble RET to minimize the
\ changes in all the other code words.  The 'hide' suppresses tail
\ recursion optimization across words.
a: next  ret hide ;a

\ Think of the top of the return stack as the IP register. We discard
\ the current IP by popping the return stack and the desired new value
\ of IP is already there waiting on the return stack!
a: exit-absolute  (  - )
	[ in-forth ]
	edge c@-t $1f and $11 = if
        edge exits-entry 
		edge dup c@-t $ef and swap c!-t exit
	then  [ in-assembler ] next ;a
a: exit-long  (  - )
	[ in-forth ]
    edge c@-t $12 = if
        edge exits-entry
		$02 edge c!-t exit
	then  [ in-assembler ] next ;a
i: exit  (  - )
	edge romHERE 3 - = if
		[ also asm ] exit-long [ previous ] exit
	then
	edge romHERE 2 - = if
		[ also asm ] exit-absolute [ previous ] exit
	then
	mark-token
	[ also asm ] next [ previous ] ;i

has 8031 [if]
\ 128 bytes of CPU ram.
$80 to SP0
$22 to RP0  \ Leave room for some variables and bit variables.
[else]
\ 256 bytes of CPU ram.
$00 to SP0  \ Let data stack wrap around to $fe:$fd.
$7f to RP0  \ Return stack is pre-incremented in a call or a push.
[then]
warn

rom-start dup cr .( ROM-START=) u. cr
has bootloader-installed 0= romming 0= and [if] $8000 or [then]
in-assembler org in-meta

i: [asm  in-assembler  state off ;i

code noop  (  - ) next c;

a: |nip   SP inc  SP inc ;a
i: nip  [[ in-assembler ]]  |nip  [[ in-immed ]] ;i

a: |dup  SP dec  'R2 @SP mov  SP dec  A @SP mov ;a
code dup  ( n - n n) |dup  next c;

a: |drop  @SP A mov  SP inc  @SP 'R2 mov  SP inc ;a
code 2drop  ( n1 n2 - ) |nip c;         \ exits through drop !!  \ ALL
code drop  ( n - ) |drop  next c;

\ Runtime code fragment for romCREATE, pushes a Parameter Field Address
\ onto the data stack.
label pfa  (  - a) ' dup call  'R2 pop  ACC pop  next c;

a: |invert  ( n1 - n2) A cpl  $ff # 'R2 xrl ;a
\ i: invert  [[ in-assembler ]]  |invert  [[ in-immed ]] ;i
code invert  ( n1 - n2) |invert  next c;

a: |flip  R2 A xch ;a
code flip  ( n1 - n2) |flip  next c;

a: |dropflip  @SP 'R2 mov  SP inc  @SP A mov  SP inc ;a

a: |1+  ( n1 - n2) A inc  0= if  R2 inc  then ;a
code 1+  ( n1 - n2) |1+  next c;

a: |2+  ( n1 - n2) 2 # A add  +C if  R2 inc  then ;a
code 2+  ( n1 - n2) |2+  next c;

a: |1-  ( n1 - n2) 0= if  R2 dec  then  A dec ;a
code 1-  ( n1 - n2) |1-  next c;

\ a: |2-  ( n1 - n2) C clr  2 # A subb  +C if  R2 dec  then ;a
\ code 2-  ( n1 - n2) |2-  next c;

\ Hand optimize cases where TOS interacts with a literal, using the
\ assembler because they require a literal operand.
\ e.g.  [asm  $ff #and ]
a: #and  ( n - ) mark-token dup # A anl  8 rshift # 'R2 anl ;a
a: #or   ( n - ) mark-token dup # A orl  8 rshift # 'R2 orl ;a
a: #xor  ( n - ) mark-token dup # A xrl  8 rshift # 'R2 xrl ;a
a: #lit  ( n - ) mark-token dup # A mov  8 rshift # R2 mov ;a

-code lit  (  - n)
    ' dup call  DPH pop  DPL pop  A clr  @A+DPTR A movc
    A R2 mov  DPTR inc  A clr  @A+DPTR A movc  DPTR inc
    DPL push  DPH push  next c;
f: literal-t  ( n - ) [t'] lit token, ,-t ;f
i: literal  ( n - ) literal-t ;i

f: reclaim-lcall-LIT  (  - n)
	romHERE 4 - @-t [t'] lit - abort" Previous LCALL not LIT"
	romHERE 2 - @-t  -5 romALLOT ;f

f: reclaim-acall-LIT  (  - n)
	romHERE 4 - c@-t 2* 2* 2* $700 and
	romHERE 2 - c@-t $ff and +
	romHERE $f800 and +
	[t'] lit - abort" Previous ACALL not LIT"
	romHERE 2 - @-t  -4 romALLOT ;f

f: reclaim-literal  (  - n)
	romHERE 5 - c@-t $12 = if  reclaim-lcall-LIT exit  then
	romHERE 4 - c@-t $1f and $11 = if  reclaim-acall-LIT exit  then
	true abort" Previous word not LIT" ;f

i: #and  (  - ) reclaim-literal [ also asm ] #and [ previous ] ;i
i: #or   (  - ) reclaim-literal [ also asm ] #or  [ previous ] ;i
i: #xor  (  - ) reclaim-literal [ also asm ] #xor [ previous ] ;i
i: #lit  (  - ) reclaim-literal [ also asm ] #lit [ previous ] ;i

\ Compile the address of the following word into the Target image.
i: [']  (  - ) ' literal-t ;i

\ Compile the ascii code of the next character in the input stream.
m: char  (  - c) BL word 1+ c@ ;m
i: [char]  (  - c) char literal-t ;i

\ Begin a comment, must be balance with a ).
i: (  (  - ) [compile] ( ;i immediate

\ Comment to end of line.
i: \  (  - ) [compile] \ ;i immediate

\ Comment to end of file.
i: \s  (  - ) [compile] \s ;i immediate

\ Attempt to convert the string at a into a number on the data stack.
\ The host is 32 bit and the target is 16 bit!
f: number-t   ( a - n)  \ 32 bit to 16 bit
    number double? if
        drop dup 16 rshift swap literal-t
    else
        drop
    then  literal-t ;f

\ [ turns off the Forth compiler, ] turns it back on again.
\ Use them to perform compile time address calculations.
m: [  (  - ) in-meta state off ;m
i: [  (  - ) [ ;i

in-forth
create pocket 258 allot
in-meta

m: ]   (  - )
    state on in-meta
    begin
        ?stack state @
    while
        begin
            begin
                tib >in @ + c@ BL = while
                1 >in +!
            repeat
	    BL word count pocket place pocket dup c@ 0= while
            drop refill 0= if  exit  then
        repeat
        target? if
            execute token,
        else
            immediate? if
                execute
            else
                count pad place BL pad count + c! pad number-t
            then
        then
    repeat ;m

\ Create a dictionary entry and compile a high level Forth word.
m: :  (  - )
	sources-entry
	!csp tcreate target definitions in-meta ] ;m

\ End the currently compiling word.
i: ;  (  - ) ?csp exit in-meta [[ meta ]] [ false to table? ;i  \ ] ;

\ End the currently compiling word, without a trailing exit.  To avoid
\ wasting memory in certain cases, like begin ... again -; for example.
i: -;  (  - ) ?csp in-meta [[ meta ]] [ false to table? ;i  \ ] ;

\ Define a new constant for the Target.
m: constant  ( n - )
    to-meta precreate [t'] lit token, dup ,-t
    [[ in-assembler ]] ret [[ to-meta ]]
    mcreate in-meta ;m

\ Create a named address reference in the target.  The address is the
\ next available space in the dictionary.  Use it to create tables of
\ data with , or C,.  Fetch the data with c@p or @p, since it is in code
\ memory.
m: create   (  - )
	( mark-token) sources-entry
	to-meta precreate pfa lcall, romHERE mcreate in-meta ;m
\ Create a named memory location in CPU RAM & to LOG... file  \ ALL
\ m: cpuCREATE  (  - ) to-meta logIRAM cpuDP @ constant ;m
m: cpuCREATE  (  - ) to-meta cpuDP @ constant logIRAM ;m \ ALL20041221
\ Create a named memory location in CPU RAM and allot 2 bytes.
m: variable  (  - ) 2 ?skip-bits cpuCREATE 2 cpuALLOT ;M
\ Create a named memory location in CPU RAM and allot one byte.
m: cvariable  (  - ) 1 ?skip-bits cpuCREATE 1 cpuALLOT ;M
\ Same as variable, but allot 4 bytes.
m: 2variable  (  - ) 4 ?skip-bits cpuCREATE 4 cpuALLOT ;M

\ Stack manipulation.
code swap  ( n1 n2 - n2 n1)
    SP 'R1 mov  R1 inc  @SP A xch  R2 A xch
    @R1 A xch  R2 A xch  next c;

code over  ( n1 n2 - n1 n2 n1)
    'SP R1 mov  ' dup call  @R1 A mov  R1 inc  @R1 'R2 mov  next c;

\ These require special handling in the single stepper.

\ ----- Internal memory, using mov ----- /

a: |c@i  ( a - c) A R1 mov  @R1 A mov  0 # R2 mov ;a
code c@i  ( a - c) |c@i  next c;

a: |@i  ( a - n) A R1 mov  @R1 A mov  A R2 mov  R1 inc  @R1 A mov ;a
code @i  ( a - n) |@i  next c;

code c!  ( c a - )  c;
code c!i  ( c a - )
    A R1 mov  @SP A mov  |nip  A @R1 mov  ' drop jump c;

code !  ( n a - )  c;
code !i  ( n a - )
    A R1 mov  ' drop call  |flip  A @R1 mov
    R2 A mov  R1 inc  A @R1 mov  ' drop jump c;

\ ----- External Data memory, using movx ----- /

\ Not every application uses external RAM.  To save 49 bytes just change
\ this 1 to a 0 to comment out these operators.
1 [if]
code c@d  ( a - c)
    A DPL mov  R2 DPH mov  @DPTR A movx  0 # R2 mov  next c;
code @d  ( a - n)
    A DPL mov  R2 DPH mov  @DPTR A movx  A R2 mov
    DPTR inc  @DPTR A movx  next c;
code c!d  ( c a - )
    A DPL mov  R2 DPH mov  @SP A mov  |nip
    A @DPTR movx  ' drop jump c;
code !d  ( n a - )
    A DPL mov  R2 DPH mov  |dropflip  A @DPTR movx  DPTR inc
    R2 A mov  A @DPTR movx  ' drop jump c;
[then]

\ If address greater than 255, use program memory, otherwise use
\ internal RAM.  This allows c@ for tables in program memory as a
\ convenience.
code c@  ( a - c)
    R2 A xch  0= if  R2 A xch  ' c@i jump  then  R2 A xch  c;
\ Program Code memory, using movc.
code c@p  ( a - c)
    A DPL mov  R2 DPH mov  A clr  A R2 mov  @A+DPTR A movc  next c;

\ If address greater than 255, use code memory, otherwise use internal
\ RAM.  This allows @ for tables in program memory as a convenience.
code @  ( a - n)
    R2 A xch  0= if  R2 A xch  ' @i jump  then  R2 A xch  c;
code @p  ( a - n)
    A DPL mov  R2 DPH mov  A clr  @A+DPTR A movc  A R2 mov
    1 # A mov  @A+DPTR A movc  next c;

\ ----- Arithmetic and logic ----- /

a: |+  ( n1 n2 - n3)
    @SP A add  R2 A xch  SP inc  @SP A addc  R2 A xch  SP inc  ;a
code +  ( n1 n2 - n3) |+  next c;

a: |+'  ( n1 n2 - n3)
    @SP A addc  R2 A xch  SP inc  @SP A addc  R2 A xch  SP inc  ;a

a: |and  ( n1 n2n - n3)
    @SP A anl  R2 A xch  SP inc  @SP A anl  R2 A xch  SP inc ;a
code and  ( n1 n2 - n3) |and  next c;

a: |or  ( n1 n2 - n3)
    @SP A orl  R2 A xch  SP inc  @SP A orl  R2 A xch  SP inc ;a
code or  ( n1 n2 - n3) |or  next c;

a: |xor  ( n1 n2 - n3)
    @SP A xrl  R2 A xch  SP inc  @SP A xrl  R2 A xch  SP inc ;a
code xor  ( n1 n2 - n3) |xor  next c;

code -  ( n1 n2 - n3)
    C clr  @SP A xch  @SP A subb  R2 A xch  SP inc
    @SP A xch  @SP A subb  R2 A xch  SP inc  next c;

a: |2*  ( n1 - n2) C clr  A rlc  R2 A xch  A rlc  R2 A xch ;a
code 2*  ( n1 - n2) |2*  next c;

a: |2/  ( n1 - n2)
    R2 A xch  7 .ACC C mov  A rrc  R2 A xch  A rrc ;i
code 2/  ( n1 - n2) |2/  next c;

code abs  ( n1 - n2)
    R2 B mov  7 .B set? if  c;
code negate  ( n1 - n2)
        ' invert call  ' 1+ jump
    then  next c;

0 [if] \ ***********
\            d c
\          * b a
\        -------
\            a*c
\          a*d
\          b*c
\      + b*d
\      ---------
code um*   ( u1 u2 - ud)
    'R2 push  'R2 R4 mov            ( d )
    ACC push  A B mov               ( c )
    @SP 'R7 mov  SP inc  'R7 push   ( b )
    @SP A mov  ACC push             ( a )
    AB mul  A @SP mov  B R6 mov     ( a*c )
    B pop  R4 A mov  AB mul         ( a*d )
    R6 A add  A R6 mov  +C if  B inc  then  B R5 mov
    B pop  ACC pop  AB mul          ( b*c )
    R6 A add  SP dec  A @SP mov  R5 A mov  B A addc  A R5 mov
    A clr  +C if  A inc  then  A R4 mov
    B pop  R7 A mov  AB mul         ( b*d )
    R5 A add  A R6 mov
    R4 A mov  B A addc  A R2 mov  R6 A mov
    next c;
[then] \ ***********
\            d c
\          * b a
\        -------
\            a*c
\          a*d
\          b*c
\      + b*d
\      ---------
code um*   ( u1 u2 - ud)
    'R2 push  'R2 R4 mov            ( d )
    ACC push  A B mov               ( c )
    @SP 'R7 mov  SP inc
    @SP A mov  R7 A xch  'R7 push   ( b )
    ACC push                        ( a )
    AB mul  A @SP mov  B R6 mov     ( a*c )
    B pop  R4 A mov  AB mul         ( a*d )
    R6 A add  A R6 mov  +C if  B inc  then  B R5 mov
    B pop  ACC pop  AB mul          ( b*c )
    R6 A add  ( *****) @SP A xch \ *****
    SP dec  A @SP mov  R5 A mov  B A addc  A R5 mov
    A clr  +C if  A inc  then  A R4 mov
    B pop  R7 A mov  AB mul         ( b*d )
    R5 A add  A R2 mov  R4 A mov  B A addc  
    R2 A xch
    next c;

: *   ( n1 n2 - n3)   um* drop ;

\ This is the unsigned division primitive in amrFORTH.
\ Divide ud, a double number, by u, a single unsigned number,
\ yielding ur the remainder, and uq the quotient,
\ both single unsigned numbers.
code um/mod  ( ud u - ur uq)
    R2 B mov  A R3 mov
    @SP 'R5 mov  SP inc  @SP 'R4 mov  SP inc
    @SP 'R7 mov  SP inc  @SP 'R6 mov  SP inc
    'SP push   16 # SP mov
    begin   C clr
        R7 A mov   A rlc   A R7 mov   R6 A mov   A rlc   A R6 mov
        R5 A mov   A rlc   A R5 mov   R4 A mov   A rlc   A R4 mov
        C 0 .B mov   \ Save the high bit of the remainder.
        C clr   R5 A mov   R3 A subb   A R1 mov
        R4 A mov   R2 A subb
        0 .B C nanl   \ Take the high remainder bit into account.
        -C if   R7 inc   A R4 mov   R1 A mov   A R5 mov   then
        SP dec   SP A mov
    0= until
    'SP pop
    R5 A mov  R4 'R2 mov  |dup
    R7 A mov  R6 'R2 mov
    next c;
\ All unsigned.
\ : /mod  ( u1 u2 - u3 u4) 0 swap um/mod ;
\ : mod   ( u1 u2 - u3) /mod drop ;
\ : /     ( u1 u2 - u3) /mod nip ;

\ ----- Comparisons ----- /

a: |0<  ( n1 n2 - flag)
    R2 B mov  7 .B C mov  ACC A subb  A R2 mov ;a
code 0<  ( n1 n2 - flag) |0<  next c;

a: |0=  ( n1 - flag)
    R2 A orl  $ff # A add  ACC A subb  A cpl  A R2 mov ;a

: =  ( n1 n2 - flag) xor -;
code 0=  ( n - flag) c;
code not  ( n - flag)  \ Logical, not bitwise.
    |0=  next c;

: u>  ( n1 n2 - flag) swap -;
code u<  ( u1 u2 - flag)
	C clr  @SP A xch  @SP A subb  R2 A mov  SP inc
	@SP A xch  @SP A subb  SP inc
	ACC A subb  A R2 mov
	next c;

: >  ( n1 n2 - flag) swap -;
code <  ( n1 n2 - flag)
	C clr  @SP A xch  @SP A subb  R2 A mov  SP inc
	$80 # A xrl  @SP A xch  $80 # A xrl
	@SP A subb  SP inc  ACC A subb  A R2 mov
	next c;

\ The return stack.
\ Note that >r and r> are called.  That means a return address needs to
\ be moved out of the way before pushing or popping the top of the data
\ stack.  This allows >r and r> to be run interactively without crashing
\ the target machine by the way.
code (>r)  ( n - )
    DPH pop   DPL pop                   \ save return in DPTR   \ ALL
    ACC push  'R2 push  ' drop call     \ push n to R-Stack     \ ALL
    DPL push  DPH push  next c;         \ restore return entry  \ ALL
code (r>)  (  - n)
    DPH pop  DPL pop  ' dup call  'R2 pop  ACC pop
    DPL push  DPH push  next c;
\ push and pop cannot be jumped to, must be called,
\ so they must hide from the optimizer.
i: push  [t'] (>r) token,  hide ;i
i: >r  [t'] (>r) token,  hide ;i
i: pop  [t'] (r>) token,  hide ;i
i: r>  [t'] (r>) token,  hide ;i

\ code i  c;
code (r@)  (  - n)
    ' dup call  RP R1 mov  R1 dec  R1 dec
    @R1 'R2 mov  R1 dec  @R1 A mov  next c;
\ i and r@ cannot be jumped to, must be called,
\ so they must hide from the optimizer.
i: i   [t'] (r@) token,  hide ;i
i: r@  [t'] (r@) token,  hide ;i

\ : */    ( u1 u2 u3 - u4) push um* pop um/mod nip ;

\ Compile time error checking for conditional expressions.
\ f: ?condition  ( ? - ) not abort" Conditionals Wrong" ;f
f: ?condition  ( ? - ) -2 u< abort" Conditionals Wrong" ;f
nowarn
f: >mark     (  - a) romHERE  0 ,-t ;f
f: >resolve  ( a - ) romHERE swap !-t ;f
f: >resolve-short  ( a1 a2 - ) [[ in-assembler ]] then then ;a
f: <mark     (  - a) romHERE ;f
f: <resolve  ( a - ) ,-t ;f

f: ?>mark     (  - ? a) true >mark ;f
f: ?>resolve  ( ? a | a1 ? a2 - )
	swap dup -1 = if  drop >resolve exit  then
	-2 = if  >resolve-short exit  then
	true abort" Conditionals Wrong" ;f
f: ?<mark     (  - ? a) true <mark ;f
f: ?<resolve  ( ? a - ) swap ?condition <resolve ;f
warn

code execute  ( a - ) ACC push  'R2 push  ' drop jump c;

-code ?branch  ( flag - )
	R2 A orl  $ff # A add  'R2 pop  ACC pop  +C if
		' 2+ call  ' execute jump
	then
	' @p call  ' execute jump c;

\ Each of the following compiler directives compiles an appropriate
\ branching word and resolves branch addresses at compile time.
\ BRANCH is optimized into an LJMP.
i: if  (  - flag a) [t'] ?branch token, ?>mark ;i
i: else  ( flag1 a1 - flag2 a2)
	romHERE branches-entry
	$02 c,-t ?>mark 2>r ?>resolve 2r> ;i
i: then  ( flag a - ) ?>resolve hide ;i
i: begin  (  - flag a) ?<mark hide ;i
i: until  ( flag a - ) [t'] ?branch token, ?<resolve ;i
i: again  ( flag a - )
	mark-token romHERE branches-entry
	swap ?condition [[ in-assembler ]] jump ;a
i: while  (  - flag a) if ;i
i: repeat  ( ?1 a1 ?2 a2 - ) 2swap again then ;i

\ Hand optimize certain conditionals.  These lay down inline code.
\ They are both smaller and faster than their high level equivalents.
\ The single-stepper needs to handle these specially.
\ 
\ The first one uses only the low byte of the top of the stack, but is
\ probably the most useful macro, a single instruction!
i: 1-dup0=until  ( flag a - )
	mark-token swap ?condition
	[[ in-assembler ]] ACC -zero until  hide ;a
i: dupif  (  - a1 flag a2)
	mark-token [[ in-assembler ]] 0 # A = if  0 # R2 = if  $80 if  
	fswap then fswap then  -2 over ;a
i: dupnotif  (  - a1 flag a2)
	mark-token [[ in-assembler ]] 0 # A = if  -2  0 # R2 = if  hide ;a
i: dup0=if  dupnotif ;i
i: dup0<if  (  - a1 flag a2)
	mark-token [[ in-assembler ]] R2 B mov  7 .B set? if  -2 over ;a
i: dup0<notif  (  - a1 flag a2)
	mark-token [[ in-assembler ]] R2 B mov  7 .B clr? if  -2 over ;a
i: dupuntil  ( flag a - )
	mark-token swap ?condition [[ in-assembler ]]
	0 # A = if  0 # R2 = if  2>r $80 until 2r>  then then  hide ;a
i: dupnotuntil  ( flag a - )
	mark-token swap ?condition [[ in-assembler ]]
	dup  0 # A = until  0 # R2 = until  hide ;a
i: dup0=until  dupnotuntil ;i
i: dup0<until  ( flag a - )
	mark-token swap ?condition [[ in-assembler ]]
	R2 B mov  7 .B set? until  hide ;a
i: dup0<notuntil  ( flag a - )
	mark-token swap ?condition [[ in-assembler ]]
	R2 B mov  7 .B clr? until  hide ;a
i: dup>r  ( n - n) mark-token [[ in-assembler ]] ACC push  'R2 push ;a
i: r>drop  (  - ) mark-token [[ in-assembler ]] RP dec  RP dec ;a

-code (next)
    ] pop pop 1- dup 0= if  drop 2+ execute exit  then
    push @p execute ;
i: for  (  - a)  [t'] (>r) token,  begin ;i
i: next  ( a - )  [t'] (next) token,  swap ?condition ,-t ;i
\ Use i or r@ to get the index of for/next loop.

\ Macros to simplify ABORT for the various versions.

has interrupts-kernel [if]
\ 8 for Register bank 0, 2+ for Head and Tail, and the buffer occupies
\ the rest of the bytes up to address $20, 'end-serial-buffer'.  That
\ makes a total of 22 bytes for the serial buffer, $20(32)-10.
    8 2+ value start-serial-buffer
	has end-serial-buffer 0= [if]
\ Avoid interfering with bit variables.
        	$20 value end-serial-buffer
	[then]		
    a: init-serial-buffer
        start-serial-buffer # 8 direct mov
        start-serial-buffer # 9 direct mov ;a
    a: ?init-interrupts  $90 # IE mov ;a
[then]

has interrupts-kernel not [if]
    a: init-serial-buffer   ;a   \ Assemble nothing!
    a: ?init-interrupts     0 # IE mov  ;a
[then]

nowarn
label cold
warn
has cygnal-chip not [if]
\ Because the bootloader does this on cygnal chips.
    $52 # SCON mov   \ Timer 1 as baud rate generator
    DEFAULT-TH1 # TH1 mov
    smod? [if]  $80 # PCON orl  [else]  $7F # PCON anl  [then]
	$20 # TMOD mov  \ Mode 2, 8 bit auto-reload.
	6 .TCON setb  \ Enable Timer 1.
[then]
has ADuC816 [if]
	0 # $D7 direct ( PLLCON) mov   \ Run at 12.582912 Mhz
[then]
\ COLD falls through into ABORT.
code abort   (  - )
    7 .IE clr
    RP0 # RP mov \ This is the machine SP.
    SP0 # SP mov \ This is the forth SP not the machine SP!  \ ALL
    init-serial-buffer
    ?init-interrupts
label 'boot
\ Replace NOOP with QUIT or GO to start FORTH, see the file end8051.fs.
	' noop ljmp c;

\ Non-interrupt driven rs232 communications.
has polled-kernel [if]
\ Save space with the call to 'drop' and 'dup' here instead of inlining.
\ Serial I/O is already pretty slow.
code emit  ( c - )
    begin  1 .SCON set? until  1 .SCON clr  A SBUF mov
    ' drop jump c;
code key  (  - c)
    begin  0 .SCON set? until  0 .SCON clr
    ' dup call  SBUF A mov  0 # R2 mov  next c;
code key?  (  - flag)
    ' dup call  0 .SCON C mov  ACC A subb  0 # R2 mov  next c;
[then]   \ polled?

\ Interrupt driven rs232 communications.
has interrupts-kernel [if]
\ The interrupt routine reserves PSW.5 as a flag bit.
\ Don't use this bit in your application.
cr .( Serial Interrupt uses PSW.5)
label serial-interrupt
    1 .SCON set? if
        1 .SCON clr  5 .PSW clr  reti
    then
    PSW push  'R0 push  8 direct R0 mov
    SBUF @R0 mov  0 .SCON clr  R0 inc
    end-serial-buffer # R0 = if then  -C if
        start-serial-buffer # R0 mov
    then
    R0 8 direct mov  'R0 pop  PSW pop
    reti c;
code emit  ( c - )
    begin  5 .PSW clr? until  5 .PSW setb
    A SBUF mov  begin  5 .PSW clr? until
    ' drop jump c;
code key  (  - c)
    ' dup call  0 # R2 mov
    begin  8 direct A mov  9 direct A xrl  0<> until
    9 direct R1 mov  @R1 A mov  R1 inc
    end-serial-buffer # R1 = if
        start-serial-buffer # R1 mov
    then
    R1 9 direct mov
    next c;
code key?  (  - flag)
    ' dup call
    8 direct A mov  9 direct A xrl
    $ff # A add  ACC A subb  A R2 mov
    next c;
[then]   \ interrupts?

\ Return the lesser of the top two stack items.
: min  ( n1 n2 - n3) over over > if  swap  then  drop ;
\ Return the greater of the top two stack items.
: max  ( n1 n2 - n3) over over < if  swap  then  drop ;

has 8031 [if]
\ Assume SP0 is $80 for 128 byte RAM.
code depth  (  - n)
    ' dup call  SP A mov  7 .ACC set? if
        A clr  A dec  A R2 mov  next  \ Stack underflow.
    then
    $7f # A anl  0= if  A R2 mov  next  then  \ Stack empty.
    $7f # A xrl  C clr  A rrc  0 # R2 mov
    next c;
[else]
\ Assume SP0 is $00 for 256 byte RAM.
code depth  (  - n)
    ' dup call  SP A mov  7 .ACC clr? if
        A clr  A dec  A R2 mov  next  \ Stack underflow.
    then
    0= if  A R2 mov  next  then  \ Stack empty.
    A cpl  C clr  A rrc  0 # R2 mov
    next c;
[then]

: (exec:)  ( n - ) dup 2* + pop + execute ;
i: exec:  (  - ) mark-token [t'] (exec:) token, true to table? ;i

m: patch-code-field
    r> @ lastxt >body @ 3 -  \ Address of code field.
    romHERE >r org  $12 c,-t ,-t  \ Patch code field.
    r> org  \ Restore dictionary, past data field.
    ;m

m: ;code
    reveal  \ Make latest 'create'd word visible.
    postpone patch-code-field  \ Patch its code field to romHERE.
    HERE host,  \ Remember that code field.
    postpone ;m  \ End that future definition.
    in-assembler ;m immediate  \ Start assembling the new code.

m: does>
    reveal  \ Make latest 'create'd word visible.
    postpone patch-code-field  \ Patch its code field to romHERE.
    HERE host,  \ Remember that code field.
    postpone ;m  \ End that future definition.
    mark-token sources-entry \ ." Marking Token in does> " romHERE u.
    [[ in-assembler ]]  |dup  'R2 pop  ACC pop
       \ Push the data field address onto the stack.
    [[ in-compiler ]] ]  \ Start compiling the rest of the new code.
    ;m immediate

-code (string)  (  - a l)
    |dup  DPH pop  DPL pop  DPL A mov  DPH 'R2 mov
    A inc  0= if  R2 inc  then
    |dup  A clr  A R2 mov  @A+DPTR A movc
    ' over call  ' over call  ' + call
    ACC push  'R2 push  |drop  ret c;

i: s"  (  - a l) [t'] (string) token,  34 string ;i

\ The text interpreter.
: number  (  - n) key key flip + ;
: perform  (  - ) number execute ;
here value vectors
' noop , ' number , ' perform , ' noop ,

: quit  (  - )
    begin
        7 emit key dup [asm $fffc #and ] if  [asm 0 #lit ]  then
        dup + [ vectors ] literal + @p execute
        depth 0< if  [char] ? emit abort  then
    again -;

\ romming 0= [if]
\ : key  (  - c) 1 emit number ;
\ : key?  (  - flag) 2 emit number ;
\ [then]

\ The following lines set the interrupt vectors to point to
\ ram for development, or to address zero when romming.
\ Address zero will contain a jump to ABORT when ABORT is
\ defined.  If the application uses interrupts, it should
\ conditionally set the vectors to point to the actual routines
\ when romming, or set the ram addresses to point to the actual
\ routines when developing.
0 value ram-vector
f: >ram-vector  ( a1 - a2) ram-vector + ;f

\ Calculate the RAM address to which the interrupt will vector.
has cygnal-chip [if]
	$200 to ram-vector
[then]

has aduc-chip [if]
	0 to ram-vector
[then]

romming not has bootloader-installed not and [if]
	$8000 to ram-vector
[then]

\ When romming assemble a jump to address 0 to cause a reset.  When
\ developing assemble a jump to an appropriate address in external RAM.
has cygnal-chip [if]
	f: vector  ( a - )
		romHERE >r >ram-vector org
		0 [[ in-assembler ]] ljmp [[ in-meta ]] r> org ;f
[else]
    romming [if]
        f: vector   ( a - )
            romHERE >r org
            0 [[ in-assembler ]] ljmp [[ in-meta ]] r> org ;f
    [else]
        f: vector   ( a - )
            romHERE >r   dup org
            >ram-vector [[ in-assembler ]] ljmp [[ in-meta ]]
            r> org ;f
    [then]
[then]

\ Patch all the interrupt vectors with a jump to reset.
\ Stop patching at rom-start.
m: all-vectors  (  - )
    $03
    begin
        dup vector 8 +  dup rom-start $ff and =
    until  drop ;m
all-vectors

has interrupts-kernel
has cygnal-chip 0= and [IF]
   ( *)romHERE in-assembler
   $23 org  serial-interrupt ljmp
   ( *)org	 in-meta
[then]

\ In order to set an interrupt vector, use the 8051 vector addresses in
\ low memory, the compiler is smart enough to revector them to RAM
\ during development and leave them in ROM when romming.

romming has cygnal-chip 0= and has atmel-chip or [if]
    m: int!  ( 'service-routine 'interrupt-vector - )
        02 ( ljmp) over c!-t 1+ !-t ;m
[else]
    m: int!  ( 'service-routine 'interrupt-vector - )
        02 ( ljmp) over >ram-vector c!-t >ram-vector 1+ !-t ;m
[then]

has interrupts-kernel
has cygnal-chip and [if]
	serial-interrupt $23 int! cr .( Serial Interrupt at $23 )
[then]

