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
    public class EPFArchive_ToExtractTests
    {
        private string EXPECTED_EXTRACT_DIR = @".\SandBox\ExpectedExtract";
        private string VALID_OUTPUT_EXTRACT_DIR = @".\SandBox\OutputExtract";
        private string MISSING_OUTPUT_EXTRACT_DIR = @".\SandBox\MissingFolder";

        private string[] TEST_ENTRIES = new string[] { "TFile1.txt", "TFile2.png" };
        private Stream _validEPFFile;
        private Stream _invalidEPFFile;

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

            _validEPFFile = File.OpenRead(@".\SandBox\ValidArchive.epf");
            _invalidEPFFile = File.OpenRead(@".\SandBox\InvalidArchive.txt");

        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (_validEPFFile != null)
            {
                _validEPFFile.Dispose();
                _validEPFFile = null;
            }

            if (_invalidEPFFile != null)
            {
                _invalidEPFFile.Dispose();
                _invalidEPFFile = null;
            }
        }

        [TestMethod()]
        public void EPFArchive_ValidInputStream_OpensArchiveWithEntries_Test()
        {
            //Arrange
            //Act
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);
            var entriesNo = epfArchive.Entries.Count;

            //Assert
            Assert.IsTrue(entriesNo != 0, "Opened EPF Archive should contain entries.");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EPFArchive_NullInputStream_Throws_Test()
        {
            //Arrange
            //Act
            var epfArchive = EPFArchive.ToExtract(null);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidDataException))]
        public void EPFArchive_InvalidInputStream_Throws_Test()
        {
            //Arrange
            //Act
            var epfArchive = EPFArchive.ToExtract(_invalidEPFFile);

            //Assert
        }

        [TestMethod()]
        public void ValidateEntryName_CorrectProposedName_ReturnsValidEntryName_Test()
        {
            //Arrange
            var proposedName = "123456789abc";
            //Act
            var validEntryName = EPFArchive.ValidateEntryName(proposedName);

            //Assert
            Assert.IsTrue(validEntryName == "123456789ABC", "Expected different validated entry name.");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateEntryName_TooLongProposedName_Throws_Test()
        {
            //Arrange
            var proposedName = "123456789abcefg";
            //Act
            var validEntryName = EPFArchive.ValidateEntryName(proposedName);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateEntryName_NullName_Throws_Test()
        {
            //Arrange
            string proposedName = null;
            //Act
            var validEntryName = EPFArchive.ValidateEntryName(proposedName);

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateEntryName_NonASCIIName_Throws_Test()
        {
            //Arrange
            string proposedName = "Błąd";
            //Act
            var validEntryName = EPFArchive.ValidateEntryName(proposedName);

            //Assert
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateEntry_Throws_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.CreateEntry("TFile1.txt", @".\SandBox\ValidEntry.png");

            //Assert
        }

        [TestMethod()]
        public void Dispose_NoException_Test()
        {   
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.Dispose();

            //Assert
        }

        [TestMethod()]
        public void ExtractAll_ValidOutputFolder_AllEntriesExtracted_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

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
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.ExtractAll(MISSING_OUTPUT_EXTRACT_DIR);

            //Assert
        }

        [TestMethod()]
        public void ExtractEntries_ValidOutputFolder_EntriesExtracted_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

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
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.ExtractEntries(MISSING_OUTPUT_EXTRACT_DIR, TEST_ENTRIES);

            //Assert
        }

        [TestMethod()]
        public void FindEntry_ExistingEntry_ReturnsEntryObject_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            var entry = epfArchive.FindEntry("TFile1.txt");

            //Assert
            Assert.IsTrue(entry != null, "Entry supose to exist in archive.");
        }

        [TestMethod()]
        public void FindEntry_NotExistingEntry_ReturnsNull_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            var entry = epfArchive.FindEntry("Huh.txt");

            //Assert
            Assert.IsTrue(entry == null, "Entry should not exist.");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveEntry_Throws_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            var result = epfArchive.RemoveEntry("TFile1.txt");

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReplaceEntry_Throws()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.ReplaceEntry("Huh.txt", @".\SandBox\Huh.png");

            //Assert
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_ThrowsInvalidOperationException_Test()
        {
            //Arrange
            var epfArchive = EPFArchive.ToExtract(_validEPFFile);

            //Act
            epfArchive.Save();

            //Assert
        }
    }
}