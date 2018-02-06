This is the generic README file for amrForth v7.

***** Release Note 09May05 *****
*****   Version 7.1.0 beta *****

As suggested by Wolfgang Allinger, we now compile the bootloader into
page zero of every application.  Finally all the code for an application
has it's source in the application directory.  Also we eliminated the
separate boot directories for each processor, where the standalone
bootloader code used to be.

We have found that in win2000 our amr.bat system doesn't work.  Gforth
is unable to call another instance of Gforth for some reason.  It works
fine in win98 and of course in Linux.  We have included the individual
batch files for compiling, downloading, and testing code in win2000, and
possibly also in XP.  The concept has been tested in win2000 though we
haven't tested the final code yet, as we don't have anything but win98
to test on.  The individual scripts also exist for Linux, so the amr and
amr.bat scripts are no longer necessary.  The see.bat script was changed
to show.bat, because in my projects I always have a script called see
that shows me my project archive.

In win2000 you will want to operate from a console window (DOS window?)
and use the c, d, t, and f commands to Compile, Download, Turnkey, and
Forth (interactive testing) you application.

See the history.txt file for more details about changes.

***** Release Note 03Dec04 *****
*****   Version 7.0 beta   *****

We have decided to stop using Tcl/Tk.  Gforth is used now for both the
compiler and the user interface.  It is text based, not a GUI.  The
system is now much smaller, simpler, and more responsive.  The key was
figuring out how to access the serial ports from Gforth in Windows.
In general, the comments below that refer to the target compiler are
still valid.  One change we made was to include a word called .rs in the
file debug.fs, which displays the return stack for the single stepper.
Otherwise we believe that the compiler, assembler, and kernel have not
changed.  The user interface has changed.

There is a script for starting the user interface, called 'amr' in Linux
and 'amr.bat' in Windows.  There is also a script called 'project' or
'project.bat' which moves needed project files into the directory of
your choice.  This stays in the amrforth/v7 home directory.  Another
script file in 'config' or 'config.bat', which must be run in a new
project directory before 'amr' or 'amr.bat' will work.  These scripts
are all copied to the project directory by the 'project' or
'project.bat' script.

Once amrForth has started, by running 'amr' or 'amr.bat', you will see
hints across the top of the screen, and configuration information across
the bottom, such as the target processor, the baudrate, and the com
port.

Type 'help' to get a slightly more verbose hint of what to do.

For Linux users, we have included a 'Makefile' for compiling your
applications.  This does essentially the same thing from the command
line that the 'c' or 'turnkey' commands do in amrForth.

The 'e' command in amrForth runs an external editor.  It is set for the
editors that we use at AM Research.  You can change to your favorite
editor by changing the definition of 'myeditor' at the top of the file
'system.fs'.  Do this in the /amrforth/v7 directory and project will
propagate it to your project directories as you make them.

***** Release Note 02Aug04 *****
*****     Version 6.4      *****

Minor bug fixes since 20May04.  Gforth 0.6.2 has been updated such that
it works in Windows 95, 98, and ME now.  We now recommend using Gforth
0.6.2 instead of 0.5.0.

***** Release Note 20May04 *****
*****     Version 6.4      *****

A gradual evolution has occurred from v6.3 and we have just now renamed
it to v6.4.x, for experimental.  The history.txt file spells out the
changes in detail.  One very noticeable change for Windows users is that
the GUI takes focus automatically now, not waiting for a mouse click.
This was irritating but took awhile to discover the cure for.  Also, for
both Linux and Windows, the display speed is dramatically increased in
the GUI, because we are now caching a whole line before feeding it to
Tcl/Tk to display, instead of feeding one character at a time.  The only
downside is that you may write a program that spits characters to the
host without sending a carriage return/linefeed, and the GUI will never
show those.  This should be a rare occurrence though, I hope.

All references below to 6.3 or 6_3 can (and should) be changed to 6.4 or
6_4 now.  It is possible for v6.3 and v6.4 to coexist since they use
different base directories.
 
***** Release Note 03Feb04 *****
*****     Version 6.3      *****
 
After getting comments from users testing version 6.3 some changes have
been made in anticipation of releasing it to all.

To help Windows users who can only have one . in a filename, we have
renamed the v6.3 directory to the v6_3 directory.

We have incorporated some of Wolfgang Allinger's comments into the
kernel8051.fs file to help make things more clear to people trying to
understand the source code.

When Bob Nash pointed out a bug in the morse code demo application we
decided to change the amrFORTH target compiler to fix it.  The target
compiler has two vocabularies, TARGET and IMMED.  The IMMED vocabulary
has words that are traditionally called immediate words in forth.  They
are words run on the host at compile time, words such as IF, THEN,
BEGIN, UNTIL.  The target compiler formerly searched first the IMMED
vocabulary and then the TARGET vocabulary.  If a new word was defined in
TARGET that had the same spelling as a word in IMMED then the TARGET
version would never be found, the IMMED version took precedence.  Now
the search order is reversed.  If a new word is defined in TARGET that
is the same as a word in IMMED, the the new target word takes
precedence.  In the process a few word names in TARGET were changed.

Some Tcl code was changed to allow the interpreter to find words that
contain {, }, or $.  These words were invisible to Tcl before.  When you
type 'words' Tcl still surrounds such words with { and }.  We expect to
fix this in a later version.

We tested the most recent versions of Tcl and Gforth.  Tcl offers no
problems.  Gforth 0.6.2 for linux has some new words that have same name
as some amrforth words.  We have surrounded those words with nowarn,warn
to suppress the compiler's 'not unique' message.

The README.linux and README.windows files have been updated and are now
included on the CD, along with this README.TXT file.

The compiler now fills page 0 with jumps into page 1 for the Cygnal
chips.  This is because the serial bootloader usually occupies page 0,
but if you download an application via JTAG the interrupt vectors were
not patched.  Now they are, along with the reset vector.

***** Release Note 16Jan04 *****
*****     Version 6.3      *****

This release has changed the way the data stack is implemented.  Now the
top of stack is cached in the A,R2 register pair.  This results a
substantially faster kernel, and takes less memory as well.  It was
originally believed that the A register could not be used to cache the
top of stack because it is used for almost everything in the 8051
instruction set.  It turns out we had to give up the @A+DPTR JMP
instruction and that's all.  It was replaced by  DPL PUSH DPH PUSH RET.
In order to optimize the logic instructions; and, or, xor, +, and -, the
order of bytes on the data stack was reversed.  The assembler macros
|dup and |drop hide this byte order, so that the application programmer
needn't worry about the change.

The disassembler has been ported into the Tcl/Tk environment, so now the
word 'see' works in the GUI, not just in gforth at the command line.
The word 'dis' will disassemble starting at an address, rather than at
the beginning of a word.  In case you don't have a label at the address
you want to disassemble.

Added support for the Atmel C51RC2 microcontroller.

The following assembler macros have been added; #and, #or, #xor, and
#lit.  They are each preceded by a 16 bit literal and assemble two
instructions inline.
    
    E.g.  [ $ff #and ]  assembles  $ff # A anl  0 # R2 anl

A number of immediate macros have also been defined in kernel8051.fs to
allow for hand optimizing your programs.  They are:

1-dup0=until  ( flag a - )
dupif  (  - a1 flag a2)
dup0=if  (  - a1 flag a2)
dup0<if  (  - a1 flag a2)
dup0<notif  (  - a1 flag a2)
dupuntil  ( flag a - )
dup0=until  ( flag a - )
dup0<until  ( flag a - )
dup0<notuntil  ( flag a - )
dup>r  ( n - n)
r>drop  (  - )

and they behave just as if you had kept the spaces in them, except
faster.  e.g.  dup0<notif  =  dup 0< not if
The single stepper doesn't know how to handle these words yet.

A Tcl bug has been fixed that made it hard to single step or disassemble
a word with certain punctuation in its name, such as ?dup.

Another command has been added to go with c for compile and t for
turnkey and d for download.  It is r for run.  The r command runs the c
command and then the d command.  This is just for convenience during
development.

The word s" has been restored to the kernel.  It is needed more often
than expected.  ." has also been added to debug.fs.

In addition to 'New Project' and 'Open Project', there is now 
'Update Project', which will overwrite the directory.  'New Project'
won't overwrite, to keep you from losing something you didn't mean to
lose.

***** Release Note 31Oct03 *****
*****     Version 6.2      *****

This release includes a single step debugger with source display.


***** Release Note 31Jan03 *****
*****     Version 6.0      *****

With this release amrForth has two windows.  One is the usual
interpreter window.  The other is an editor window.  Our reason for
adding an editor is that we are preparing for adding a single stepper
with source code display.  This is the first step.  Also it avoids
problems we were having with Notepad in windows.

*****

Version 6 is a pretty big change from version 5.

On the target we have changed from a direct threaded inner interpreter
to native machine code or call threading.  The new compiler tries to be
smart enough to change redundant calls to jumps, in the word exit, so
that efficient tail recursion is possible.  Most words now end in a jump
to the last word instead of a call followed by a return.  Also if you
use exit inside a definition, it will change a preceding call into a
jump.  This is a thinly disguised goto.

We have streamlined things a little bit in other ways.  Do,loop is no
longer supported.  It occupied over 200 bytes and was just too
complicated.  For,next is there when you need a counted loop.  For,next
counts down from the number on top of the stack to one, so that 10 for
next will execute 10 times.  This is a recent change.  The older version
counted to zero and executed 11 times.  The word i still exists, and
returns the index to the for next loop.  It is equivalent to r@.  Unlike
the version 5 for,next loop, this one uses a 16 bit index.

We have eliminated does> and ;code.  They seem to have less value in a
call threaded environment and waste space with dead code when not used.
Several other seldom used primitives have been eliminated too, such as
1+ 1- 2+ 2- tuck rot.  If you really need them they are easy to
implement and we can help you do so if need be.  (As of 16July2003, we
have restored does> and ;code.  We found we couldn't live without it
after all.  The new versions cost nothing until invoked however, no dead
code.)

It is possible now to switch back and forth between assembler and forth
in a single definition, if it seems to help solve a problem.  See the
definition of C2reset in the file ~/amrforth/v6_3/300/jtag/main.fs in
Linux and \amrforth\v6_3\300\jtag\main.fs in Windows for an example.

Another new feature is the ability to put heads on the target for a
stand alone interpreter.  See the file headers.fs in the same jtag
directory and main.fs for examples of usage.  Read carefully as the
heads are not included automatically.

On the host we are using Tcl/Tk in order to get a GUI and serial port
access in a fairly portable way on both Windows and Linux.  Version 5
was for Linux only.

Once everything is installed and configured the system works pretty much
the same in Windows and Linux.  One apparent difference is that the
serial port hangs sometimes in Windows and the only fix seems to be to
exit and restart Windows.  You might be able to avoid the problem by
always typing bye to exit instead of clicking on the window's close
button.  Actually I haven't seen this problem for quite some time.  If
you see it, or anything else puzzling, let us know by email at
support@amresearch.com.  Any comments or suggestions are welcome.  (As
of 16July03, we think this problem has been solved.  If it happens to
you with the latest system, please let us know!).

Please look at the HISTORY file in ~/amrforth/v6_3 or \amrforth\v6_3.
We are keeping a TODO list at the top and documenting bug fixes and
feature additions below.  Once again comments and suggestions are
welcome.

Also look at the README.windows or README.linux files in the linux and
windows subdirectories on the CD.  They will help with installation.

We have begun to integrate an editor into amrForth v6.  It is based on a
free editor written in Tcl/Tk by Joseph Acosta.  The full tknotepad has
been included in a subdirectory off amrforth/v6_3/.  It is based on
window's notepad, but is superior in some respects such as command line
switches and extensibility.

Good luck, have fun, and let us know how it works.

