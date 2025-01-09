\ Tue 31 Dec 20:54:14 UTC 2024

: allrlow ( - )
  7 .P1 clr
  6 .P1 clr
  5 .P1 clr
  4 .P1 clr
  3 .P1 clr
  \ 2 .P1 clr
  1 .P1 clr
  0 .P1 clr
  7 .P0 clr
  6 .P0 clr
  3 .P0 clr
  2 .P0 clr
  1 .P0 clr
  0 .P0 clr
;

code initPCA
        $40 invert # PCA0MD anl  \ Clear watchdog enable bit.
        $00 # PCA0MD anl \ amr bootloader disagrees or omits this
	next c;

code initPortIO
        $cf # P0MDOUT orl  \ all pins output, push pull -- except 4 and 5
	$ff # P0MDIN orl   \ No analog, all digital, port 0.x

        $ff # P1MDOUT orl
	$ff # P1MDIN  orl  \ sketchy

	$01 # XBR0 mov  \ Enable TX and RX on P0.4, P0.5.
	$40 # XBR1 mov  \ Enable crossbar and weak pull-ups.
	next c;

: init initPCA initPortiO allrlow ;

\ end.
