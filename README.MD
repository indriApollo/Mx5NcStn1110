# Mx5NcStn1110

### References

https://www.scantool.net/scantool/downloads/98/stn11xx21xx_frpm-c.pdf

https://github.com/timurrrr/RaceChronoDiyBleDevice/blob/master/can_db/mazda_mx5_nc.md  
http://www.madox.net/blog/projects/mazda-can-bus/

### Create a fake virtual serial port pair on Linux

`socat -dd pty,rawer,echo=0 pty,rawer,echo=0`

Will create two serial ports at `/dev/pts/*`
