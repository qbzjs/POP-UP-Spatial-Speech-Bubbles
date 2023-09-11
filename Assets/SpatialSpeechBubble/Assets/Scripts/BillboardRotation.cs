using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardRotation : MonoBehaviour {
  public Transform TargetToLook = null;
  public bool invert = false;

  private void Awake()
  {
    if (TargetToLook == null) {
      Reset();
    } 
  }

  private void
  Reset() {
    TargetToLook = Camera.main.transform;
  }

  private void
  LateUpdate () {
    transform.rotation = invert ?
                           Quaternion.LookRotation(-TargetToLook.forward, TargetToLook.up) :
                           TargetToLook.rotation;
  }
}