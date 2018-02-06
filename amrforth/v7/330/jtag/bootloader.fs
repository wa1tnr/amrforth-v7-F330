\ bootloader.fs
processor s" C8051F300" $= [if]  include bootloader300.fs  [then]
processor s" C8051F310" $= [if]  include bootloader310.fs  [then]
processor s" C8051F330" $= [if]  include bootloader330.fs  [then]
processor s" C8051F017" $= [if]  include bootloader017.fs  [then]
processor s" C8051F06x" $= [if]  include bootloader061.fs  [then]

