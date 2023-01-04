using System;
using System.Collections.Generic;
using System.Text;
using CannedBytes.Midi.IO;

namespace CannedBytes.Midi.Samples.RecordToFile.Midi
{
    public  class MidiFileEventExt:MidiFileEvent
    {
        private string counter;

        public string Counter { get => MidiFileEventExt.counterClass++.ToString(); set => counter = value; }
        
         static int counterClass = 0;
    }
}
