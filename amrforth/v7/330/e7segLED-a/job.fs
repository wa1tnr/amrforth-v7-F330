\ job.fs

\ all live code - no comments at all.
\ therefore the commented lines should be uncommented, unless testing (as is the case at this moment).
\ 08 Mar 2018  2223z

: mine (  - ) 55 drop ;
1 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  include main.fs
  include little.fs
[then]
