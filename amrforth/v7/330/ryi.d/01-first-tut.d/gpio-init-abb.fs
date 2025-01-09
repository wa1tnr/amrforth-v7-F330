\ gpio-init-a.fs
\ Thu  9 Jan 17:17:01 UTC 2025

5 spaces .( loading gpio-init-a.fs) cr

code initPCA
        $40 invert # PCA0MD anl  \ Clear watchdog enable bit.
        $00 # PCA0MD anl \ amr bootloader disagrees or omits this
	next c;

code initPortIO
        \ $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
	\ $ff # P0MDIN orl   \ No analog, all digital, port 0.x

        $07 # P1MDOUT mov
	$07 # P1MDIN orl   \ digital, not analog, P1.1, P1.0
        $13 # P1SKIP mov
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
	next c;

: init initPCA initPortiO ;

\ end.
