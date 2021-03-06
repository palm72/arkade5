﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #44
    /// </summary>
    public class ControlNoSuperclassesHasRegistrations : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private readonly Stack<Class> _classes = new Stack<Class>();
        private readonly List<Class> _superClassesWithRegistration = new List<Class>();

        public override string GetName()
        {
            return Noark5Messages.ControlNoSuperclassesHasRegistrations;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            bool multipleArchiveParts = _superClassesWithRegistration.GroupBy(c => c.ArchivePartSystemId).Count() > 1;

            foreach (var superClassWithRegistration in _superClassesWithRegistration)
            {
                var message = new StringBuilder(string.Format(
                    Noark5Messages.ControlNoSuperclassesHasRegistrationsMessage, superClassWithRegistration.SystemId));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(
                            Noark5Messages.ArchivePartSystemId, superClassWithRegistration.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Error, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klasse"))
            {
                if (_classes.Any())
                    _classes.Peek().HasSubclass = true;

                _classes.Push(new Class { ArchivePartSystemId = _currentArchivePartSystemId });
            }

            if (eventArgs.Path.Matches("registrering", "klasse"))
                _classes.Peek().HasRegistration = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "klasse"))
                _classes.Peek().SystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
            {
                Class examinedClass = _classes.Pop();

                if (examinedClass.IsSuperClassWithRegistration())
                    _superClassesWithRegistration.Add(examinedClass);
            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePartSystemId = null; // Reset
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public string ArchivePartSystemId { get; set; }
            public bool HasSubclass { get; set; }
            public bool HasRegistration { get; set; }

            public bool IsSuperClassWithRegistration()
            {
                return HasSubclass && HasRegistration;
            }
        }
    }
}
