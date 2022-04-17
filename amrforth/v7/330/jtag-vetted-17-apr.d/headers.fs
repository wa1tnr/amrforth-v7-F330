\ headers.fs  Headers on the target.
in-meta decimal

1 value header-bytes
8192 512 3 * - 3 - value end-headers
0 end-headers c!-t  \ End of list.
end-headers value current-header

\ Pointer to first header in list.
create start-headers  0 ,

m: next-header  (  - )
	current-header 6 - to current-header
	header-bytes 6 + to header-bytes
	current-header there 6 erase ;m

\ Followed by a name.
m: header  ( a - )
	next-header BL word dup count cr type
	dup c@ 3 min current-header swap
	for  >r count r> tuck c!-t 1 +  next  2drop
	current-header 4 + !-t ;m

\ Headers are relocatable, tack them onto end of dictionary to save
\ downloading time.
m: patch-headers  (  - )
	romHERE start-headers !-t
	current-header there romHERE there header-bytes move
	header-bytes romALLOT ;m

warnings off
\ 4 byte terminal input buffer.
variable tib 2 allot
warnings on

code erase-tib  (  - )
	A clr
	A tib 0 + direct mov
	A tib 1 + direct mov
	A tib 2 + direct mov
	A tib 3 + direct mov
	next c;

: echo  ( c - c) dup emit ;

code ++tib  (  - ) tib direct inc  next c;

: query  (  - )
	erase-tib 1  \ >in
	begin	key dup 33 < if  space 2drop exit  then  echo ++tib
		over 4 < if  over tib +  over swap c!  then
		drop 1 +
	again ;

\ : .tib  (  - ) tib count . count emit count emit count emit drop ;

: match  ( addr - flag)
\	tib 3 for
	tib 4 for
		over c@ over c@ - if  2drop 0 pop drop exit  then
		push 1 +  pop 1 +
	next  2drop -1 ;

: find  (  - addr flag)
	start-headers @
	begin	dup c@ while
		dup match if  4 + @ -1 exit  then  6 +
	repeat  0 ;

\ : go  (  - ) cr query .tib find . . go ;

: .ok  (  - ) s"  Ok" type cr ;

: huh?  (  - ) drop s"  ?" type cr abort ;

: interpret  (  - )
	depth 0< if  huh? exit  then
	query find if  execute .ok interpret exit  then  huh? ;

warnings off
: number  (  - n)
	0 begin
		key
		dup [char] 0 <  over [ char 9 1 + ] literal < not or if
			drop exit
		then
		echo [char] 0 - swap 10 * +
	again ;
' number header #
warnings on

' .s header .s
' . header .
' dup header dup
' drop header drop
' swap header swap
' + header +
' * header *
' - header -
' / header /
' mod header mod

