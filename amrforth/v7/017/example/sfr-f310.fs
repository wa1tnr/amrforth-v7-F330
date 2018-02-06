\ sfr-f310.fs  Cygnal C8051F310
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

\ .( Loading sfr-f310.fs) cr

in-assembler

\ $80 sfr P0	$80 isBIT .P0
\ $81 sfr SP
\ $82 sfr DPL
\ $83 sfr DPH
\ $8f sfr PCON

\ $88 sfr TCON	$88 isBIT .TCON
\ $89 sfr TMOD
\ $8a sfr TL0
\ $8b sfr TL1
\ $8c sfr TH0
\ $8d sfr TH1
$8e sfr CKCON
$8f sfr PSCTL

\ $90 sfr P1	$90 isBIT .P1
$91 sfr TMR3CN
$92 sfr TMR3RLL
$93 sfr TMR3RLH
$94 sfr TMR3L
$95 sfr TMR3H

$98 sfr SCON0	$98 isBIT .SCON0
$99 sfr SBUF0
$9a sfr CPT1CN
$9b sfr CPT0CN
$9c sfr CPT1MD
$9d sfr CPT0MD
$9e sfr CPT1MX
$9f sfr CPT0MX

\ $a0 sfr P2	$a0 isBIT .P2
$a1 sfr SPI0CFG
$a2 sfr SPI0CKR
$a3 sfr SPI0DAT
$a4 sfr P0MDOUT
$a5 sfr P1MDOUT
$a6 sfr P2MDOUT
$a7 sfr P3MDOUT

\ $a8 sfr IE	$a8 isBIT .IE
$a9 sfr CLKSEL
$aa sfr EMI0CN

\ $b0 sfr P3	$b0 isBIT .P3
$b1 sfr OSCXCN
$b2 sfr OSCICN
$b3 sfr OSCICL
$b6 sfr FLSCL
$b7 sfr FLKEY

\ $b8 sfr IP	$b8 isBIT .IP
$ba sfr AMX0N
$bb sfr AMX0P
$bc sfr ADC0CF
$bd sfr ADC0L
$be sfr ADC0H

$c0 sfr SMB0CN	$c0 isBIT .SMB0CN
$c1 sfr SMB0CF
$c2 sfr SMB0DAT
$c4 sfr ADC0GTL
$c5 sfr ADC0GTH
$c6 sfr ADC0LTL
$c7 sfr ADC0LTH

$c8 sfr TMR2CN	$c8 isBIT .TMR2CN
$ca sfr TMR2RLL
$cb sfr TMR2RLH
$cc sfr TMR2L
$cd sfr TMR2H

\ $d0 sfr PSW	$d0 isBIT .PSW
$d1 sfr REF0CN
$d4 sfr P0SKIP
$d5 sfr P1SKIP
$d6 sfr P2SKIP

$d8 sfr PCA0CN	$d8 isBIT .PCA0CN
$d9 sfr PCA0MD
$da sfr PCA0CPM0
$db sfr PCA0CPM1
$dc sfr PCA0CPM2
$dd sfr PCA0CPM3
$de sfr PCA0CPM4

\ $e0 sfr ACC	$e0 isBIT .ACC
$e1 sfr XBR0
$e2 sfr XBR1
$e4 sfr IT01CF
$e6 sfr EIE1

$e8 sfr ADC0CN	$e8 isBIT .ADC0CN
$e9 sfr PCA0CPL1
$ea sfr PCA0CPH1
$eb sfr PCA0CPL2
$ec sfr PCA0CPH2
$ed sfr PCA0CPL3
$ee sfr PCA0CPH3
$ef sfr RSTSRC

\ $f0 sfr B	$f0 isBIT .B
$f1 sfr P0MDIN
$f2 sfr P1MDIN
$f3 sfr P2MDIN
$f4 sfr P3MDIN
$f6 sfr EIP1

\ $f8 sfr CPT0CN	$f8 isBIT .CPT0CN
$f8 sfr SPI0CN	$f8 isBIT .SPI0CN
$f9 sfr PCA0L
$fa sfr PCA0H
$fb sfr PCA0CPL0
$fc sfr PCA0CPH0
$fd sfr PCA0CPL4
$fe sfr PCA0CPH4
$ff sfr VDM0CN

