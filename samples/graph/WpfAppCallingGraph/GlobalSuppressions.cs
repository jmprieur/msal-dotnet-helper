
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage.CSharp.Reliability", "AvoidAsyncVoid:Avoid async void", Justification = "False positive. Only Event handlers (this is the case here) can be async void", Scope = "member", Target = "~M:WpfAppCallingGraph.MainWindow.SignInSignOut_Click(System.Object,System.Windows.RoutedEventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage.CSharp.Reliability", "AvoidAsyncVoid:Avoid async void", Justification = "False positive. Only Event handlers (this is the case here) can be async void", Scope = "member", Target = "~M:WpfAppCallingGraph.MainWindow.CredentialRequired_Click(System.Object,System.Windows.RoutedEventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage.CSharp.Reliability", "AvoidAsyncVoid:Avoid async void", Justification = "False positive. Only Event handlers (this is the case here) can be async void", Scope = "member", Target = "~M:WpfAppCallingGraph.MainWindow.Window_Initialized(System.Object,System.EventArgs)")]