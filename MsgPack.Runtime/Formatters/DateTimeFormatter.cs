using System;

namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class DateTimeFormatter : IFormatter<DateTime>
    {
        public const sbyte TypeCode = -1;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const long BclSecondsAtUnixEpoch = 62135596800;
        private const int NanosecondsPerTick = 100;

        void IFormatter<DateTime>.Write(DateTime value, MsgPackStream stream, IContext context)
        {
            var secondsSinceBclEpoch = value.Ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - BclSecondsAtUnixEpoch;
            var nanoseconds = (value.Ticks % TimeSpan.TicksPerSecond) * NanosecondsPerTick;

            // Reference:
            // struct timespec {
            //     long tv_sec;  // seconds
            //     long tv_nsec; // nanoseconds
            // } time;
            // if ((time.tv_sec >> 34) == 0) {
            //     uint64_t data64 = (time.tv_nsec << 34) | time.tv_sec;
            //     if (data64 & 0xffffffff00000000L == 0) {
            //         // timestamp 32
            //         uint32_t data32 = data64;
            //         serialize(0xd6, -1, data32)
            //     }
            //     else {
            //         // timestamp 64
            //         serialize(0xd7, -1, data64)
            //     }
            // }
            // else {
            //     // timestamp 96
            //     serialize(0xc7, 12, -1, time.tv_nsec, time.tv_sec)
            // }

            unchecked
            {
                if (seconds >> 34 == 0)
                {
                    var data64 = (ulong)((nanoseconds << 34) | seconds);
                    if ((data64 & 0xffffffff00000000ul) == 0ul)
                    {
                        var data32 = (uint)data64;
                        StreamWriter.WriteExtensionHeader(new ExtensionHeader(TypeCode, 4u), stream);
                        stream.WriteUInt32(data32);
                    }
                    else
                    {
                        StreamWriter.WriteExtensionHeader(new ExtensionHeader(TypeCode, 8u), stream);
                        stream.WriteUInt64(data64);
                    }
                }
                else
                {
                    StreamWriter.WriteExtensionHeader(new ExtensionHeader(TypeCode, 12u), stream);
                    stream.WriteUInt32((uint)nanoseconds);
                    stream.WriteInt64(seconds);
                }
            }
        }

        DateTime IFormatter<DateTime>.Read(MsgPackStream stream, IContext context)
        {
            // Reference:
            // ExtensionValue value = deserialize_ext_type();
            //     struct timespec result;
            // switch(value.length) {
            //     case 4:
            //         uint32_t data32 = value.payload;
            //         result.tv_nsec = 0;
            //         result.tv_sec = data32;
            //     case 8:
            //         uint64_t data64 = value.payload;
            //         result.tv_nsec = data64 >> 34;
            //         result.tv_sec = data64 & 0x00000003ffffffffL;
            //     case 12:
            //         uint32_t data32 = value.payload;
            //         uint64_t data64 = value.payload + 4;
            //         result.tv_nsec = data32;
            //         result.tv_sec = data64;
            //     default:
            //     // error
            // }

            var code = stream.Peek();
            var header = StreamReader.ReadExtensionHeader(stream);
            if (header.TypeCode != TypeCode)
            {
                throw new MsgPackExtensionException(header.TypeCode, TypeCode);
            }

            return Unpack(stream, header.Length);
        }

        internal static DateTime Unpack(MsgPackStream stream, uint length)
        {
            long seconds, nanoseconds;

            switch (length)
            {
                case 4:
                    nanoseconds = 0;
                    seconds = stream.ReadUInt32();
                    break;

                case 8:
                    var data64 = stream.ReadUInt64();
                    nanoseconds = (long)(data64 >> 34);
                    seconds = (long)(data64 & 0x00000003fffffffful);
                    break;

                case 12:
                    nanoseconds = stream.ReadUInt32();
                    seconds = stream.ReadInt64();
                    break;

                default: throw new MsgPackException("Invalid timestemp size {0}", length);
            }

            return UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / NanosecondsPerTick);
        }
    }
}
