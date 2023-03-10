namespace CannedBytes.Midi
{
    using System;
    using System.IO;

    /// <summary>
    /// The MidiInBufferManager manages <see cref="MidiBufferStream"/> instances on behalf of
    /// a <see cref="MidiInPort"/> instance.
    /// </summary>
    public sealed class MidiInBufferManager : MidiBufferManager
    {
        /// <summary>
        /// Initializes the buffer manager on the Midi In <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        internal MidiInBufferManager(MidiInPort port)
            : base(port, FileAccess.Read)
        { }

        /// <summary>
        /// Returns the <paramref name="buffer"/> to the pool.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>Call this method when the <paramref name="buffer"/> is no longer needed.</remarks>
        public override void ReturnBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            // do not re-add buffers during a Reset (or Close) that is meant to return all
            // buffers from the MidiInPort to the buffer manager.
            if (!MidiPort.HasStatus(MidiPortStatus.Reset | MidiPortStatus.Closed))
            {
                // returned buffers are added to the midi in port again
                // to make them available for recording sysex.
                AddBufferToPort(buffer);
            }
            else
            {
                OnUnprepareBuffer(buffer);
                base.ReturnBuffer(buffer);
            }
        }

        /// <summary>
        /// Prepares the <paramref name="buffer"/> for the Midi In Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>This method is not intended to be called by client code.</remarks>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiInPrepareHeader(
                         MidiPort.MidiSafeHandle,
                         buffer.ToIntPtr(),
                         (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }

        /// <summary>
        /// Un-prepares the <paramref name="buffer"/> for the Midi In Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>This method is not intended to be called by client code.</remarks>
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiInUnprepareHeader(
                         MidiPort.MidiSafeHandle,
                         buffer.ToIntPtr(),
                         (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }

        /// <summary>
        /// Initializes the buffer pool of the buffer manager.
        /// </summary>
        /// <param name="bufferCount">Specify 0 for no buffers.</param>
        /// <param name="bufferSize">Specify greater than 0.</param>
        public override void Initialize(int bufferCount, int bufferSize)
        {
            if (bufferSize <= 0)
            {
                // default buffer size
                bufferSize = 1024 * 4;
            }

            base.Initialize(bufferCount, bufferSize);

            if (MidiPort.IsOpen)
            {
                RegisterAllBuffers();
            }
        }

        /// <summary>
        /// Gets the Midi In Port.
        /// </summary>
        public new MidiInPort MidiPort
        {
            get { return (MidiInPort)base.MidiPort; }
        }

        /// <summary>
        /// Registers all buffers in the pool with the Midi In Port.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the Midi In Port is not open.</exception>
        /// <remarks>After <see cref="M:MidiInPort.Reset"/> has been called, all buffers are
        /// returned to this buffer manager. Use this function to register all the (unused) buffers
        /// with the midi port again in order to receive long midi messages.
        /// Note that the <see cref="P:MidiInPort.IsOpen"/> must be true - the port must be open.</remarks>
        public void RegisterAllBuffers()
        {
            if (!MidiPort.IsOpen)
            {
                throw new InvalidOperationException(Properties.Resources.MidiInPort_PortNotOpen);
            }

            // add unused buffers to port
            MidiBufferStream buffer = RetrieveBuffer();
            while (buffer != null)
            {
                OnPrepareBuffer(buffer);
                AddBufferToPort(buffer);

                buffer = RetrieveBuffer();
            }
        }

        /// <summary>
        /// Adds the <paramref name="buffer"/> to the midi port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        private void AddBufferToPort(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            // make sure the stream is rewound.
            buffer.Position = 0;

            int result = NativeMethods.midiInAddBuffer(
                         MidiPort.MidiSafeHandle,
                         buffer.ToIntPtr(),
                         (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }
    }
}