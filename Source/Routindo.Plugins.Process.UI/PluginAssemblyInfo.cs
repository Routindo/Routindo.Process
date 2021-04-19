using Routindo.Contract.Attributes;
using Routindo.Plugins.Process.Components.Actions;
using Routindo.Plugins.Process.UI.Views;

[assembly: ComponentConfigurator(typeof(StartProcessActionView), StartProcessAction.ComponentUniqueId, "Configurator for Start Process Action")]