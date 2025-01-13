\ adc017int.fs
\ Using the 10 bit A/D on the f017 in an interrupt routine,
\ over-sampled to get 14 bits of data.

$90 constant START-CONVERSION

\ Variables for use only in assembler definitions.
\ No need for forth executables on the target.
a: adc-flag    [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: adc-counter [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: total1      [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: total2      [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: total3      [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: acc1        [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: acc2        [[ cpuHERE ]] literal direct ;a  1 cpuALLOT
a: acc3        [[ cpuHERE ]] literal direct ;a  1 cpuALLOT

code init-adc  (  - )
	ACC push
	$80 invert # IE anl  \ Disable interrupts globally.
	A clr
	A adc-counter mov  \ 256 samples.
	$08 # acc1 mov  \ Start accumulator with 0.5, for rounding.
	A acc2 mov
	A acc3 mov
	A adc-flag mov  \ Total not ready.
	$00 # AMX0CF mov    \ All inputs are single ended.
	$00 # AMX0SL mov    \ Select AIN0, single ended.
	$80 # ADC0CF mov  \ $80 SAR=16 gain=1
	$80 # ADC0CN mov  \ $80 Always tracking
	$03 # REF0CN mov  \ Internal Ref., bias on, temp sensor off.
\ enable the interrupt and start the first conversion.
	$02 # EIE2 orl
	$80 # IE orl
	START-CONVERSION # ADC0CN mov
	ACC pop
	next c;

label adcint
	ACC push  PSW push
\ Accumulate this sample.
	ADC0L A mov   acc1 A add   A acc1 mov
	ADC0H A mov   acc2 A addc  A acc2 mov
	A clr  acc3 A addc  A acc3 mov
\ Start the next conversion, clear interrupt flag.
	START-CONVERSION # ADC0CN mov
\ Decrement the counter.
	adc-counter dec
\ Leave unless finished accumulating.
	adc-counter A mov  0<> if
		PSW pop  ACC pop  reti
	then
\ Save total and restart accumulation.
	A clr
	acc1 total1 mov  8 # acc1 mov
	acc2 total2 mov  A acc2 mov
	acc3 total3 mov  A acc3 mov
	1 # adc-flag mov
	PSW pop  ACC pop  reti c;
adcint $7b int!

code adc@  (  - n)
	|dup
	begin  adc-flag A mov  0<> until  0 # adc-flag mov
	IE push  \ Save the interrupts flag.
	$7f # IE anl  \ Disable interrupts globally.
	total1 R1 mov
	total2 R2 mov
	total3 R3 mov
	IE pop  \ Restore the interrupts flag.
	4 # R7 mov  \ Divide by 16.
	begin	C clr
		R3 A mov  A rrc  A R3 mov
		R2 A mov  A rrc  A R2 mov
		R1 A mov  A rrc  A R1 mov
	R7 -zero until
	R1 A mov
\	R4 A mov  Apush
\	R5 A mov  Apush
	next c;
