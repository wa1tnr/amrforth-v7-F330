0 [if]   sfr-cygnal.fs   Special Function Registers for the Cygnal parts.
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
in-assembler

\ Bit addressable Special Function Registers.
$C8 SFR T2CON   $C8 isBIT .T2CON

\ Not bit addressable Special Function Registers.
$CA SFR RCAP2L  $CB SFR RCAP2H   $CC SFR TL2     $CD SFR TH2

in-meta

$d8 sfr PCA0CN  $d8 isBIT .PCA0CN
$d9 sfr PCA0MD

$da sfr PCA0CPM0  $db sfr PCA0CPM1  $dc sfr PCA0CPM2
                  $dd sfr PCA0CPM3  $de sfr PCA0CPM4
$ea sfr PCA0CPL0  $eb sfr PCA0CPL1  $ec sfr PCA0CPL2
                  $ed sfr PCA0CPL3  $ee sfr PCA0CPL4
$e9 sfr PCA0L
$fa sfr PCA0CPH0  $fb sfr PCA0CPH1  $fc sfr PCA0CPH2
                  $fd sfr PCA0CPH3  $fe sfr PCA0CPH4
$f9 sfr PCA0H

$e1 sfr XBR0
$e2 sfr XBR1
$e3 sfr XBR2

$e6 sfr EIE1

$a4 sfr PRT0CF
$a5 sfr PRT1CF

$b2 sfr OSCICN  $b1 sfr OSCXCN
$8e sfr CKCON
$ff sfr WDTCN

