using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPF.UI
{
    public enum DialogAnswer
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7
    }

    public enum QuestionDialogButtons
    {
        OK = 0,
        OKCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5
    }

    public class FileDialogResult
    {
        public FileDialogResult(DialogAnswer answer, string[] fileNames)
        {
            Answer = answer;
            FileNames = fileNames;
        }

        public DialogAnswer Answer { get; private set; }
        public string[] FileNames { get; private set; }
        public string FileName { get { return FileNames.FirstOrDefault(); } }

    }

    public interface IDialogProvider
    {
        void ShowMessage(string text, string caption);
        DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons);
        FileDialogResult ShowOpenFileDialog(string title, string filter, bool mutliSelect);
    }
}
