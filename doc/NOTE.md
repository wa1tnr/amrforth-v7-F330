Wed 13 Apr 12:01:55 UTC 2022

Definitive:

  gforth v7 compiles cleanly (with a bit of work)
  but simply will not download.  c2 is okay, though.

  The only solution found is to use gforth v6 with
  this F330D port, or only use c2 when it's time
  to upload firmware to the target.

  Specific check: tried the max3232 interface (which
  still works fine).  Did not help the gforth v6/v7
  issue, however.

  Easiest solution was to download the packaged .deb
  from a Debian repository, to acquire the two files
  needed (gforth-0.6.2 and gforth.fi).

  Make a soft link:

    $ ln -s gforth-0.6.2 gforth

  Use ./gforth to call gforth in the shell scripts
  for things like ./d and ./c

    $  cat c
   ./gforth -m 1M -e false ./compile.fs -e bye
    $  cat c2
   ./gforth -m 1M -e "false constant windows" ./c2.fs -e quit
    $  cat d
   ./gforth -m 1M ./amrfconf.fs -e "false constant windows downloader included"
    $  cat f
   ./gforth -m 1M -e "false constant windows" ./interpret.fs -e "-1 to alone? black background go"


Some places to inspect for issues related to the path to gforth:

   d:1:./gforth -m 1M ./amrfconf.fs -e "false constant windows downloader included"
   c2:1:./gforth -m 1M -e "false constant windows" ./c2.fs -e quit
   c:1:./gforth -m 1M -e false ./compile.fs -e bye
   colorscheme.fs:33:		s" ./gforth -m 1M " pad place ;
   colorscheme.fs:37:		s" ./gforth -m 1M " pad place ;
   f:1:./gforth -m 1M -e "false constant windows" ./interpret.fs -e "-1 to alone? black background go"
   amr:2:./gforth -e "false constant windows" ./system.fs

Note that in each instance, the path to gforth was restricted to the
version of it copied to the current directory (v6 in this case).

The idiom './gforth' of course points to the soft link to the
longer-named binary file ./gforth-0.6.2 which is present (along
with 'gforth.fi') in the current working directory.

END.
