using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderGetter : MonoBehaviour {

    [SerializeField]
    private GameObject obj;

    public GameObject GetObject() { return obj; }
}
