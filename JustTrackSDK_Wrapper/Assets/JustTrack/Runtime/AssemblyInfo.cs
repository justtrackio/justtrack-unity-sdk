using System.Runtime.CompilerServices;

// Allow the Editor test assembly to access internal members of the Runtime assembly
[assembly: InternalsVisibleTo("_Tests.Editor")]
[assembly: InternalsVisibleTo("_Tests.PlayMode")]
