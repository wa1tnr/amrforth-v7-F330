\ end8051.fs
0 [if]   end8051.fs   Final patches for the amrForth virtual machine.
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

in-forth

has tiny-program [if]  \s  [then]

\ .( loading end8051.fs ) cr

in-meta

\ has cygnal-chip [if]
\     a: init-page0  (  - )
\         0 org  $200 ljmp  \ reset vector to page 1.
\         $200 $03 do  i org  i $200 + ljmp  8 +loop ;a
\ [then]

\ Remember where the end of the rom dictionary is
label 'end

\ Patch the interpreter or application into ABORT
	'boot org
romming [if]   ' go   [else]   ' quit   [then]  ljmp

\ Patch COLD into the reset vector
	0
has cygnal-chip [if]
	$200 +  \ Skip over the bootloader in page 0, Cygnal parts.
\	init-page0  \ In case JTAG downloader used.
[then]
has bootloader-installed not romming not and [if]
	$8000 +  \ For the old boards.
[then]
        org
	cold ljmp
	c;

\ Restore the rom dictionary pointer
'end org

