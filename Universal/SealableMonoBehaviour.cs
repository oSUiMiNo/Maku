using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
// SealableMonoBehaviour �Ɉړ��ς�
//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
/// <summary>
/// �y�g�����z
/// 1: �Ȃɂ��̊��N���X�ɂ������N���X�ɂ�����p�������Ă����B
/// 
/// 2: project��Assets�t�H���_�����ɁA�ucsc.rsp�v�Ƃ����t�@�C����ǉ��B�X�ɂ���̒��g�̃e�L�X�g���A
/// �u-warnaserror+:0114�v�Ƃ��������Ă����B
/// ����ɂ��A�h���N���X�� Start() �Ȃǂ��������Ă��܂����ۂɁA�G���[�ɂ��Ă����̂ŁA
/// ����Ċ��N���X�ł��h���N���X�ł��ʂ̏�����������Start()�����������Ă��܂��āA
/// ���N���X�� Start()�Ȃǂɏ������K�v�ȏ������㏑������Ă��܂����Ƃ�h����B
/// 
/// 3: �h����ł� Start()�����̊֐����g�������̂ł���΁A
/// ���N���X�� SubStart() �݂����ȉ��z���\�b�h�����A���N���X�� Start()�̒��ŌĂ�ł����A
/// �h���N���X�ŃI�[�o�[���C�h����B
/// </summary>
public class SealableMonoBehaviour : MonoBehaviour //MonoBehaviourMyExtention
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
}

