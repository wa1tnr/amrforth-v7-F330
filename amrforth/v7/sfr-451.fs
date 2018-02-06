0 [if]   sfr-451.fs   Special Function Registers for the 80c451.
Copyright (C) 2001 by AM Research, Inc.

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

\ ------------------------ 8xC451 Enhancements ----------------------------
IN-ASSEMBLER

\ Bit addressable Special Function Registers.
$C0 SFR P4      $C0 isBIT .P4
$C8 SFR P5      $C8 isBIT .P5
$D8 SFR P6      $D8 isBIT .P6
$E8 SFR CSR     $E8 isBIT .CSR

IN-META

