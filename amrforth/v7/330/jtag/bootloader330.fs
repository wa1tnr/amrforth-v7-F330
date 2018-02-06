0 [if]   bootloader330.fs (was 310.fs; adapted by cwh)  - DO NOT PROPAGATE
Copyright (C) 1991-2005 by AM Research, Inc.

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

\ ALL20030814   ROM used also in hex.

[then]

\ Host downloader repeatedly sends $a5 to the target system until it
\ gets a $5a in response. Then host sends "amr". The target responds
\ with $a5. The host sends a byte containing the number of 512 byte
\ pages to be sent. The target erases those pages and responds with $5a.
\ Then the host sends pages beginning with page 1, at address $200 or
\ 512. The target responds after each page is received with a byte
\ containing the loop counter, i.e. number of pages remaining, including
\ the one just received. In other words it counts down from the total
\ number of pages to 1.

\ Flash program memory can't be written unless PSCTL.0 is set. This bit
\ is clear during normal operation. It is only set in the bootloader
\ immediately before DPTR is loaded with $200. Even if a rogue program
\ jumps into the middle of the bootloader it can't overwrite the
\ bootloader because DPTR is loaded with $200 in the bootloader right
\ after PSCTL.0 is set. The only way the bootloader might be ruined is
\ by having PSCTL.0 set in the user's code and then accidentally jumping
\ into the bootloader at the point where DPTR was just loaded with $200.
\ Not very likely.

\ The bootloader sets up the crossbar such that:
\	TX= P0.4
\	RX= P0.5

in-meta decimal

\ Vector the interrupts into page 1.
	0 there $63 erase  \ Clear interrupt vectors.

label INTERRUPT-VECTORS
	$03 org  $203 ljmp
	$0b org  $20b ljmp
	$13 org  $213 ljmp
	$1b org  $21b ljmp
	$23 org  $223 ljmp
	$2b org  $22b ljmp
	$33 org  $233 ljmp
	$3b org  $23b ljmp
	$43 org  $243 ljmp \ reserved; not used on F310 nor F330.
	$4b org  $24b ljmp
	$53 org  $253 ljmp
	$5b org  $25b ljmp
	$63 org  $263 ljmp
	$6b org  $26b ljmp \ reserved; used on F310 as Comparator1
	$73 org  $273 ljmp

	$7b org  \ Code starts here, after interrupts (on both F310 F330).

\ ------------------------------------------------------

\ Entry point of the main program if not bootloading.
romHERE ( *) $200 org
label main  c; ( *) org

\ Subroutines go here, before the entry point.

a: unlock   $a5 # FLKEY mov  $f1 # FLKEY mov  ;a
a: blink-delay 
	A clr  A R7 mov  A R6 mov  10 # R5 mov
	begin begin begin
	R7 -zero until
	R6 -zero until
	R5 -zero until
	;a
	
label 'EMIT
	begin  1 .SCON0 set? until  1 .SCON0 clr  A SBUF0 mov  ret
label 'KEY
	begin  0 .SCON0 set? until  0 .SCON0 clr  SBUF0 A mov	ret
label 'KEY-OR-TIMEOUT
	$f1 # TMOD anl  $01 # TMOD orl  \ 16 bit timer 0.
	0 # TH0 mov  0 # TL0 mov  \ Timeout period.
	5 .TCON clr  4 .TCON setb  \ Start Timer0.
	begin
		0 .SCON0 set? if
			0 .SCON0 clr  SBUF0 A mov  ret
		then
		5 .TCON set? if  \ Timer0 overflow.
			4 .TCON clr  \ Stop Timer0.
			main jump  \ Run normal program.
		then
	again

a: blink-main  begin  2 .P0 cpl  blink-delay  again ;a

label ENTRY
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
\ ----- initialization code goes here, before MAIN.
	$14 # P0MDOUT orl  \ P0.2 and P0.4 are outputs, push pull.
	$ff # P0MDIN orl  \ No analog, all digital.
	$01 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
\ Setup serial port.
	$c3 # OSCICN mov  \ Full speed internal, 24.5 MHz.
	$00 # CKCON mov  \ T1 uses SYSCLK/12.
	$12 # SCON0 mov  \ 8 bit UART mode, TX ready.
	$20 # TMOD mov  \ Mode 2, 8 bit auto-reload.
	$96 # TH1 mov  \ 9600 baud, at 24.5MHz.
	6 .TCON setb  \ Enable Timer 1.
\ Only bootload if the password is received in time.
	'KEY-OR-TIMEOUT call
	$a5 # A xrl  0<> if  blink-main ( jump)  then
	$5a # A mov  'EMIT call  \ Signal host we're ready.
	'KEY-OR-TIMEOUT call
	char a # A xrl  0<> if  blink-main ( jump)  then
	'KEY-OR-TIMEOUT call
	char m # A xrl  0<> if  blink-main ( jump)  then
	'KEY-OR-TIMEOUT call
	char r # A xrl  0<> if  blink-main ( jump)  then
\ Setup the download.	
	$a5 # A mov  'EMIT call  \ Signal readiness to host.
	'KEY call  \ Get number of pages.
	0= if  blink-main ( jump)  then  \ Don't download 0 pages.
	63 # A = if else -C if  62 # A mov  then then  \ Clip.
	A R7 mov  \ Save number of pages in A.
\ Erase pages.
	3 # PSCTL mov  \ Enable Writing and Erasing.
	$200 # DPTR mov  \ First page to erase.
	begin	unlock  A @DPTR movx  DPH inc  DPH inc
	R7 -zero until
	A R7 mov  \ Restore page counter.
	$5a # A mov  'EMIT call  \ Signal readiness to host.
\ Accept bytes and write them into flash. Always start at address $200,
\   The program downloaded must have its entry point at $200!
	1 # PSCTL mov  \ Disable erasing, but leave writing enabled.
	$200 # DPTR mov  \ Start at page 1, address 512.
	begin
		0 # R6 mov
		begin
			'KEY call  unlock  A @DPTR movx  DPTR inc
			'KEY call  unlock  A @DPTR movx  DPTR inc
		R6 -zero until
		R7 A mov  'EMIT call
	R7 -zero until
	0 # PSCTL mov  \ Disable further writing to flash.
	'KEY call  \ Wait for signal to start running program.
	main jump c;

HERE ( *)               \ Remember dictionary pointer
0 ORG                   \ Reset vector at address 0
label RESET
	ENTRY ljmp c;   \ Reset jumps to ENTRY point
( *) ORG                \ Restore dictionary pointer

