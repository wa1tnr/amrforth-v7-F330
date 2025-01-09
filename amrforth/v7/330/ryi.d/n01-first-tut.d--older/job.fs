\ job.fs

include ustimer-cwh.fs
include gpio-lib-a.fs
include gpio-init-a.fs

0 [if] \ a zero here will disable this code block
  include ustimer-cwh.fs
  include main.fs
  include little.fs
[then]

