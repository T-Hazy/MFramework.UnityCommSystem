namespace MFramework.CommSystem
{
    public class DataContainer
    {
        public byte[] data { get; protected set; }
        public int dataLength => data.Length;

        protected DataContainer()
        {
        }

        public DataContainer(byte[] data)
        {
            this.data = data;
        }
    }
}