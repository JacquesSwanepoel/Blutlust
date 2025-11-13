using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BLUDLUST01_EasyMaterializer : MonoBehaviour
{
	[SerializeField] private BLUDLUST01_EasyNamesObject names;

	[Space(15)]

	public LeftArm leftArm;
	[Serializable]
	public class LeftArm
	{
		public Material Finger1s;
		public Material Finger2s;
		public Material Finger3s;
		public Material Knuckles;
		public Material Metacarpals;
		public Material Wrist;
		public Material Blades;
		public Material LowArm1;
		public Material LowArm2;
		public Material Elbow;
		public Material UpArms;
		public Material Joints;
	}

	public RightArm rightArm;
	[Serializable]
	public class RightArm
	{
		public Material Finger1s;
		public Material Finger2s;
		public Material Finger3s;
		public Material Hand;
		public Material Wrist;
		public Material LowArm;
		public Material Elbow;
		public Material UpArms;
	}

	[Space(15)]

	[SerializeField] private bool materializeRightArm = false;
	[SerializeField] private bool materializeLeftArm = false;

	[Space(15)]

	[SerializeField] private bool loadPresetChangerFromNames = false;

	[Space(15)]

	[SerializeField] private bool changeLeftArmPresetObject = false;
	[SerializeField] private bool changeRightArmPresetObject = false;
	[SerializeField] private bool safetySwitch = false;

	[Space(15)]

	[SerializeField] private List<Transform> AllParts = new List<Transform>();
	[SerializeField] private List<Transform> MaterializableParts = new List<Transform>();


	[Space(15)]

	public PresetChanger presetChanger;
	[Serializable]
	public class PresetChanger
	{
		public List<Transform> LFinger1s;
		public List<Transform> LFinger2s;
		public List<Transform> LFinger3s;
		public List<Transform> Knuckles;
		public List<Transform> Metacarpals;
		public List<Transform> LWrist;
		public List<Transform> Blades;
		public List<Transform> LowArm1;
		public List<Transform> LowArm2;
		public List<Transform> LElbow;
		public List<Transform> LUpArms;
		public List<Transform> Joints;
		[Space(20)]
		public List<Transform> RFinger1s;
		public List<Transform> RFinger2s;
		public List<Transform> RFinger3s;
		public List<Transform> Hand;
		public List<Transform> RWrist;
		public List<Transform> LowArm;
		public List<Transform> RElbow;
		public List<Transform> RUpArms;
	}

	private void OnValidate()
	{
		if (materializeRightArm)
		{
			GetAllChildren();

			MaterializeRightArm();

			materializeRightArm = false;
		}

		if (materializeLeftArm)
		{
			GetAllChildren();

			MaterializeLeftArm();

			materializeLeftArm = false;
		}

		if (changeLeftArmPresetObject && safetySwitch && names)
		{
			names.ChangeList(names.leftArm.Finger1s, presetChanger.LFinger1s);
			names.ChangeList(names.leftArm.Finger2s, presetChanger.LFinger2s);
			names.ChangeList(names.leftArm.Finger3s, presetChanger.LFinger3s);
			names.ChangeList(names.leftArm.Knuckles, presetChanger.Knuckles);
			names.ChangeList(names.leftArm.Metacarpals, presetChanger.Metacarpals);
			names.ChangeList(names.leftArm.Wrist, presetChanger.LWrist);
			names.ChangeList(names.leftArm.LowArm1, presetChanger.LowArm1);
			names.ChangeList(names.leftArm.LowArm2, presetChanger.LowArm2);
			names.ChangeList(names.leftArm.Blades, presetChanger.Blades);
			names.ChangeList(names.leftArm.Elbow, presetChanger.LElbow);
			names.ChangeList(names.leftArm.UpArms, presetChanger.LUpArms);
			names.ChangeList(names.leftArm.Joints, presetChanger.Joints);

			changeLeftArmPresetObject = false;

			print("Changed Left Preset");
		}

		if (changeRightArmPresetObject && safetySwitch && names)
		{
			names.ChangeList(names.rightArm.Finger1s, presetChanger.RFinger1s);
			names.ChangeList(names.rightArm.Finger2s, presetChanger.RFinger2s);
			names.ChangeList(names.rightArm.Finger3s, presetChanger.RFinger3s);
			names.ChangeList(names.rightArm.Hand, presetChanger.Hand);
			names.ChangeList(names.rightArm.Wrist, presetChanger.RWrist);
			names.ChangeList(names.rightArm.LowArm, presetChanger.LowArm);
			names.ChangeList(names.rightArm.Elbow, presetChanger.RElbow);
			names.ChangeList(names.rightArm.UpArms, presetChanger.RUpArms);

			changeRightArmPresetObject = false;

			print("Changed Right Preset");
		}

		if (loadPresetChangerFromNames && names)
		{
			GetAllChildren();

			LoadPresetChangerFromNames();

			loadPresetChangerFromNames = false;
		}
	}

	private void MaterializeLeftArm()
	{
		if (!names) { return; }

		foreach (Transform mPart in MaterializableParts)
		{
			ChangeMaterial(mPart, names.leftArm.Finger1s, leftArm.Finger1s);
			ChangeMaterial(mPart, names.leftArm.Finger2s, leftArm.Finger2s);
			ChangeMaterial(mPart, names.leftArm.Finger3s, leftArm.Finger3s);
			ChangeMaterial(mPart, names.leftArm.Knuckles, leftArm.Knuckles);
			ChangeMaterial(mPart, names.leftArm.Metacarpals, leftArm.Metacarpals);
			ChangeMaterial(mPart, names.leftArm.Wrist, leftArm.Wrist);
			ChangeMaterial(mPart, names.leftArm.Blades, leftArm.Blades);
			ChangeMaterial(mPart, names.leftArm.LowArm1, leftArm.LowArm1);
			ChangeMaterial(mPart, names.leftArm.LowArm2, leftArm.LowArm2);
			ChangeMaterial(mPart, names.leftArm.Elbow, leftArm.Elbow);
			ChangeMaterial(mPart, names.leftArm.UpArms, leftArm.UpArms);
			ChangeMaterial(mPart, names.leftArm.Joints, leftArm.Joints);
		}
	}

	private void MaterializeRightArm()
	{
		if (!names) { return; }

		foreach (Transform mPart in MaterializableParts)
		{
			ChangeMaterial(mPart, names.rightArm.Finger1s, rightArm.Finger1s);
			ChangeMaterial(mPart, names.rightArm.Finger2s, rightArm.Finger2s);
			ChangeMaterial(mPart, names.rightArm.Finger3s, rightArm.Finger3s);
			ChangeMaterial(mPart, names.rightArm.Wrist, rightArm.Wrist);
			ChangeMaterial(mPart, names.rightArm.Hand, rightArm.Hand);
			ChangeMaterial(mPart, names.rightArm.LowArm, rightArm.LowArm);
			ChangeMaterial(mPart, names.rightArm.Elbow, rightArm.Elbow);
			ChangeMaterial(mPart, names.rightArm.UpArms, rightArm.UpArms);
		}
	}

	private void ChangeMaterial(Transform mPart, List<string> nameOfPart, Material material)
	{
		foreach (String name in nameOfPart)
		{
			if (mPart.name == name && material)
			{
				mPart.GetComponent<Renderer>().material = material;
			}
		}
	}

	private void LoadPresetChangerFromNames()
	{
		presetChanger.LFinger1s.Clear();
		presetChanger.LFinger2s.Clear();
		presetChanger.LFinger3s.Clear();
		presetChanger.Knuckles.Clear();
		presetChanger.Metacarpals.Clear();
		presetChanger.LWrist.Clear();
		presetChanger.LowArm1.Clear();
		presetChanger.LowArm2.Clear();
		presetChanger.Blades.Clear();
		presetChanger.LElbow.Clear();
		presetChanger.LUpArms.Clear();
		presetChanger.Joints.Clear();

		presetChanger.RFinger1s.Clear();
		presetChanger.RFinger2s.Clear();
		presetChanger.RFinger3s.Clear();
		presetChanger.Hand.Clear();
		presetChanger.RWrist.Clear();
		presetChanger.LowArm.Clear();
		presetChanger.RElbow.Clear();
		presetChanger.RUpArms.Clear();

		foreach (Transform mPart in MaterializableParts)
		{
			AddPartToPreset(mPart, names.leftArm.Finger1s, presetChanger.LFinger1s);
			AddPartToPreset(mPart, names.leftArm.Finger2s, presetChanger.LFinger2s);
			AddPartToPreset(mPart, names.leftArm.Finger3s, presetChanger.LFinger3s);
			AddPartToPreset(mPart, names.leftArm.Knuckles, presetChanger.Knuckles);
			AddPartToPreset(mPart, names.leftArm.Metacarpals, presetChanger.Metacarpals);
			AddPartToPreset(mPart, names.leftArm.Wrist, presetChanger.LWrist);
			AddPartToPreset(mPart, names.leftArm.LowArm1, presetChanger.LowArm1);
			AddPartToPreset(mPart, names.leftArm.LowArm2, presetChanger.LowArm2);
			AddPartToPreset(mPart, names.leftArm.Blades, presetChanger.Blades);
			AddPartToPreset(mPart, names.leftArm.Elbow, presetChanger.LElbow);
			AddPartToPreset(mPart, names.leftArm.UpArms, presetChanger.LUpArms);
			AddPartToPreset(mPart, names.leftArm.Joints, presetChanger.Joints);

			AddPartToPreset(mPart, names.rightArm.Finger1s, presetChanger.RFinger1s);
			AddPartToPreset(mPart, names.rightArm.Finger2s, presetChanger.RFinger2s);
			AddPartToPreset(mPart, names.rightArm.Finger3s, presetChanger.RFinger3s);
			AddPartToPreset(mPart, names.rightArm.Hand, presetChanger.Hand);
			AddPartToPreset(mPart, names.rightArm.Wrist, presetChanger.RWrist);
			AddPartToPreset(mPart, names.rightArm.LowArm, presetChanger.LowArm);
			AddPartToPreset(mPart, names.rightArm.Elbow, presetChanger.RElbow);
			AddPartToPreset(mPart, names.rightArm.UpArms, presetChanger.RUpArms);
		}
	}

	private void AddPartToPreset(Transform mPart, List<string> nameOfParts, List<Transform> presetList)
	{
		foreach (string name in nameOfParts)
		{
			if (name == mPart.name)
			{
				presetList.Add(mPart);
			}
		}
	}

	public void GetAllChildren()
	{
		AllParts.Clear();

		int count = transform.childCount;
		for (int i = 0; i < count; i++)
		{
			AllParts.Add(transform.GetChild(i));
		}

		for (int i = 0; i < AllParts.Count; i++)
		{
			int count1 = AllParts[i].childCount;

			for (int x = 0; x < count1; x++)
			{
				AllParts.Add(AllParts[i].GetChild(x));
			}
		}

		MaterializableParts.Clear();

		foreach (Transform child in AllParts)
		{
			if (child.transform.TryGetComponent<Renderer>(out Renderer mRenderer))
			{
				MaterializableParts.Add(child);				
			}
		}
	}
}
