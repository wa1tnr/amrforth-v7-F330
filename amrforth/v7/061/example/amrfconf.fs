: processor  s" C8051F06x" ;
: sfr-file  s" sfr-f061.fs" ;
: downloader  s" download-cygnal.fs" ;
9600 constant baudrate
create frequency 24500000 ,
97 constant default-TH1
true constant smod?
691 constant rom-start
create polled-kernel
1 value com?
