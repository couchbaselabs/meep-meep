using System.Reflection;

#if DEBUG
[assembly: AssemblyProduct("MeepMeep (Debug)")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyProduct("MeepMeep (Release)")]
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyDescription("MeepMeep - A super simple workload utility for the Couchbase .NET client.")]
[assembly: AssemblyCompany("Couchbase")]
[assembly: AssemblyCopyright("Copyright © 2017 Couchbase")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("0.1.0.*")]
[assembly: AssemblyFileVersion("0.1.0")]
