@echo off
if exist %1\nul goto copier
echo Where is the project?
goto bottom
:copier
copy amr %1
copy amr.bat %1
copy config %1
copy config.bat %1
copy config.fs %1
copy compile.fs %1
copy vtags.fs %1
copy metacomp.fs %1
copy asm8051.fs %1
copy sfr-31.fs %1
copy sfr-32.fs %1
copy sfr-537.fs %1
copy sfr-552.fs %1
copy sfr-812.fs %1
copy sfr-816.fs %1
copy sfr-f000.fs %1
copy sfr-f061.fs %1
copy sfr-f300.fs %1
copy sfr-f310.fs %1
copy kernel8051.fs %1
copy debug.fs %1
copy end8051.fs %1
copy bin2hex.fs %1
copy basic.fs %1
copy basicinterpreter.fs %1
copy system.fs %1
copy strings.fs %1
copy ansi.fs %1
copy colorscheme.fs %1
copy dis8051.fs %1
copy interpret.fs %1
copy singlestep.fs %1
copy download-cygnal.fs %1
copy download-aduc.fs %1
copy download-oldamr.fs %1
copy serial-linux.fs %1
copy serial-windows.fs %1
copy c2.fs %1
copy jtag.fs %1
copy jtag_c2.fs %1
copy mathunsigned.fs %1
copy mathfloored.fs %1
copy mathsymmetric.fs %1
copy bootloader.fs %1
copy bootloader300.fs %1
copy bootloader310.fs %1
copy bootloader017.fs %1
copy bootloader061.fs %1
copy c.bat %1
copy d.bat %1
copy t.bat %1
copy b.bat %1
copy f.bat %1
copy c2.bat %1
copy jtag.bat %1
copy show.bat %1
:bottom
