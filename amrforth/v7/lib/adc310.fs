\ adc310.fs

\ Use P3.1 by default, but can be changed by 'use-3.n'.
code init-adc  (  - )
	$11 # AMX0P mov  \ Input channel = P3.1.
	$1f # AMX0N mov  \ Single ended input.
	$0a # REF0CN mov  \ VREF=Vdd, Bias on.
	$e1 # P3MDIN mov  \ Analog input at P3.1,2,3,4.
	$40 # ADC0CF mov  \ Configuration, right justified.
	$80 # ADC0CN mov  \ Start tracking, ready to convert.
	next c;

code use-3.1  (  - ) $11 # AMX0P mov  next c;
code use-3.2  (  - ) $12 # AMX0P mov  next c;
code use-3.3  (  - ) $13 # AMX0P mov  next c;
code use-3.4  (  - ) $14 # AMX0P mov  next c;

code adc@  (  - n)
	4 .ADC0CN setb  \ Start a conversion.
	' dup call
	begin  5 .ADC0CN set? until  \ Wait for conversion.
	5 .ADC0CN clr
	ADC0H 'R2 mov  ADC0L A mov
	next c;

: adc1@  (  - n) use-3.1 adc@ ;
: adc2@  (  - n) use-3.2 adc@ ;
: adc3@  (  - n) use-3.3 adc@ ;
: adc4@  (  - n) use-3.4 adc@ ;

