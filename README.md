# MSI fan speed control utility

An alternative to rather bloat utilities provided by MSI itself.

# Platform support

As for now only 64-bit Windows is supported, but according to [this](https://wiki.ubuntu.com/Kernel/Reference/WMI)
it's possible to make WMI ACPI work under Linux, so ``definitions.mof`` file from this project and some
hacking should be enough to port this thing to Linux.

# Building

Can be built out-of-the-box by ``Visual Studio 2015``, solution/project files can be rather easily upgraded/downgraded to desired
version of studio/msvc compiler as no ultra-modern cutting edge MS technologies are used.

You'll need those files from ``MsiFanControl\bin\Release`` or ``MsiFanControl\bin\Debug``:
```
CLAP.dll
MsiFanControl.exe
MsiFanControl.exe.config
MsiWmiAcpiMof.dll
MsiWmiAcpiMof.reg
```

# Using

## First use

Run ``msifancontrol install`` and follow instructions. This is needed only once, and changes are system-wide 
(meaning it's not necessary to do this for each user profile, just once will be enough)

_Pro tip:_ use ``msifancontrol install /y`` for Chuck Norris mode (no questions asked)

## Using different profiles

1. _Auto_: Default behaviour of fans. In this profile they will work as if no custom control software were ever used.
   I'm not a big fan of this mode. Get it? This is funny, right? Aaw, my sense of humor is even worse than my tech skills.
2. _Basic_: Basic control mode allows to adjust overall speed of fans with one "offset" value in [-15;15] range
3. _Advanced_: Advanced control mode allows to adjust the curve which represents fan speed function of temperature. 
   Exact temperatures at which next tier kicks in are unknown, those tiers are possibly hardware model dependent.
   What is known, is that they go from most cool to most hot. Range for each value is [0;150], don't ask me why, MSI design.

_Examples:_

```
msifancontrol advanced /cpu:20,45,55,65,70,75 /gpu:0,20,40,60,80,80
msifancontrol basic /value:-15
msifancontrol auto
```

# How it works?

Using built-in [wmiacpi.sys](https://msdn.microsoft.com/en-us/library/windows/hardware/dn614028(v=vs.85).aspx) 
Windows driver with MSI custom-made [MOF](https://msdn.microsoft.com/en-us/library/aa823192(v=vs.85).aspx) definitions.
This [repo](https://github.com/Microsoft/Windows-driver-samples/tree/master/wmi/wmiacpi) could also help to understand what is going on.
