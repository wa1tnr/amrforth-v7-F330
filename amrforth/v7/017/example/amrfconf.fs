: processor  s" C8051F017" ;
: sfr-file  s" sfr-f000.fs" ;
: downloader  s" download-cygnal.fs" ;
9600 constant baudrate
create frequency 24000000 ,
243 constant default-TH1
true constant smod?
691 constant rom-start
create polled-kernel
1 value com?
