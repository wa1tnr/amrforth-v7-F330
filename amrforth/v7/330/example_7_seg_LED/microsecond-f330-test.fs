\ main.fs  JTAG driver for F300
in-meta decimal

0 bit-variables

a: 1us  5 # R7 mov  begin  R7 -zero until   ;a

\ Measured on a scope, not just calculated.
\ Maximum of 255 us allowed, since it uses 8 bits.
\ The high byte of the count is ignored.
code us  ( c - )
	begin  1us  ACC -zero until
	' drop jump c;

: ms  ( n - )
	dup 0= if  drop exit  then
	1 - 250 us 250 us 250 us 250 us  ms ;

\ ----- JTAG interface ----- /

warnings off
variable buffer 62 allot
warnings on

cvariable addr

code addr!  ( a - )
	A addr direct mov  ' drop jump c;

code addr+  (  - ) addr direct inc  next c;

: !+  ( c - ) addr c@ c!  addr+ ;

: @+  (  - c) addr c@ c@  addr+ ;

\ 4 wire JTAG interface.
$01 constant TCK  a: .TCK  0 .P0 ;a
$02 constant TMS  a: .TMS  1 .P0 ;a
$04 constant TDI  a: .TDI  2 .P0 ;a
$08 constant TDO  a: .TDO  3 .P0 ;a
\ Serial port on P0.4 and P0.5

cvariable flash-timing

: 2MHz   $86 flash-timing c! ;  ' 2MHz  header 2mhz
: 11MHz  $88 flash-timing c! ;  ' 11MHz header 11mhz
: 24MHz  $89 flash-timing c! ;  ' 24MHz header 24mhz

code startup  (  - )
	$40 invert # PCA0MD anl  \ Clear watchdog enable bit.
\ ----- initialization code goes here, before MAIN.
	$14 # P0MDOUT orl  \ P0.2 and P0.4 are outputs, push pull.
	$ff # P0MDIN orl  \ No analog, all digital.
	$03 # XBR1 mov  \ Enable TX and RX on P0.4, P0.5.
	$40 # XBR2 mov  \ Enable crossbar and weak pull-ups.
\ Setup serial port.
	$17 # OSCICN mov  \ Full speed internal, 24.5 MHz.
	$00 # CKCON mov  \ T1 uses SYSCLK/12.
	$12 # SCON0 mov  \ 8 bit UART mode, TX ready.
	$20 # TMOD mov  \ Mode 2, 8 bit auto-reload.
	$96 # TH1 mov  \ 9600 baud, at 24.5MHz.
	6 .TCON setb  \ Enable Timer 1.
	next c;

code _init  (  - )
	TCK TMS TDI + +
	dup # P0MDIN orl
	dup # P0MDOUT orl  \ Outputs.
	# P0 orl  \ All low.
	$40 # XBR2 orl  \ Enable crossbar, weak pullups.
	next c;

\ State, always strobes the clock.
code S  ( flag - )
	0<> if
		.TMS setb
	else
		.TMS clr
	then
	' drop call
\ clocK strobe.
code K  (  - )
	.TCK setb  .TCK clr
	next c;
' S header s

code +k  .tck setb  next c;
code -k  .tck clr   next c;
' +k header +k
' -k header -k

\ data Out, no clock strobe.
code O@  (  - flag)
	' dup call
	A clr  .TDO set? if  A dec  then
	A R2 mov  next c;
' o@ header o@

: Out  ( n1 mask - n2) O@ and swap 2/ or ;

\ data In, no clock strobe.
code I!  ( flag - )
	0<> if  .TDI setb  ' drop jump  then
	.TDI clr  ' drop jump c;
' i! header i!

: In  ( n1 - n2) dup 1 and I! 2/ ;

: TAP-reset  (  - )  1 S K K K K  0 S ;

: run-test/idle  (  - ) 1 S  1 S  0 S ;

: shift-IR  (  - ) 1 S  1 S  0 S  0 S ;

: shift-DR  (  - ) 1 S  0 S  0 S ;

\ Write only.
: IR-Scan  ( instruction - )
	shift-IR
\	14 for  In K  next  In drop
	15 for  In K  next  In drop
	run-test/idle ;

\ Read only, 32 bits.
: read-idcode  (  - d)
	$1004 IR-Scan  \ Write IDCODE address.
	0 In drop  \ Keep TDI low.
	shift-DR
	0 16 for  $8000 Out K  next
	0 15 for  $8000 Out K  next  $8000 Out
	run-test/idle ;

: run  (  - ) $0fff IR-Scan ;
' run header run

: halt  (  - ) $1fff IR-Scan ;
' halt header halt

: reset  (  - ) $2fff IR-Scan ;
' reset header reset

: suspend  (  - ) $4fff IR-Scan ;
' suspend header suspend

: id  (  - d) TAP-reset reset read-idcode ;

: busy?  (  - flag)
	0 In drop  \ Keep TDI low.
	shift-DR 0 1 Out run-test/idle ;

: poll-busy  (  - ) begin  busy? not until ;

: initiate-read  (  - ) shift-DR  2  In K  In drop  run-test/idle ;

: read-8-bits  (  - c)
	initiate-read poll-busy
	shift-DR  0 0 Out K  \ Shift out the busy bit.
	7 for  $80 Out K  next
	$80 Out  run-test/idle ;

: read-16-bits  (  - n)
	initiate-read poll-busy
	shift-DR  0 0 Out K  \ Shift out the busy bit.
	15 for  $8000 Out K  next
	$8000 Out run-test/idle ;

: read-FLASHCON  (  - c) $4082 IR-Scan read-8-bits ;
: read-FLASHSCL  (  - c) $4085 IR-Scan read-8-bits ;
: read-FLASHADR  (  - n) $4084 IR-Scan read-16-bits ;
: read-FLASHDAT  (  - n)
	$4083 IR-Scan
	initiate-read poll-busy
		\ Shift out busy FLfail and FLbusy bits.
	shift-DR 0 0 Out K 0 Out K 0 Out K
	7 for  $80 Out K  next 
	$80 Out  run-test/idle ;

: FLbusy?  (  - flag)
	$4083 IR-Scan
	initiate-read poll-busy
	shift-DR 0 0 Out K -1 Out run-test/idle ;

: poll-FLbusy  (  - ) begin  FLbusy? not until ;

: write-8-bits  ( c - )
	shift-DR
	8 for  In K  next  drop  \ 8 bits of data.
	3 In K In drop  \ Write opcode.
	run-test/idle ;

: write-16-bits  ( n - )
	shift-DR
	16 for  In K  next  drop  \ 16 bits of data.
	3 In K In drop  \ Write opcode.
	run-test/idle ;

: write-FLASHSCL  ( c - ) $4085 IR-Scan write-8-bits ;
: write-FLASHCON  ( c - ) $4082 IR-Scan write-8-bits ;
: write-FLASHADR  ( n - ) $4084 IR-Scan write-16-bits ;
: write-FLASHDAT  ( c - ) $4083 IR-Scan write-8-bits ;

: read-flash-byte  (  - c)
	2 write-FLASHCON  \ Read mode.
	read-FLASHDAT drop  \ Initiate read.
	0 write-FLASHCON  \ Poll mode.
	poll-FLbusy
	read-FLASHDAT ;

: load-address  ( addr - )
	flash-timing c@ write-FLASHSCL
	( addr) write-FLASHADR ;

: remoteC@  ( addr - c) load-address read-flash-byte ;

: remote-dump  ( addr len - )
	halt 
	swap load-address
	0 16 um/mod swap if  1 +  then
	for  cr 16 for  read-flash-byte hb.  next next
	;
' remote-dump header dump

: dump-line  (  - ) cr 16 for  read-flash-byte hb.  next ;

\ Blocks are local.
\ Blocks start at top of memory and grow downward.
\ The top 512 bytes are reserved for the factory.
: block  ( n - a) 512 * negate [ $2000 512 - 512 - ] literal + ;

\ Pages are remote.
\ Pages start at bottom of memory and grow upward.
: page  ( n - a) 512 * ;

: remote-show  ( page - ) page 512 remote-dump ;
: rs  remote-show ;
' remote-show header rs

: local-show  ( block# - )
	block [ 512 16 / ] literal for
		cr 16 for  count hb.  next
	next  drop ;
: ls  local-show ;
' local-show header ls

: write-flash-byte  ( c - )
	$10 write-FLASHCON  \ Write mode.
	( c) write-FLASHDAT
	0 write-FLASHCON  \ Poll mode.
	poll-FLbusy ;

: remoteC!  ( c addr - ) load-address write-flash-byte ;

: remote-erase  ( addr - )
	load-address
	$20 write-FLASHCON  \ Erase mode.
	$a5 write-FLASHDAT  \ Yes, erase.
	0 write-FLASHCON  \ Poll mode.
	poll-FLbusy
	;
: erase-page-0  (  - ) 0 remote-erase ;
' erase-page-0 header erase0

: erase-all  (  - ) $7dff remote-erase ;
' erase-all header erase-all

a: unlock  $a5 # FLKEY mov  $F1 # FLKEY mov  ;a

\ Write to local flash.  Must have been erased already.
code localC!  ( c addr - )
	1 # PSCTL mov  \ Enable flash write.
	A DPH mov  R2 DPL mov  ' drop call
	unlock  A @DPTR movx
	0 # PSCTL mov  \ Disable flash write.
	' drop jump c;
' localC! header c!

: localC@  ( addr - c) c@p ;
' localC@ header c@

\ Erase local flash.
code local-erase  ( addr - )
	3 # PSCTL mov  \ Enable flash write and erase.
	A DPH mov  R2 DPL mov
	unlock  A @DPTR movx
	0 # PSCTL mov  \ Disable flash write.
	' drop jump c;


: delay  ( n - ) for next ;

: compare  ( block page - )
	page load-address block
	512 for
		dup localC@ read-flash-byte
		- if  pop 2drop 0 exit  then
		1 +
	next drop -1
	;
' compare header compare

: fill-buffer  (  - ) buffer addr! 64 for  key !+  next ;

: write-buffer  (  - )
    buffer addr! 64 for  @+ write-flash-byte  next ;

: write-page  (  -  ) 8 for  fill-buffer write-buffer  0 emit  next ;

\ Run from outside, not directly in the interpreter.
: jtag-download  (  - )
	halt erase-all 65 emit  \ Signal erasure done, ready for download.
	halt 0 load-address
	key for  write-page  next
	;
' jtag-download header jtag

cvariable dump-page  \ Half page actually.

: jtdump  (  - ) 0 dup dump-page c! 0 h. rs cr ;
' jtdump header dump

: dump-next  ( - )
	dump-page c@ 1 + dup dump-page c!  dup 512 * h. rs cr ;
' dump-next header next

\ ----- C2 interface ----- /

\ 2 wire C2 (JTAG) interface.
$40 constant C2D   a: .C2D  6 .P0 ;a
$80 constant C2CK  a: .C2CK  7 .P0 ;a

a: C2strobe  .C2CK clr  .C2CK setb  ;a
a: C2Bit-out  C .C2D mov  C2Strobe  ;a
a: C2Bit-in  C2Strobe  .C2D C mov   ;a
a: C2D-Release  .C2D setb  C2D invert # P0MDOUT anl   ;a
a: C2D-Engage   C2D # P0MDOUT orl   ;a
a: C2CK-Release  C2CK invert # P0MDOUT anl   ;a
a: C2CK-Engage   C2CK # P0MDOUT orl   ;a

: C2reset  (  - )
	[ in-assembler  C2CK-Engage  .C2CK clr  in-meta ] 40 us
	[ in-assembler  .C2CK setb  in-meta ] ;
	
code start  (  - ) C2CK-Engage  C2Strobe  next c;

code stop  (  - ) C2D-Release  C2Strobe  C2CK-Release  next c;

code wait  (  - )
	\ C2D-Release
	begin	C2Bit-in
		+C if  next  then
	again c;

code read-byte  (  - c)
	' dup call  A clr  A R2 mov
	8 # R7 mov
	begin  C2Bit-in  A rrc  R7 -zero until
	next c;

code ins  ( c - )
	C2D-Engage
code length  ( c - )
	A rrc  C2Bit-out
	A rrc  C2Bit-out
	' drop jump c;

code write-byte  ( c - )
	8 # R7 mov
	begin  A rrc  C2Bit-out  R7 -zero until
	' drop jump c;

code C2strobe  (  - ) C2Strobe  next c;
code C2D-release  (  - ) C2D-release  next c;

: C2a!  ( c - ) start 3 ins write-byte stop ;

: C2a@  (  - c) start 2 ins C2D-release read-byte stop ; 

: C2d@  (  - c)
	start 0 ins 0 length
	C2D-release wait read-byte stop ;

: C2d!  ( c - )
	start 1 ins 0 length write-byte
	C2D-release wait stop ;

: C2id@  (  - c) 0 C2a! 5 us C2d@ ;

$02 constant FPCTL
$b4 constant FPDAT

: poll-InBusy  (  - )
	C2a@ 2 and if  poll-InBusy exit  then ;

: poll-OutReady  (  - )
	C2a@ 1 and if  exit  then poll-OutReady ;

: ..  [char] . emit ;

: C2init  (  - )
	C2reset 5 us  FPCTL C2a!  2 C2d!  1 C2d!  40 ms ;

code split  ( a - c1 c2)
	'R2 R7 mov  0 # R2 mov
	' dup call  R7 A mov
	next c;

6 constant BLOCK-READ
7 constant BLOCK-WRITE
8 constant PAGE-ERASE
3 constant DEVICE-ERASE

: .?  [char] ? emit ;

: C2data@  (  - c) poll-OutReady C2d@ ;
: C2data!  ( c - ) C2d! poll-InBusy ;

: C2dump  ( addr - )
	C2init FPDAT C2a!
	BLOCK-READ C2data!
	C2data@ $0d - if  .? exit  then
	split C2data! C2data!  0 C2data!  \ 256 byte block.
	C2data@ $0d - if  .? exit  then
	256 for
		i $0f and 0= if  10 emit  then
		C2data@ hb.
	next ;

: C2dump-next  ( - )
	dump-page c@ 1 + dup dump-page c!
	256 * dup h. C2dump ;
' C2dump-next header C2next

: C2dump0  (  - )
    0 dump-page c!  0 h. 0 C2dump ;
' C2dump0 header C2dump

: C2erase-all  (  - )
	C2init FPDAT C2a!
	DEVICE-ERASE C2data!
	C2data@ $0d - if  .? exit  then
	$de C2data!  $ad C2data!  $a5 C2data!
	poll-OutReady ;
' C2erase-all header C2erase

\ 64 byte buffer.
: C2write-buffer  ( addr - )
	FPDAT C2a!
	BLOCK-WRITE C2data!
	C2data@ $0d - if  .? exit  then
	split C2data! C2data!  64 C2data!
	C2data@ $0d - if  .? exit  then
	buffer addr! 64 for  @+ C2data! ( 10 us)  next
	poll-OutReady ;

: C2write-page  ( addr1 - addr2)
	8 for	fill-buffer dup C2write-buffer 64 + 0 emit
	next ;

\ Run from Tcl, not directly in the interpreter.
: C2-download  (  - )
	C2erase-all 65 emit  \ Signal erasure done, ready for download.
	0 key for  C2write-page  next  drop ;
' C2-download header C2download

\ ----- Main Program ----- /

: init  (  - ) _init 24MHz TAP-reset ;

code toggle  2 .P0 cpl  next c;

: test-delay  (  - ) toggle 10 delay  test-delay ;

: test-us  (  - )
	10 us toggle
	10 us toggle
	test-us ;

: test  (  - ) init begin  c2d@ drop  100 us  again ;
: test1 (  - ) init begin  c2a@ drop  100 us  again ;

: test0  (  - ) init begin  c2id@ drop 100 us  again ;

: go  (  - ) startup init interpret ;

patch-headers

