\ jtag_c2.fs
include amrfconf.fs
windows [if]
	include serial-windows.fs
[else]	include serial-linux.fs
[then]	open-comm

0 value fid
0 value romHERE

: open-object-file  (  - )
	s" rom.bin" r/o open-file throw to fid
	fid file-size throw drop to romHERE
	;

: close-object-file  (  - ) fid close-file throw ;

: download-page  (  - )
	8 0 do
		64 0 do
			pad 1 fid read-file throw drop
			pad c@ emit-s
		loop
		key-s drop
	loop
	;

: wait-ok  (  - )
	begin	key-s dup [char] O - while
		dup 13 - if  dup emit  then  drop
	repeat  drop
	key-s drop key-s drop key-s drop ;

: download-all  ( addr len - )
	open-object-file
	clear-sbuf
	cr ." Downloading rom.bin" cr
	( addr len) type-s 10 emit-s
	." 512 byte pages: "
	begin  key-s [char] A = until
	romHERE 512 /mod swap 0= 0= 1 and + dup emit-s
	dup 0 ?do  dup . 1 - download-page  loop  drop
	close-object-file
	wait-ok ;

: simple  ( addr len - ) clear-sbuf type-s 10 emit-s wait-ok ;

