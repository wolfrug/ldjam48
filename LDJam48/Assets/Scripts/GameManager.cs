using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameStateEvent : UnityEvent<GameState> { }

public enum GameStates {
    NONE = 0000,
    INIT = 1000,
    LATE_INIT = 1100,
    GAME = 2000,
    NARRATIVE = 3000,
    INVENTORY = 4000,
    DEFEAT = 5000,
    WIN = 6000,
    PAUSE = 7000,

}

[System.Serializable]
public class GameState {
    public GameStates state;
    public GameStates nextState;
    public GameStateEvent evtStart;

}

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public bool initOnStart = true;
    [NaughtyAttributes.ReorderableList]
    public GameState[] gameStates;
    [SerializeField]
    private GameState currentState;
    public float lateInitWait = 0.1f;
    private Dictionary<GameStates, GameState> gameStateDict = new Dictionary<GameStates, GameState> { };
    private BasicAgent player;
    private InventoryController m_playerInventory;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy (gameObject);
        }
        foreach (GameState states in gameStates) {
            gameStateDict.Add (states.state, states);
        }
    }
    void Start () {
        if (initOnStart) {
            Invoke ("Init", 1f); // uncomment if not going via mainmenu
        };
        //AudioManager.instance.PlayMusic ("MusicBG");
    }

    [NaughtyAttributes.Button]
    public void Init () {
        SetState (GameStates.INIT);
        //Invoke ("FixTerribleBug", 5f);
        //NextState ();
    }

    public void FixTerribleBug () {
        Invoke (nameof (StartStoryLate), 0.2f);
    }
    void StartStoryLate () {
        InkWriter.main.StartStory ();
    }

    void Late_Init () {
        currentState = gameStateDict[GameStates.LATE_INIT];
        Debug.Log ("Invoking late init");
        currentState.evtStart.Invoke (currentState);
        if (currentState.nextState != GameStates.NONE) {
            NextState ();
        }
    }
    public void NextState () {
        if (currentState.nextState != GameStates.NONE) {
            if (gameStateDict[currentState.state].nextState == GameStates.LATE_INIT) { // late init inits a bit late and only works thru nextstate
                Invoke ("Late_Init", lateInitWait);
                // Debug.Log ("Invoking late init");
                return;
            } else {
                Debug.Log ("Invoking Next State " + "(" + gameStateDict[currentState.state].nextState.ToString () + ")");
                SetState (gameStateDict[currentState.state].nextState);
            };
        }
    }
    public void SetState (GameStates state) {
        if (state != GameStates.NONE) {
            GameState = state;
        };
    }
    public GameStates GameState {
        get {
            if (currentState != null) {
                return currentState.state;
            } else {
                return GameStates.NONE;
            }
        }
        set {
            Debug.Log ("Changing state to " + value);
            currentState = gameStateDict[value];
            currentState.evtStart.Invoke (currentState);
            if (currentState.nextState != GameStates.NONE) {
                NextState ();
            };
        }
    }

    public void WinGame () {
        GameState = GameStates.WIN;
        Debug.Log ("Victory!!");
        SceneManager.LoadScene ("EndScene");
    }
    public void Defeat () {

        currentState = gameStateDict[GameStates.DEFEAT];
        currentState.evtStart.Invoke (currentState);
    }

    public void Restart () {
        Time.timeScale = 1f;
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name, LoadSceneMode.Single);
    }

    public void DualLoadScenes () {
        SceneManager.LoadScene ("ManagersScene", LoadSceneMode.Additive);
        SceneManager.LoadScene ("SA_Demo", LoadSceneMode.Additive);
    }

    public void BackToMenu () {
        Time.timeScale = 1f;
        SceneManager.LoadScene ("MainMenu");
    }

    public void SaveGame (int slot) {
        Debug.Log ("Saving in slot " + slot);
        ES3AutoSaveMgr.Current.settings.path = "LDJam48_RatPack_" + slot.ToString () + ".es3";
        ES3AutoSaveMgr.Current.Save ();
    }
    public void LoadGame (int slot) {
        Debug.Log ("Loading from slot " + slot);
        ES3AutoSaveMgr.Current.settings.path = "LDJam48_RatPack_" + slot.ToString () + ".es3";
        ES3AutoSaveMgr.Current.Load ();
    }
    public void Pause () {
        GameState oldState = currentState;
        GameState pauseState = gameStateDict[GameStates.PAUSE];
        GameState = GameStates.PAUSE;
        pauseState.evtStart.Invoke (gameStateDict[GameStates.PAUSE]);
        StartCoroutine (PauseWaiter (oldState.state));
        Time.timeScale = 0f;
    }
    public void UnPause () {
        GameState = GameStates.NONE;
        Time.timeScale = 1f;
    }
    IEnumerator PauseWaiter (GameStates continueState) {
        yield return new WaitUntil (() => GameState != GameStates.PAUSE);
        GameState = continueState;
    }

    public void InitInventoryEvents () {
        Debug.Log ("Attempting to init inventory events");
        PlayerInventory = InventoryController.GetInventoryOfType (InventoryType.PLAYER, null, false);
        foreach (InventoryController lootableInventory in InventoryController.GetAllInventories (InventoryType.NONE, null, false)) {
            Debug.Log ("Adding events to " + InventoryController.GetAllInventories (InventoryType.NONE, null, false).Count + " inventories");
            Debug.Log ("Next inventory is: " + lootableInventory.name);
            lootableInventory.InitInventory (lootableInventory.data, lootableInventory.clearOnStart);
            lootableInventory.inventoryOpenedEvent.AddListener (OpenInventory);
            lootableInventory.inventoryClosedEvent.AddListener (CloseInventory);
        }
        // Also init looking for player lol

    }
    public void InitDoozy () {
        Debug.Log ("Starting Doozy");
        Doozy.Engine.GameEventMessage.SendEvent ("InitDoozy");
    }
    void OpenInventory (InventoryController otherInventory) {
        SetState (GameStates.INVENTORY);
        Debug.Log ("Inventory opened " + otherInventory.gameObject);
        if (otherInventory.type == InventoryType.LOOTABLE || otherInventory.type == InventoryType.CRAFTING) { // auto-open player inventory when opening lootable container
            PlayerInventory.Visible = true;
        }
    }
    void CloseInventory (InventoryController otherInventory) {

        //  Debug.Log ("Inventory closed " + otherInventory.gameObject);
        if (otherInventory.type == InventoryType.LOOTABLE) {
            PlayerInventory.Visible = false;
        }
        if (otherInventory.type == InventoryType.PLAYER) {
            InventoryController.CloseAllInventories (InventoryType.LOOTABLE);
            InventoryController.CloseAllInventories (InventoryType.CRAFTING);
        }
        if (InventoryController.GetAllInventories ().Count == 0 && GameState == GameStates.INVENTORY) {
            SetState (GameStates.GAME);
        }
    }

    public void OpenJournal () {
        if (GameState == GameStates.GAME) {
            InkWriter.main.GoToKnot ("OpenJournalExt");
        }
    }
    public void OpenOwnInventory () {
        if (GameState != GameStates.GAME) {
            return;
        } else {
            PlayerInventory.Visible = true;
            //AudioManager.instance.PlaySFX ("ClickButton");
        }
    }
    public void OpenCraftingInventory () {
        if (GameState == GameStates.INVENTORY) {
            InventoryController.GetInventoryOfType (InventoryType.CRAFTING, null, false).Visible = true;
        } else if (GameState != GameStates.GAME) {
            return;
        } else {
            InventoryController.GetInventoryOfType (InventoryType.CRAFTING, null, false).Visible = true;
            //AudioManager.instance.PlaySFX ("ClickButton");
        }
    }

    public void Ink_CheckHasItem (object[] inputVariables) {
        // variable 0 -> m_id of item looked for
        // variable 1 -> ink variable name to set value to (-1 does not have, 0+ has with count)

        string inkVariableName = (string) inputVariables[1];
        string m_id = (string) inputVariables[0];
        ItemData data = InventoryController.GetDataByID (m_id);
        if (data == null) {
            Debug.LogWarning ("No such item with ID" + m_id);
            InkWriter.main.story.variablesState[(inkVariableName)] = -1;
            return;
        }
        int returnVariable = PlayerInventory.CountItem (data);
        InkWriter.main.story.variablesState[(inkVariableName)] = returnVariable;
    }
    public void Ink_ConsumeItem (object[] inputVariables) {
        //variable 0 -> m_id of item looked for
        //variable 1 -> integer of number of items to consume
        // note -> it's up to the inkist to check that there is enough before committing, there is no confirmation!
        int amount = (int) inputVariables[1];
        string m_id = (string) inputVariables[0];
        ItemData data = InventoryController.GetDataByID (m_id);
        if (data == null) {
            Debug.LogWarning ("No such item with ID" + m_id);
            return;
        }
        int returnVariable = PlayerInventory.CountItem (data);
        if (!PlayerInventory.DestroyItemAmount (data, amount)) {
            Debug.LogWarning ("Failed to destroy the required amount of item " + m_id + "(" + m_id + ")");
        }
    }
    public void Ink_AddItem (object[] inputVariables) {
        //variable 0 -> m_id of item looked for
        //variable 1 -> integer of number of items to add
        // note -> it's up to the inkist to check that there is enough before committing, there is no confirmation!
        int amount = (int) inputVariables[1];
        string m_id = (string) inputVariables[0];
        ItemData data = InventoryController.GetDataByID (m_id);
        if (data == null) {
            Debug.LogWarning ("No such item with ID" + m_id);
            return;
        }
        if (!PlayerInventory.AddItem (data, amount)) {
            Debug.LogWarning ("Failed to add the required amount of item " + m_id + "(" + m_id + ")");
        }
    }

    public void AddItem (ItemData item) {
        Doozy.Engine.GameEventMessage.SendEvent ("ShowPlayerInventory");
        PlayerInventory.AddItem (item, 1);
    }

    public BasicAgent Player {
        get {
            if (player == null) {
                player = GameObject.FindGameObjectWithTag ("Player").GetComponent<BasicAgent> ();
                //mover.targetAgent = player.navMeshAgent;
            };
            return player;
        }
    }
    public InventoryController PlayerInventory {
        get {
            if (m_playerInventory == null) {
                m_playerInventory = InventoryController.GetInventoryOfType (InventoryType.PLAYER, null, false);
            }
            return m_playerInventory;
        }
        set {
            m_playerInventory = value;
        }
    }

    // Various game-specific things
    public void StartDrowning () {
        SurvivalManager.instance.StartPermanentEffect (DamageType.HEALTH, -0.1f, 1f, "drowning");
    }
    public void StopDrowning () {
        SurvivalManager.instance.StopPermanentEffect ("drowning");
    }
    public void RefillOxygen (float amountPercentage) {
        float maxOxygen = SurvivalManager.instance.GetHealthBar (DamageType.OXYGEN).minMaxHealth.y;
        maxOxygen *= amountPercentage;
        Debug.Log ("Current oxygen to be added: " + maxOxygen + " percentage: " + amountPercentage);
        SurvivalManager.instance.SpawnHealthEffect (DamageType.OXYGEN, maxOxygen / 2f, 2f);
    }
    public void StartRegularOxygenLoss (float dps = 0.01f) {
        SurvivalManager.instance.StartPermanentEffect (DamageType.OXYGEN, -dps, 1f, "oxygenLoss");
    }
    public void StopRegularOxygenLoss () {
        SurvivalManager.instance.StopPermanentEffect ("oxygenLoss");
    }
    public void ShowElevatorButtons () {
        Doozy.Engine.GameEventMessage.SendEvent ("ShowElevatorButtons");
    }
    public void HideElevatorButtons () {
        Doozy.Engine.GameEventMessage.SendEvent ("HideElevatorButtons");
    }
    public void PlayVoiceOver (object[] inputVariables) {
        string id = inputVariables[0] as string;
        AudioManager.instance.PlaySFX (id);
    }
    public void GoToLevel0 () {
        SetState (GameStates.WIN);
    }
    public void Death () {
        InkWriter.main.story.variablesState["died"] = true;
        GoToLevel0 ();
    }

}