0 [if]   sfr-816.fs   Special Function Registers for the ADuC816.
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

in-assembler

\ Bit addressable Special Function Registers.
$C0 sfr WDCON   $C0 isBIT .WDCON
$C8 sfr T2CON   $C8 isBIT .T2CON
$D8 sfr ADCSTAT $D8 isBIT .ADCSTAT
$E8 sfr I2CCON  $E8 isBIT .I2CCON
$F8 sfr SPICON  $F8 isBIT .SPICON

\ Not bit addressable Special Function Registers.
$84 sfr DPP
$9A sfr I2CDAT  $9B sfr I2CADD
$A1 sfr TIMECON $A2 sfr HTHSEC  $A3 sfr SEC     $A4 sfr MIN
$A5 sfr HOUR	$A6 sfr INTVAL
$A9 sfr IEIP2
$B9 sfr ECON
$BC sfr EDATA1  $BD sfr EDATA2  $BE sfr EDATA3  $BF sfr EDATA4
$C2 sfr CHIPID  $C6 sfr EADRL
$CA sfr RCAP2L  $CB sfr RCAP2H  $CC sfr TL2     $CD sfr TH2
$D1 sfr ADCMODE $D2 sfr ADC0CON $D3 sfr ADC1CON
$D4 sfr SF	$D5 sfr ICON	$D7 sfr PLLCON
$DA sfr ADC0M	$DB sfr ADC0H
$DC sfr ADC1L	$DD sfr ADC1H	$DF sfr PSMCON
$E2 sfr OF0M	$E3 sfr OF0H	$E4 sfr OF1L	$E5 sfr OF1H
$EA sfr GN0M	$EB sfr GN0H	$EC sfr GN1L	$ED sfr GN1H
$F7 sfr SPIDAT
$FB sfr DACL	$FC sfr DACH	$FD sfr DACCON

in-meta
