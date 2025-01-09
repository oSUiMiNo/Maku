//using UnityEngine;
//using UnityEngine.AddressableAssets;

//public class Test_AnimClipReplacer : MonoBehaviour
//{
//    [SerializeField] Animator animator;
//    [SerializeField] AnimationClip clip_Run;
//    [SerializeField] AnimationClip clip_SitDown;
//    AnimClipReplacer replacer;


//    void Start()
//    {
//        replacer = new AnimClipReplacer(animator, "RUN00_F");
//    }


//    async void Update()
//    {
//        // ステート遷移
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            if (animator.GetInteger("State") == 0)
//                animator.SetInteger("State", 1);
//            else
//            if (animator.GetInteger("State") == 1)
//                animator.SetInteger("State", 0);
//        }
//        else
//        // クリップ差し替え
//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            if (replacer.Exe(clip_Run))
//                Debug.Log($"差し替え：ステート[RUN00_F] <== クリップ[RUN00_F]");
//            else
//            if (replacer.Exe(clip_SitDown))
//                Debug.Log($"差し替え：ステート[RUN00_F] <== クリップ[SitDown]");
//        }
//        else
//        // AddressableAsset化したクリップ差し替え
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            if (replacer.Exe(await Addressables.LoadAssetAsync<AnimationClip>("SitDown").Task))
//            {
//                Debug.Log($"差し替え：ステート[RUN00_F] <== Addressable[SitDown]");
//            }
//            else
//            {
//                replacer.Reset();
//                Debug.Log($"差し替え：ステート[RUN00_F] <== リセット");
//            }
//        }
//    }
//}
