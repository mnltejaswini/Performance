// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;

namespace MultipartPostClient
{
    public enum DataGenerationType
    {
        Binary =    1 << 0,
        Text =      1 << 1,
    }

    internal interface IDataGenerator
    {
        int Read(byte[] buffer, int offset, int count);
    }

    internal static class DataGeneratorFactory
    {
        private abstract class RandomDataGeneratorBase
        {
            protected readonly Random Random;

            protected RandomDataGeneratorBase()
            {
                Random = new Random(DateTime.UtcNow.Millisecond);
            }
        }

        private sealed class RandomBinaryDataGenerator : RandomDataGeneratorBase, IDataGenerator
        {
            public int Read(byte[] buffer, int offset, int count)
            {
                Random.NextBytes(buffer);
                return count;
            }
        }

        private sealed class RandomIso8859TextDataGenerator : RandomDataGeneratorBase, IDataGenerator
        {
            public int Read(byte[] buffer, int offset, int count)
            {
                for (var iter = 0; iter < buffer.Length; ++iter)
                {
                    buffer[iter] = GetIso8859Byte();
                }
                return count;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private byte GetIso8859Byte()
            {
                if (Random.NextDouble() < 0.5)
                {
                    return (byte) Random.Next(0x20, 0x7E);
                }
                return (byte) Random.Next(0xA0, 0xFF);
            }
        }

        public static IDataGenerator GetNewDataGenerator(DataGenerationType dataGenerationType)
        {
            switch (dataGenerationType)
            {
                case DataGenerationType.Binary:
                    return new RandomBinaryDataGenerator();
                case DataGenerationType.Text:
                    return new RandomIso8859TextDataGenerator();
                default:
                    throw new InvalidOperationException(nameof(dataGenerationType));
            }
        }
    }

    public class RandomDataStreamContent : HttpContent
    {
        private const int DefaultBufferSize = 4096;
        
        private readonly int _bufferSize;
        private readonly long _desiredLength;
        private readonly Action<long> _progressUpdater;
        private readonly IDataGenerator _generator;

        public RandomDataStreamContent(DataGenerationType dataGenerationType, long desiredLength, Action<long> progressUpdater = null) :
            this(dataGenerationType, desiredLength, DefaultBufferSize, progressUpdater) { }

        public RandomDataStreamContent(DataGenerationType dataGenerationType, long desiredLength, int bufferSize, Action<long> progressUpdater = null)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }
            
            _bufferSize = bufferSize;
            _progressUpdater = progressUpdater;
            _desiredLength = desiredLength;
            _generator = DataGeneratorFactory.GetNewDataGenerator(dataGenerationType);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Serializing data");
                var buffer = new byte[_bufferSize];
                var uploaded = 0;
                
                while (uploaded < _desiredLength)
                {
                    var length = _generator.Read(buffer, 0, buffer.Length);
                    if (length <= 0) break;

                    uploaded += length;
                    if (uploaded > _desiredLength)
                    {
                        var delta = (int)(uploaded - _desiredLength);
                        uploaded -= delta;
                        length -= delta;
                    }

                    stream.Write(buffer, 0, length);
                    _progressUpdater?.Invoke(uploaded);
                }
                Console.WriteLine($"Serialized { _desiredLength } bytes");
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
