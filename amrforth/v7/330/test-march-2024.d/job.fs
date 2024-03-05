\ job.fs
include ustimer-cwh.fs
include main.fs
: mine (  - ) 55 drop ;
0 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  include main.fs
  include little.fs
[then]
