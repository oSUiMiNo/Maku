using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjBounds : MonoBehaviour
{
    /// <summary>
    /// �q�I�u�W�F�N�g�̓���Bounds
    /// </summary>
    public Bounds childObjBounds;

    void Start()
    {
        // 
        childObjBounds = CalcLocalObjBounds(this.gameObject);

        // Bounds�̑傫���ƌ`�󂪌����ڂɕ�����悤�R���C�_�[��ǉ�����
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        // �v�Z���ꂽ�o�E���h�{�b�N�X�ɍ��킹�ăR���C�_�[�̑傫���ƈʒu��ύX����
        collider.center = childObjBounds.center;
        collider.size = childObjBounds.size;
    }

    /// <summary>
    /// ���݃I�u�W�F�N�g�̃��[�J�����W�ł̃o�E���h�v�Z
    /// </summary>
    private Bounds CalcLocalObjBounds(GameObject obj)
    {
        // �w��I�u�W�F�N�g�̃��[���h�o�E���h���v�Z����
        Bounds totalBounds = CalcChildObjWorldBounds(obj, new Bounds());

        // ���[�J���I�u�W�F�N�g�̑��΍��W�ɍ��킹�ăo�E���h���Čv�Z����
        // �I�u�W�F�N�g�̃��[���h���W�ƃT�C�Y���擾����
        Vector3 ObjWorldPosition = this.transform.position;
        Vector3 ObjWorldScale = this.transform.lossyScale;

        // �o�E���h�̃��[�J�����W�ƃT�C�Y���擾����
        Vector3 totalBoundsLocalCenter = new Vector3(
            (totalBounds.center.x - ObjWorldPosition.x) / ObjWorldScale.x,
            (totalBounds.center.y - ObjWorldPosition.y) / ObjWorldScale.y,
            (totalBounds.center.z - ObjWorldPosition.z) / ObjWorldScale.z);
        Vector3 meshBoundsLocalSize = new Vector3(
            totalBounds.size.x / ObjWorldScale.x,
            totalBounds.size.y / ObjWorldScale.y,
            totalBounds.size.z / ObjWorldScale.z);

        Bounds localBounds = new Bounds(totalBoundsLocalCenter, meshBoundsLocalSize);

        return localBounds;
    }

    /// <summary>
    /// �q�I�u�W�F�N�g�̃��[���h���W�ł̃o�E���h�v�Z�i�ċA�����j
    /// </summary>
    private Bounds CalcChildObjWorldBounds(GameObject obj, Bounds bounds)
    {
        // �w��I�u�W�F�N�g�̑S�Ă̎q�I�u�W�F�N�g���`�F�b�N����
        foreach (Transform child in obj.transform)
        {
            // ���b�V���t�B���^�[�̑��݊m�F
            MeshFilter filter = child.gameObject.GetComponent<MeshFilter>();

            if (filter != null)
            {
                // �I�u�W�F�N�g�̃��[���h���W�ƃT�C�Y���擾����
                Vector3 ObjWorldPosition = child.position;
                Vector3 ObjWorldScale = child.lossyScale;

                // �t�B���^�[�̃��b�V����񂩂�o�E���h�{�b�N�X���擾����
                Bounds meshBounds = filter.mesh.bounds;

                // �o�E���h�̃��[���h���W�ƃT�C�Y���擾����
                Vector3 meshBoundsWorldCenter = meshBounds.center + ObjWorldPosition;
                Vector3 meshBoundsWorldSize = Vector3.Scale(meshBounds.size, ObjWorldScale);

                // �o�E���h�̍ŏ����W�ƍő���W���擾����
                Vector3 meshBoundsWorldMin = meshBoundsWorldCenter - (meshBoundsWorldSize / 2);
                Vector3 meshBoundsWorldMax = meshBoundsWorldCenter + (meshBoundsWorldSize / 2);

                // �擾�����ŏ����W�ƍő���W���܂ނ悤�Ɋg��/�k�����s��
                if (bounds.size == Vector3.zero)
                {
                    // ���o�E���h�̃T�C�Y���[���̏ꍇ�̓o�E���h����蒼��
                    bounds = new Bounds(meshBoundsWorldCenter, Vector3.zero);
                }
                bounds.Encapsulate(meshBoundsWorldMin);
                bounds.Encapsulate(meshBoundsWorldMax);
            }

            // �ċA����
            bounds = CalcChildObjWorldBounds(child.gameObject, bounds);
        }
        return bounds;
    }
}