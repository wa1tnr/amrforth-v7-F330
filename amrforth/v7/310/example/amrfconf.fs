: processor  s" C8051F310" ;
: sfr-file  s" sfr-f310.fs" ;
: downloader  s" download-cygnal.fs" ;
9600 constant baudrate
create frequency 24500000 ,
250 constant default-TH1
true constant smod?
635 constant rom-start
create polled-kernel
1 value com?
