using System.Collections.Generic;
using Pixonic.MsgPack.Formatters;

namespace Pixonic.MsgPack
{
    using FormattersMap = Dictionary<System.Type, IFormatter>;

    public sealed class Serializer : IContext
    {
        private readonly FormattersMap _formatters = new FormattersMap();
        private readonly MsgPackStream _stream = new MsgPackStream();

        private string _trace = string.Empty;

        public Serializer(bool useBuiltInFormatters = true)
        {
            if (useBuiltInFormatters)
            {
                BuiltInFormatters.Register(this);
            }
        }

        public byte[] Serialize<T>(T value, long initialCapacity = 1024)
        {
            try
            {
                _stream.BeginWrite(initialCapacity);
                GetFormatter<T>().Write(value, _stream, this);
                return _stream.Output.ToArray();
            }
            catch (MsgPackException e)
            {
                throw new MsgPackSerializationException(e.Message, _trace);
            }
            finally
            {
                _trace = string.Empty;
                _stream.Reset();
            }
        }

        public T Deserialize<T>(byte[] bytes, long offset = 0)
        {
            try
            {
                _stream.BeginRead(bytes, offset);
                return GetFormatter<T>().Read(_stream, this);
            }
            catch (MsgPackException e)
            {
                throw new MsgPackSerializationException(e.Message, _trace);
            }
            finally
            {
                _trace = string.Empty;
                _stream.Reset();
            }
        }

        public void RegisterFormatter<T>(IFormatter<T> formatter)
        {
            _formatters.Add(typeof(T), formatter);
        }

        IFormatter<T> IContext.ResolveFormatter<T>()
        {
            return GetFormatter<T>();
        }

        IFormatter IContext.ResolveFormatter(System.Type type)
        {
            IFormatter formatter;
            if (_formatters.TryGetValue(type, out formatter))
            {
                return _formatters[type];
            }

            throw new MsgPackException("Unable to resolve formatter for type {0}", type);
        }

        void IContext.Trace(string message)
        {
            _trace = message;
        }

        private IFormatter<T> GetFormatter<T>()
        {
            try
            {
                return (IFormatter<T>)_formatters[typeof(T)];
            }
            catch (System.InvalidCastException)
            {
                throw new MsgPackException("Unable to resolve formatter for type {0}", typeof(T));
            }
        }
    }
}
