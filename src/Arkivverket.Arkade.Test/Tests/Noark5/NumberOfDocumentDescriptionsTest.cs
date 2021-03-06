﻿using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDocumentDescriptionsTest
    {
        [Fact]
        public void ShouldReturnNumberOfDocumentDescriptions()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("somesubelement", "some value"))))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            // No documentdescription element
                                            new XmlElementHelper().Add("somesubelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDocumentDescriptions());

            testRun.Results.First().Message.Should().Be("Antall dokumentbeskrivelser: 1");
        }
    }
}
