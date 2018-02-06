0 [if]   sfr-537.fs   Special Function Registers for the 80c537.
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

\ ----- 8xC537 Enhancements ----- /
IN-ASSEMBLER

\ Bit addressable Special Function Registers.
$98 SFR S0CON   $98 isBIT .S0CON
$A8 SFR IEN0    $A8 isBIT .IEN0
$B8 SFR IEN1    $B8 isBIT .IEN1
$C0 SFR IRCON   $C0 isBIT .IRCON
$C8 SFR T2CON   $C8 isBIT .T2CON
$D8 SFR ADCON0  $D8 isBIT .ADCON0
$E8 SFR P4      $E8 isBIT .P4
$F8 SFR P5      $F8 isBIT .P5

\ Not bit addressable Special Function Registers.
$86 SFR WDTREL  $92 SFR DPSEL   $9A SFR IEN2
$9B SFR S1CON   $9C SFR S1BUF   $9D SFR S1REL   $99 SFR S0BUF
$A9 SFR IP0     $B9 SFR IP1
$C1 SFR CCEN    $C2 SFR CCL1    $C3 SFR CCH1
$C4 SFR CCL2    $C5 SFR CCH2    $C6 SFR CCL3    $C7 SFR CCH3
$C9 SFR CC4EN   $CA SFR CRCL    $CB SFR CRCH
$CC SFR TL2     $CD SFR TH2     $CE SFR CCL4    $CF SFR CCH4
$D2 SFR CML0    $D3 SFR CMH0    $D4 SFR CML1    $D5 SFR CMH1
$D6 SFR CML2    $D7 SFR CMH2    $D9 SFR ADDAT   $DC SFR ADCON1
$DA SFR DAPR    $DB SFR P7      $DD SFR P8
$DE SFR CTRELL  $DF SFR CTRELH  $E1 SFR CTCON
$E2 SFR CML3    $E3 SFR CMH3    $E4 SFR CML4    $E5 SFR CMH4
$E6 SFR CML5    $E7 SFR CMH5
$E9 SFR MD0     $EA SFR MD1     $EB SFR MD2     $EC SFR MD3
$ED SFR MD4     $EE SFR MD5     $EF SFR ARCON
$F2 SFR CML6    $F3 SFR CMH6    $F4 SFR CML7    $F5 SFR CMH7
$F6 SFR CMEN    $F7 SFR CMSEL   $FA SFR P6

IN-META

