0 [if]   sfr-C51RC2.fs   Special Function Registers for the Atmel.
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

\ ------------------- AT89C51RC2 Enhancements ---------------------
IN-ASSEMBLER

\ Bit addressable Special Function Registers.
$a8 SFR IEN0    $a8 isBIT .IEN0
$b8 SFR IPL0    $b8 isBIT .IPL0
$c8 SFR T2CON   $c8 isBIT .T2CON
$d8 SFR CCON    $d8 isBIT .CCON

\ Not bit addressable Special Function Registers.
$8e SFR AUxr    $8f SFR CKCON0  $97 SFR CKRL
$9a SFR BRL     $9b SFR BDRCON  $9c SFR KBLS
$9d SFR KBE     $9e SFR KBF
$a2 SFR AUXR1   $a6 SFR WDTRST  $a7 SFR WDTPRG
$a9 SFR SADDR   $af SFR CKCON1
$b1 SFR IEN1    $b2 SFR IPL1    $b3 SFR IPH1
$b7 SFR IPH0    $b9 SFR SADEN   
$cb SFR SPCON   $cc SFR SPSTA   $cd SFR SPDAT   $c9 SFR T2MOD
$ca SFR RCAP2L  $cb SFR RCAP2H  $cc SFR TL2     $cd SFR TH2
$d1 SFR FCON    $d9 SFR CMOD    $da SFR CCAPM0  $db SFR CCAPM1
$dc SFR CCAPM2  $dd SFR CCAPM3  $de SFR CCAPM4
$e9 SFR CL      $ea SFR CCAP0L  $eb SFR CCAP1L
$ec SFR CCAP2L  $ed SFR CCAP3L  $ee SFR CCAP4L
$f9 SFR CH      $fa SFR CCAP0H  $fb SFR CCAP1H
$fc SFR CCAP2H  $fd SFR CCAP3H  $fe SFR CCAP4H

IN-META

