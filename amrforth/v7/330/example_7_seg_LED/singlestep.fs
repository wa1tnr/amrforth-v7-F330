\ singlestep.fs

0 [if]   singlestep.fs   Single step debugger for amrForth for 8051. 
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

variable nested

: not  ( flag - flag') 0= ;
: 2+  ( n1 - n2) 2 + ;

include labels.log	\ Defines .label
include exits.log	\ Defines exit-point?
include branches.log	\ Defines branch-entry?
include sources.log	\ Defines source-filename$

only forth also root definitions
vocabulary hidden-symbols
only forth also hidden-symbols definitions also forth
include ./hidden-symbols.log  \ Read the hidden-symbol table.
only forth also definitions

create object-code  64 1024 * allot

: c@-t  ( a - c) object-code + c@ ;
: @-t  ( a - n) object-code + count 256 * swap c@ + ;

0 value fid  \ File ID.
0 value romHERE

: read-object-code  (  - )
	s" rom.bin" r/o open-file throw to fid
	object-code [ 64 1024 * ] literal fid read-file throw
	to romHERE  fid close-file throw ;
read-object-code

3 constant bytes/target-record
64 1024 * bytes/target-record * constant target-markers-size
create target-markers target-markers-size allot
: read-target-markers  (  - )
	s" target-markers.bin" r/o open-file throw to fid
	target-markers target-markers-size fid  read-file throw drop
	fid close-file throw ;
read-target-markers

: column#  ( a1 - a2) bytes/target-record * target-markers + ;
: line#  ( a1 - a2) column# 1+ ;

: target@  ( a - n) @-t ;
: targetC@  ( a - c) c@-t ;

: [t']  (  - addr)
	also symbols ' previous execute
	postpone literal ; immediate

: [hidden']  (  - addr)
	also hidden-symbols ' previous execute
	postpone literal ; immediate

\ ----- A major design goal was not to alter or burden the target in any
\       way in order to single step.  This means that some operations
\       need to be simulated on the host.

variable ip-t  \ The simulated target instruction pointer.

true value final-exit

\ >r and r> are defined on the target such that they can be used
\ interactively without crashing the system.  No need for a simulator.
: remote->R  ( n - ) [t'] (>r) >execute ;
: remote-R>  (  - n) [t'] (r>) >execute ;

\ We need to save the address of the word under test whenever
\ we nest into another word, so that we can return to this one.
\ The variable 'CFA gets pushed to and popped from the NEST-STACK.

variable 'cfa

128 constant nstack-size

variable nest-stack   nstack-size allot

: init-ns  (  - ) nest-stack dup CELL+ swap ! ;

: push-ns  ( n - )
        nest-stack @ nest-stack nstack-size + < not
        abort" Nesting stack overflow."
        nest-stack @ ! 1 cells nest-stack +! ;

: pop-ns   (  - n)
	nest-stack @ nest-stack cell+ < if
		init-ns true abort" Nesting stack underflow."
	then
	-1 cells nest-stack +!   nest-stack @ @ ;

: init-simulator-stacks  (  - ) init-ns ;

\ ----- Displaying a menu ----- /

: .default-instructions   (  - )
	cr >red+white
." CR,BL-step, C-cont, F-forth, N-nest, U-unnest, R-refresh, Q-quit:"
	red+white> ;

: .unnest-instructions   (  - )
	cr >red+white
." Press any key to stop unnesting and return to single stepping. "
	red+white> ;

: .continue-instructions   (  - )
	cr >red+white
." Press any key to stop continuous mode and return to single stepping. "
	red+white> ;

: .forth-instructions   (  - )
	cr >red+white
." Your commands will be executed, <ENTER> returns to single stepping. "
	red+white> ;

variable which-instructions

: re.instructions   (  - )   0 which-instructions ! ;

: .instructions   (  - )
	which-instructions @ 1 = if  .unnest-instructions   exit  then
	which-instructions @ 2 = if  .continue-instructions exit  then
	which-instructions @ 3 = if  .forth-instructions    exit  then
	.default-instructions ;

\ ----- Display the source code directly ----- /

\ : max-source-length   (  - u)   rows 2/ ;
6 value max-source-length
6 value #source-lines
1 value top-line

\ Scrolling offset controls display of a definition that is too long
\ to fit into MAX-SOURCE-LENGTH lines.
0 value scrolling-offset

0 value source-fid

: check-file  ( n - ) abort" File Access Error" ;

: ?close-source-file  (  - )
	source-fid ?dup if  close-file drop 0 to source-fid  then ;

0 [if]
: find-source  ( target-cfa - )
	?close-source-file
	dup source-filename$ r/o open-file check-file to source-fid
	line# w@ to top-line ;
[then]

: read-source-line  (  - a u)
	pad dup 258 source-fid read-line check-file and ;

variable current-line#

: ++line#   (  - )   1 current-line# +! ;

0 [if]
: cursor-line#   (  - n)   ip-t @ line# w@ ;
[then]

: absolute-address@  ( a1 - a2)
	dup c@-t 2* 2* 2* $0700 and swap 1+ c@-t +
	ip-t @ 2 + $f800 and + ;

: address@  ( a1 - a2 flag)
	dup c@-t $1f and $11 = if  absolute-address@ true exit  then
	dup c@-t $12 = if  1 + @-t true exit  then
	dup c@-t $1f and $01 = if  absolute-address@ 1 exit  then
	dup c@-t $02 = if  1 + @-t 1 exit  then
	0 0 ;

: does?  ( a - flag)
	false
	over 0 + c@-t $18 - if  nip exit  then
	over 1 + c@-t $a6 - if  nip exit  then
	over 2 + c@-t $02 - if  nip exit  then
	over 3 + c@-t $18 - if  nip exit  then
	over 4 + c@-t $f6 - if  nip exit  then
	over 5 + c@-t $d0 - if  nip exit  then
\	over 6 + c@-t $d0 - if  nip exit  then
	nip 0= ;

: cursor-column#  (  - n)
\	ip-t @ address@ drop does? if
\		ip-t @ address@ drop column# c@ exit
\	then
	ip-t @ column# c@ ;

0 [if]
: adjust-scrolling-offset  (  - )
	cursor-line# top-line - max-source-length ( 2) 3 - -
	0 max to scrolling-offset ;

: .scrolling-offset  (  - )
	cursor-line# . top-line . scrolling-offset . ;
[then]

: back-one-word   ( a - u)
	1 swap 1-
	begin
		1- dup c@ BL > while
		1 under+
	repeat
	drop ;		

: highlight-word   ( a u - )
	2dup + back-one-word >r 2dup r@ - type
	>highlight dup r> - /string type >norm ;

: type-with-cursor   ( a u - )
	dup cursor-column# = if  highlight-word exit  then
	over cursor-column# 1-
	highlight-word
	cursor-column# 1- /string type ;

: erase-source   (  - )
	0 1 at-xy #source-lines 2+ cols * spaces ;

: target-.s  (  - )
	data-stack-colors [t'] .s ( >execute)
	clear-sbuf 2 emit-s sputter -listen 2 spaces
	return-stack-colors [t'] .rs
	clear-sbuf 2 emit-s sputter -listen interpreter-colors ;

: this-instruction  (  - c) ip-t @ c@-t ;

0 [if]
: .source   (  - )
	source-colors
        erase-source 0 0 at-xy
	0. source-fid reposition-file check-file
	0 current-line# !
	top-line 2 - 0 max scrolling-offset + 0 ?do
		++line#
		read-source-line 2drop
	loop
	#source-lines 0 do
		cr read-source-line  ( a u)
		++line# current-line# @ cursor-line# = if
			type-with-cursor
		else
			type
		then
	loop
	cr default-colors ." Data Stack: " target-.s 
	.instructions
	;
[then]

\ : source-line  ( tcfa - n) line# w@ ;

\ : source-length  ( target-cfa - line#)
\	dup source-line swap
\	begin	1+ dup column# c@ $FC > until
\	line# w@ - abs 1+ ;

0 [if]
: prepare-source-code  ( target-cfa - )
	\ dup source-length max-source-length min to #source-lines
	max-source-length to #source-lines
	find-source ;
[then]

: next-address  (  - addr)
	ip-t @ c@-t $1f and $11 = if  ip-t @ 2 + exit  then
	ip-t @ c@-t $12 = if  ip-t @ 3 + exit  then
	0 ;

: dots  ( n - ) 0 do  [char] . emit  loop ;

: max-name-length  (  - n) cols 2/ 2/ 20 max 32 min ;

: bottom-line  (  - n) ( rows 2 -) bottom 1 - ;

\ ----- Recognizing the optimizations ----- /

: +byte  (  - c) ip-t @ + c@-t ;

: is_1-dup0=until  (  - flag)
	false
	0 +byte $d5 - if  exit  then
	1 +byte $e0 - if  exit  then
	0= ;

: is_dupif  (  - flag)
	false
	0 +byte $b4 - if  exit  then
	1 +byte $00 - if  exit  then
	3 +byte $ba - if  exit  then
	4 +byte $00 - if  exit  then
	6 +byte $80 - if  exit  then
	0= ;
\ Order of usage matters!
: is_dupuntil  (  - flag)
	is_dupif dup if  drop 7 +byte $80 and 0= not  then ;

\ Check is_dupif first!
: is_dupnotif  (  - flag)
	false
	0 +byte $b4 - if  exit  then
	1 +byte $00 - if  exit  then
	3 +byte $ba - if  exit  then
	4 +byte $00 - if  exit  then
	0= ;
\ Order of usage matters!
: is_dupnotuntil  (  - flag)
	is_dupnotif dup if  drop 5 +byte $80 and 0= not  then ;

: is_dup0<if  (  - flag)
	false
	0 +byte $8a - if  exit  then
	1 +byte $f0 - if  exit  then
	2 +byte $30 - if  exit  then
	3 +byte $f7 - if  exit  then
	0= ;
\ Order of usage matters!
: is_dup0<until  (  - flag)
	is_dup0<if dup if  drop 4 +byte $80 and 0= not  then ;

: is_dup0<notif  (  - flag)
	false
	0 +byte $8a - if  exit  then
	1 +byte $f0 - if  exit  then
	2 +byte $20 - if  exit  then
	3 +byte $f7 - if  exit  then
	0= ;
\ Order of usage matters!
: is_dup0<notuntil  (  - flag)
	is_dup0<notif dup if  drop 4 +byte $80 and 0= not  then ;

: is_dup>r  (  - flag)
	false
	0 +byte $c0 - if  exit  then
	1 +byte $e0 - if  exit  then
	2 +byte $c0 - if  exit  then
	3 +byte $02 - if  exit  then
	0= ;

: is_r>drop  (  - flag)
	false
	0 +byte $15 - if  exit  then
	1 +byte $81 - if  exit  then
	2 +byte $15 - if  exit  then
	3 +byte $81 - if  exit  then
	0= ;

: is_#and  (  - flag)
	false
	0 +byte $54 - if  exit  then
	2 +byte $53 - if  exit  then
	3 +byte $02 - if  exit  then
	0= ;

: is_#or  (  - flag)
	false
	0 +byte $44 - if  exit  then
	2 +byte $43 - if  exit  then
	3 +byte $02 - if  exit  then
	0= ;

: is_#xor  (  - flag)
	false
	0 +byte $64 - if  exit  then
	2 +byte $63 - if  exit  then
	3 +byte $02 - if  exit  then
	0= ;

: is_#lit  (  - flag)
	false
	0 +byte $74 - if  exit  then
	2 +byte $7a - if  exit  then
	0= ;

0 [if]
: is_does  (  - flag)
	false
	0 +byte $18 - if  exit  then
	1 +byte $a6 - if  exit  then
	2 +byte $02 - if  exit  then
	3 +byte $18 - if  exit  then
	4 +byte $f6 - if  exit  then
	5 +byte $d0 - if  exit  then
	6 +byte $d0 - if  exit  then
	0= ;
[then]

: show-literal  (  - )
	1 +byte 4 +byte 256 * or >put
	[t'] u. >execute ;

: show-LIT  (  - )
	1 +byte 3 +byte 256 * or >put
	[t'] u. >execute ;

: (word+stack)  (  - )
	target-.s clrtoeol cr
	max-name-length dots 0 bottom-line at-xy
	ip-t @ branch-point?   if  ." branch  "        exit  then
	this-instruction $22 = if  ." exit  "          exit  then
	is_1-dup0=until        if  ." 1-dup0=until  "  exit  then
	is_dupuntil            if  ." dupuntil  "      exit  then
	is_dupif               if  ." dupif  "         exit  then
	is_dupnotuntil         if  ." dupnotuntil  "    exit  then
	is_dupnotif            if  ." dupnotif  "       exit  then
	is_dup0<until          if  ." dup0<until  "    exit  then
	is_dup0<if             if  ." dup0<if  "       exit  then
	is_dup0<notuntil       if  ." dup0<notuntil  " exit  then
	is_dup0<notif          if  ." dup0<notif  "    exit  then
	is_dup>r               if  ." dup>r  "         exit  then
	is_r>drop              if  ." r>drop  "        exit  then
	is_#and                if  show-literal ." #and  "   exit  then
	is_#or                 if  show-literal ." #or  "    exit  then
	is_#xor                if  show-literal ." #xor  "   exit  then
	is_#lit                if  show-LIT     ." #lit  "   exit  then
	ip-t @ address@  ( addr flag)
	over does? if  2drop ." does>  " exit  then
	case
		-1 of  .label drop  endof
		 1 of  .label drop ." ; "  endof
		 ." Can't single step this " abort
	endcase ;

: word+stack  (  - )
	adjust-scrolling-offset (word+stack)
	ip-t @ address@ drop does? if
		ip-t @ address@ drop find-source
	then
	.source
	max-name-length bottom-line at-xy ;

: .XXXX  ( n - )
	base @ >r hex 0 <# # # # # #> type space r> base ! ;

: maybe-branch  (  - flag)
	ip-t @ branch-point? dup not if  exit  then
\ SJMP  (branch)
	this-instruction $80 = if
		ip-t @ 1+ c@-t ip-t +!
		word+stack exit
	then
\ AJMP  (branch)
	this-instruction $1f and $01 = if
		ip-t @ absolute-address@ ip-t !
		word+stack exit
	then
\ LJMP  (branch)	
	this-instruction $02 = if
		ip-t @ 1+ @-t ip-t !
		word+stack exit
	then
	;	

: handle-EXIT   (  - )
	nested @ 0= if  true to final-exit exit  then
	pop-ns dup 'cfa ! find-source
	remote-R> >get dup ip-t ! 0= if  true to final-exit  then
	nested-colors ." <--Unnesting" space
	re.instructions -1 nested +! word+stack ;

: handle-LJMP  (  - )
	ip-t @ dup 1+ @-t swap
	exit-point? if  >execute handle-EXIT exit  then
	ip-t ! word+stack ;
	
: handle-AJMP  (  - )
	ip-t @ dup absolute-address@
	swap exit-point? if  >execute handle-EXIT exit  then
	ip-t ! word+stack ;

: put-LIT  ( n - )
	ip-t +! ip-t @ @-t >put 2 ip-t +! word+stack ;

: put-?BRANCH  ( n - )
	ip-t +! >get if
		2 ip-t +!
	else	ip-t @ @-t ip-t !
	then	word+stack ;

: put-(NEXT)  ( n - )
	ip-t +! remote-R> >get 1 - dup 0= if
		drop 2 ip-t +!
	else	>put remote->R ip-t @ @-t ip-t !
	then	word+stack ;

: put-(STRING)  ( n - )
	ip-t +! ip-t @ >put [t'] count >execute
	ip-t @ c@-t 1 + ip-t +! word+stack ;

: put-(EXEC:)  ( n - )
	ip-t +! >get 3 * ip-t +! word+stack ;

: handle-LCALL  (  - )
	ip-t @ 1+ @-t
	case
		[hidden'] LIT      of  3 put-LIT       endof
		[hidden'] ?BRANCH  of  3 put-?BRANCH   endof
		[hidden'] (next)   of  3 put-(NEXT)    endof
		[hidden'] (string) of  3 put-(STRING)  endof
		[t'] (exec:)       of  3 put-(EXEC:)   endof
		dup >execute 3 ip-t +! word+stack
	endcase
	;

: handle-ACALL  (  - )
	ip-t @ absolute-address@
	case
		[hidden'] LIT      of  2 put-LIT       endof
		[hidden'] ?BRANCH  of  2 put-?BRANCH   endof
		[hidden'] (next)   of  2 put-(NEXT)    endof
		[hidden'] (string) of  2 put-(STRING)  endof
		[t'] (exec:)       of  2 put-(EXEC:)   endof
		dup >execute 2 ip-t +! word+stack
	endcase
	;

: handle-SJMP  (  - ) ip-t @ 1+ c@-t ip-t +! word+stack ;

: sign_extend  ( c - n) dup $80 and if  $ffffff00 or  then ;

: jump-relative  ( i - ) +byte sign_extend ip-t +! ;

: handle_1-dup0=until  (  - )
	3 ip-t +! >get dup $ff00 and swap $00ff and 1 - tuck or >put if
		-1 jump-relative
	then	word+stack ;

: handle_dupif  (  - )
	8 ip-t +! >get dup >put 0= if  -1 jump-relative  then
	word+stack ;

: handle_dupnotif  (  - )
	6 ip-t +! >get dup >put if  -1 jump-relative  then
	word+stack ;

: handle_dup0<if  (  - )
	5 ip-t +! >get dup >put
	$8000 and 0= if  -1 jump-relative  then
	word+stack ;

: handle_dup0<notif  (  - )
	5 ip-t +! >get dup >put
	$8000 and if  -1 jump-relative  then
	word+stack ;

: handle_dup>r  (  - )
	4 ip-t +! [t'] dup >execute remote->R word+stack ;

: handle_r>drop  (  - )
	4 ip-t +! remote-R> [t'] drop >execute word+stack ;

: handle_#and  (  - )
	5 ip-t +!
	-4 +byte -1 +byte 256 * or >put
	[t'] and >execute word+stack ;

: handle_#or  (  - )
	5 ip-t +!
	-4 +byte -1 +byte 256 * or >put
	[t'] or >execute word+stack ;

: handle_#xor  (  - )
	5 ip-t +!
	-4 +byte -1 +byte 256 * or >put
	[t'] xor >execute word+stack ;

: handle_#lit  (  - )
	4 ip-t +!
	[t'] drop >execute
	-3 +byte -1 +byte 256 * or >put
	word+stack ;

: handle_does  (  - )
	2 ip-t @ c@-t $12 = if  1 +  then  ip-t @ + >put
	ip-t @ address@ drop
	9 + ( dup .) ip-t ! word+stack ;

: one-step  (  - )
	maybe-branch if  exit  then
	ip-t @ address@ drop does? if  handle_does exit  then
	this-instruction $02 = if	  handle-LJMP   exit  then
	this-instruction $12 = if	  handle-LCALL  exit  then
	this-instruction $1f and $01 = if handle-AJMP   exit  then
	this-instruction $1f and $11 = if handle-ACALL  exit  then
	this-instruction $22 = if   	  handle-EXIT   exit  then
	this-instruction $80 = if	  handle-SJMP   exit  then
	is_1-dup0=until        if  handle_1-dup0=until  exit  then
	is_dupif               if  handle_dupif         exit  then
	is_dupnotif            if  handle_dupnotif      exit  then
	is_dup0<if             if  handle_dup0<if       exit  then
	is_dup0<notif          if  handle_dup0<notif    exit  then
	is_dup>r               if  handle_dup>r         exit  then
	is_r>drop              if  handle_r>drop        exit  then
	is_#and                if  handle_#and          exit  then
	is_#or                 if  handle_#or           exit  then
	is_#xor                if  handle_#xor          exit  then
	is_#lit                if  handle_#lit          exit  then
	true abort" Can't call this word"
	;
	
: step-continuous  (  - )
	2 which-instructions !
	begin	ip-t @ exit-point? if
			re.instructions exit
		then
		one-step key? until
	re.instructions ;

: refuse-nest  (  - ) ." Unable to nest into this word " cr ;

: simulated-nest  (  - )
	ip-t @ address@ 0= if  drop refuse-nest exit  then
	'cfa @ push-ns
	next-address >put remote->R  dup ip-t ! find-source 1 nested +!
	nested-colors ." <--Nesting " word+stack ;

variable 'one-step  \ A forward reference.

: simulated-unnest   (  - )
\ Steps quickly to the next EXIT
	1 which-instructions !
	begin	ip-t @ exit-point? not while
		one-step key? if
			re.instructions exit
		then
	repeat  re.instructions cr ;

: do-forth  (  - )
	\ .forth-instructions
	3 which-instructions !
        begin   word+stack ( target-.s) ." Interpreting: "
		pad 128 blank  s" please " pad place
		pad count + 80 accept dup while
		>r  pad count r> + evaluate cr
        repeat
	drop re.instructions word+stack ;

: refresh-display  (  - ) 'cfa @ prepare-source-code word+stack ;

: act  ( c - )
        toupper
	dup [char] C = if  drop step-continuous       exit  then
	dup [char] F = if  drop do-forth              exit  then
	dup [char] N = if  drop simulated-nest        exit  then
	dup [char] U = if  drop simulated-unnest      exit  then
	dup [char] R = if  drop refresh-display       exit  then
        dup [char] Q = if  drop cr true to final-exit exit  then
        dup 13 = over BL = or if  drop one-step       exit  then
	DROP ;

: debug-t   ( a - )
	ip-t !
	\ singlestepper-status
	max-name-length bottom-line at-xy
	word+stack false to final-exit
        begin
		key act final-exit
        until	target-.s
\	target-status
	;

: step  (  - )
	page 2 bottom scroll-window
	cr hello-singlestepper
\	singlestepper-status
	hello-singlestepper
	init-simulator-stacks  nested off
	also symbols ' previous execute
	dup 'cfa ! dup push-ns dup prepare-source-code
	cr debug-t
\	welcome-interpreter target-status
	alone? if  reset-scrolling 0 bottom at-xy  then
	hello-forth-interpreter
	;

