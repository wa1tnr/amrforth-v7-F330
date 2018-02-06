# amrforth-v7-F330
amrFORTH v7.1.0_beta +C8051F330 +CP2104 for gforth 0.6.2

This project produces a functioning JTAG/C2 interface between
a pair of C8051F330 MCU's.  Serial bootloading/downloading is
supported through Gforth 0.6.2 (but no later).  Some amrFORTH
functionality commented-out to mesh with Gforth 0.6.2 -- these
functions work well with later Gforth versions (0.7.x) (or at
least do not obstruct more common uses of the system).

However, this version of amrFORTH does not permit serial bootload/
download when used with later versions of Gforth (at least not
with CP2104 PiUART, the intended interface to the host PC).

As it took many hours of troubleshooting to come up with that
point of view, it may be in error.  The project was looking to
fail the serial download capability, until the rollback of
Gforth strategy was tried.  That works, and is the current
state of this project.
