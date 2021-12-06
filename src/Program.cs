﻿using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Cassiopeia
{
    static class Program
    {
        public static bool ModoOscuro = false;

        [STAThread]
        static void Main(String[] args)
        {
            //preparar la aplicación para que ejecute formularios y demás.
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Kernel.ParseArgs(args);
            int console;
            if (Kernel.Console)
            {
                console = Kernel.AllocConsole();
                Console.CancelKeyPress += (sender, args) => Kernel.Quit();
            }
#if DEBUG
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
#endif
            Log.InitLog();
            /*LOADING PROCESS*/
            Log.Instance.PrintMessage("Starting...", MessageType.Info);

            Stopwatch StartStopwatch = Stopwatch.StartNew();
            //Checking arguments

            //Load configuration
            Kernel.LoadConfig();
            //Loading languages, else it will not load the textbox.
            Kernel.LoadLanguages();

            if (!Kernel.MetadataStream)
            {
                //Create genres
                Kernel.InitGenres();
                //Create program
                Kernel.CreateProgram();
                //Load the files
                Kernel.LoadFiles();
                //Init Spotify
                Kernel.InitSpotify();
                //Create player Instance
                Kernel.InitPlayer();
            }
            else
                Kernel.InitSpotify();
            //We're done!
            StartStopwatch.Stop();
            Log.Instance.PrintMessage("Application loaded!", MessageType.Correct, StartStopwatch, TimeType.Seconds);

            //Before everything... check updates
#if !DEBUG
            if(Kernel.CheckUpdates)
                Kernel.CheckForUpdates();
#endif
            //ApplicationStart
            Kernel.StartApplication();
            //Program halts here until Application.Exit is called.
            Kernel.Quit();
        }
    }
}