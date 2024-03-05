namespace DataStructs
{ 
    public struct byte2
    {
        public byte x;
        public byte y;

        public byte2(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte2(int x, int y) : this()
        {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public bool Equals(byte2 other)
        {
            return (x == other.x && y == other.y);
        }

        public static byte2 operator +(byte2 a, byte2 b) => new byte2(a.x + b.x, a.y + b.y);

        public static byte2 operator -(byte2 a, byte2 b) => new byte2(a.x - b.x, a.y - b.y);
         
    }
}
