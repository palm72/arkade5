using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Mets;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Metadata
{
    public class DiasMetsCreator : MetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            DirectoryInfo rootDirectory = archive.WorkingDirectory.Root().DirectoryInfo();

            if (rootDirectory.Exists)
                metadata.FileDescriptions = GetFileDescriptions(rootDirectory, rootDirectory);

            if (archive.WorkingDirectory.HasExternalContentDirectory())
            {
                DirectoryInfo externalContentDirectory = archive.WorkingDirectory.Content().DirectoryInfo();

                if (externalContentDirectory.Exists)
                {
                    var fileDescriptions = GetFileDescriptions(externalContentDirectory, externalContentDirectory);

                    foreach (FileDescription fileDescription in fileDescriptions)
                        fileDescription.Name = Path.Combine("content", fileDescription.Name);

                    metadata.FileDescriptions.AddRange(fileDescriptions);
                }
            }

            mets mets = Create(metadata);

            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }
    }
}
