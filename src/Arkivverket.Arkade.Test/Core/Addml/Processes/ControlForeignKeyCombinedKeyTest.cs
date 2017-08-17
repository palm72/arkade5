﻿using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlForeignKeyCombinedKeyTest
    {
        private readonly AddmlFieldDefinition _messageIdPrimaryKeyFieldDef;
        private readonly AddmlFieldDefinition _attachmentNumberPrimaryKeyFieldDef;
        private readonly AddmlRecordDefinition _attachmentRecordDef;
        private readonly AddmlRecordDefinition _recordWithBothCombinedAndSingleForeignKeyDef;

        private readonly AddmlFieldDefinition _primaryKeyFieldDef;
        private readonly AddmlFieldDefinition _foreignKeyMessageIdFieldDef;
        private readonly AddmlFieldDefinition _foreignKeyAttachmentNumberFieldDef;
        private readonly AddmlFieldDefinition _foreignKeyDocumentIdFieldDef;

        private readonly AddmlRecordDefinition _documentRecordDef;
        private readonly AddmlFieldDefinition _documentPrimaryKeyFieldDef;


        public ControlForeignKeyCombinedKeyTest()
        {
            _attachmentRecordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(new AddmlFlatFileDefinitionBuilder().Build())
                .Build();

            _messageIdPrimaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_attachmentRecordDef)
                .WithName("MessageId")
                .IsPartOfPrimaryKey(true)
                .Build();
            _attachmentNumberPrimaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_attachmentRecordDef)
                .WithName("AttachmentNumber")
                .IsPartOfPrimaryKey(true)
                .Build();

            _documentRecordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(new AddmlFlatFileDefinitionBuilder().Build())
                .Build();
            _documentPrimaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_documentRecordDef)
                .WithName("DocumentId")
                .IsPartOfPrimaryKey(true)
                .Build();

            AddmlForeignKey addmlForeignKeyAttachment = new AddmlForeignKey("fk1-attachment");
            AddmlForeignKey addmlForeignKeyDocument = new AddmlForeignKey("fk2-document");

            _recordWithBothCombinedAndSingleForeignKeyDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(new AddmlFlatFileDefinitionBuilder().Build())
                .WithRecordProcess(ControlForeignKey.Name)
                .WithForeignKey(addmlForeignKeyAttachment)
                .WithForeignKey(addmlForeignKeyDocument)
                .Build();
            _primaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordWithBothCombinedAndSingleForeignKeyDef)
                .WithName("Id")
                .IsPartOfPrimaryKey(true)
                .Build();
            _foreignKeyMessageIdFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordWithBothCombinedAndSingleForeignKeyDef)
                .WithName("MessageId")
                .Build();
            _foreignKeyAttachmentNumberFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordWithBothCombinedAndSingleForeignKeyDef)
                .WithName("AttachmentNumber")
                .Build();

            addmlForeignKeyAttachment.ForeignKeys.Add(_foreignKeyMessageIdFieldDef);
            addmlForeignKeyAttachment.ForeignKeys.Add(_foreignKeyAttachmentNumberFieldDef);
            addmlForeignKeyAttachment.ForeignKeyReferenceIndexes.Add(_messageIdPrimaryKeyFieldDef.GetIndex());
            addmlForeignKeyAttachment.ForeignKeyReferenceIndexes.Add(_attachmentNumberPrimaryKeyFieldDef.GetIndex());
            addmlForeignKeyAttachment.ForeignKeyReferenceFields.Add(_messageIdPrimaryKeyFieldDef);
            addmlForeignKeyAttachment.ForeignKeyReferenceFields.Add(_attachmentNumberPrimaryKeyFieldDef);

            _foreignKeyDocumentIdFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordWithBothCombinedAndSingleForeignKeyDef)
                .WithName("DocumentId")
                .Build();

            addmlForeignKeyDocument.ForeignKeys.Add(_foreignKeyDocumentIdFieldDef);
            addmlForeignKeyDocument.ForeignKeyReferenceIndexes.Add(_documentPrimaryKeyFieldDef.GetIndex());
            addmlForeignKeyDocument.ForeignKeyReferenceFields.Add(_documentPrimaryKeyFieldDef);
        }


        [Fact]
        public void ShouldReturnErrorWhenCombinedForeignKeysReferenceDoesNotExist()
        {
            var attachmentDataRecord = new Record(_attachmentRecordDef, 
                new Field(_messageIdPrimaryKeyFieldDef, "556677"), 
                new Field(_attachmentNumberPrimaryKeyFieldDef, "1"));

            var testRecord = new Record(_recordWithBothCombinedAndSingleForeignKeyDef, 
                new Field(_primaryKeyFieldDef, "1234"), 
                new Field(_foreignKeyMessageIdFieldDef, "1001"), 
                new Field(_foreignKeyAttachmentNumberFieldDef, "1"));

            TestRun testRun = ControlForeignKeySingleTest.RunTest(attachmentDataRecord, testRecord);
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnSuccessWhenCombinedForeignKeysAndSingleForeignKeyReferencesExist()
        {
            var attachmentDataRecord = new Record(_attachmentRecordDef,
                new Field(_messageIdPrimaryKeyFieldDef, "1001"),
                new Field(_attachmentNumberPrimaryKeyFieldDef, "1"));

            var documentRecord = new Record(_documentRecordDef,
                new Field(_documentPrimaryKeyFieldDef, "42"));

            var testRecord = new Record(_recordWithBothCombinedAndSingleForeignKeyDef,
                new Field(_primaryKeyFieldDef, "1234"),
                new Field(_foreignKeyMessageIdFieldDef, "1001"),
                new Field(_foreignKeyAttachmentNumberFieldDef, "1"),
                new Field(_foreignKeyDocumentIdFieldDef, "42"));

            TestRun testRun = ControlForeignKeySingleTest.RunTest(attachmentDataRecord, documentRecord, testRecord);
            testRun.IsSuccess().Should().BeTrue();
        }

    }
}