\ gpio-init-abb.fs
\ Sat 11 Jan 21:05:18 UTC 2025

5 spaces .( loading gpio-init-abb.fs) cr

code initPCA
        $40 invert # PCA0MD anl  \ Clear watchdog enable bit.
	next c;

code initPortIO
        $07 # P1MDOUT mov
	$07 # P1MDIN orl  \ digital, not analog, P1.1, P1.0
        $13 # P1SKIP mov  \ accomodate pushbutton if wanted
	$40 # XBR1 mov    \ Enable crossbar and weak pull-ups.
	next c;

: init initPCA initPortiO ;

\ end.
