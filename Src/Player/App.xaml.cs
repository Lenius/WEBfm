using System;
using System.Reflection;
using System.Windows;

namespace Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (IsRunning)
            {
                MessageBox.Show("Kører i forvejen");
                Shutdown(0);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var requestAssembly = new AssemblyName(args.Name);

                if (requestAssembly.Name != "NAudio")
                {
                    return null;
                }

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Player.Dist.NAudio.dll"))
                {
                    if (stream != null)
                    {
                        var assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                        return Assembly.Load(assemblyData);
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IsRunning => System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1;
    }
}
