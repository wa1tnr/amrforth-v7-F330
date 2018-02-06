\ sfr-f061.fs  Cygnal C8051F061.
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

in-assembler

\ --------------

\ $80 sfr P0    \ all pages

\ $81 sfr SP    \ all pages

\ $82 sfr DPL   \ all pages

\ $83 sfr DPH   \ all pages

$84 sfr SFRPAGE \ all pages

$85 sfr SFRNEXT \ all pages

$86 sfr SFRLAST \ all pages

\ $87 sfr PCON  \ all pages

\ --------------

\ $88 sfr TCON  \ pg 0
$88 sfr CPT0CN  \ pg 1
$88 sfr CPT1CN  \ pg 2
$88 sfr CPT2CN  \ pg 3

\ $89 sfr TMOD  \ pg 0
$89 sfr CPT0MD  \ pg 1
$89 sfr CPT1MD  \ pg 2
$89 sfr CPT2MD  \ pg 3

\ $8a sfr TL0   \ pg 0
$8a sfr OSCICN  \ pg f

\ $8b sfr TL1   \ pg 0
$8b sfr OSCICL  \ pg f

\ $8c sfr TH0   \ pg 0
$8c sfr OSCXCN  \ pg f

\ $8d sfr TH1   \ pg 0

$8e sfr CKCON   \ pg 0

$8f sfr PSCTL   \ pg 0

\ --------------

\ $90 sfr P1    \ all pages

$91 sfr SSTA0   \ pg 0

$96 sfr SFRPGCN \ pg f

$97 sfr CLKSEL  \ pg f

\ --------------

$98 sfr SCON0  $98 isBIT .SCON0   \ pg 0
$98 sfr SCON1  $98 isBIT .SCON1   \ pg 1

$99 sfr SBUF0   \ pg 0
$99 sfr SBUF1   \ pg 1

$9a sfr SPI0CFG \ pg 0

$9b sfr SPI0DAT \ pg 0

$9c sfr P4MDOUT \ pg f

$9d sfr SPI0CKR \ pg 0
$9d sfr P5MDOUT \ pg f

$9e sfr P6MDOUT \ pg f

$9f sfr P7MDOUT \ pg f

\ --------------

\ $a0 sfr P2      \ all pages

$a1 sfr EMI0TC  \ pg 0

$a2 sfr EMI0CN  \ pg 0

$a3 sfr EMI0CF  \ pg 0

$a4 sfr P0MDOUT \ pg f

$a5 sfr P1MDOUT \ pg f

$a6 sfr P2MDOUT \ pg f

$a7 sfr P3MDOUT \ pg f

\ --------------

\ $a8 sfr IE      \ all pages

$a9 sfr SADDR0  \ pg 0

$ad sfr P1MDIN  \ pg f

$ae sfr P2MDIN  \ pg f

\ --------------

\ $b0 sfr P3    \ all pages

$b7 sfr FLSCL   \ pg 0
$b7 sfr FLACL   \ pg f

\ --------------

\ $b8 sfr IP    \ all pages

$b9 sfr SADEN0  \ pg 0

$ba sfr AMX2CF  \ pg 2
$ba sfr ADC0CPT \ pg f

$bb sfr AMX0SL  \ pg 0
$bb sfr AMX2SL  \ pg 2
$bb sfr ADC0CCF \ pg f

$bc sfr ADC0CF  \ pg 0
$bc sfr ADC1CF  \ pg 1
$bc sfr ADC2CF  \ pg 2

$be sfr ADC0L   \ pg 0
$be sfr ADC1L   \ pg 1
$be sfr ADC2L   \ pg 2

$bf sfr ADC0H   \ pg 0
$bf sfr ADC1H   \ pg 1
$bf sfr ADC2H   \ pg 2

\ --------------

$c0 sfr SMB0CN	$c0 isBIT .SMB0CN   \ pg 0
$c0 sfr CAN0STA $c0 isBIT .CAN0STA  \ pg 1

$c1 sfr SMB0STA \ pg 0

$c2 sfr SMB0DAT \ pg 0

$c3 sfr SMB0ADR \ pg 0

$c4 sfr ADC0GTL \ pg 0
$c4 sfr ADC2GTL \ pg 2

$c5 sfr ADC0GTH \ pg 0
$c5 sfr ADC2GTH \ pg 2

$c6 sfr ADC0LTL \ pg 0
$c6 sfr ADC2LTL \ pg 2

$c7 sfr ADC0LTH \ pg 0
$c7 sfr ADC2LTH \ pg 2

\ --------------

$c8 sfr TMR2CN  $c8 isBIT .TMR2CN   \ pg 0
$c8 sfr TMR3CN  $c8 isBIT .TMR3CN   \ pg 1
$c8 sfr TMR4CN  $c8 isBIT .TMR4CN   \ pg 2
$c8 sfr P4      $c8 isBIT .P4       \ pg f

$c9 sfr TMR2CF  \ pg 0
$c9 sfr TMR3CF  \ pg 1
$c9 sfr TMR4CF  \ pg 2

$ca sfr RCAP2L  \ pg 0
$ca sfr RCAP3L  \ pg 1
$ca sfr RCAP4L  \ pg 2

$cb sfr RCAP2H  \ pg 0
$cb sfr RCAP3H  \ pg 1
$cb sfr RCAP4H  \ pg 2

$cc sfr TMR2L   \ pg 0
$cc sfr TMR3L   \ pg 1
$cc sfr TMR4L   \ pg 2

$cd sfr TMR2H   \ pg 0
$cd sfr TMR3H   \ pg 1
$cd sfr TMR4H   \ pg 2

$cf sfr SMB0CR  \ pg 0

\ --------------

\ $d0 sfr PSW   \ all pages

$d1 sfr REF0CN  \ pg 0
$d1 sfr REF1CN  \ pg 1
$d1 sfr REF2CN  \ pg 2

$d2 sfr DAC0L   \ pg 0
$d2 sfr DAC1L   \ pg 1

$d3 sfr DAC0H   \ pg 0
$d3 sfr DAC1H   \ pg 1

$d4 sfr DAC0CN  \ pg 0
$d4 sfr DAC1CN  \ pg 1

\ --------------

$d8 sfr PCA0CN	$d8 isBIT .PCA0CN   \ pg 0
$d8 sfr CAN0DATL $d8 isBIT .CAN0DATL \ pg 1
$d8 sfr DMA0CN  $d8 isBIT .DMA0CN   \ pg 3
$d8 sfr P5      $d8 isBIT .P5       \ pg f

$d9 sfr PCA0MD      \ pg 0
$d9 sfr CAN0DATH    \ pg 1
$d9 sfr DMA0DAL     \ pg 3

$da sfr PCA0CPM0    \ pg 0
$da sfr CAN0ADR     \ pg 1
$da sfr DMA0DAH     \ pg 3

$db sfr PCA0CPM1    \ pg 0
$db sfr CAN0TST     \ pg 1
$db sfr DMA0DSL     \ pg 3

$dc sfr PCA0CPM2    \ pg 0
$dc sfr DMA0DSH     \ pg 3

$dd sfr PCA0CPM3    \ pg 0
$dd sfr DMA0IPT     \ pg 3

$de sfr PCA0CPM4    \ pg 0
$de sfr DMA0IDT     \ pg 3

$df sfr PCA0CPM5    \ pg 0

\ --------------

\ $e0 sfr ACC   \ all pages

$e1 sfr PCA0CPL5 \ pg 0
$e1 sfr XBR0    \ pg f

$e2 sfr PCA0CPH5 \ pg 0
$e2 sfr XBR1    \ pg f

$e3 sfr XBR2    \ pg f    

$e4 sfr XBR3    \ pg f    

$e6 sfr EIE1    \ all pages

$e7 sfr EIE2    \ all pages

\ --------------

$e8 sfr ADC0CN	$e8 isBIT .ADC0CN   \ pg 0
$e8 sfr ADC1CN	$e8 isBIT .ADC1CN   \ pg 1
$e8 sfr ADC2CN	$e8 isBIT .ADC2CN   \ pg 2
$e8 sfr P6      $e8 isBIT .P6       \ pg f

$e9 sfr PCA0CPL2    \ pg 0

$ea sfr PCA0CPH2    \ pg 0

$eb sfr PCA0CPL3    \ pg 0

$ec sfr PCA0CPH3    \ pg 0

$ed sfr PCA0CPL4    \ pg 0

$ee sfr PCA0CPH4    \ pg 0

$ef sfr RSTSRC      \ pg 0

\ --------------

\ $f0 sfr B     \ all pages

$f6 sfr EIP1    \ all pages

$f7 sfr EIP2    \ all pages

\ --------------

$f8 sfr SPI0CN	$f8 isBIT .SPI0CN   \ pg 0
$f8 sfr CAN0CN	$f8 isBIT .CAN0CN   \ pg 1
$f8 sfr DMA0CF	$f8 isBIT .DMA0CF   \ pg 3
$f8 sfr P7  	$f8 isBIT .P7       \ pg f

$f9 sfr PCA0L       \ pg 0
$f9 sfr DMA0CTL     \ pg 3

$fa sfr PCA0H       \ pg 0
$fa sfr DMA0CTH     \ pg 3

$fb sfr PCA0CPL0    \ pg 0
$fb sfr DMA0CSL     \ pg 3

$fc sfr PCA0CPH0    \ pg 0
$fc sfr DMA0CSH     \ pg 3

$fd sfr PCA0CPL1    \ pg 0
$fd sfr DMA0BND     \ pg 3

$fe sfr PCA0CPH1    \ pg 0
$fe sfr DMA0ISW     \ pg 3

$ff sfr WDTCN       \ all pages

\ --------------

