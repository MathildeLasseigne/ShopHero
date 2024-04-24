using MidiPlayerTK;
using MPTK.NAudio.Midi;
using MPTKDemoCatchMusic;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Midi player doc : https://mptkapi.paxstellar.com/d7/deb/class_midi_player_t_k_1_1_midi_file_player.html#a83e9c52c8838f586a97ed0c3d0eafe48
/// </summary>
public class MidiGameController : MonoBehaviour
{
    public MidiFilePlayer midiFilePlayer;

    public double startJumpMilli = 0;

    private float initVolume;

    private double tickLastNote;

    private Action<bool> notePlayedEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (midiFilePlayer != null)
        {
            // No listener defined, set now by script. NotesToPlay will be called for each new notes read from Midi file
            midiFilePlayer.OnEventNotesMidi.AddListener(FilterNotePlayedMPTKEvent);
            midiFilePlayer.OnEventStartPlayMidi.AddListener(info => InputMidiStartPositionMilli());
            initVolume = midiFilePlayer.MPTK_Volume;
        }
        else
            Debug.Log("MidiGameController: no MidiFilePlayer detected");
        
    }

    public void LoadMidiFile(string midiName)
    {
        //Load midi file
        midiFilePlayer.MPTK_MidiName = midiName;
        tickLastNote = midiFilePlayer.MPTK_TickLastNote;
    }

    public double GetMidiBPM()
    {
        return midiFilePlayer.MPTK_Tempo;
    }


    public MidiGameController SetStartJump(double startDelayMilli)
    {
        this.startJumpMilli = startDelayMilli;
        Debug.Log("Start jump = " + startDelayMilli);
        return this;
    }

    /// <summary>
    /// Start play midi after a certain amount of milliseconds
    /// </summary>
    /// <param name="delayMilli"></param>
    public void StartPlayAt(double delayMilli)
    {
    }

    /// <summary>
    /// Play the midi. Take into account the start jump
    /// </summary>
    public void PlayMidi()
    {
        midiFilePlayer.MPTK_Play();
    }

    private void InputMidiStartPositionMilli()
    {
        Debug.Log("Midi do jump to start position");
        midiFilePlayer.MPTK_Position = startJumpMilli;
    }


    /// <summary>
    /// Wait for delayRampDown milliseconds while bringing volume to zero before stopping player and calling callback
    /// </summary>
    /// <param name="delayRampDown"></param>
    /// <param name="callback"></param>
    public void Stop(float delayRampDown, Action callback)
    {
        StartCoroutine(ChangeVolumeWithinDelay(delayRampDown, initVolume, 0, () => {
            midiFilePlayer.MPTK_Stop(false);
            midiFilePlayer.MPTK_Volume = initVolume;
            callback?.Invoke();
        }));
    }


    public void Stop()
    {
        midiFilePlayer.MPTK_Stop(false);
    }


    private IEnumerator ChangeVolumeWithinDelay(float delayVolumeMilli, float startVolume, float endVolume, Action callback)
    {
        float t = 0f;
        float step = 5f;
        WaitForSeconds delay = new WaitForSeconds(step / 60);
        while (t < delayVolumeMilli)
        {
            midiFilePlayer.MPTK_Volume = Mathf.Lerp(startVolume, endVolume, t);
            t += step;
            yield return delay;
        }
        callback?.Invoke();
    }

    public void FilterNotePlayedMPTKEvent(List<MPTKEvent> notes)
    {
        bool isNoteOn = false;
        bool isLastNote = false;
        foreach (MPTKEvent mptkEvent in notes) // Doubles or triple notes. All in the same tick
        {
            if(mptkEvent != null && mptkEvent.Command == MPTKCommand.NoteOn)
            {
                isLastNote |= mptkEvent.Tick == tickLastNote;
                //notePlayedEvent?.Invoke(isLastNote);
                isNoteOn = true;
            }
        }
        //Debug.Log("Notes number = " + notes.Count);
        if (isNoteOn)
            notePlayedEvent?.Invoke(isLastNote);
    }

    public int GetNoteNumber(int channel = -1)
    {
        int count = 0;
        MidiLoad midiloaded = midiFilePlayer.MPTK_Load();
        long currentTick = 0;
        if (midiloaded != null)
        {
            List<MPTKEvent> listEvents = midiloaded.MPTK_ReadMidiEvents();
            foreach(MPTKEvent mptkEvent in listEvents)
            {
                if (mptkEvent != null && mptkEvent.Command == MPTKCommand.NoteOn && (channel == -1 || (mptkEvent.Channel == channel)))
                {
                    if(currentTick != mptkEvent.Tick) //Avoid counting double notes
                    {
                        count++;
                        currentTick = mptkEvent.Tick;
                    }
                }
            }
        }
        return count;
    }



    /// <summary>
    /// From the speed of the music and the distance travelled by notes, how long will it takes for notes to arrive ?
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public double CalculateDelayFromDistance(float distance, float speed)
    {
        //double speed = GetMidiBPM() / 60; // == bps == beat per second
        // speed == distance / time

        double time = distance / speed; //sec
        time *= 1000; //milli sec
        return time;
    }

    public void SuscribeToNotePlayedEvent(Action<bool> callback)
    {
        notePlayedEvent += callback;
    }

    public void UnsuscribeToNotePlayedEvent(Action<bool> callback)
    {
        notePlayedEvent -= callback;
    }
}
