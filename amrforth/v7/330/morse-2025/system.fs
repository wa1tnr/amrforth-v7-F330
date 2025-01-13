\ system.fs

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

only forth also definitions
warnings off

include amrfconf.fs
include colorscheme.fs

: c  (  - )
	hello-compiler
	gforth-string s" -e false ./compile.fs -e bye" pad +place
	pad count system ;

: turnkey  (  - )
	hello-turnkey
	gforth-string s" -e true ./compile.fs -e bye" pad +place
	pad count system ;
: t  turnkey ;

: d  (  - )
	hello-downloader
	gforth-string s" ./amrfconf.fs " pad +place
	windows-string count pad +place
	s" -e 'downloader included'" pad +place
	pad count system ;

\ Forth interpreter.
: f  (  - )
	hello-forth-interpreter
	gforth-string windows-string count pad +place
	s" ./interpret.fs -e go" pad +place
	pad count system
	hello-system ;

\ Basic interpreter.
: b  (  - )
	hello-basic-interpreter
	gforth-string windows-string count pad +place
	s" ./basicinterpreter.fs -e go" pad +place
	pad count system
	hello-system ;

: jtag  (  - )
	hello-jtag
	gforth-string windows-string count pad +place
	s" ./jtag.fs -e quit" pad +place
	pad count system
	hello-system ;

: c2  (  - )
	hello-c2
	gforth-string windows-string count pad +place
	s" ./c2.fs -e quit" pad +place
	pad count system
	hello-system ;

: e  (  - )
	1 word count myeditor pad +place
	pad count system 
	hello-system ;

0 [if]
create argument 256 allot
: see  (  - )
	hello-decompiler
	BL word count escape-quotes argument place
	gforth-string windows-string count pad +place
	s\" ./dis8051.fs -e \"see " pad +place
	argument count pad +place
	s\"  bye\"" pad +place
	pad count system cr
	hello-system ;
[then]

: help  (  - )
	yellow foreground text_bold
	cr ." Available commands are:"
	cr ."   c = compile for debugging."
	cr ."   d = download object code to target."
	cr ."   turnkey = turnkey the application."
	cr ."   f = run forth interpreter on target."
	cr ."   b = run basic interpreter on target.  A BASIC program"
	cr ."       needs to have been compiled previously."
	cr ."   config = Choose processor, COM port, etc.
	cr ."   step <word> = single step <word> on target.  This must"
	cr ."       be run from the forth interpreter."
	cr ."   see <word> = disassemble/decompile <word>."
	cr ."   jtag = run the JTAG interpreter, for JTAG downloads."
	cr ."   c2 = run the C2 (JTAG) interpreter, for C2 downloads."
	cr ."   bye = exit to the operating system."
	cr ." Notes:"
	cr ."      The compiler words, c and turnkey, load the file named"
	cr ."   job.fs.  You should include the files of your application"
	cr ."   in job.fs.  When interpreting, green text comes from the"
	cr ."   host, red text comes from the target."
	cr ."      For more detailed help, point your web browser to"
	cr ."   http://www.amresearch.com on the web.  For local help,"
	cr ."   file:///amrforth/www.amresearch.com/v6 in Windows, and"
	cr ."   file://$HOME/amrforth/www.amresearch.com/v6 in Linux."
	cr ."   Documentation for v6 may not be up to date for v7."
	cr ."   Expect more specific v7 documentation as the release"
	cr ."   matures. "
	default-colors
	;
	
: restart  (  - )
	reset-scrolling text_normal page
	." You must restart amrforth to accept the new configuration."
	cr
	bye ;

: bye  (  - ) reset-scrolling text_normal page bye ;

: config   (  - )
	hello-config
	gforth-string s" ./config.fs" pad +place
	pad count system
	default-colors restart ;

: interpret-line  (  - )
	pad 84 blank pad 80 accept pad swap evaluate ." ok " ;

: clear-stack  (  - ) depth 0 ?do  drop  loop ;

: system-interpreter  (  - )
	cr hello-system
	begin
		['] interpret-line catch if
			clear-stack ." ?"
		then
		cr hello-system
	again ;

: bottom  (  - n) rows windows 0= if  1 -  then ;

warnings on
default-colors page
2 bottom scroll-window
system-interpreter
