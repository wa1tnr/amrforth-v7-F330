\ code 2p0 2 .P0 cpl next c;

: init startup ;

: test init
  begin
      \ ." test program 22:42z Fri 15 Apr" cr 2600 ms
      ." test program 12:56z Mon 18 Apr" cr 2600 ms
  again ;

: run test ;

\ : go 1 drop begin 1 drop again -;
\ : go init 100 ms localtest begin testtt again -;
