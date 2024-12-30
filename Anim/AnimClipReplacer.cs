using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class AnimClipReplacer
{
    public Animator Animator { get; private set; }
    public string StateName { get; private set; }
    public AnimationClip DefaultClip { get; private set; }
    public AnimationClip CurrentClip => overrideController[StateName];
    AnimatorOverrideController overrideController { get; set; }


    public AnimClipReplacer(Animator animator, string stateName)
    {
        Animator = animator;
        StateName = stateName;
        // AnimationOverrideControllerの生成と割り当て
        overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = overrideController;
        // 対象ステートの存在確認
        if (overrideController[stateName] == null)
            Debug.LogAssertion($"ステート {stateName} は存在しない");
        // デフォルトのクリップを保存
        DefaultClip = CurrentClip;
    }


    public bool Exe(AnimationClip newClip) => AllocateClip(StateName, newClip);
    public void Reset() => Exe(DefaultClip);


    bool AllocateClip(string stateName, AnimationClip newClip)
    {
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[Animator.layerCount];
        for (int i = 0; i < Animator.layerCount; i++)
        {
            layerInfo[i] = Animator.GetCurrentAnimatorStateInfo(i);
        }
        if (overrideController[stateName] == null)
        {
            Debug.LogAssertion($"ステート {stateName} は存在しない");
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