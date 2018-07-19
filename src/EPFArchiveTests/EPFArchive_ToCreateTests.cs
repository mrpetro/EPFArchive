using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Threading;

namespace EPFArchiveTests
{
    [TestClass()]
    public class EPFArchive_ToCreateTests
    {
        private string EXPECTED_EXTRACT_DIR = @".\SandBox\ExpectedExtract";
        private string VALID_OUTPUT_EXTRACT_DIR = @".\SandBox\OutputExtract";

        private string[] TEST_ENTRIES = new string[] { "TFile1.txt", "TFile2.png" };
        private Stream _newEPFFileStream;
        private Stream _readonlyEPFFileStream;

        [TestInitialize()]
        public void Initialize()
        {
            if (Directory.Exists("SandBox"))
                Directory.Delete("SandBox", true);

            Thread.Sleep(100);

            Directory.CreateDirectory(@".\SandBox");
            Helpers.DeployResource(@".\SandBox\ValidArchive.epf", "ValidArchive.epf");
            Helpers.DeployResource(@".\SandBox\InvalidArchive.txt", "InvalidArchive.txt");

            Directory.CreateDirectory(EXPECTED_EXTRACT_DIR);

            foreach (var testEntry in TEST_ENTRIES)
                Helpers.DeployResource($@"{EXPECTED_EXTRACT_DIR}\{testEntry}", testEntry);

            Directory.CreateDirectory(VALID_OUTPUT_EXTRACT_DIR);

            Thread.Sleep(100);

            _newEPFFileStream = File.Create(@".\SandBox\NewArchive.epf");
            _readonlyEPFFileStream = File.OpenRead(@".\SandBox\ValidArchive.epf");
        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (_newEPFFileStream != null)
            {
                _newEPFFileStream.Dispose();
                _newEPFFileStream = null;
            }

            if (_readonlyEPFFileStream != null)
            {
                _readonlyEPFFileStream.Dispose();
                _readonlyEPFFileStream = null;
            }
        }

        [TestMethod()]
        public void EPFArchive_ValidInputStream_CreatesEmptyArchive_Test()
        {
            //Arrange
            //Act
            var epfArchive = EPFArchive.ToCreate();
            var entriesNo = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(entriesNo == 0, "Created EPF Archive should not contain any entries.");
        }

        [TestMethod()]
        public void CreateEntry_CreatesNewEntry_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToCreate();

            //Act
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(entriesNo == 1, "Created EPF Archive should contain one entry.");

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_ChangesModeToUpdate_Throws_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToCreate();
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Act
            epfArchive.Save();

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveAs_InvalidStream_Throws_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToCreate();
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Act
            epfArchive.SaveAs(_readonlyEPFFileStream);

            //Assert
        }

        [TestMethod()]
        public void SaveAs_ValidStream_IsModifiedChanged_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToCreate();
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Act
            var oldValue = epfArchive.IsModified;
            epfArchive.SaveAs(_newEPFFileStream);
            var newValue = epfArchive.IsModified;

            //Assert
            Assert.IsTrue(oldValue == true &&
                newValue == false , "Saving created archive should change IsModified propery from true to false.");
        }
    }
}