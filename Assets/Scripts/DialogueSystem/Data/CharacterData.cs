using System.Collections.Generic;
using UnityEngine;
namespace DialogueSystem.Data
{
    //This is a ScriptableObject for the Character Data
    [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/DialogueCharacter")]//Create a ScriptableObject name file and a name in the menu when you click create ScriptableObject
    public class CharacterData : ScriptableObject
    {
        //All the Character data you can fill in
        [SerializeField] private string characterName;
        [SerializeField] private AudioClip speakingSound;
        [SerializeField] private CharacterState defaultState; //Get the data from another class called CharacterState
        [SerializeField] private List<CharacterState> states;
        
        //Use lambda expression to get the data from the private var but can't change it from outside?
        public string CharacterName => characterName;
        public AudioClip SpeakingSound => speakingSound;
        public CharacterState DefaultState => defaultState;

        //
        public CharacterState GetState(Emotion fromEmotion)
        {
            //if parameter is the same as default emotion then return defaultState
            if (fromEmotion == Emotion.Default)
            {
                return defaultState;
            }
            
            //Find the state that macth the fromEmotion and return the data in the charater
            var state =
                states.Find(emotion => emotion.EmotionLabel == fromEmotion)
                ?? defaultState;
            
            return state;
        }
    }
}
