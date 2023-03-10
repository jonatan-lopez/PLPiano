namespace CannedBytes.Midi.IO
{
    using CannedBytes.Midi.Message;
    using System;
    using System.Globalization;

    /// <summary>
    /// Writes midi messages to an out-stream buffer.
    /// </summary>
    /// <remarks>Uses the <see cref="MidiStreamEventWriter"/> internally.</remarks>
    public class MidiMessageOutStreamWriter : DisposableBase
    {
        /// <summary>
        /// Constructs a new instance on the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        public MidiMessageOutStreamWriter(MidiBufferStream stream)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            BaseStream = stream;
            StreamWriter = new MidiStreamEventWriter(stream);
        }

        /// <summary>
        /// Gets a reference to the buffer stream that is written to.
        /// </summary>
        protected MidiBufferStream BaseStream { get; private set; }

        /// <summary>
        /// Gets the stream event writer.
        /// </summary>
        protected MidiStreamEventWriter StreamWriter { get; private set; }

        /// <summary>
        /// Indicates if the stream has enough room to write the specified <paramref name="message"/> to the stream.
        /// </summary>
        /// <param name="message">Must no be null.</param>
        /// <returns>Returns true if the message can be written.</returns>
        public virtual bool CanWrite(IMidiMessage message)
        {
            Check.IfArgumentNull(message, nameof(message));

            if (message is MidiShortMessage shortMessage)
            {
                return StreamWriter.CanWriteShort();
            }

            if (message is MidiLongMessage longMessage)
            {
                return StreamWriter.CanWriteLong(longMessage.GetData());
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture,
                    "The type '{0}' is not supported for message argument.", message.GetType().FullName), nameof(message));
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        public virtual void Write(IMidiMessage message, int deltaTime)
        {
            Check.IfArgumentNull(message, nameof(message));

            if (message is MidiShortMessage shortMessage)
            {
                Write(shortMessage, deltaTime);
                return;
            }

            if (message is MidiLongMessage longMessage)
            {
                Write(longMessage, deltaTime);
                return;
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture,
                    "The type '{0}' is not supported for message argument.", message.GetType().FullName), nameof(message));
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        public virtual void Write(MidiShortMessage message, int deltaTime)
        {
            Check.IfArgumentNull(message, nameof(message));

            StreamWriter.WriteShort(message.Data, deltaTime);
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        public virtual void Write(MidiLongMessage message, int deltaTime)
        {
            Check.IfArgumentNull(message, nameof(message));

            if (message is MidiSysExMessage sysexMessage)
            {
                StreamWriter.WriteLong(sysexMessage.GetData(), deltaTime);
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture,
                    "The type '{0}' is not supported for (long) message argument.", message.GetType().FullName), nameof(message));
        }

        /// <inheritdocs/>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed &&
                disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                StreamWriter.Dispose();
            }
        }
    }
}