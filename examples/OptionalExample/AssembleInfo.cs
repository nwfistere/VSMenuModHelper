using OptionalExample;
using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(ModInfo.Description)]
[assembly: AssemblyDescription(ModInfo.Description)]
[assembly: AssemblyCompany(ModInfo.Company)]
[assembly: AssemblyProduct(ModInfo.Name)]
[assembly: AssemblyCopyright("Copyright " + ModInfo.Author + " 2023")]
[assembly: AssemblyTrademark(ModInfo.Company)]
[assembly: AssemblyVersion(ModInfo.Version)]
[assembly: AssemblyFileVersion(ModInfo.Version)]

[assembly: MelonInfo(typeof(OptionalExample.OptionalExample), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.Download)]
[assembly: MelonGame("poncle", "VampireSurvivors")]
[assembly: MelonOptionalDependencies("VSMenuHelper")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

