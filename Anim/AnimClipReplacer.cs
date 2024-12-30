using UnityEngine;

public class AnimClipReplacer
{
    Animator Animator { get; set; }
    string StateName { get; set; }
    AnimatorOverrideController overrideController { get; set; }
    public bool Exe(AnimationClip newClip) => AllocateClip(StateName, newClip);


    public AnimClipReplacer(Animator animator, string clipName)
    {
        Animator = animator;
        StateName = clipName;
        // AnimationOverrideControllerの生成と割り当て
        overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = overrideController;
    }


    bool AllocateClip(string stateName, AnimationClip newClip)
    {
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[Animator.layerCount];
        for (int i = 0; i < Animator.layerCount; i++)
        {
            layerInfo[i] = Animator.GetCurrentAnimatorStateInfo(i);
        }
        if (overrideController[stateName] == null)
        {
            Debug.LogWarning($"ステート {stateName} は存在しない");
            return false;
        }
        else
        if (overrideController[stateName] == newClip)
        {
            Debug.LogWarning($"ステート {stateName} には既に目当ての AnimationClip {newClip.name} が割り当てられている");
            return false;
        }
        // AnimationClipを差し替えて、強制的にアップデート
        // ステートがリセットされる
        overrideController[stateName] = newClip;
        Animator.Update(0.0f);
        // ステートを戻す
        for (int i = 0; i < Animator.layerCount; i++)
        {
            Animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
        }
        return true;
    }
}