namespace CannedBytes.Midi.Components
{
    using System;

    /// <summary>
    /// The MidiRunningStatusSender implements a "running status" algorithm used for
    /// sending short midi messages.
    /// </summary>
    /// <remarks>Typically this sender chain component is positioned right before the
    /// Midi Out Port because it can alter the output of short midi messages. Other
    /// chain components might not be able to handle the different format.</remarks>
    public class MidiDataRunningStatusSender : MidiDataSenderChain, IMidiDataSender, IInitializeByMidiPort
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public MidiDataRunningStatusSender()
        {
            EnableRunningStatus = true;
        }

        /// <summary>
        /// Gets the current running status.
        /// </summary>
        public byte RunningStatus { get; private set; }

        /// <summary>
        /// Backing field for the <see cref="EnableRunningStatus"/> property.
        /// </summary>
        private bool enableRunningStatus;

        /// <summary>
        /// Gets or sets a value that enables or disables the running status.
        /// </summary>
        /// <remarks>Setting this property will reset the running status value.</remarks>
        public bool EnableRunningStatus
        {
            get
            {
                return enableRunningStatus;
            }

            set
            {
                enableRunningStatus = value;
                RunningStatus = 0;
            }
        }

        /// <summary>
        /// This will send the short midi message to the next sender chain component.
        /// </summary>
        /// <param name="data">The sort midi message.</param>
        /// <remarks>If the status of this midi message is the same as the previous,
        /// the status is removed from the <paramref name="data"/>.</remarks>
        public void ShortData(int data)
        {
            if (EnableRunningStatus)
            {
                MidiData eventData = new MidiData(data);

                if (eventData.Status == RunningStatus)
                {
                    data = eventData.RunningStatusData;
                }
                else
                {
                    RunningStatus = eventData.Status;
                }
            }

            NextSenderShortData(data);
        }

        /// <summary>
        /// Sends the long midi message to the next sender chain component.
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <remarks>Long midi messages will reset the running status.</remarks>
        public void LongData(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, nameof(buffer));

            RunningStatus = 0;
            NextSenderLongData(buffer);
        }

        /// <summary>
        /// Initializes the sender component with the Midi Out Port.
        /// </summary>
        /// <param name="port">The Midi Out Port.</param>
        public void Initialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, "port");
            Check.IfArgumentNotOfType<MidiOutPort>(port, "port");

            port.StatusChanged += new EventHandler(MidiPort_StatusChanged);
        }

        /// <summary>
        /// Removes any reference to/from the <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi Out Port.</param>
        public void Uninitialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, "port");
            Check.IfArgumentNotOfType<MidiOutPort>(port, "port");

            port.StatusChanged -= new EventHandler(MidiPort_StatusChanged);
        }

        /// <summary>
        /// Called to dispose the object instance.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            // no op.
        }

        /// <summary>
        /// Event handler for the port status changed event.
        /// </summary>
        /// <param name="sender">The originating midi port.</param>
        /// <param name="e">Not used.</param>
        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            if (port.HasStatus(MidiPortStatus.Closed | MidiPortStatus.Reset))
            {
                RunningStatus = 0;
            }
        }
    }
}