using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������A�^�b�`�����I�u�W�F�N�g���N���b�N����ƁA
/// �G�t�F�N�g�̃v���n�u���W�F�l���[�g���Ă����B
/// </summary>
public class OnClick_GenarateMeshParticle : MonoBehaviourMyExtention
{
    /// <summary>
    /// MeshParticle ���A�^�b�`�����v���n�u�B
    /// ��������̃X�N���v�g���� Instantiate() ���Ă���B
    /// Instantiate() ����ƁA�X�ɂ���(MeshParticle��)���Ŕ��菈���ɓ���A
    /// �ʂ�����A���ۂ̃G�t�F�N�g�� Instantiate() �����B
    /// </summary>
    MeshParticle meshParticlePrefab;

    [SerializeField] float interval = 1;


    float lastClickTime;

    private void Start()
    {
        LoadResources();
    }

    void LoadResources()
    {
        //�����̎��Ƀ��[�h�܂ł��̂͒x�����Ȃ̂Ő�ɃL���b�V�����Ƃ��āA
        //�������͂���� Instantiate ���邾��
        GameObject effectPrefab = (GameObject)Resources.Load("MeshParticleEffect");
        meshParticlePrefab = effectPrefab.GetComponent<MeshParticle>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastClickTime > interval)
        {
            lastClickTime = Time.time;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                MeshParticle mp = Instantiate(meshParticlePrefab);
                mp.Model = hit.collider.gameObject;
            }
        }
    }
}
