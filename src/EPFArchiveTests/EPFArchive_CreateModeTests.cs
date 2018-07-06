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
    public class EPFArchive_CreateModeTests
    {
        private string EXPECTED_EXTRACT_DIR = @".\SandBox\ExpectedExtract";
        private string VALID_OUTPUT_EXTRACT_DIR = @".\SandBox\OutputExtract";

        private string[] TEST_ENTRIES = new string[] { "TFile1.txt", "TFile2.png" };
        private Stream _newEPFFile;

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

            _newEPFFile = File.Create(@".\SandBox\NewArchive.epf");

        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (_newEPFFile != null)
            {
                _newEPFFile.Dispose();
                _newEPFFile = null;
            }
        }

        [TestMethod()]
        public void EPFArchive_ValidInputStream_CreatesEmptyArchive_Test()
        {
            //Arrange
            //Act
            var epfArchive = new EPFArchive(_newEPFFile, EPFArchiveMode.Create);
            var entriesNo = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(entriesNo == 0, "Created EPF Archive should not contain any entries.");
        }

        [TestMethod()]
        public void CreateEntry_CreatesNewEntry_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_newEPFFile, EPFArchiveMode.Create);

            //Act
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(entriesNo == 1, "Created EPF Archive should contain one entry.");

        }


        [TestMethod()]
        public void Save_ChangesModeToUpdate_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_newEPFFile, EPFArchiveMode.Create);
            epfArchive.CreateEntry("TFile1.txt", $@"{EXPECTED_EXTRACT_DIR}\TFile1.txt");
            var entriesNo = epfArchive.Entries.Count;

            //Act
            var oldMode = epfArchive.Mode;
            epfArchive.Save();
            var newMode = epfArchive.Mode;

            //Assert
            Assert.IsTrue(oldMode == EPFArchiveMode.Create &&
                newMode == EPFArchiveMode.Update , "Saving created archive should change it's Mode to Update");
        }
    }
}