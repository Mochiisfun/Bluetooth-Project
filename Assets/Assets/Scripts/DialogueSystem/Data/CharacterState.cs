using System;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Data
{
     //CharacterState data
     [Serializable]
     public class CharacterState
     {
          [SerializeField] private Emotion emotionLabel; 
          [SerializeField, Range(0, 2)] private float speakingSoundPitch = 1;
          [SerializeField] private Optional<AudioClip> reactionSound;
          [SerializeField] private Optional<Sprite> characterFace;
          
          //Use lambda expression to get the data from the private var but can't change it from outside?
          public Emotion EmotionLabel => emotionLabel;
          public float SpeakingSoundPitch => speakingSoundPitch;
          public Optional<AudioClip> ReactionSound => reactionSound;
          public Optional<Sprite> CharacterFace => characterFace;
     }
}