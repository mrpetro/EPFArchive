using EPF.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace EPFArchive.UI.WPF
{
    public class DialogProvider : IDialogProvider
    {
        private MessageBoxButton ToMessageBoxButtons(QuestionDialogButtons buttons)
        {
            switch (buttons)
            {
                case QuestionDialogButtons.OK:
                    return MessageBoxButton.OK;
                case QuestionDialogButtons.OKCancel:
                    return MessageBoxButton.OKCancel;
                case QuestionDialogButtons.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                case QuestionDialogButtons.YesNo:
                    return MessageBoxButton.YesNo;
                default:
                    throw new InvalidOperationException("Unknown QuestionDialogButtons value conversion!");
            }
        }

        private DialogAnswer ToDialogAnswerWF(DialogResult result)
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

        private DialogAnswer ToDialogAnswer(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.None:
                    return DialogAnswer.None;
                case MessageBoxResult.OK:
                    return DialogAnswer.OK;
                case MessageBoxResult.Cancel:
                    return DialogAnswer.Cancel;
                case MessageBoxResult.Yes:
                    return DialogAnswer.Yes;
                case MessageBoxResult.No:
                    return DialogAnswer.No;
                default:
                    throw new InvalidOperationException("Unknown DialogResult value conversion!");
            }
        }

        public void ShowMessage(string text, string caption)
        {
            System.Windows.MessageBox.Show(text, caption);
        }

        public DialogAnswer ShowReplaceFileQuestion(string text, string caption)
        {
            return ToDialogAnswer(System.Windows.MessageBox.Show(text, caption, ToMessageBoxButtons(QuestionDialogButtons.YesNoCancel)));
        }

        public DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons)
        {
            return ToDialogAnswer(System.Windows.MessageBox.Show(text, caption, ToMessageBoxButtons(buttons)));
        }

        public FileDialogResult ShowOpenFileDialog(string title, string filter, bool mutliSelect)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Title = title;
            fileDialog.Filter = filter;
            fileDialog.Multiselect = mutliSelect;

            DialogAnswer answer;
            string[] fileNames;

            if (fileDialog.ShowDialog() == true)
            {
                answer = DialogAnswer.OK;
                fileNames = fileDialog.FileNames;
            }
            else
            {
                answer = DialogAnswer.Cancel;
                fileNames = null;
            }

            return new FileDialogResult(answer, fileNames);
        }

        public FileDialogResult ShowOpenFileDialog(string title, string filter, string initialDirectory, string fileName = null)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Title = title;
            fileDialog.Filter = filter;
            fileDialog.InitialDirectory = initialDirectory;
            fileDialog.FileName = fileName;

            DialogAnswer answer;
            string[] fileNames;

            if (fileDialog.ShowDialog() == true)
            {
                answer = DialogAnswer.OK;
                fileNames = fileDialog.FileNames;
            }
            else
            {
                answer = DialogAnswer.Cancel;
                fileNames = null;
            }

            return new FileDialogResult(answer, fileNames);
        }

        public FileDialogResult ShowSaveFileDialog(string title, string filter, string initialDirectory, string fileName = null)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog();

            fileDialog.Title = title;
            fileDialog.Filter = filter;
            fileDialog.InitialDirectory = initialDirectory;
            fileDialog.FileName = fileName;

            DialogAnswer answer;
            string[] fileNames;

            if (fileDialog.ShowDialog() == true)
            {
                answer = DialogAnswer.OK;
                fileNames = fileDialog.FileNames;
            }
            else
            {
                answer = DialogAnswer.Cancel;
                fileNames = null;
            }

            return new FileDialogResult(answer, fileNames);
        }

        public FolderBrowserResult ShowFolderBrowserDialog(string title, string initialDirectory)
        {
            using (var folderBrower = new FolderBrowserDialog())
            {
                folderBrower.Description = title;
                folderBrower.SelectedPath = initialDirectory;
                var answer = ToDialogAnswerWF(folderBrower.ShowDialog());
                var selectedDirectory = folderBrower.SelectedPath;
                return new FolderBrowserResult(answer, selectedDirectory);
            }
        }
    }
}
