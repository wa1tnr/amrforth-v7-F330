\ menus.fs
include ansi.fs

: top  (  - ) 0 0 at-xy ;
: bottom  (  - ) 0 rows at-xy ;
: newline  (  - ) cols rows at-xy cr ;

variable normal-background
: remember-background  (  - ) attr @ normal-background ! ;
: restore-background  (  - ) normal-background @ attr! ;

: menu-colors  (  - ) <a Cyan >bg White >fg bold a> attr! ;
: normal-colors  (  - ) <a Black >fg a> attr! ;

variable menu-string

create main-menu
	," Compile  Download  Interpret  Turnkey  conFigure "
main-menu menu-string !

\ These don't seem to work.
\ : save-cursor  (  - ) ESC[ 7 emit ;
\ : restore-cursor  (  - ) ESC[ 8 emit ;
\ : window  ( x y - ) 1+ swap 1+ swap ESC[ pn ;pn [char] r emit ;

: .menuline  (  - )
	newline top menu-colors
	menu-string @ count dup >r type  cols r> - spaces
	bottom normal-colors ;

: go  (  - ) page .menuline key drop ;


