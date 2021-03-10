﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Paratext.PluginInterfaces;

namespace ChapterWordCloudPlugin
{
    /// <summary>
    /// Simple plugin that shows a word cloud based on the text of the current chapter.
    /// </summary>
	[PublicAPI]
	public class ChapterWordCloudPlugin : IParatextWindowPlugin
    {
        public const string pluginName = "Chapter Word Cloud";
		public string Name => pluginName;
		public string Description => "Shows a \"Word Cloud\" of the current chapter.";
		public Version Version => new Version(2, 0);
		public string VersionString => Version.ToString();
		public string Publisher => "SIL/UBS";
        public IEnumerable<KeyValuePair<string, XMLDataMergeInfo>> MergeDataInfo => null;

        public IEnumerable<WindowPluginMenuEntry> PluginMenuEntries
		{
			get
            {
                yield return new WindowPluginMenuEntry("&" + pluginName + "...", Run, PluginMenuLocation.ScrTextTools);
			}
		}

		/// <summary>
		/// Called by Paratext when the menu item created for this plugin was clicked.
		/// </summary>
		private static void Run(IWindowPluginHost host, IParatextChildState windowState)
		{
			host.ShowEmbeddedUi(new WordCloudControl(), windowState.Project);
		}
	}
}