\ job.fs
cr .( job.fs loading w timestamp of )
   .( Sat 11 Jan 21:05:18 UTC 2025) cr

include stdlib.fs
include clocks-tut-01.fs

0 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  include main.fs
  include little.fs
[then]

\ end.
