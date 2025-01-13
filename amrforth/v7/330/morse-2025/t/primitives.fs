0 [if]  primitives.fs  Forth Primitives not included in the kernel.
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

decimal in-host

: target-cells   ( n1 - n2)   2* ;

\ Unsigned division that rounds rather than truncates.
: u/round   ( u1 u2 - u3)
    ?dup if  >r 0 r@ um/mod swap 2* r> < not 1 and +  then ;

\ Signed version of above.
: /round   ( n1 n2 - n3)
    ?dup if  >r s>d r@ fm/mod swap 2* r> < not 1 and +  then ;
	
in-compiler

nowarn
0 constant false
-1 constant true
warn

code +'  ( n1 n2 - n3) |+'  next c;
: d+  ( d1 d2 - d3) push swap push + pop pop +' ;

: ?dnegate  ( d1 sign - d2) 0< not if  exit  then -;    
: dnegate  ( d1 - d2) push invert pop invert 1 0 d+ ;

: dabs  ( d1 - d2) dup 0< if  dnegate  then ;

: d-  ( d1 d2 - d3) dnegate d+ ;

: d=  ( d1 d2 - flag) push swap push =  pop pop = and ;

: 2!  ( n1 n2 a - ) dup >r ! r> 2+ ! ;

: 2@  ( a - n1 n2) dup 2+ @ swap @ ;

: 2!d  ( n1 n2 a - ) dup >r !d r> 2+ !d ;

: 2@d  ( a - n1 n2) dup 2+ @d swap @d ;

: +!  ( n a - ) dup >r @ + r> ! ;

: +!d  ( n a - ) dup >r @d + r> !d ;

: 2dup  ( n2 n2 - n1 n2 n1 n2) over over ;

: tuck  ( n1 n2 - n2 n1 n2) swap over ;

: within  ( n lo hi - flag) over - push - pop u< ;

: ?dup  ( n - n n | 0) dup if  dup  then ;

: um/round  ( ud u1 - u2)
    ?dup if  \ Avoid division by zero.
        push r@ um/mod swap 2* pop < not 1 and + exit
    then  drop ;

: u*/round  ( u1 u2 u3 - u4) push um* pop um/round ;

: <=  ( n1 n2 - flag) > not ;

\ Multiply two signed single precision integers,
\ result is signed double precision.
: m*  ( n1 n2 - d) 2dup xor >r abs swap abs um* r> ?dnegate ;

\ Change a single precision number to a double precision number.
: s>d  ( n - d) dup 0< ;

nowarn
32 constant BL
warn

\ Change character to uppercase.
: upc  ( c1 - c2)
	dup [char] a [ char z 1+ ] literal within if  $df and  then ;

\ Also sometimes called under+
: u+  ( n1 n2 n3 - n1+n3 n2) swap push + pop ;

: umin  ( u1 u2 - u3) 2dup u< not if  swap  then  drop ;

: rot  ( n1 n2 n3 - n2 n3 n1) push swap pop swap ;

\ Single precision version of ?negate.
\ Counts on NEGATE being a CODE word.
code ?negate   ( n1 n2 - n3)
    R2 A mov  7 .ACC clr? if  ' drop jump  then
    |drop  ' negate jump c;

