\ optimized.fs

: #1  2 begin  dup . 1-dup0=until  . ;
: #2  0 dupif  ." non-"  then  drop ." zero" ;
: #3  0 dupnotif  ." zero"  then  drop ;
: #4  -1 dup0<if  dup .  then  . ;
: #5  -1 dup0<notif  dup .  then  . ;

: #6  5 begin  1- dupnotuntil drop ;
: #7  5 begin  1- dupuntil drop ;
: #8  5 begin  1- dup0<until drop ;
: #9  -5 begin  1+ dup0<notuntil drop ;

: #10  dup>r noop r>drop noop ;

: #11  (  - ) 10 and  [asm 10 #and ] noop ;

: #12  (  - ) $ffff [asm $1234 #and ] noop ;
: #13  (  - ) $ffff [asm $1234 #or ] noop ;
: #14  (  - ) $ffff [asm $1234 #xor ] noop ;
: #15  (  - ) $ffff [asm $1234 #lit ] noop ;

: #16  (  - ) $2345 #and ;

