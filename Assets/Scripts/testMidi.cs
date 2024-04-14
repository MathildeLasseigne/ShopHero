using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using MidiParser;


public class testMidi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const string Path = "test.mid";

        Console.WriteLine("Parsing: {0}\n", Path);

        var midiFile = new MidiFile(Path);

        Console.WriteLine("Format: {0}", midiFile.Format);
        Console.WriteLine("TicksPerQuarterNote: {0}", midiFile.TicksPerQuarterNote);
        Console.WriteLine("TracksCount: {0}", midiFile.TracksCount);

        foreach (var track in midiFile.Tracks)
        {
            Console.WriteLine("\nTrack: {0}\n", track.Index);

            foreach (var midiEvent in track.MidiEvents)
            {
                const string Format = "{0} Channel {1} Time {2} Args {3} {4}";
                if (midiEvent.MidiEventType == MidiEventType.MetaEvent)
                {
                    Console.WriteLine(
                        Format,
                        midiEvent.MetaEventType,
                        "-",
                        midiEvent.Time,
                        midiEvent.Arg2,
                        midiEvent.Arg3);
                }
                else
                {
                    Console.WriteLine(
                        Format,
                        midiEvent.MidiEventType,
                        midiEvent.Channel,
                        midiEvent.Time,
                        midiEvent.Arg2,
                        midiEvent.Arg3);
                }
            }
        }

        Console.WriteLine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
