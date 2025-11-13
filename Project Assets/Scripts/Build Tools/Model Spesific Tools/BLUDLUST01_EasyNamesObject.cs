using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class BLUDLUST01_EasyNamesObject : ScriptableObject
{
    public LeftArm leftArm;
    [Serializable]
    public class LeftArm
	{
        public List<string> Finger1s;
        public List<string> Finger2s;
        public List<string> Finger3s;
        public List<string> Knuckles;
        public List<string> Metacarpals;
        public List<string> Wrist;
        public List<string> Blades;
        public List<string> LowArm1;
        public List<string> LowArm2;
        public List<string> Elbow;
        public List<string> UpArms;
        public List<string> Joints;
    }

    public RightArm rightArm;
    [Serializable]
    public class RightArm
    {
        public List<string> Finger1s;
        public List<string> Finger2s;
        public List<string> Finger3s;
        public List<string> Wrist;
        public List<string> Hand;
        public List<string> LowArm;
        public List<string> Elbow;
        public List<string> UpArms;
    }

    public void ChangeList(List<string> listThatWillBeChanged, List<Transform> referenceList)
	{
        listThatWillBeChanged.Clear();

		foreach (Transform Part in referenceList)
		{
            listThatWillBeChanged.Add(Part.name);
		}
	}
}
