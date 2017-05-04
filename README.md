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

``TODO``

# How it works?

``TODO``
