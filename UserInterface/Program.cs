using Avalonia;
using Visualization;

namespace UserInterface
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            // configuring avalonia app
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .SetupWithoutStarting();

            ApplicationConfiguration.Initialize();
            if (args.Length == 0)
                System.Windows.Forms.Application.Run(new Form1());

            else
                System.Windows.Forms.Application.Run(new Form1(args));

        }
    }
}