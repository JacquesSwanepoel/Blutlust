using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game {

	public class Materializer : MonoBehaviour
	{
		public Material material = null;

		[SerializeField] private Transform[] specificChildren = null;

		public bool getSpecificChildren = false;

		public bool getAllChildren = false;

		public bool materialize = false;

		private bool _hasChildren = false;

		private List<Transform> childs = new List<Transform>();

		public void OnValidate()
		{
			Buttons();
		}

		public void Buttons()
		{
			if (getAllChildren)
			{
				GetAllChildren();

				getAllChildren = false;
			}

			////////////

			if (getSpecificChildren)
			{
				GetSelectedChildren(specificChildren);

				getSpecificChildren = false;
			}

			////////////

			if (materialize)
			{
				Materialize(material, _hasChildren);

				materialize = false;
			}
		}

		public void GetAllChildren()
		{
			childs.Clear();

			FindEveryChild(transform);
			for (int i = 0; i < childs.Count; i++)
			{
				FindEveryChild(childs[i]);
			}

			_hasChildren = true;

			print("Got All Children of " + transform.name);
		}

		public void GetSelectedChildren(Transform[] specificChildren)
		{
			if (specificChildren.Length < 1) { return; }

			childs.Clear();

			for (int i = 0; i < specificChildren.Length; i++)
			{
				if (specificChildren[i] != null)
				{
					childs.Add(specificChildren[i].transform);

					_hasChildren = true;
				}
			}

			print("Got Spesific Children of " + transform.name);
		}

		public void Materialize(Material material, bool hasChildren)
		{
			if (!hasChildren) { return; }

			for (int i = 0; i < childs.Count; i++)
			{
				childs[i].TryGetComponent<MeshRenderer>(out MeshRenderer mesh);
				if (mesh != null)
				{
					mesh.material = material;
				}
			}

			print("Materialized " + transform.name);
		}


		private void FindEveryChild(Transform parent)
		{
			int count = parent.childCount;
			for (int i = 0; i < count; i++)
			{
				childs.Add(parent.GetChild(i));
			}
		}
	}
}