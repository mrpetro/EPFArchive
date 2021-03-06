﻿using EPF.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPF.UI
{
    public class DialogProvider : IDialogProvider
    {
        private MessageBoxButtons ToMessageBoxButtons(QuestionDialogButtons buttons)
        {
            switch (buttons)
            {
                case QuestionDialogButtons.OK:
                    return MessageBoxButtons.OK;
                case QuestionDialogButtons.OKCancel:
                    return MessageBoxButtons.OKCancel;
                case QuestionDialogButtons.AbortRetryIgnore:
                    return MessageBoxButtons.AbortRetryIgnore;
                case QuestionDialogButtons.YesNoCancel:
                    return MessageBoxButtons.YesNoCancel;
                case QuestionDialogButtons.YesNo:
                    return MessageBoxButtons.YesNo;
                case QuestionDialogButtons.RetryCancel:
                    return MessageBoxButtons.RetryCancel;
                default:
                    throw new InvalidOperationException("Unknown QuestionDialogButtons value conversion!");
            }
        }

        private DialogAnswer ToDialogAnswer(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.None:
                    return DialogAnswer.None;
                case DialogResult.OK:
                    return DialogAnswer.OK;
                case DialogResult.Cancel:
                    return DialogAnswer.Cancel;
                case DialogResult.Abort:
                    return DialogAnswer.Abort;
                case DialogResult.Retry:
                    return DialogAnswer.Retry;
                case DialogResult.Ignore:
                    return DialogAnswer.Ignore;
                case DialogResult.Yes:
                    return DialogAnswer.Yes;
                case DialogResult.No:
                    return DialogAnswer.No;
                default:
                    throw new InvalidOperationException("Unknown DialogResult value conversion!");
            }
        }

        public void ShowMessage(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        public DialogAnswer ShowReplaceFileQuestion(string text, string caption)
        {
            return ToDialogAnswer(MessageBox.Show(text, caption, ToMessageBoxButtons(QuestionDialogButtons.YesNoCancel)));
        }

        public DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons)
        {
            return ToDialogAnswer(MessageBox.Show(text, caption, ToMessageBoxButtons(buttons)));
        }

        public FileDialogResult ShowOpenFileDialog(string title, string filter, bool mutliSelect)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = title;
                fileDialog.Filter = filter;
                fileDialog.Multiselect = mutliSelect;
                var answer = ToDialogAnswer(fileDialog.ShowDialog());
                var fileNames = fileDialog.FileNames;
                return new FileDialogResult(answer, fileNames);
            }
        }

        public FileDialogResult ShowOpenFileDialog(string title, string filter, string initialDirectory, string fileName = null)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = title;
                fileDialog.Filter = filter;
                fileDialog.InitialDirectory = initialDirectory;
                fileDialog.FileName = fileName;

                var answer = ToDialogAnswer(fileDialog.ShowDialog());
                var fileNames = fileDialog.FileNames;
                return new FileDialogResult(answer, fileNames);
            }
        }

        public FileDialogResult ShowSaveFileDialog(string title, string filter, string initialDirectory, string fileName = null)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = title;
                fileDialog.Filter = filter;
                fileDialog.InitialDirectory = initialDirectory;
                fileDialog.FileName = fileName;

                var answer = ToDialogAnswer(fileDialog.ShowDialog());
                var fileNames = fileDialog.FileNames;
                return new FileDialogResult(answer, fileNames);
            }
        }

        public FolderBrowserResult ShowFolderBrowserDialog(string title, string initialDirectory)
        {
            using (var folderBrower = new FolderBrowserDialog())
            {
                folderBrower.Description = title;
                folderBrower.SelectedPath = initialDirectory;
                var answer = ToDialogAnswer(folderBrower.ShowDialog());
                var selectedDirectory = folderBrower.SelectedPath;
                return new FolderBrowserResult(answer, selectedDirectory);
            }
        }
    }
}
