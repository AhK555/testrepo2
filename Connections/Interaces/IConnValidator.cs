namespace CadMatePipe.Connections
{
    internal interface IConnValidator
    {
        IConnection Connection { get; }
        IInvalidAction InvalidAction { get; }
        bool Validate();
    }
}