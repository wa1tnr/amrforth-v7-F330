\ c2.fs
include jtag_c2.fs

only forth also root definitions
vocabulary c2
: wait only c2 quit ;
only forth also c2 also definitions
: download  (  - ) s" C2download" download-all ;
: erase  (  - ) s" C2erase" simple ;
: dump  (  - ) s" C2dump" simple ;
: next  (  - ) s" C2next" simple ;
: n next ;
: words words ;
: bye bye ;
: .s .s ;

wait

