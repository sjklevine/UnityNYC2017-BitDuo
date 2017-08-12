﻿using UnityEngine;
using VRTK;
using System.Collections;

public class PlatformGrabHelper : MonoBehaviour {

	[SerializeField] GameObject helperArrows;
	void Start (){
		if (GetComponent<VRTK_InteractableObject>() == null){
			Debug.LogError("Required to be attached to an Object that has the VRTK_InteractableObject script attached to it");
			return;
		}

		GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(ObjectGrabbed);
		GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(ObjectUngrabbed);
	}

	private void ObjectGrabbed(object sender, InteractableObjectEventArgs e){
		helperArrows.SetActive(true);
	}

	private void ObjectUngrabbed(object sender, InteractableObjectEventArgs e){
		helperArrows.SetActive(false);
	}

}