bootload_vector_explainer.md

Mon 14 Nov 13:20:06 UTC 2022

An attempt to explain the vectors and such.

Memory Map

9.2.1 Program Memory

Memory flash space is 0x0000 to 0x1DFF  (0xFFFF is 65535 decimal).

0x1DFF is 7679 - 8 kb on-chip flashROM.

Flash is read-only, unless the Program Store Write Enable bit
(PSCTL.0) is set; the MOVX write instruction handles this.



