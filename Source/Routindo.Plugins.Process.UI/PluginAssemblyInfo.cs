using Routindo.Contract.Attributes;
using Routindo.Plugins.Process.Components.Actions;
using Routindo.Plugins.Process.UI.Views;

[assembly: ComponentConfigurator(typeof(StartProcessActionView), StartProcessAction.ComponentUniqueId, "Configurator for Start Process Action")]
[assembly: ComponentConfigurator(typeof(KillProcessByNameActionView), KillProcessByNameAction.ComponentUniqueId, "Configurator for Kill Process by name Action")]
[assembly: ComponentConfigurator(typeof(KillProcessByIdActionView), KillProcessByIdAction.ComponentUniqueId, "Configurator for kill Process by Id Action")]