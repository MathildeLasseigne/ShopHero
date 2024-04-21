using MidiPlayerTK;
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

    private Action notePlayedEvent;

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

    public double GetMidiBPM()
    {
        return midiFilePlayer.MPTK_Tempo;
    }


    public MidiGameController SetStartJump(double startDelayMilli)
    {
        this.startJumpMilli = startDelayMilli;
        return this;
    }

    /// <summary>
    /// Start play midi after a certain amount of milliseconds
    /// </summary>
    /// <param name="delayMilli"></param>
    public void StartPlayAt(double delayMilli)
    {
    }

    public void PlayMidi()
    {
        midiFilePlayer.MPTK_Play();
    }

    private void InputMidiStartPositionMilli()
    {
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
        foreach (MPTKEvent mptkEvent in notes)
        {
            if(mptkEvent != null && mptkEvent.Command == MPTKCommand.NoteOn)
            {
                notePlayedEvent?.Invoke();
            }
        }
    }



    /// <summary>
    /// From the speed of the music and the distance travelled by notes, how long will it takes for notes to arrive ?
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public double CalculateDelayFromDistance(float distance)
    {
        double speed = GetMidiBPM() / 60; // == bps == beat per second
        // speed == distance / time

        double time = distance / speed;
        return time;
    }

    public void SuscribeToNotePlayedEvent(Action callback)
    {
        notePlayedEvent += callback;
    }

    public void UnsuscribeToNotePlayedEvent(Action callback)
    {
        notePlayedEvent -= callback;
    }
}
