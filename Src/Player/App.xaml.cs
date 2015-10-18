using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Linq;

namespace Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                    return Assembly.Load(assemblyData);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsRunning
        {
            get
            {
                return System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;

            }
        }
    }
}
