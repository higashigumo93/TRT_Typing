using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{

    // “®‚­‘¬‚³‚ðŒˆ‚ß‚é
    [SerializeField] float speed;

    // ˆÚ“®æ‚ÆˆÚ“®ƒ|ƒCƒ“ƒg‚ðŒˆ‚ß‚é
    [SerializeField] float endPos; // ‚±‚±‚Ü‚Å
    [SerializeField] float movePos; // ‚±‚±‚©‚ç

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed, 0, 0);

        // endPos‚Ü‚Å—ˆ‚½‚çmovePos‚ÉˆÚ“®‚³‚¹‚é
        if (transform.position.x > endPos)
        {
            transform.position = new Vector3(movePos, transform.position.y, 0);
        }
    }
}
