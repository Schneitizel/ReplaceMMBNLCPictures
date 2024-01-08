using System.Collections.Generic;

namespace ReplacePictures
{
    public struct customTable
    {
        public customTable(int id, byte[] name, long length)
        {
            Id = id;
            Name = name;
            Length = length;
        }

        public int Id { get; }
        public byte[] Name { get; }
        public long Length { get; }

        public override string ToString() => $"({Id}, {Name})";
    }
}
