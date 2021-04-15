# Changing a logger's settings "on the fly"

- Talk about when/why you'd want to do this (change the log level)
- How it's accomplished - by binding to Microsoft.Extensions.Configuration. When it changes, your logger changes.
- Introduce ReloadingLogger (and ReloadingLogProvider?)
  - When reloading is not necessary
    - When registered transient/scoped and used short-term
    - Still need to bind options to configurtation
- Two mechanisms for reloading
  - COF
    -  Probably supports swapping log providers (and maybe adding/removing)
  - DI extension methods + configuration-bound options
    - Probably does not support changing log providers
