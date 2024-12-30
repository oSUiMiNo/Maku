using UnityEngine;

public class Test_AnimClipReplacer : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip clip_Run;
    [SerializeField] AnimationClip clip_SitDown;
    AnimClipReplacer replacer;


    void Start()
    {
        replacer = new AnimClipReplacer(animator, "RUN00_F");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (animator.GetInteger("State") == 0)
                animator.SetInteger("State", 1);
            else
            if (animator.GetInteger("State") == 1)
                animator.SetInteger("State", 0);
        }
        else
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (replacer.Exe(clip_Run))
                Debug.Log($"差し替え：ステート[RUN00_F] <== クリップ[RUN00_F]");
            else
            if (replacer.Exe(clip_SitDown))
                Debug.Log($"差し替え：ステート[RUN00_F] <== クリップ[SitDown]");
        }
    }
}
