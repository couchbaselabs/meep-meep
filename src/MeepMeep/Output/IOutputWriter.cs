using System;

namespace MeepMeep.Output
{
    /// <summary>
    /// Defines an output writer which is used to output messages, status, results etc.
    /// </summary>
    public interface IOutputWriter
    {
        bool Verbose { get; set; }

        void Write(string output, params object[] formattingArgs);
        void Write(MeepMeepOptions options);
        void Write(WorkloadResult workloadResult);
        void Write(string value, Exception ex);
    }
}