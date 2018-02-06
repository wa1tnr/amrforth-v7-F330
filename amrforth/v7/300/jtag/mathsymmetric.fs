\ mathsymmetric.fs 

0 [if]   mathsymmetric.fs 
    Symmetric signed division operators.
Copyright (C) 2004 by AM Research, Inc.

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

\ Adds the carry to n1 and n2.
code +'  ( n1 n2 - n3) |+'  next c;

: d+  ( d1 d2 - d3) >r swap >r + r> r> +' ;

: dabs  ( d1 - d2) dup0<if  swap invert swap invert 1 0 d+  then ;

: ?negate  ( n1 n2 - n3) 0< if  negate  then ;

: sm/rem  ( d n - r q)
	over >r over over xor >r  \ Save the signs.
	>r dabs r> abs            \ Make everything positive.
	um/mod r> ?negate         \ Apply sign to quotient.
	swap r> ?negate swap      \ Apply sign to remainder.
	;

: /mod  ( n1 n2 - n3 n4) 0 swap sm/rem ;
: mod   ( n1 n2 - n3) /mod drop ;
: /     ( n1 n2 - n3) /mod nip ;
: */    ( n1 n2 n3 - n4) push um* pop sm/rem nip ;

