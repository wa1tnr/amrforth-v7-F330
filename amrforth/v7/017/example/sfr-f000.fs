\ sfr-f000.fs  Cygnal C8051F000,015,017
0 [if]
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

\ .( Loading sfr-f000.fs) cr

in-assembler

\ $80 sfr P0
\ $81 sfr SP
\ $82 sfr DPL
\ $83 sfr DPH
\ $8f sfr PCON

\ $88 sfr TCON
\ $89 sfr TMOD
\ $8a sfr TL0
\ $8b sfr TL1
\ $8c sfr TH0
\ $8d sfr TH1
$8e sfr CKCON
$8f sfr PSCTL

\ $90 sfr P1
$91 sfr TMR3CN
$92 sfr TMR3RLL
$93 sfr TMR3RLH
$94 sfr TMR3L
$95 sfr TMR3H

\ $98 sfr SCON
\ $99 sfr SBUF
$9a sfr SPI0CFG
$9b sfr SPI0DAT
$9d sfr SPI0CKR
$9e sfr CPT0CN
$9f sfr CPT1CN

\ $a0 sfr P2
$a4 sfr PRT0CF
$a5 sfr PRT1CF
$a6 sfr PRT2CF
$a7 sfr PRT3CF

\ $a8 sfr IE
$ad sfr PRT1IF
$af sfr EMI0CN

\ $b0 sfr P3
$b1 sfr OSCXCN
$b2 sfr OSCICN
$b6 sfr FLSCL
$b7 sfr FLACL

\ $b8 sfr IP
$ba sfr AMX0CF
$bb sfr AMX0SL
$bc sfr ADC0CF
$be sfr ADC0L
$bf sfr ADC0H

$c0 sfr SMB0CN	$c0 isBIT .SMB0CN
$c1 sfr SMB0STA
$c2 sfr SMB0DAT
$c3 sfr SMB0ADR
$c4 sfr ADC0GTL
$c5 sfr ADC0GTH
$c6 sfr ADC0LTL
$c7 sfr ADC0LTH

$c8 sfr T2CON	$c8 isBIT .T2CON
$ca sfr RCAP2L
$cb sfr RCAP2H
$cc sfr TL2
$cd sfr TH2
$cf sfr SMB0CR

\ $d0 sfr PSW
$d1 sfr REF0CN
$d2 sfr DAC0L
$d3 sfr DAC0H
$d4 sfr DAC0CN
$d5 sfr DAC1L
$d6 sfr DAC1H
$d7 sfr DAC1CN

$d8 sfr PCA0CN	$d8 isBIT .PCA0CN
$d9 sfr PCA0MD
$da sfr PCA0CPM0
$db sfr PCA0CPM1
$dc sfr PCA0CPM2
$dd sfr PCA0CPM3
$de sfr PCA0CPM4

\ $e0 sfr ACC
$e1 sfr XBR0
$e2 sfr XBR1
$e3 sfr XBR2
$e6 sfr EIE1
$e7 sfr EIE2

$e8 sfr ADC0CN	$e8 isBIT .ADC0CN
$e9 sfr PCA0L
$ea sfr PCA0CPL0
$eb sfr PCA0CPL1
$ec sfr PCA0CPL2
$ed sfr PCA0CPL3
$ee sfr PCA0CPL4
$ef sfr RSTSRC

\ $f0 sfr B
$f6 sfr EIP1
$f7 sfr EIP2

$f8 sfr SPI0CN	$f8 isBIT .SPI0CN
$f9 sfr PCA0H
$fa sfr PCA0CPH0
$fb sfr PCA0CPH1
$fc sfr PCA0CPH2
$fd sfr PCA0CPH3
$fe sfr PCA0CPH4
$ff sfr WDTCN

