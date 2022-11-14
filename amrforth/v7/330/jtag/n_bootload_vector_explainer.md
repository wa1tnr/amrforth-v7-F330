# Bootload Vector Explained

*An attempt to explain the vectors and such.*

## Memory Map

### 9.2.1 Program Memory

Memory flash space is `0x0000` to `0x1DFF`  (`0xFFFF` is `65535` decimal).

`0x1DFF` is `7679` - 8 kb on-chip flashROM.

Flash is read-only, unless the **Program Store Write Enable** bit
(**`PSCTL.0`**) is set; the `MOVX` write instruction handles this.


**From bootloader330.fs**

> The host PC downloader (running **gforth**) repeatedly sends `$a5` to
> the target system until it gets a `$5a` in response. Then host sends
> `"amr"`. The target responds with `$a5`. The host sends a byte containing
> the number of `512` byte pages to be sent. The target erases those
> pages and responds with `$5a`.
> 
> Then the host sends pages beginning with `page 1`, at address `$200`
> or `512` decimal. The target responds after each page is received,
> with a byte containing the loop counter, i.e. number of pages remaining,
> including the one just received. In other words, it counts down from
> the total number of pages, to `1`.
>
> Flash program memory can't be written unless `PSCTL.0` is set. This
> bit is clear during normal operation. It is only set in the bootloader
> immediately before `DPTR` is loaded with `$200`. Even if a rogue
> program jumps into the middle of the bootloader, it can't overwrite
> the bootloader, because `DPTR` is loaded with `$200` in the
> bootloader right after `PSCTL.0` is set. The only way the bootloader
> might be ruined is by having `PSCTL.0` set in the user's code&hellip;
> and then accidentally jumping into the bootloader, at the point where
> `DPTR` was just loaded with `$200`.
> 
> Not very likely.
>
> The bootloader sets up the crossbar such that:

    TX= P0.4
    RX= P0.5

```
in-meta decimal

\ Vector the interrupts into page 1.
	0 there $63 erase  \ Clear interrupt vectors.

label INTERRUPT-VECTORS
	$03 org  $203 ljmp
	$0b org  $20b ljmp
	$13 org  $213 ljmp
	$1b org  $21b ljmp
	$23 org  $223 ljmp
	$2b org  $22b ljmp
	$33 org  $233 ljmp
	$3b org  $23b ljmp
	$43 org  $243 ljmp \ reserved; not used on F310 nor F330.
	$4b org  $24b ljmp
	$53 org  $253 ljmp
	$5b org  $25b ljmp
	$63 org  $263 ljmp
	$6b org  $26b ljmp \ reserved; used on F310 as Comparator1
	$73 org  $273 ljmp

	$7b org  \ Code starts here, after interrupts (on both F310 F330).

\ ------------------------------------------------------

\ Entry point of the main program if not bootloading.
romHERE ( *) $200 org
label main  c; ( *) org
```

<!--
`1234567890123456789012345678901234567890123456789012345678901234567..]..345`
-->

**Mon 14 Nov 13:20:06 UTC 2022**

**n_bootload_vector_explainer.md**

**END.**
