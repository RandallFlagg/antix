﻿namespace Antix.IO
{
    public class FileChangedEvent
    {
        public string Path { get; set; }
        public FileChangedEventType Type { get; set; }
        public string OldPath { get; set; }

        public override string ToString()
        {
            switch (Type)
            {
                default:
                    return string.Format("{0} {1}", Path, Type);
                case FileChangedEventType.Renamed:
                    return string.Format("{0} {1} {2}", OldPath, Type, Path);
            }
        }
    }
}