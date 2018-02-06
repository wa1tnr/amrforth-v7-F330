0 [if]   serial-windows.fs
	 Serial Communications in Gforth and Windows.
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
has baudrate [if]
	baudrate 19200 = [if]
		14 value current-baudrate  \ 19200
	[else]  13 value current-baudrate  \ 9600
	[then]
[else]
( 9600) 13 value current-baudrate
[then]


\ a list of four strings
here ," /dev/ttyS3"
here ," /dev/ttyS2"
here ," /dev/ttyS1"
here ," /dev/ttyUSB0"
create 'portnames   , , , ,

: "portname   (  - a u)   
	com? 4 min 1 max 1- cells 'portnames + @ count ;

: .port  (  - )  "portname type ." > " ;

\ In Windows we don't have stty, so we use functions in the cygwin1.dll.

: f2c  ( addr1 len - addr2)
	here place 0 here count + c!
	here dup c@ 1 + allot 1 + align ;

require \PROGRA~1\gforth\fflib.fs
library cygwin \PROGRA~1\gforth\cygwin1.dll

cygwin open ptr int (int) open
cygwin close int (int) close
cygwin read int ptr int (int) read
cygwin write int ptr int (int) write
cygwin ioctl int int ptr (int) ioctl
cygwin tcgetattr int ptr (int) tcgetattr
cygwin tcsetattr int int ptr (int) tcsetattr
\ cygwin cfgetispeed ptr (int) cfgetispeed
\ cygwin cfgetospeed ptr (int) cfgetospeed
cygwin cfsetispeed ptr int (int) cfsetispeed
cygwin cfsetospeed ptr int (int) cfsetospeed

create termios 4 4 + 4 + 4 + 2 + 64 + 4 + 4 + allot

 0 termios + constant c_iflag
 4 termios + constant c_oflag
 8 termios + constant c_cflag
12 termios + constant c_lflag

\ 0
\ $0001 or ( IGNBRK)
\ $0002 or ( BRKINT)
\ $0008 or ( PARMRK)
\ $0020 or ( ISTRIP)
\ $0040 or ( INLCR)
\ $0080 or ( IGNCR)
\ $0100 or ( ICRNL)
\ $0400 or ( IXON)
  $05eb invert constant ~c_i_raw

\ 0
\ $0001 or ( OPOST)
  $0001 invert constant ~c_o_raw

\ 0
\ $0001 or ( ISIG)
\ $0002 or ( ICANON) 
\ $0008 or ( ECHO)
\ $0040 or ( ECHONL)
\ $8000 or ( IEXTEN)
  $804b invert constant ~c_l_raw

\ 0
\ $0030 or ( CSIZE)
\ $0100 or ( PARENB)
  $0130 invert constant ~c_c_raw

\ 0
\ $0030 or ( CS8)
  $0030 constant c_c_raw

\ Borrowed from 'man termios' in Linux.
: makeraw  (  - )
	c_iflag @ ~c_i_raw and c_iflag !
	c_oflag @ ~c_o_raw and c_oflag !
	c_lflag @ ~c_l_raw and c_lflag !
	c_cflag @ ~c_c_raw and c_c_raw or c_cflag !
	;

0 value serial-fd

: key-s  (  - c)
	serial-fd pad 1 read 1 - throw
	pad c@ ;

: key?-s  (  - n)
	serial-fd $541b ( FIONREAD) pad ioctl drop
	pad @ ;

: emit-s  ( c - )
	pad c!
	serial-fd pad 1 write drop ;

: open-comm   (  - )
	"portname f2c $0902 ( O_RDWR|O_NOCTTY|O_NDELAY) open
	to serial-fd
	serial-fd termios tcgetattr throw
	makeraw
	serial-fd termios current-baudrate cfsetispeed throw
	serial-fd termios current-baudrate cfsetospeed throw
     	serial-fd 0 ( TCSANOW) termios tcsetattr throw ;
	
: close-comm  (  - ) serial-fd close throw 0 to serial-fd ;

warnings off
: bye  (  - ) serial-fd if  close-comm  then  bye ;
warnings on

: port   ( n - )
	create	,
	does>	close-comm @ to com? open-comm ;
1 port com1
2 port com2
3 port com3
4 port com4

: type-s  ( addr len - ) 0 do  count emit-s  loop  drop ;

: clear-sbuf  (  - ) begin   key?-s while   key-s drop   repeat ;

