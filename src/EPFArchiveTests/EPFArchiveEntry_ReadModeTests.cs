using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace EPFArchiveTests
{
    [TestClass()]
    public class EPFArchiveEntry_ReadModeTests
    {
        private string EXPECTED_EXTRACT_DIR = @".\SandBox\ExpectedExtract";
        private string VALID_OUTPUT_EXTRACT_DIR = @".\SandBox\ValidOutput";
        private string INVALID_OUTPUT_EXTRACT_DIR = @".\SandBox\InvalidOutput";

        private string NOT_EXISTING_ENTRY_NAME = "Huh.txt";
        private string EXISTING_ENTRY_NAME = "TFile1.txt";
        private string EXPECTED_EXTRACTED_FILE_NAME = "TFile1.txt";

        private Stream _readonlyEPFArchiveFile;
        private Stream _readWriteEPFArchiveFile;

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

            Helpers.DeployResource($@"{EXPECTED_EXTRACT_DIR}\{EXPECTED_EXTRACTED_FILE_NAME}", EXPECTED_EXTRACTED_FILE_NAME);

            Directory.CreateDirectory(VALID_OUTPUT_EXTRACT_DIR);

            Thread.Sleep(100);

            _readonlyEPFArchiveFile = File.OpenRead(@".\SandBox\ReadOnlyValidArchive.epf");
            _readWriteEPFArchiveFile = File.Open(@".\SandBox\ReadWriteValidArchive.epf", FileMode.Open, FileAccess.ReadWrite);
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
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractTo_InvalidOutputFolder_Throws_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Read);
            var epfArchiveEntry = epfArchive.FindEntry(EXISTING_ENTRY_NAME);

            //Act
            epfArchiveEntry.ExtractTo(INVALID_OUTPUT_EXTRACT_DIR);

            //Assert
        }

        [TestMethod()]
        public void ExtractTo_ValidOutputFolder_ExtractsEntry_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Read);
            var epfArchiveEntry = epfArchive.FindEntry(EXISTING_ENTRY_NAME);

            //Act
            epfArchiveEntry.ExtractTo(VALID_OUTPUT_EXTRACT_DIR);

            var areSame = Helpers.FileEquals($@"{EXPECTED_EXTRACT_DIR}\{EXPECTED_EXTRACTED_FILE_NAME}",
                                   $@"{ VALID_OUTPUT_EXTRACT_DIR}\{EXISTING_ENTRY_NAME}");

            //Assert
            Assert.IsTrue(areSame, "Extracted entry file should be exact as expected file");
        }

        [TestMethod()]
        public void Open_ReturnsStream_Test()
        {
            //Arrange
            var epfArchive = new EPFArchive(_readWriteEPFArchiveFile, EPFArchiveMode.Read);
            var epfArchiveEntry = epfArchive.FindEntry(EXISTING_ENTRY_NAME);

            //Act
            var entryStream1 = epfArchiveEntry.Open();
            var entryStream2 = epfArchiveEntry.Open();

            //Assert
            Assert.IsTrue(entryStream1 != entryStream2, "Entry streams should be different.");
        }
    }
}