using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNoPhisics : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public GameObject TestTarget;
	
	// Update is called once per frame
	void FixedUpdate () {
        var rigidbody = this.GetComponent<Rigidbody>();
        // 座標の書きかえ
        // 以外とそれっぽい動き
        // this.transform.position += this.transform.forward / 30;

        // 速度の変更
        // これだとなぜか1fだけ重力がのる
        /*
        if(Physics.CheckSphere(this.transform.position - Vector3.down * 0.25f,
                               0.26f)) {
            rigidbody.useGravity = false;
            rigidbody.velocity = this.transform.forward * 2;
        } else {
            rigidbody.useGravity = true;

        }
        */
        // MovePosition
        // rigidbodyのpositionを変更
        // 他のものに当たった時の挙動がきもい
        // this.GetComponent<Rigidbody>().MovePosition(this.transform.position + this.transform.forward / 60);


    }
       
    void Update() {
        var a = TestTarget.transform.eulerAngles;
        var b = transform.eulerAngles;
        var c = Quaternion.Angle(TestTarget.transform.rotation, transform.rotation);
        Debug.Log(string.Format("{0}, {1}, {2}", a, b, c));
        //transform.Rotate(new Vector3(0,1,0), 1.0f);
    }
}
