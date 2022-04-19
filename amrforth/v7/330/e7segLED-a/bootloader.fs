\ bootloader.fs
processor s" C8051F300" $=ngfv6 [if]  include bootloader300.fs  [then]
processor s" C8051F310" $=ngfv6 [if]  include bootloader310.fs  [then]
processor s" C8051F330" $=ngfv6 [if]  include bootloader330.fs  [then]
processor s" C8051F017" $=ngfv6 [if]  include bootloader017.fs  [then]
processor s" C8051F06x" $=ngfv6 [if]  include bootloader061.fs  [then]
