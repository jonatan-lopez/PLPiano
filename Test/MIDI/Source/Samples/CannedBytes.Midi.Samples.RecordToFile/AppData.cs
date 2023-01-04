using CannedBytes.Midi.IO;
using CannedBytes.Midi.Samples.RecordToFile.Midi;
using CannedBytes.Midi.Samples.RecordToFile.UI;
using System.Collections.Generic;
using System.Windows.Threading;

namespace CannedBytes.Midi.Samples.RecordToFile
{
    public class AppData :ViewModelBase
    {
        public AppData(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;

            MidiInPorts = new MidiInPortCapsCollection();
            MidiReceiver = new MidiReceiver(this);
            Events = new List<MidiFileEventExt>();
        }

        public Dispatcher Dispatcher { get; private set; }

        public MidiInPortCapsCollection MidiInPorts { get; private set; }

        public MidiInPortCaps SelectedMidiInPort { get; set; }

        internal MidiReceiver MidiReceiver { get; private set; }

        public List<MidiFileEventExt> Events { get; private set; }

        protected string _LastKey;
        public string LastKey{ 
            get { return _LastKey; } 
            set
                    {
                        _LastKey = value;
                        OnPropertyChanged(nameof(LastKey));
                    }
                }
        internal static AppData Singleton { get => singleton; set => singleton = value; }

        private static AppData singleton;
    }
}