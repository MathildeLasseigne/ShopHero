using System.Collections.Generic;
using UnityEngine;

public class RythmeInterpreter : MonoBehaviour
{

    #region Vars

    [SerializeField] public TapGameController laneRouge;
    [SerializeField] public TapGameController laneBleue;
    [SerializeField] public TapGameController laneJaune;

    /// <summary>
    /// The rouge, bleue, jaune lanes in a list.
    /// <br/> For ease of use. Use <code>allLanes.ForEach(l -> l.methode())</code>
    /// </summary>
    public List<TapGameController> allLanes = new List<TapGameController>();

    [SerializeField] private MidiGameController midiController;


    [SerializeField] private IngredientValue lanesValues = new IngredientValue();


    //Difficulty

    [Header("Difficulty")]
    [SerializeField] Difficultylvl currentDifficulty = Difficultylvl.Easy;

    [SerializeField, Range(0, 100)] private int veryEasyRate = 20;
    [SerializeField, Range(0, 100)] private int easyRate = 30;
    [SerializeField, Range(0, 100)] private int mediumRate = 50;
    [SerializeField, Range(0, 100)] private int hardRate = 70;

    public enum Difficultylvl { VeryEasy = 0, Easy, Medium, Hard, Hell }


    [SerializeField] private int chanceForLong = 50;


    //Instantiation vars

    private List<TapGameController> lanesUsedInThisInstatiationList = new List<TapGameController>();

    #endregion

    public struct TileType
    {
        public bool isLongEnd;
        public bool isLongStart;

        public TileType(bool isLongEnd = false, bool isLongStart = false)
        {
            this.isLongEnd = isLongEnd;
            this.isLongStart = isLongStart;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        //For ease of writing
        allLanes.Add(laneRouge);
        allLanes.Add(laneBleue);
        allLanes.Add(laneJaune);

        if (laneRouge == null || laneBleue == null || laneJaune == null)
            Debug.LogWarning("[RythmeInterpreter] Lanes were not added in editor");
    }


    #region Setters

    /// <summary>
    /// Set the difficulty as an Enum.
    /// </summary>
    /// <param name="difficulty"> range from 0 to 3</param>
    private void SetDifficulty(int difficulty)
    {
        currentDifficulty = (Difficultylvl)difficulty;
    }

    /// <summary>
    /// Set the lanes values (ingredient values for each lane) 
    /// <br>Call before Load</br>
    /// </summary>
    /// <param name="values"></param>
    public RythmeInterpreter SetLanesValues(IngredientValue values)
    {
        lanesValues.Copy(values);
        return this;
    }

    #endregion



    #region Game start / end

    /// <summary>
    /// Reset all vars and clear all lists. Must call before using the object
    /// </summary>
    /// <returns></returns>
    public RythmeInterpreter Init()
    {
        lanesUsedInThisInstatiationList.Clear();
        currentDifficulty = Difficultylvl.VeryEasy;
        lanesValues.RemoveAll();

        if (laneRouge == null || laneBleue == null || laneJaune == null)
            Debug.Log("[RythmeInterpreter] Lanes not loaded properly");

        allLanes.ForEach(l => l.Init());
        return this;
    }

    /// <summary>
    /// Load and calculate all provided vars. 
    /// <br/>Load Midi info and calculate data. Any setting to be added like Ingredient value for exemple should be done 
    /// between call to Init() and LoadGame()
    /// </summary>
    /// <param name="midiName"></param>
    public RythmeInterpreter LoadGame(string midiName/*params*/)
    {
        SetDifficulty((int)Utils.MaxFloat(lanesValues.rougeValue, lanesValues.bleuValue, lanesValues.jauneValue));

        CheckLaneMidiUse();

        lanesValues.Add(new IngredientValue(1, 1, 1)); //Even if lane at 0, will still have some tiles

        if (midiController)
        {
            //midiController.LoadMidiFile(midiName);           //Load the correct file. Uncomment to set the midi file from code

            midiController.SetMidiTempo(Data.mainInstance.mainConfig.midiCorrectedTempo);
            //double bpsMidi = midiController.GetMidiBPM() / 60;


            double bpsMidi = Data.mainInstance.mainConfig.midiCorrectedTempo / 60;
            Debug.Log("Midi bpm  = " + midiController.GetMidiBPM() + " & bps = " + bpsMidi);

            allLanes.ForEach((l) => l.SetSpeedFromTempo(bpsMidi));

            //use Lane rouge as reference lane

            midiController.SetStartJump(midiController.CalculateDelayFromDistance(laneRouge.GetLaneDistance(), laneRouge.GetSpeed()));

        }
        else
        {
            Debug.LogWarning("Midi controller for [Rythme Interpreter] is missing !!");
        }

        allLanes.ForEach( l=> l.LoadGame(""));

        return this;
    }



    

    #endregion


    #region Start / Stop

    /// <summary>
    /// Start RythmeInterpreter and all lanes (if they are showing).
    /// Should only be called after Init and Load
    /// </summary>
    public void StartGame()
    {
        if (midiController)
        {
            midiController.SuscribeToNotePlayedEvent(InstantiateTileFromNote);
            midiController.PlayMidi();
        }

        allLanes.ForEach(l => {
            if (l.IsShowing())
                l.StartGame();
        });
    }

    public RythmeInterpreter StopGame()
    {
        midiController?.Stop();
        midiController?.UnsuscribeToNotePlayedEvent(InstantiateTileFromNote);
        StopAllCoroutines();

        allLanes.ForEach(l=>l.StopGame());
        return this;
    }

    #endregion

    /// <summary>
    /// <b> ! Change "lanesValues" !</b> and remove probability to use lane if the lane do not use midi
    /// </summary>
    private void CheckLaneMidiUse()
    {
        //if do not use midi, remove probability to use lane
        if (laneRouge.GetDoNotUseMidi())
            lanesValues.AddRouge(-100);
        if (laneBleue.GetDoNotUseMidi())
            lanesValues.AddBleu(-100);
        if (laneJaune.GetDoNotUseMidi())
            lanesValues.AddJaune(-100);
    }

    #region Tile instanciation



    private void InstantiateTileFromNote(bool isLastNote)
    {
        TileType tileType = new TileType(false, false);

        bool tileAndLanePreparationDone = false;
        if (isLastNote)
        {
            tileAndLanePreparationDone = CheckLastNoteLanesToInstantiateWithLongTiles();
            if(tileAndLanePreparationDone)
                tileType.isLongEnd = true;
        }

        if (!tileAndLanePreparationDone)
        {
            bool makeTile = IsTileInstanciatedFromDifficulty();
            if(! makeTile)
                return;

            if (TryStartNewLong() && !isLastNote)
                tileType.isLongStart = true; //May be changed depending on lanes

            ChooseLane();
        }

        foreach(TapGameController lane in lanesUsedInThisInstatiationList)
        {
            InstantiateTileForLane(lane, tileType);
        }

        lanesUsedInThisInstatiationList.Clear();

    }



    /// <summary>
    /// Decide if the tile is instanciated depending on the difficulty.
    /// The easier the difficulty is, the less probable it is for tiles to be instanciated.
    /// </summary>
    /// <returns></returns>
    private bool IsTileInstanciatedFromDifficulty()
    {
        if (currentDifficulty == Difficultylvl.Hell)
            return true;
        else
        {
            int randomValue = UnityEngine.Random.Range(0, 100);
            switch (currentDifficulty)
            {
                case Difficultylvl.VeryEasy:
                    return randomValue <= veryEasyRate;
                case Difficultylvl.Easy:
                    return randomValue <= easyRate;
                case Difficultylvl.Medium:
                    return randomValue <= mediumRate;
                case Difficultylvl.Hard:
                    return randomValue <= hardRate;
                default: return true;
            }
        }
    }

    private bool TryStartNewLong()
    {
        if (UnityEngine.Random.Range(1, 100) <= chanceForLong)
            return true;
        else return false;
    }

    /// <summary>
    /// Put the choosen lanes in lanesUsedInThisInstatiationList depending on the lane values
    /// </summary>
    private void ChooseLane()
    {
        float nbParts = lanesValues.rougeValue + lanesValues.bleuValue + lanesValues.jauneValue;
        float stepPerc = 100f / nbParts;

        float partRouge = lanesValues.rougeValue * stepPerc;
        float partBleu = lanesValues.bleuValue * stepPerc;
        float partJaune = lanesValues.jauneValue * stepPerc;

        float percMin = 0f;

        Vector2 rougePerc = new Vector2(percMin, percMin + partRouge);
        percMin += partRouge;
        Vector2 bleuPerc = new Vector2(percMin, percMin + partBleu);
        percMin += partBleu;
        Vector2 jaunePerc = new Vector2(percMin, percMin + partJaune);


        int laneNb = 0;
        float randomResult = UnityEngine.Random.Range(0f, 100f);

        if (randomResult > rougePerc.x && randomResult <= rougePerc.y)
            laneNb = 1;
        else if (randomResult > bleuPerc.x && randomResult <= bleuPerc.y)
            laneNb = 2;
        else if (randomResult > jaunePerc.x && randomResult <= jaunePerc.y)
            laneNb = 3;


        switch (laneNb)
        {
            case 1: lanesUsedInThisInstatiationList.Add(laneRouge);
                break;
            case 2: lanesUsedInThisInstatiationList.Add(laneBleue);
                break;
            case 3: lanesUsedInThisInstatiationList.Add(laneJaune);
                break;
            default: break;
        }
    }


    private void InstantiateTileForLane(TapGameController lane, TileType tileType)
    {
        if (lane.GetIsNextTileLong()) //Prevent from starting new long tile when end needed, and force end tile
        {
            tileType.isLongEnd = true;
            tileType.isLongStart = false;
        }

        lane.InstantiateTileFromType(tileType);
    }

    /// <summary>
    /// Check if one of the last lanes had to finish their long notes. If yes, add it to the list of lanes used.
    /// Multiple lanes can be added
    /// </summary>
    /// <returns>True if Lanes were added</returns>
    private bool CheckLastNoteLanesToInstantiateWithLongTiles()
    {
        bool laneDistributionDone = false;
        if (laneRouge.GetIsNextTileLong())
        {
            lanesUsedInThisInstatiationList.Add(laneRouge);
            laneDistributionDone = true;
        }
        if (laneBleue.GetIsNextTileLong())
        {
            lanesUsedInThisInstatiationList.Add(laneBleue);
            laneDistributionDone = true;
        }
        if (laneJaune.GetIsNextTileLong())
        {
            lanesUsedInThisInstatiationList.Add(laneJaune);
            laneDistributionDone = true;
        }
        return laneDistributionDone;
    }

    private void UseLane(TapGameController lane)
    {
        if(! lanesUsedInThisInstatiationList.Contains(lane))
            lanesUsedInThisInstatiationList.Add(lane);
    }

    #endregion


}
