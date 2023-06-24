using UnityEngine.VFX;

//VFX���j�b�g�ɂ���R���|�[�l���g�̐݌v�������B

public abstract class VFXUnitComponent : MonoBehaviourMyExtention
{
    public VisualEffect vfx;

    void Start()
    {
        // ���������� VFXGraph �̎��̂������ɃZ�b�g����B
        // �K���q�I�u�W�F�N�g��1�ڂɎ��̂�u���Ă����B
        vfx = transform.GetChild(0).GetComponent<VisualEffect>();
        Init();
    }

    protected virtual void Init() { }
    public abstract void SetProperties();
}
