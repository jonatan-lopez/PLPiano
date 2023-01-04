using System;
using System.Text;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using CannedBytes.Midi.Samples.RecordToFile.UI;

namespace CannedBytes.Midi.Samples.RecordToFile.Midi
{
    internal sealed class MidiReceiver : DisposableBase, IMidiDataReceiver
    {
        private readonly AppData _appData;
        private readonly MidiInPort _inPort;

        private readonly MidiMessageFactory _factory = new MidiMessageFactory();

        public MidiReceiver(AppData appData)
        {
            _appData = appData;
            _inPort = new MidiInPort
            {
                Successor = this
            };
        }

        public void Start(int portId)
        {
            _appData.Events.Clear();

            _inPort.Open(portId);
            _inPort.Start();
        }

        public void Stop()
        {
            _inPort.Stop();
            _inPort.Close();
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            // we're not doing sys-ex.
        }

        public void ShortData(int data, long timestamp)
        {
            var evnt = new MidiFileEventExt
            {
                Message = _factory.CreateShortMessage(data),
                AbsoluteTime = timestamp
            };
            if (evnt.Message.GetType().ToString() != "CannedBytes.Midi.Message.MidiSysRealtimeMessage")
            {
                _appData.Events.Add(evnt);
                _appData.LastKey = FormatMsg((CannedBytes.Midi.Message.MidiChannelMessage)evnt.Message);
                
                NewModelView.Singleton.LastKey = _appData.LastKey;
            }
        }

        private string FormatMsg(MidiChannelMessage message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ST:"+message.Status.ToString());
            sb.Append(" P1:" + message.Parameter1.ToString());
            sb.Append(" P2:" + message.Parameter2.ToString());
            sb.Append(" DT:" + message.Data.ToString());

            return sb.ToString();
        }

        #endregion IMidiDataReceiver Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            _inPort.Dispose();
        }
    }
}