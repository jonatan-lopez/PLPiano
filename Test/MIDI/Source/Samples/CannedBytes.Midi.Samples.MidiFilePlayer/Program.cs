using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CannedBytes.Midi.Samples.MidiFilePlayer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string midiFileName = null;
            int outPortId = 0;

            if (args.Length > 0)
            {
                midiFileName = args[0];
            }

            if (args.Length > 1)
            {
                outPortId = Int16.Parse(args[1]);
            }

            if (args.Length <= 0)
            {
                Console.WriteLine("Usage: MidiFilePlayer 'C:\\Folder\\MidiFile.mid' [MidiOutPortId]");
                return;
            }

            Console.WriteLine("Reading midi file: " + midiFileName);
            var fileData = MidiFile.Read(midiFileName);

            IEnumerable<MidiFileEvent> notes = null;

            // merge all track notes and filter out sysex and meta events
            foreach (var track in fileData.Tracks)
            {
                if (notes == null)
                {
                    notes = from note in track.Events
                            where !(note.Message is MidiLongMessage)
                            select note;
                }
                else
                {
                    notes = (from note in track.Events
                             where !(note.Message is MidiLongMessage)
                             select note).Union(notes);
                }
            }

            // order track notes by absolute-time.
            notes = from note in notes
                    orderby note.AbsoluteTime
                    select note;

            // At this point the DeltaTime properties are invalid because other events from other
            // tracks are now merged between notes where the initial delta-time was calculated for.
            // We fix this in the play back routine.

            WriteHeaderInfoToConsole(fileData.Header);

            var caps = MidiOutPort.GetPortCapabilities(outPortId);
            MidiOutPortBase outPort = null;

            try
            {
                outPort = ProcessStreamingConsole(outPortId, fileData, notes, caps);
            }
            catch (Exception e)
            {
                Console.WriteLine();

                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            if (outPort != null)
            {
                outPort.Reset();
                if (!outPort.BufferManager.WaitForBuffersReturned(1000))
                {
                    Console.WriteLine("Buffers failed to return in 1 sec.");
                }

                outPort.Close();
                outPort.Dispose();
            }
        }

        private static MidiOutPortBase ProcessStreaming(int outPortId, MidiFile fileData,
            IEnumerable<MidiFileEvent> notes, MidiOutPortCaps caps)
        {
            var outPort = new MidiOutStreamPort();
            outPort.Open(outPortId);
            outPort.BufferManager.Initialize(10, 1024);
            outPort.TimeDivision = fileData.Header.TimeDivision;

            // TODO: extract Tempo from meta messages from the file.
            // 120 bpm (uSec/QuarterNote).
            outPort.Tempo = 500000;

            Console.WriteLine(String.Format("Midi Out Stream Port '{0}' is now open.", caps.Name));

            MidiMessageOutStreamWriter writer = null;
            MidiBufferStream buffer = null;
            MidiFileEvent lastNote = null;

            foreach (var note in notes)
            {
                if (writer == null)
                {
                    // brute force buffer aqcuirement.
                    // when callbacks are implemented this will be more elegant.
                    do
                    {
                        buffer = outPort.BufferManager.RetrieveBuffer();

                        if (buffer != null) break;

                        Thread.Sleep(50);
                    } while (buffer == null);

                    writer = new MidiMessageOutStreamWriter(buffer);
                }

                if (writer.CanWrite(note.Message))
                {
                    if (lastNote != null)
                    {
                        // fixup delta time artifically...
                        writer.Write(note.Message, (int)(note.AbsoluteTime - lastNote.AbsoluteTime));
                    }
                    else
                    {
                        writer.Write(note.Message, (int)note.DeltaTime);
                    }
                }
                else
                {
                    outPort.LongData(buffer);
                    writer = null;

                    Console.WriteLine("Buffer sent...");

                    if (!outPort.HasStatus(MidiPortStatus.Started))
                    {
                        outPort.Restart();
                    }
                }

                lastNote = note;
            }

            return outPort;
        }

        private static MidiOutPortBase ProcessStreamingConsole(int outPortId, MidiFile fileData,
    IEnumerable<MidiFileEvent> notes, MidiOutPortCaps caps)
        {
            //var outPort = new MidiOutStreamPort();
            //outPort.Open(outPortId);
            //outPort.BufferManager.Initialize(10, 1024);
            //outPort.TimeDivision = fileData.Header.TimeDivision;

            // TODO: extract Tempo from meta messages from the file.
            // 120 bpm (uSec/QuarterNote).
            //outPort.Tempo = 500000;

            Console.WriteLine(String.Format("Midi Out Console.", caps.Name));

            //MidiMessageOutStreamWriter writer = null;
            //MidiBufferStream buffer = null;
            MidiFileEvent lastNote = null;

            foreach (var note in notes)
            {

                int time = (int)(lastNote != null ? (note.AbsoluteTime - lastNote.AbsoluteTime) : note.DeltaTime);
                WriteNoteConsole(note, time);
               
                lastNote = note;
            }
           

            return null;
        }

        private static void WriteNoteConsole(MidiFileEvent note, int time)
        {
           if (note.Message!=null)
            {
                if (note.Message is CannedBytes.Midi.Message.MidiChannelMessage)
                {
                    MidiChannelMessage message = (MidiChannelMessage)note.Message;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" TM:" + note.AbsoluteTime);
                    sb.Append(" CM:" + message.Command.ToString());
                    sb.Append(" ST:" + message.Status.ToString());
                    sb.Append(" P1:" + message.Parameter1.ToString());
                    sb.Append(" P2:" + message.Parameter2.ToString());
                    sb.Append(" DT:" + message.Data.ToString());

                    Console.WriteLine(sb.ToString());
                }
                else
                {

                }
            }
            else
            {

            }
        }

        private static void WriteHeaderInfoToConsole(MThdChunk mThdChunk)
        {
            Console.WriteLine("Number of tracks: " + mThdChunk.NumberOfTracks);
            Console.WriteLine("Number of format: " + mThdChunk.Format);
            Console.WriteLine("Number of time division: " + mThdChunk.TimeDivision);
        }
    }
}