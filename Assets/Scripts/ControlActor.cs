using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogoDigital.Lipsync;
using DigitalRuby.RainMaker;
using UnityEngine.Windows.Speech;
using System.Linq;


public class ControlActor : MonoBehaviour {
    LipSync lipSync;
    public bool choreography = false;
    public bool interpret = false;
    public LipSyncData tobe;
    private LipSyncData data;
    public LipSyncData blade;
    public RainScript rain;
    public GameObject decoration;
    public GameObject spotlight;
    public GameObject setLighting;
    public GameObject deck;
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    Animator anim;
    public Vector3 initPos;
    public Vector3 currentPos;
    // Use this for initialization
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        Debug.Log(args.text);
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();

        }
    }
    void Start () {
        anim = GetComponent<Animator>();
        lipSync = GetComponent<LipSync>();
        rain.enabled = false;

        initPos = this.transform.position;

        //Create keywords for keyword recognizer

        keywords.Add("to be", () =>
        {
            Debug.Log("To be monologue!");
            data = tobe;
        });

        keywords.Add("blade runner", () =>
        {
            data = blade;
        });

        keywords.Add("rain", () =>
        {
            Debug.Log("Rain is on!");
            rain.enabled = true;
        });

        keywords.Add("no rain", () =>
        {
            Debug.Log("No rain!");
            rain.enabled = false;
        });

        keywords.Add("action", () =>
        {
            Debug.Log("Action!");

            if (choreography)
            {
                anim.SetTrigger("start");
                Debug.Log("Choreography!");
            }
            if (interpret)
            {
                anim.SetTrigger("interpret");
                Debug.Log("Interpret!");
            }

            lipSync.Play(data);

        });

        keywords.Add("blocking", () =>
        {
            Debug.Log("Blocking!");
        });

        keywords.Add("spot light off", () =>
        {
            Debug.Log("Spot light off!");
            spotlight.SetActive(false);
        });

        keywords.Add("set lighting", () =>
        {
            Debug.Log("Set lights on!");
            setLighting.SetActive(true);
        });

        keywords.Add("decoration", () =>
        {
            Debug.Log("Stage decoration on!");
            decoration.SetActive(true);
            deck.SetActive(false);
        });

        keywords.Add("spot light on", () =>
        {
            Debug.Log("Spot light on!");
            spotlight.SetActive(true);
        });

        keywords.Add("interpret", () =>
        {
            Debug.Log("Actor interprets!");
            interpret = true;
            choreography = false;
        });

        keywords.Add("learn the movement", () =>
        {
            Debug.Log("Actor is given choreography!");
            interpret = false;
            choreography = true;
        });

        keywords.Add("subtle", () =>
        {

            Debug.Log("Making it subtle");
            List<PhonemeShape> phonemes = lipSync.phonemes;
            List<PhonemeShape> newPhonemes = new List<PhonemeShape>();

            foreach (PhonemeShape p in phonemes) {
                List<float> weights = new List<float>();
                for (int i = 0; i < p.weights.Count; i++) {

                    weights.Add(p.weights[i] * 0.4f);

                }
                p.weights = weights;
                newPhonemes.Add(p);
            }
            lipSync.phonemes = newPhonemes;
            Debug.Log("Making it subtle");

        });

        keywords.Add("dry run", () =>
        {
            Debug.Log("Dry run of a scene!");
            List<PhonemeShape> phonemes = lipSync.phonemes;
            List<PhonemeShape> newPhonemes = new List<PhonemeShape>();

            foreach (PhonemeShape p in phonemes)
            {
                List<float> weights = new List<float>();
                for (int i = 0; i < p.weights.Count; i++)
                {

                    weights.Add(p.weights[i] * 1.5f);

                }
                p.weights = weights;
                newPhonemes.Add(p);
            }
            lipSync.phonemes = newPhonemes;

        });

        keywords.Add("your own death", () =>
        {
            Debug.Log("Making it subtle");
            List<PhonemeShape> phonemes = lipSync.phonemes;
            List<PhonemeShape> newPhonemes = new List<PhonemeShape>();

            foreach (PhonemeShape p in phonemes)
            {
                List<float> weights = new List<float>();
                for (int i = 0; i < p.weights.Count; i++)
                {

                    weights.Add(p.weights[i] * 0.7f);

                }
                p.weights = weights;
                newPhonemes.Add(p);
            }
            lipSync.phonemes = newPhonemes;
            Debug.Log("Making it subtle");

        });

        keywords.Add("cut", () =>
        {
            Debug.Log("Cut!");
            anim.SetTrigger("end");
            anim.ResetTrigger("interpret");
            this.transform.position = new Vector3(transform.position.x, initPos.y, transform.position.z);
            lipSync.Stop(true);
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
        currentPos = this.transform.position;
    }
}
