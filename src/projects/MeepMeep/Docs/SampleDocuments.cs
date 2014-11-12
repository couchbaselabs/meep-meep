using System;
using System.IO;
using System.Text;

namespace MeepMeep.Docs
{
    public static class SampleDocuments
    {
        public static string Default = "{\"stringValue\":\"Lorem ipsum dolor sit amet, consectetur cras amet.\", \"intValue\": 42, \"dateTimeValue\": \"2013-06-23T20:10:01.504Z\", \"arrayValue\": [1,2,3,4,5]}";

        public static string ReadJsonSampleDocument(string docSamplePath)
        {
            if (string.IsNullOrWhiteSpace(docSamplePath))
                return null;

            if (!File.Exists(docSamplePath))
                throw new Exception(string.Format("Could not find file with JSON document at path: \"{0}\".", docSamplePath));

            return File.ReadAllText(docSamplePath, Encoding.UTF8);
        }
    }
}