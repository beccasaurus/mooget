     ___________________________________
    < NuGet + Super Cow Powers = MooGet >
     -----------------------------------
            \   ^__^
             \  (oo)\_______
                (__)\       )\/\
                    ||----w |
                    ||     ||

MooGet
======

**HERE BE DRAGONS**

This was a side-project I was playing around with to experiment with:

 - Some crazy C# patterns
 - Creating a package management system (and test-driving it)
 - Assorted evil

One day, I *would* like to see an open-source, *truly* cross-platform implementation of NuGet with globally/locally installable packages and **NO IDE REQUIRED.**  I don't know if we'll ever see it but who knows?

### Directory Layout

 - `src` The MooGet source code
   - `core` T core MooGet classes
   - `cli` The code for the moo.exe command line application
   - `ext` Extension methods

 - `spec` NUnit specifications (tests) for MooGet
   - `core` Specs for the code MooGet classes
   - `cli` Integration tests that run the moo.exe application

 - `lib` External library code/assemblies
   - `test` Assemblies that we reference for our test suite, eg. `nunit.framework.dll`
   - `*` Projects included as git submodules. their code is compiled with the test of the MooGet source code

License
-------

MooGet is released under the MIT license.
