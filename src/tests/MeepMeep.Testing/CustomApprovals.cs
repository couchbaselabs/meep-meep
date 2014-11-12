using System;
using ApprovalTests;

namespace MeepMeep.Testing
{
    public static class CustomApprovals
    {
        public static void VerifyGitFriendly(string text)
        {
            Approvals.Verify(text.Replace(" " + Environment.NewLine, Environment.NewLine).Trim());
        }
    }
}