\ adc017.fs
\ Using the 10 bit A/D on the f017.

14 value bitsofadc

code init-adc  (  - )
	0 # AMX0CF mov    \ All inputs are single ended.
	0 # AMX0SL mov    \ Select AIN0, single ended.
	$80 # ADC0CF mov  \ $80 SAR=16 gain=1
	$80 # ADC0CN mov  \ $80 Always tracking
	$03 # REF0CN mov  \ Internal Ref., bias on, temp sensor off.
	next c;

10 bitsofadc = [if]
code adc@  (  - n)  c; [then]
code adc10@  (  - n)
	$90 # ADC0CN mov  \ Start the conversion.
	|dup
	begin  5 .ADC0CN set? until  \ Wait for conversion complete.
	ADC0l A mov  ADC0H R2 mov
	next c;

12 bitsofadc = [if]
code adc@  (  - n)  c;
: adc12@  (  - n) 2 15 for  adc10@ +  next  2/ 2/ ;
[then]


14 bitsofadc = [if]
\ : slow-adc14@  (  - n)
\	8 0 255 for  adc@ 0 d+  next  16 um/mod nip ;
code adc@  (  - n)  c;
code adc14@  (  - n)
	|dup
	A clr  A R5 mov  A R6 mov  A R7 mov
	8 # A mov  A R4 mov  \ Start with 0.5, for rounding later.
	begin	$90 # ADC0CN mov  \ Start the conversion.
		begin  5 .ADC0CN set? until  \ Wait for conversion.
		ADC0L A mov  R4 A add  A R4 mov
		ADC0H A mov  R5 A addc  A R5 mov
		A clr  R6 A addc  A R6 mov
	R7 -zero until
	4 # R7 mov
	begin	C clr
		R6 A mov  A rrc  A R6 mov
		R5 A mov  A rrc  A R5 mov
		R4 A mov  A rrc  A R4 mov
	R7 -zero until
    R4 A mov  R5 'R2 mov
	next c;
[then]

16 bitsofadc = [if]
\ : slow-adc16@  (  - n)
\	32 0 4095 for  adc@ 0 d+  next  64 um/mod nip ;

code adc@  (  - n)  c;
code adc16@  (  - n)
	|dup
	A clr  A R5 mov  A R6 mov
	32 # A mov  A R4 mov  \ Start with 0.5, for rounding later.
	16 # B mov
	begin	\ A total of 4096 samples accumulated.
		A clr  A R7 mov
		begin	4 .ADC0CN setb  begin  4 .ADC0CN clr? until
			ADC0L A mov  R4 A add  A R4 mov
			ADC0H A mov  R5 A addc  A R5 mov
			A clr  R6 A addc  A R6 mov
		R7 -zero until
	B -zero until
	6 # R7 mov   \ Divide by 64, 2^6.
	begin	C clr
		R6 A mov  A rrc  A R6 mov
		R5 A mov  A rrc  A R5 mov
		R4 A mov  A rrc  A R4 mov
	R7 -zero until
	R4 A mov  R5 'R2 mov
	next c;
[then]

\S Timing tests.

\ 1,000,000 iterations.
: test10  (  - ) 100 for 10000 for  adc@ drop  next next ;

\ 10,000 iterations.
: test12  (  - ) 10000 for  adc12@ drop  next ;

\ 1000 iterations, high level.
: test14slow  (  - ) 1000 for  slow-adc14@ drop  next ;

\ 1000 iterations, coded.
: test14  (  - ) 1000 for  adc14@ drop  next ;

\ 100 iterations, high level.
: test16slow  (  - ) 100 for  slow-adc16@ drop  next ;

\ 100 iterations, coded.
: test16  (  - ) 100 for  adc16@ drop  next ;
