﻿using System;
using System.Windows.Forms;
using PKHeX.Core;
using PokeFilename.API;

namespace PokeFilename
{
    public class PokeFilename : IPlugin
    {
        public string Name => "PokéFilename";
        public int Priority => 1; // Loading order, lowest is first.

        // Initialized on plugin load
        public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
        public IPKMView PKMEditor { get; private set; } = null!;

        public void Initialize(params object[] args)
        {
            Console.WriteLine($"Loading {Name}...");
            SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
            PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView);
            var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
            LoadMenuStrip(menu);

            SetNamerSettings();
        }

        public static void SetNamerSettings()
        {
            var settings = Utils.Settings;
            CustomNamer.Regular = settings.RegularFormat;
            CustomNamer.Gameboy = settings.GameboyFormat;
            EntityFileNamer.Namer = Utils.Create(settings.PKMNamer.ToString());
        }

        private void LoadMenuStrip(ToolStrip menuStrip)
        {
            var items = menuStrip.Items;
            if (!(items.Find("Menu_Tools", false)[0] is ToolStripDropDownItem tools))
                throw new ArgumentException(nameof(menuStrip));
            AddPluginControl(tools);
        }

        private void AddPluginControl(ToolStripDropDownItem tools)
        {
            var ctrl = new ToolStripMenuItem(Name) { Image = GUI.Resources.pokeball };
            ctrl.Click += OpenSettings;
            ctrl.Name = "Menu_PokeFilenameSettings";
            tools.DropDownItems.Add(ctrl);
        }

        private static void OpenSettings(object sender, EventArgs e)
        {
            var settings = Utils.Settings;
            using var form = new SettingsForm(settings);
            form.ShowDialog();
        }

        public void NotifySaveLoaded()
        {
            Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
        }

        public bool TryLoadFile(string filePath)
        {
            Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
            return false; // no action taken
        }
    }
}