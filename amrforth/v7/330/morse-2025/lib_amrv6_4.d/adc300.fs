\ adc300.fs   8 bit adc for g300
in-compiler

code init-adc  (  - )
    $80 # AMX0SL mov  \ Single ended, P0.0
    $41 # ADC0CF mov  \ Gain=1, Timing ???
    $80 # ADC0CN mov  \ ADC is enabled.
    $0a # REF0CN mov  \ Use VDD as voltage reference.
    $fe # P0MDIN anl  \ P0.0 analog input.
    $01 # XBR0 orl    \ Skip over P0.0
    next c;

code adc@  (  - c)
    $90 # ADC0CN mov
    begin	5 .ADC0CN set? until
    |dup  ADC0 A mov  0 # R2 mov
    next c;

