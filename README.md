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

## Using advanced profiles

There are two parameters (both mandatory) to set up advanced profile:
1. Fan type: ``cpu`` or ``gpu``, pretty self-explanatory
2. Values: this is array of six fan rotation speeds in percent in range 0-150 (don't ask me why, it's MSI design).
   Exact temperatures at which next tier kicks in are unknown, and possible are hardware model dependent. What is known,
   is that they're go from most cool to most hot.

_Examples:_

```
msifancontrol advanced -t:cpu -v:20,45,55,65,70,75
msifancontrol advanced -t:gpu -v:0,20,40,60,80,80
```

# How it works?

``TODO``
