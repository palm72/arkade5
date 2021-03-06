﻿using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlOrganizationNumberTest
    {
        [Fact]
        public void ShouldReportErrorWhenInvalidOrganizationNumbersAreFound()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlOrganizationNumber test = new ControlOrganizationNumber();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "914994780")); // ok
            test.Run(new Field(fieldDefinition, "914994781")); // invalid checksum
            test.Run(new Field(fieldDefinition, "91499478")); // invalid length
            test.Run(new Field(fieldDefinition, "91499478A")); // invalid characters
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig organisasjonsnummer: 914994781");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig organisasjonsnummer: 91499478");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("Ugyldig organisasjonsnummer: 91499478A");
        }
    }
}