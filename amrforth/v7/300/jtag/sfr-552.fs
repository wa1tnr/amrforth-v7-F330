0 [if]   sfr-552.fs   Special Function Registers for the 80c552.
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

\ ----- 8xC552 Enhancements ----- /
IN-ASSEMBLER

nowarn
\ Bit addressable Special Function Registers.
$98 SFR S0CON   $98 isBIT .S0CON
$A8 SFR IEN0    $A8 isBIT .IEN0
$B8 SFR IP0     $B8 isBIT .IP0
$C0 SFR P4      $C0 isBIT .P4
$C8 SFR TM2IR   $C8 isBIT .TM2IR
$D8 SFR S1CON   $D8 isBIT .S1CON
$E8 SFR IEN1    $E8 isBIT .IEN1
$F8 SFR IP1     $F8 isBIT .IP1

\ Not bit addressable Special Function Registers.
$99 SFR S0BUF   $A9 SFR CML0    $AA SFR CML1    $AB SFR CML2
$AC SFR CTL0    $AD SFR CTL1    $AE SFR CTL2    $AF SFR CTL3
$C4 SFR P5      $C5 SFR ADCON   $C6 SFR ADCH
$C9 SFR CMH0    $CA SFR CMH1    $CB SFR CMH2
$CC SFR CTH0    $CD SFR CTH1    $CE SFR CTH2    $CF SFR CTH3
$D9 SFR S1STA   $DA SFR S1DAT   $DB SFR S1ADR
$EA SFR TM2CON  $EB SFR CTCON   $EC SFR TML2    $ED SFR TMH2
$EE SFR STE     $EF SFR RTE
$FC SFR PWM0    $FD SFR PWM1    $FE SFR PWMP    $FF SFR T3
warn

IN-META

