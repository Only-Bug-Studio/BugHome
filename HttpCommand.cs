enum HttpCommandType { OK, Close, GO_ON };
class BreakpointData
{
    public BreakpointData() { }
    public BreakpointData(string x, Int64 y)
    {
        fileName = x;
        position = y;
    }
    public string fileName;
    public Int64 position;
}
class HttpCommand
{
    public HttpCommand(HttpCommandType t, byte[] b)
    {
        type = t;
        re = b;
    }
    public HttpCommand(HttpCommandType t, HttpServerMessage h)
    {
        type = t;
        re = h.ToBytes();
    }
    public HttpCommand(HttpCommandType t, byte[] b, BreakpointData d)
    {
        type = t;
        re = b;
        data = d;
    }
    public HttpCommand() { }
    public HttpCommandType type;
    public byte[] re;
    public BreakpointData data;

}