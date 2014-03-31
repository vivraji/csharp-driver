namespace Cassandra
{
    internal class OutputUnprepared : OutputError
    {
        readonly PreparedQueryNotFoundInfo _info = new PreparedQueryNotFoundInfo();
        internal void Load(CassandraErrorType code, string message, BEBinaryReader cb)
        {
            var len = cb.ReadInt16();
            _info.UnknownID = new byte[len];
            cb.Read(_info.UnknownID, 0, len);
        }
        public override DriverException CreateException()
        {
            return new PreparedQueryNotFoundException(Message, _info.UnknownID);
        }
    }
}