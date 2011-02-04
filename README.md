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

Installation
------------

Usage
-----

Hosting Repositories
--------------------

Extending
---------

Contibuting
-----------

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
