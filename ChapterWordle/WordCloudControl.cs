﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Gma.CodeCloud.Controls.TextAnalyses.Extractors;
using Gma.CodeCloud.Controls.TextAnalyses.Processing;
using Paratext.PluginInterfaces;

namespace ChapterWordCloudPlugin
{
    public partial class WordCloudControl : EmbeddedPluginControl
    {
		private IVerseRef m_reference;
		private IProject m_project;

		public WordCloudControl()
		{
			InitializeComponent();
            Title = ChapterWordCloudPlugin.pluginName;
		}

		public WordCloudControl(IVerseRef reference, IProject project) : this()
        {
            Initialize(project, reference);
        }

		private void Initialize(IProject project, IVerseRef reference)
		{
			m_project = project;
			m_reference = reference;
			UpdateWordle();
		}

		#region Implementation of EmbeddedPluginControl
		public override void OnAddedToParent(IPluginChildWindow parent)
		{
			parent.VerseRefChanged += Parent_VerseRefChanged;
            parent.ProjectChanged += Parent_ProjectChanged;
		}

		public override string GetState()
		{
			return null;
		}

		public override void RestoreFromState(IVerseRef reference, IProject project, string state)
		{
            Initialize(project, reference);
		}
		#endregion

        private void Parent_VerseRefChanged(IPluginChildWindow sender, IVerseRef oldReference, IVerseRef newReference)
		{
            Debug.Assert(oldReference.Equals(m_reference));
			m_reference = newReference;
			if (oldReference.BookNum != newReference.BookNum || oldReference.ChapterNum != newReference.ChapterNum)
				UpdateWordle();
		}

        private void Parent_ProjectChanged(IPluginChildWindow sender, IProject newProject)
        {
            m_project = newProject;
            UpdateWordle();
        }

        private void UpdateWordle()
        {
            progressBar1.Hide();
            var tokens = m_project.GetUSFMTokens(m_reference.BookNum, m_reference.ChapterNum).OfType<IUSFMTextToken>();
            var text = string.Join(" ", tokens);

            IProgressIndicator progress = new ProgressBarWrapper(progressBar1);

            IEnumerable<string> terms = new StringExtractor(text, progress);
            if (!terms.Any())
                terms = new[] {"Empty", "chapter"};

            cloudControl.WeightedWords = terms.CountOccurences().SortByOccurences();

            progressBar1.Hide();
        }

        #region ProgressBarWrapper class
        private class ProgressBarWrapper : IProgressIndicator
        {
            private readonly ProgressBar m_ProgressBar;

            public ProgressBarWrapper(ProgressBar toolStripProgressBar)
            {
                m_ProgressBar = toolStripProgressBar;
            }

            public int Value
            {
                get { return m_ProgressBar.Value; }
                set { m_ProgressBar.Value = value; }
            }

            public virtual int Maximum
            {
                get { return m_ProgressBar.Maximum; }
                set { m_ProgressBar.Maximum = value; }
            }

            public virtual void Increment(int value)
            {
                m_ProgressBar.Increment(value);
                Application.DoEvents();
            }
        }
        #endregion
	}
}