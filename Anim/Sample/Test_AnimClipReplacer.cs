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
//        // �X�e�[�g�J��
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            if (animator.GetInteger("State") == 0)
//                animator.SetInteger("State", 1);
//            else
//            if (animator.GetInteger("State") == 1)
//                animator.SetInteger("State", 0);
//        }
//        else
//        // �N���b�v�����ւ�
//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            if (replacer.Exe(clip_Run))
//                Debug.Log($"�����ւ��F�X�e�[�g[RUN00_F] <== �N���b�v[RUN00_F]");
//            else
//            if (replacer.Exe(clip_SitDown))
//                Debug.Log($"�����ւ��F�X�e�[�g[RUN00_F] <== �N���b�v[SitDown]");
//        }
//        else
//        // AddressableAsset�������N���b�v�����ւ�
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            if (replacer.Exe(await Addressables.LoadAssetAsync<AnimationClip>("SitDown").Task))
//            {
//                Debug.Log($"�����ւ��F�X�e�[�g[RUN00_F] <== Addressable[SitDown]");
//            }
//            else
//            {
//                replacer.Reset();
//                Debug.Log($"�����ւ��F�X�e�[�g[RUN00_F] <== ���Z�b�g");
//            }
//        }
//    }
//}
