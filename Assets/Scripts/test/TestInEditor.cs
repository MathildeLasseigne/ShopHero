using MidiPlayerTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInEditor : MonoBehaviour
{
    public int noteCount = 0;

    public MidiFilePlayer midiPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if(midiPlayer != null)
            midiPlayer.OnEventNotesMidi.AddListener(midiCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void midiCount(List<MPTKEvent> notes)
    {
        foreach (MPTKEvent mptkEvent in notes)
        {
            if (mptkEvent != null && mptkEvent.Command == MPTKCommand.NoteOn)
            {
                noteCount++;
            }
        }
    }
}
