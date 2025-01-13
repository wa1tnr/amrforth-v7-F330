\ catch.fs   catch and throw
in-compiler

code sp@  (  - n) ' dup call  SP A mov  0 # R2 mov  next c;
code rp@  (  - n)
	' dup call  DPH pop  DPL pop
	RP A mov  0 # R2 mov  DPL push  DPH push
	next c;
code sp!  ( n - ) A SP mov  ' drop jump c;
\ Think of the top of the return stack as IP, the Interpreter Pointer of
\ a threaded forth.  It needs to be preserved.  The rest of the return
\ stack is the forth return stack.
code rp!  ( n - )
	DPH pop  DPL pop  A RP mov  DPL push  DPH push
	' drop jump c;

variable frame

: catch  ( a - 0|n)
	sp@ push frame @ push rp@ frame ! execute
	pop frame ! r>drop 0 ;

: throw  ( n1 - n2)
	frame @ dup0=if  abort  then  rp!
	pop frame !  pop swap push sp! drop pop ;

: initialize-frame  (  - ) 0 frame ! ;

