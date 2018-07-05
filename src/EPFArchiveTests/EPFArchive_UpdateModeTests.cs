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
    public class EPFArchive_UpdateModeTests
    {
        private string EXPECTED_EXTRACT_DIR = @".\SandBox\ExpectedExtract";
        private string VALID_OUTPUT_EXTRACT_DIR = @".\SandBox\OutputExtract";
        private string MISSING_OUTPUT_EXTRACT_DIR = @".\SandBox\MissingFolder";

        private string[] TEST_ENTRIES = new string[] { "TFile1.txt", "TFile2.png" };
        private Stream _readonlyEPFArchiveFile;
        private Stream _readWriteEPFArchiveFile;
        private Stream _notEPFArchiveFile;

        [TestInitialize()]
        public void Initialize()
        {
            if (Directory.Exists("SandBox"))
                Directory.Delete("SandBox", true);

            Thread.Sleep(100);

            Directory.CreateDirectory(@".\SandBox");
            Helpers.DeployResource(@".\SandBox\ReadOnlyValidArchive.epf", "ValidArchive.epf");
            Helpers.DeployResource(@".\SandBox\ReadWriteValidArchive.epf", "ValidArchive.epf");
            Helpers.DeployResource(@".\SandBox\InvalidArchive.txt", "InvalidArchive.txt");

            Directory.CreateDirectory(EXPECTED_EXTRACT_DIR);

            foreach (var testEntry in TEST_ENTRIES)
                Helpers.DeployResource($@"{EXPECTED_EXTRACT_DIR}\{testEntry}", testEntry);

            Directory.CreateDirectory(VALID_OUTPUT_EXTRACT_DIR);

            Thread.Sleep(100);

            _readonlyEPFArchiveFile = File.OpenRead(@".\SandBox\ReadOnlyValidArchive.epf");
            _readWriteEPFArchiveFile = File.Open(@".\SandBox\ReadWriteValidArchive.epf", FileMode.Open, FileAccess.ReadWrite);
            _notEPFArchiveFile = File.Open(@".\SandBox\InvalidArchive.txt", FileMode.Open, FileAccess.ReadWrite);

        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (_readonlyEPFArchiveFile != null)
            {
                _readonlyEPFArchiveFile.Dispose();
                _readonlyEPFArchiveFile = null;
            }

            if (_readWriteEPFArchiveFile != null)
            {
                _readWriteEPFArchiveFile.Dispose();
                _readWriteEPFArchiveFile = null;
            }

            if (_notEPFArchiveFile != null)
            {
                _notEPFArchiveFile.Dispose();
                _notEPFArchiveFile = null;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EPFArchive_ReadOnlyInputStream_Throws_Test()
        {
            //Arrange
            //Act
            var epfArchive = new EPFArchive(_readonlyEPFArchiveFile, EPFArchiveMode.Update);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EPFArchive_NullInputStream_Throws_Test()
        {
            //Arrange
            //Act
            var epfArchive = new EPFArchive(null, EPFArchiveMode.Update);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidDataException))]
        public void EPFArchive_InvalidInputStream_Throws_Test()
        {
            //Arrange
            //Act
            var epfArchive = new EPFArchive(_notEPFArchiveFile, EPFArchiveMode.Update);

            //Assert
        }

        [TestMethod()]
        public void CreateEntry_NotExistingEntryName_ValidInputFile_ReturnsEntryObject_Test()
        {
            //Arrange
            Helpers.DeployResource(@".\SandBox\ValidEntry.png", "ValidEntry.png");
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var entry = epfArchive.CreateEntry("VALID_ENTRY", @".\SandBox\ValidEntry.png");

            //Assert
            Assert.IsTrue(entry != null, "Created entry object should not be null.");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEntry_ExistingEntryName_ValidInputFile_Throws_Test()
        {
            //Arrange
            Helpers.DeployResource(@".\SandBox\ValidEntry.png", "ValidEntry.png");
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.CreateEntry("TFile1.txt", @".\SandBox\ValidEntry.png");

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEntry_ExistingEntryName_InvalidInputFile_Throws_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.CreateEntry("Huh.txt", @".\SandBox\Huh.txt");

            //Assert
        }


        [TestMethod()]
        public void Dispose_CloseInputStream_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.Dispose();

            //Assert
            Assert.IsTrue(!_readWriteEPFArchiveFile.CanRead, "Input stream should be closed");
        }

        [TestMethod()]
        public void Dispose_LeaveInputStreamOpen_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update, true);

            //Act
            epfArchive.Dispose();

            //Assert
            Assert.IsTrue(_readWriteEPFArchiveFile.CanRead, "Input stream should be left open");
        }

        [TestMethod()]
        public void ExtractAll_ValidOutputFolder_AllEntriesExtracted_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ExtractAll(VALID_OUTPUT_EXTRACT_DIR);

            //Assert
            int samefilesNo = 0;
            foreach (var entryName in TEST_ENTRIES)
            {
                if (Helpers.FileEquals($@"{EXPECTED_EXTRACT_DIR}\{entryName}",
                                       $@"{VALID_OUTPUT_EXTRACT_DIR}\{entryName}"))
                    samefilesNo++;
            }

            Assert.IsTrue(samefilesNo == TEST_ENTRIES.Length,
                          "Some of extracted files content is different than templates.");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractAll_InvalidOutputFolder_Throws_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ExtractAll(MISSING_OUTPUT_EXTRACT_DIR);

            //Assert
        }

        [TestMethod()]
        public void ExtractEntries_ValidOutputFolder_EntriesExtracted_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ExtractEntries(VALID_OUTPUT_EXTRACT_DIR, TEST_ENTRIES);

            //Assert
            int samefilesNo = 0;
            foreach (var entryName in TEST_ENTRIES)
            {
                if (Helpers.FileEquals($@"{EXPECTED_EXTRACT_DIR}\{entryName}",
                                       $@"{VALID_OUTPUT_EXTRACT_DIR}\{entryName}"))
                    samefilesNo++;
            }

            Assert.IsTrue(samefilesNo == TEST_ENTRIES.Length,
                          "Some of extracted files content is different than templates.");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractEntries_InvalidOutputFolder_Throws_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ExtractEntries(MISSING_OUTPUT_EXTRACT_DIR, TEST_ENTRIES);

            //Assert
        }

        [TestMethod()]
        public void FindEntry_ExistingEntry_ReturnsEntryObject_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var entry = epfArchive.FindEntry("TFile1.txt");

            //Assert
            Assert.IsTrue(entry != null, "Entry supose to exist in archive.");
        }

        [TestMethod()]
        public void FindEntry_NotExistingEntry_ReturnsNull_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var entry = epfArchive.FindEntry("Huh.txt");

            //Assert
            Assert.IsTrue(entry == null, "Entry should not exist.");
        }

        [TestMethod()]
        public void RemoveEntry_ExistingEntryName_EntryRemoved_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var beforeRemove = epfArchive.Entries.Count;
            var result = epfArchive.RemoveEntry("TFile1.txt");
            var afterRemove = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(result, "RemoveEntry should return true.");
            Assert.IsTrue(beforeRemove > afterRemove, "There should be less entries after remove.");
        }

        [TestMethod()]
        public void RemoveEntry_NotExistingEntryName_ReturnsFalse_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var beforeRemove = epfArchive.Entries.Count;
            var result = epfArchive.RemoveEntry("Huh.txt");
            var afterRemove = epfArchive.Entries.Count;

            //Assert
            Assert.IsFalse(result, "RemoveEntry should return false.");
            Assert.IsTrue(beforeRemove == afterRemove, "Entries number should not change.");
        }

        [TestMethod()]
        public void ReplaceEntry_ExistingEntryName_ExistingInputFile_ReturnsEntryObjectTest()
        {
            //Arrange
            Helpers.DeployResource(@".\SandBox\ValidEntry.png", "ValidEntry.png");
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var result = epfArchive.ReplaceEntry("TFile1.txt", @".\SandBox\ValidEntry.png");

            //Assert
            Assert.IsTrue(result != null, "Entry supose to exist in archive.");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplaceEntry_NotExistingEntryName_ExistingInputFile_ThrowsArgumentException()
        {
            //Arrange
            Helpers.DeployResource(@".\SandBox\ValidEntry.png", "ValidEntry.png");
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ReplaceEntry("Huh.txt", @".\SandBox\ValidEntry.png");

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplaceEntry_NotExistingEntryName_NotExistingInputFile_Throws()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            epfArchive.ReplaceEntry("Huh.txt", @".\SandBox\Huh.png");

            //Assert
        }

        [TestMethod()]
        public void Save_SavesData_Test()
        {
            //Arrange
            Helpers.DeployResource(@".\SandBox\ValidEntry.png", "ValidEntry.png");
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Update);

            //Act
            var epfEntry = epfArchive.CreateEntry("NewEntry.png", @".\SandBox\ValidEntry.png");
            epfArchive.Save();
            epfArchive.Dispose();
            using (var savedFile = File.OpenRead(@".\SandBox\ReadWriteValidArchive.epf"))
            {
                epfArchive = new EPFArchive(savedFile, EPFArchiveMode.Read);
                epfArchive.ExtractEntries(VALID_OUTPUT_EXTRACT_DIR, new string[] { "NewEntry.png" });
            }

            var areSame = Helpers.FileEquals(@".\SandBox\ValidEntry.png",
                                   $@"{ VALID_OUTPUT_EXTRACT_DIR}\NewEntry.png");

                //Assert
            Assert.IsTrue(areSame, "Extracted entry should be exact as saved entry");
        }
    }
}