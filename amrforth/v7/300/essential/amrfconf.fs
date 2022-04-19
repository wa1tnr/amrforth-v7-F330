: processor  s" C8051F300" ;
: sfr-file  s" sfr-f300.fs" ;
: downloader  s" download-cygnal.fs" ;
9600 constant baudrate
create frequency 24500000 ,
250 constant default-TH1
true constant smod?
611 constant rom-start
create polled-kernel
1 value com?
