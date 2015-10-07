// Guids.cs
// MUST match guids.h
using System;

namespace DivyaSinghal.StrutViewer
{
    static class GuidList
    {
        public const string guidStrutViewerPkgString = "c936831d-efb8-458d-aeb2-749d81ef993c";
        public const string guidStrutViewerCmdSetString = "dc27c4f3-268d-4217-aeda-7a836d226b76";
        public const string guidToolWindowPersistanceString = "8e542e5e-1de8-46fb-a35c-077b9bb0cac5";

        public static readonly Guid guidStrutViewerCmdSet = new Guid(guidStrutViewerCmdSetString);
    };
}