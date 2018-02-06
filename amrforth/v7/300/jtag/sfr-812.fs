0 [if]   sfr-812.fs   Special Function Registers for the ADuC812.
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
$D8 sfr ADCCON2 $D8 isBIT .ADCCON2
$E8 sfr I2CCON  $E8 isBIT .I2CCON
$F8 sfr SPICON  $F8 isBIT .SPICON

\ Not bit addressable Special Function Registers.
$84 sfr DPP
$9A sfr I2CDAT  $9B sfr I2CADD
$A9 sfr IE2
$B9 sfr ECON    $BA sfr ETIM1   $BB sfr ETIM2
$BC sfr EDATA1  $BD sfr EDATA2  $BE sfr EDATA3  $BF sfr EDATA4
$C4 sfr ETIM3   $C6 sfr EADRL
$CA sfr RCAP2L  $CB sfr RCAP2H  $CC sfr TL2     $CD sfr TH2
$D2 sfr DMAL    $D3 sfr DMAH    $D4 sfr DMAP
$D9 sfr ADCDATAL        $DA sfr ADCDATAH        $DF sfr PSMCON
$EF sfr ADCCON1
$F1 sfr ADCOFSL $F2 sfr ADCOFSH $F3 sfr ADCGAINL $F4 sfr ADCGAINH
$F5 sfr ADCCON3 $F7 sfr SPIDAT
$F9 sfr DAC0L   $FA sfr DAC0H   $FB sfr DAC1L   $FC sfr DAC1H
$FD sfr DACCON

in-meta

