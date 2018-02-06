gforth -m 1M -e "true constant windows" ./interpret.fs $1 -e "cr text_normal bye"
rem A file full of commands is executed on the target.
rem Begin each line with please for command to reach target,
rem otherwise host executes the command.
