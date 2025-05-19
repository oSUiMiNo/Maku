using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using System;
using UniRx;

// ������A�^�b�`�����I�u�W�F�N�g���N���b�N�����
// �G�t�F�N�g�̃v���n�u�𐶐�
public class Test_MeshParticle : MonoBehaviour
{
    // ���t���[���N���b�N���m���Ď�
    void Start() => Observable.EveryUpdate()
    .Where(_ => Input.GetMouseButtonDown(0))
    .ThrottleFirst(TimeSpan.FromSeconds(1)) // �A�ŋ֎~
    .Subscribe(_ =>
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) return;
        if (hit.collider.gameObject == gameObject)
        {
            MeshParticle eff = ResourcesUtil.GetCompo<MeshParticle>("MeshParticleEffect");
            eff.Targ = hit.collider.gameObject;
        }
    })
    .AddTo(this);
}
