0 [if]   serial_ports.fs   Serial Communications in Gforth and Linux.
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

: has  (  - flag) BL word find nip 0<> ; immediate
has com? 0= [if]  1 value com?  [then]
has baudrate 0= [if]  9600 value baudrate  [then]

\ a list of four strings
here ," /dev/ttyS3"
here ," /dev/ttyS2"
here ," /dev/ttyS1"
here ," /dev/ttyS0"
create 'portnames   , , , ,

: "portname   (  - a u)   
	com? 4 min 1 max 1- cells 'portnames + @ count ;

: .port  (  - )  "portname type ." > " ;

\ Newer versions of stty allow the -F option to configure a serial port
\ other than the one you're using for your terminal.  Older versions
\ accomplish the same thing with < for redirecting input from the serial
\ port.  The version with < probably works for all systems.
\ : "stty   (  - a u)   s" stty -F " ;
: "stty   (  - a u)   s" stty < " ;

: "N81   (  - a u)
	s"  -parenb cs8 -cstopb -crtscts raw -echo" ;

: n81   (  - )
	"stty pad place   "portname pad +place
	"N81 pad +place   pad count system ;

: baud   ( n - )
	"stty pad place   "portname pad +place
	0 <# #s bl hold #> pad +place   pad count system ;

variable serial-fid

: ?serial-problem   ( error-code - )
	?dup if   cr .error abort   then ;
: open-comm   (  - )
	N81 baudrate baud
	"portname r/o open-file ?serial-problem serial-fid ! ;

: close-comm   (  - )   serial-fid @ close-file drop ;

warnings off
: bye   (  - )   serial-fid @ if   close-comm   then   bye ;
warnings on

: port   ( n - )
	create	,
	does>	close-comm @ to com? open-comm ;
1 port com1
2 port com2
3 port com3
4 port com4

: key-s   (  - c)
	pad 1 serial-fid @ read-file 0= 1 and - ?serial-problem
	pad c@ ;

: key?-s   (  - flag)
    serial-fid @ key?-file ;

: emit-s   ( c - )
	"portname w/o open-file ?serial-problem
	tuck emit-file ?serial-problem close-file ?serial-problem ;

: type-s  ( addr len - ) 0 do  count emit-s  loop  drop ;

: clear-sbuf   (  - )   begin   key?-s while   key-s drop   repeat ;

