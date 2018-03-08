\ jtag.fs
include jtag_c2.fs

only forth also root definitions
vocabulary jtag
: wait only jtag quit ;
only forth also jtag also definitions
: download  (  - ) s" jtag" download-all ;
: erase  (  - ) s" erase-all" simple ;
: dump  (  - ) s" dump" simple ;
: next  (  - ) s" next" simple ;
: n next ;
: run  (  - ) s" run" simple ;
: reset  (  - ) s" reset" simple ;
: halt  (  - ) s" halt" simple ;
: suspend  (  - ) s" suspend" simple ;
: words words ;
: bye bye ;
: .s .s ;

wait

