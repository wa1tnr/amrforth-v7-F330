\ config.fs - DO NOT PROPAGATE - F330 variant - cwh 2012
\ even when F330 checks out, ttyUSB0 is still non-standard. CHANGE b4 propagate.

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

warnings off

1 value com?
1 value kernel?

include strings.fs
include ansi.fs

variable fid

: open-amrfconf.fs  (  - )
	s" amrfconf.fs" w/o open-file throw fid ! ;

: close-amrfconf.fs  (  - )
	fid @ close-file throw ;

create 'crlf  13 c, 10 c,  \ DOS format.
: (write)  ( addr len - ) fid @ write-file throw ;
: cr-file  (  - ) 'crlf 2 (write) ;
: spit  ( c - ) pad c! pad 1 (write) ;
: write  ( addr len - ) (write) cr-file ;

\ Read to end of line and compile the string into the target,
\ leaving its address on the stack at compile time for inclusion in a
\ table.
: string  (  - addr) here 1 parse here over 1 + allot place align ;

\ The index is screened first, won't be out of range, so don't bother
\ checking.
: choose  ( addr i - ) cells + @ count write ;

: choices  ( n - )
	here >r 0 do  ,  loop  r>
	create  ,
	does>  ( i - ) @ swap choose ;

string create interrupts-kernel
string create polled-kernel
create kernel-strings , ,
: write-kernel-type  (  - ) kernel-strings kernel? 1 - choose ;

string 2 value com?
string 1 value com?
create com-strings , ,
: write-comport  (  - ) com-strings com? 1 - choose ;

string : processor  s" AT89C51RC2" ;
string : processor  s" 80c537" ;
string : processor  s" 80c552" ;
string : processor  s" 80c451" ;
string : processor  s" 8052" ;
string : processor  s" 8051" ;
string : processor  s" ADuC816" ;
string : processor  s" ADuC812" ;
string : processor  s" C8051F330" ;
string : processor  s" C8051F06x" ;
string : processor  s" C8051F310" ;
string : processor  s" C8051F300" ;
string : processor  s" C8051F017" ;
13 choices write-processor 

string : sfr-file  s" sfr-C51RC2.fs" ;
string : sfr-file  s" sfr-537.fs" ;
string : sfr-file  s" sfr-552.fs" ;
string : sfr-file  s" sfr-451.fs" ;
string : sfr-file  s" sfr-32.fs" ;
string : sfr-file  s" sfr-31.fs" ;
string : sfr-file  s" sfr-816.fs" ;
string : sfr-file  s" sfr-812.fs" ;
string : sfr-file  s" sfr-f330.fs" ;
string : sfr-file  s" sfr-f061.fs" ;
string : sfr-file  s" sfr-f310.fs" ;
string : sfr-file  s" sfr-f300.fs" ;
string : sfr-file  s" sfr-f000.fs" ;
13 choices write-sfr-file

string : downloader  s" download-atmel.fs" ;
string : downloader  s" download-oldamr.fs" ;
string : downloader  s" download-aduc.fs" ;
string : downloader  s" download-cygnal.fs" ;
4 choices write-downloader

: write-f017  (  - )
	0 write-processor
	0 write-sfr-file
	0 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 24000000 ," write
	s" 243 constant default-TH1" write
	s" true constant smod?" write
	s" 691 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-f30x  (  - )
	1 write-processor
	1 write-sfr-file
	0 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 24500000 ," write
	s" 250 constant default-TH1" write
	s" true constant smod?" write
	s" 611 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-f31x  (  - )
	2 write-processor
	2 write-sfr-file
	0 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 24500000 ," write
	s" 250 constant default-TH1" write
	s" true constant smod?" write
	s" 635 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-f06x  (  - )
	3 write-processor
	3 write-sfr-file
	0 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 24500000 ," write
	s" 97 constant default-TH1" write
	s" true constant smod?" write
	s" 691 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-f33x  (  - )
	4 write-processor
	4 write-sfr-file
	0 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 24500000 ," write
	s" 250 constant default-TH1" write
	s" true constant smod?" write
	s" 635 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-ADuC812  (  - )
	5 write-processor
	5 write-sfr-file
	1 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 11059200 ," write
	s" 250 constant default-TH1" write
	s" true constant smod?" write
	s" 75 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-ADuC816  (  - )
	6 write-processor
	6 write-sfr-file
	1 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 12589120 ," write
	s" 249 constant default-TH1" write
	s" true constant smod?" write
	s" 75 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-8051  (  - )
	7 write-processor
	7 write-sfr-file
	2 write-downloader
	s" 19200 constant baudrate" write
	s" create frequency 11059200 ," write
	s" $fd constant default-TH1" write
	s" true constant smod?" write
	s" $2b constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-8052  (  - )
	8 write-processor
	8 write-sfr-file
	2 write-downloader
	s" 19200 constant baudrate" write
	s" create frequency 11059200 ," write
	s" $fd constant default-TH1" write
	s" true constant smod?" write
	s" $33 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-80c451  (  - )
	9 write-processor
	9 write-sfr-file
	2 write-downloader
	s" 19200 constant baudrate" write
	s" create frequency 11059200 ," write
	s" $fd constant default-TH1" write
	s" true constant smod?" write
	s" $2b constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-80c552  (  - )
	10 write-processor
	10 write-sfr-file
	2 write-downloader
	s" 19200 constant baudrate" write
	s" create frequency 11059200 ," write
	s" $fd constant default-TH1" write
	s" true constant smod?" write
	s" $7b constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-80c537  (  - )
	11 write-processor
	11 write-sfr-file
	2 write-downloader
	s" 19200 constant baudrate" write
	s" create frequency 11059200 ," write
	s" $fd constant default-TH1" write
	s" true constant smod?" write
	s" $93 constant rom-start" write
	write-kernel-type
	write-comport
	;

: write-AT89C51RC2  (  - )
	12 write-processor
	12 write-sfr-file
	3 write-downloader
	s" 9600 constant baudrate" write
	s" create frequency 22118400 ," write
	s" 250 constant default-TH1" write
	s" true constant smod?" write
	s" $4b constant rom-start" write
	write-kernel-type
	write-comport
	;

: invalid  (  - ) ." That choice wasn't valid." ;

create processor-list
	' invalid ,
	' write-f017 ,
	' write-f30x ,
	' write-f31x ,
	' write-f06x ,
	' write-f33x ,
	' write-ADuC812 ,
	' write-ADuC816 ,
	' write-8051 ,
	' write-8052 ,
	' write-80c451 ,
	' write-80c552 ,
	' write-80c537 ,
	' write-AT89C51RC2 ,
: which-processor  ( i - )
	dup 14 u< and cells processor-list + @ execute ;

: processor  ( i - )
	open-amrfconf.fs which-processor close-amrfconf.fs ;

: config-welcome  (  - )
	save_cursor 0 0 at-xy
	blue background cyan foreground text_bold 
	." Configure system by choosing a processor." clrtoeol
	restore_cursor ;

\ Empty string returns a 0.
: input  (  - n) pad dup 5 accept s>unumber? nip and ;


: choose-comport  (  - )
	begin
		." Choose a com port:" cr
		." 1 = COM1 or /dev/ttyUSB0" cr \ local change only! ttyS0 canonical cwh
		." 2 = COM2 or /dev/ttyS1" cr
		input dup 3 u< and ?dup
	until
	to com? ;

: choose-kernel  (  - )
	begin
		." Choose a kernel type:" cr
		." 1 = polled serial" cr
		." 2 = interrupt driven serial" cr
		input dup 3 u< and ?dup
	until
	to kernel? ;

: choose-processor  (  - )
	begin
		." Choose a processor:" cr
		."  1 = C8051F017" cr
		."  2 = C8051F300" cr
		."  3 = C8051F310" cr
		."  4 = C8051F06x" cr
		."  5 = C8051F330" cr
		."  6 = ADuC812" cr
		."  7 = ADuC816" cr
		."  8 = 8051" cr
		."  9 = 8052" cr
		." 10 = 80c451" cr
		." 11 = 80c552" cr
		." 12 = 80c537" cr
		." 13 = AT89C51RC2" cr
		input dup 14 u< and ?dup
	until
	processor cr 
	." Writing amrfconf.fs" cr ;

cr config-welcome
choose-comport cr
choose-kernel cr
choose-processor cr
bye

