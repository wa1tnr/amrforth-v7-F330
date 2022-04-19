\ colorscheme.fs

0 [if]   colorscheme.fs  Central definitions for the colorscheme.
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

include strings.fs
include ansi.fs

create windows-string
windows [if]
	," -e 'true constant windows' "
	: myeditor  (  - ) s" gvim " pad place ;
	: gforth-string  (  - addr len)
		s" ./gforth -m 1M " pad place ;
[else]	," -e 'false constant windows' "
	: myeditor  (  - ) s" vim " pad place ;
	: gforth-string  (  - addr len)
		s" ./gforth -m 1M " pad place ;
[then]

: escape-quotes  ( addr len - addr' len')
	pad c! pad count 0 do
		over c@ dup [char] " = if
			over [char] \ swap c! 1 under+
			pad c@ 1 + pad c!
		then
		over c! 1 + 1 under+
	loop	2drop pad count ;

: bottom  (  - n) rows windows 0= if  1 -  then ;

: default-colors  (  - )
	text_normal black background white foreground clrtoeol ;
: disasm-colors  (  - ) yellow foreground text_bold clrtoeol ;
: basic-colors  (  - ) yellow foreground text_bold ;
: interpreter-colors  (  - ) green foreground text_bold clrtoeol ;
: target-colors  (  - ) red foreground ;
: label-colors  (  - ) red foreground ;
: opcode-colors  (  - ) cyan foreground ;
: return-stack-colors  (  - ) red foreground ;
: data-stack-colors  (  - ) cyan foreground ;
: nested-colors  (  - ) yellow foreground ;
: source-colors  (  - ) cyan foreground ;

: >highlight  (  - ) red background white foreground ;
: >norm  (  - ) black background cyan foreground ;

: >red+white  (  - ) red background white foreground ;
: red+white>  (  - ) clrtoeol >norm cr ;

: top-line  ( addr len - )
	0 0 at-xy blue background cyan foreground text_bold
	type clrtoeol ;
	
: bottom-line  ( addr len - )
	0 bottom at-xy clrtoeol
	0 bottom 1 + at-xy red background white foreground text_bold
	." amrFORTH v7.1.0_beta " type space
	processor type space baudrate . ." baud "
	windows if  ." COM" com?  else  ." /dev/ttyS" com? 1 -  then
	. space clrtoeol ;

: hello  ( addr1 len1 addr2 len2 - )
	save_cursor top-line bottom-line restore_cursor default-colors ;

: hello-system  (  - )
	s" HOST"
s" <c>ompile <d>ownload turnkey <f>orth <b>asic config see jtag c2 help bye"
	hello ;

: hello-compiler  (  - )
	s" COMPILER"
	s" "
	hello ;

: hello-turnkey  (  - )
	s" TURNKEY COMPILER"
	s" "
	hello ;

: hello-downloader  (  - )
	s" DOWNLOADER"
	s" "
	hello ;

: hello-forth-interpreter  (  - )
	s" FORTH interpreter"
	s" see step words bye"
	hello ;

: hello-basic-interpreter  (  - )
	s" BASIC interpreter"
	s" let print gosub pause wait high low words bye"
	hello ;

: hello-decompiler  (  - )
	s" DECOMPILER/DISASSEMBLER"
	s" Press q to quit disassembling, any other key to continue."
	page cr hello ;

: hello-singlestepper  (  - )
	s" SINGLESTEPPER"
	s"  "
	hello ;

: hello-config  (  - )
	s" CONFIG"
	s"  "
	hello ;

: hello-jtag  (  - )
	s" JTAG"
	s" download erase dump next reset run halt suspend bye"
	hello ;

: hello-c2  (  - )
	s" C2"
	s" download erase dump next bye"
	hello ;

