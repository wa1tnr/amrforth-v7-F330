0 [if]   sfr-32.fs   Special Function Registers for the 8032.
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

\ ------------------------ 80C32/52 Enhancements --------------------------
IN-ASSEMBLER

\ Bit addressable Special Function Registers.
$C8 SFR T2CON   $C8 isBIT .T2CON

\ Not bit addressable Special Function Registers.
$CA SFR RCAP2L  $CB SFR RCAP2H   $CC SFR TL2     $CD SFR TH2

IN-META

