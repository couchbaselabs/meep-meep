using System.Runtime.InteropServices;
﻿using System.Reflection;

#if DEBUG
[assembly: AssemblyProduct("MeepMeep (Debug)")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyProduct("MeepMeep (Release)")]
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyDescription("MeepMeep - A super simple workload utility for the Couchbase .Net client.")]
[assembly: AssemblyCompany("Daniel Wertheim")]
[assembly: AssemblyCopyright("Copyright © 2013 Daniel Wertheim")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("0.1.0.*")]
[assembly: AssemblyFileVersion("0.1.0")]
