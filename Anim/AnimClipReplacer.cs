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
        // AnimationOverrideController�̐����Ɗ��蓖��
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
            Debug.LogWarning($"�X�e�[�g {stateName} �͑��݂��Ȃ�");
            return false;
        }
        else
        if (overrideController[stateName] == newClip)
        {
            Debug.LogWarning($"�X�e�[�g {stateName} �ɂ͊��ɖړ��Ă� AnimationClip {newClip.name} �����蓖�Ă��Ă���");
            return false;
        }
        // AnimationClip�������ւ��āA�����I�ɃA�b�v�f�[�g
        // �X�e�[�g�����Z�b�g�����
        overrideController[stateName] = newClip;
        Animator.Update(0.0f);
        // �X�e�[�g��߂�
        for (int i = 0; i < Animator.layerCount; i++)
        {
            Animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
        }
        return true;
    }
}