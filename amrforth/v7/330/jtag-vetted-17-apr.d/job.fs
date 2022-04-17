\ job
include headers.fs
include main.fs

\ AmrForth v6 expects a bootloader at zero page to jump to its entry
\ point at address $200.  This patches the reset vector to jump to $200
\ for a board without the amrForth bootloader.

in-assembler
romHERE ( *)
0 org  $200 ljmp
( *) org
in-meta

