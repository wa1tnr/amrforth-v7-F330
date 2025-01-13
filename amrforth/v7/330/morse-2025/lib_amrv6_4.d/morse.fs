\ morse.fs
cr
.( morse.fs - closely patched upstream for )
.( Mon 13 Jan 16:39:38 UTC 2025) cr

\ Use the kernel with the serial interrupt to be safe.

\ ----- Using the Programmable Counter Array ----- /
\ ----- as a millisecond timer ----- /

code init-pca  (  - )
    $08 # PCA0MD mov    \ Sysclk/1
    $49 # PCA0CPM0 mov  \ Software Timer (Compare) Mode
    $40 # PCA0CN mov    \ Start the timer.
    $10 # EIE1 orl      \ Enable pca interrupt, F330D
    $80 # IE orl        \ Enable global interrupts.
    next c;

\ 24.5 MHz = 24500 cycles per millisecond.
24500 $ff and constant ms-lo
24500 256 / $ff and constant ms-hi

cvariable wpm-counter

label pca-interrupt  (  - )
    ACC push  PSW push
    0 .PCA0CN set? if
        wpm-counter direct A mov
        0<> if  wpm-counter direct dec  then
        \ Set up for next interrupt in 1 ms.
        ms-lo # A mov  PCA0CPL0 A add  A PCA0CPL0 mov
        ms-hi # A mov  PCA0CPH0 A addc  A PCA0CPH0 mov
    then
    $40 # PCA0CN mov  \ Clear interrupt bit.
    PSW pop  ACC pop
    reti c;
pca-interrupt $5b int! \ $5b on F330D same as $4b F300

code ms  ( c - )
    A wpm-counter direct mov
    begin  wpm-counter direct A mov  0= until
    |drop  next c;

\ Using PARIS as the standard sized word,
\ there are 50 counts or space values per word.
\ ms = (1/((wpm*50)/60sec/min))*1000
\ ms = 1000/(wpm*50/60)
\ ms = 1000*60/wpm*50
\ ms = 1200/wpm

cvariable duration
: wpm  ( wpm - ) 1200 swap / duration c! ;

\ ----- Using Timer2 to make the sound ----- /

code t!  ( n - )
    R2 TMR2RLH mov  A TMR2RLL mov
    |drop  next c;

code noise-on  (  - )
    \ Auto-reload, timer mode, timer on.
    $04 # TMR2CN mov
    5 .IE setb  7 .IE setb
    next c;

code noise-off  (  - )
    5 .IE clr
    $00 # TMR2CN mov  \ Timer off.
    2 .P0 clr \ for LED flasher
    next c;

code init-timer2  (  - )
    $04 # P0MDOUT orl  \ pin 0.2 output.
    next c;

label t2-interrupt
    2 .P0 cpl      \ Toggle noise maker.
    7 .TMR2CN clr  \ Clear interrupt bit.
    reti c;
t2-interrupt $2b int!

\ T2 runs at a rate of 24.5 Mhz divided by 12.
\ This means 24.5/12=2.0416666 us per step.
\ That equals 0.000002041666 sec per step.
\ Invert that to get 489796 steps per sec.
\ Frequency = cycles per sec = (steps/sec)/steps
\ Let n = number of steps per toggle, that's half a cycle.
\ freq = (489796/n)*2  (Multiply by 2 for a full cycle.
\ freq = 979592/n
\ n = 979592/freq
\ subtract n from 65535 and store that
\ in the timer reload registers.

: hz>steps  ( n1 - n2)
    30 max 2000 min
    push 979592. pop um/mod nip ;

\ Specify the frequency of the sound in Hz.
: hz  ( n - ) init-timer2 hz>steps invert t! ;

\ ----- Morse Code Characters ----- /
\ letters sent high speed with spacing for lower overall speed

: spc  (  - )
  duration c@ \ ms ;
  dup ms
  dup ms
  ms ; \ double or more, the canonical time

: spc-el  (  - ) duration c@ ms ; \ standard length dot dash elements
: lsp  (  - ) spc spc ;
: wsp  (  - ) spc spc spc spc ;
: di   (  - ) noise-on spc-el noise-off spc-el ;
: dit  (  - ) di ; \ alias F330D
: dah  (  - ) noise-on spc-el spc-el spc-el noise-off spc-el ;

: /a   di dah           lsp ;
: /b   dah di di dit    lsp ;
: /c   dah di dah dit   lsp ;
: /d   dah di dit       lsp ;
: /e   dit              lsp ;
: /f   di di dah dit    lsp ;
: /g   dah dah dit      lsp ;
: /h   di di di dit     lsp ;
: /i   di dit           lsp ;
: /j   di dah dah dah   lsp ;
: /k   dah di dah       lsp ;
: /l   di dah di dit    lsp ;
: /m   dah dah          lsp ;
: /n   dah dit          lsp ;
: /o   dah dah dah      lsp ;
: /p   di dah dah dit   lsp ;
: /q   dah dah di dah   lsp ;
: /r   di dah dit       lsp ;
: /s   di di dit        lsp ;
: /t   dah              lsp ;
: /u   di di dah        lsp ;
: /v   di di di dah     lsp ;
: /w   di dah dah       lsp ;
: /x   dah di di dah    lsp ;
: /y   dah di dah dah   lsp ;
: /z   dah dah di dit   lsp ;

: /0   dah dah dah dah dah   lsp ;
: /1   di  dah dah dah dah   lsp ;
: /2   di  di  dah dah dah   lsp ;
: /3   di  di  di  dah dah   lsp ;
: /4   di  di  di  di  dah   lsp ;
: /5   di  di  di  di  dit   lsp ;
: /6   dah di  di  di  dit   lsp ;
: /7   dah dah di  di  dit   lsp ;
: /8   dah dah dah di  dit   lsp ;
: /9   dah dah dah dah dit   lsp ;

: full_stop         di  dah di  dah di  dah   lsp ;
: comma             dah dah di  di  dah dah   lsp ;
: colon             dah dah dah di  di  dit   lsp ;
: question_mark     di  di  dah dah di  dit   lsp ;
: apostrophe        di  dah dah dah dah dit   lsp ;
: hyphen            dah di  di  di  di  dah   lsp ;
: fraction_bar      dah di  di  dah dit       lsp ;
: parentheses       dah di  dah dah di  dah   lsp ;
: quotation_mark    di  dah di  di  dah dit   lsp ;

: within  ( n lo hi - flag) over - push - pop u< ;

: upc  ( c1 - c2)
    dup [char] a [ char z 1 + ] literal
    within if  $df and  then ;

: bad  (  - ) ;

: translate  ( c - )
    dup emit
    upc -31 + 0 max 65 min exec:
    bad     \ All control characters
    wsp     \ BL
    quotation_mark \ "
    bad     \ !
    bad     \ #
    bad     \ $
    bad     \ %
    bad     \ &
    apostrophe      \ '
    parentheses     \ ( )
    parentheses     \ ( )
    bad     \ *
    bad     \ +
    comma   \ ,
    hyphen  \ -
    full_stop       \ .
    fraction_bar    \ /
    /0 /1 /2 /3 /4 /5 /6 /7 /8 /9
    colon   \ :
    bad     \ ;
    bad     \ <
    bad     \ =
    bad     \ >
    question_mark   \ ?
    bad     \ @
    /a /b /c /d /e /f /g /h /i /j /k /l /m
    /n /o /p /q /r /s /t /u /v /w /x /y /z
    bad     \ [
    bad     \ \
    bad     \ ]
    bad     \ ^
    bad     \ _
    bad     \ the rest
    -;

: send  ( addr len - )
    for  count translate  next  drop ;

: x-on   (  - ) $11 emit ;  \ ^Q for Continue
: x-off  (  - ) $13 emit ;  \ ^S for Stop

\ This is where the x-on/x-off is handled.
\ We only turn x-on when we have an empty buffer.
\ This is as conservative as we can get.
: key-echo  (  - c)
    key? not if  x-on  then  key x-off dup emit ;

: get-number  (  - n)
    0
    begin
        key-echo
        dup [char] 0 [ char 9 1 + ] literal
        within not if  drop exit  then
        [ char 0 negate ] literal + swap 10 * +
    again -;

: change-hz   (  - ) get-number hz ;
: change-wpm  (  - ) get-number wpm ;

: check  ( c1 - c2)
    dup [char] ` = if
        drop key-echo dup [char] \ = if
            drop key-echo upc
            dup [char] H = if
                drop change-hz key-echo exit
            then
            dup [char] W = if
                drop change-wpm key-echo exit
            then
        then
    then ;

: init  (  - ) init-pca  1300 Hz  10 wpm ; \ 5 okay

: go  (  - )
    init begin  key-echo check translate  again -;

\ ----- Interactive testing ----- /

\ Should take 1 minute at 5 wmp.
: paris5  (  - )
    s" paris paris paris paris paris" send ;

\ about 7 wpm with current timings
: paris7 (  - )
    s" paris paris paris paris paris " send
    s" paris paris " send ;

: paris (  - ) paris7 ; \ alias

: sos  (  - )  s" sos" send ;
: abc  s" abc" send ;
: 123  s" 123" send ;
: test s" The quick brown fox jumped over the lazy dog." send ;

\ end.
