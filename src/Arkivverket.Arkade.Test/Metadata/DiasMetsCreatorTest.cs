﻿using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Test.Core;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Metadata
{
    public class DiasMetsCreatorTest : MetsCreatorTest
    {
        [Fact]
        public void ShouldSaveCreatedDiasMetsFileToDisk()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Metadata\\DiasMetsCreator";

            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .Build();

            new DiasMetsCreator().CreateAndSaveFile(archive, ArchiveMetadata);

            string metsFilePath = Path.Combine(workingDirectory, "dias-mets.xml");

            File.Exists(metsFilePath).Should().BeTrue();
        }
    }
}
