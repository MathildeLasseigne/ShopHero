using MidiPlayerTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInEditor : MonoBehaviour
{
    public MidiGameController midi;

    public int noteCount = 0;

    public MidiFilePlayer midiPlayer;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        /*if(midiPlayer != null)
            midiPlayer.OnEventNotesMidi.AddListener(midiCount);*/

        midi.SetMidiTempo(103);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Play music")]
    public void PlayMusic()
    {
        /*StartCoroutine(playMidi()); //Not synchro
        StartCoroutine(playAudio());*/
        audioSource.Play(); //This order is synchro
        midi.PlayMidi();
    }

    private IEnumerator playAudio()
    {
        audioSource.Play();
        yield return null;
    }

    private IEnumerator playMidi()
    {
        midi.PlayMidi();
        yield return null;
    }

    [ContextMenu("LogCount")]
    public void LogMidiCount()
    {
        if (midi != null)
        {
            Debug.Log("Note number in midi : " + midi.GetNoteNumber());
        }
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
