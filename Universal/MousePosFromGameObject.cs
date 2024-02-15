using UnityEngine;

public class MousePosFromGameObject
{
    private Vector3 mOffset;
    private float mZCoord;
    bool useOffset = false;


    //�ŏ��̃����N���b�N�ڂň�x�R���X�g���N�^���Ă�
    public MousePosFromGameObject(GameObject target, bool useOffset)
    {
        this.useOffset = useOffset;

        ///<summary>
        ///�^�[�Q�b�g�̃��[���h���W���X�N���[�����W�ɕϊ�
        /// z�ɂ́A�J��������̋���������炵���B
        /// </summary>
        mZCoord = Camera.main.WorldToScreenPoint(target.transform.position).z;

        ///<summary>
        ///�^�[�Q�b�g�̃|�W�V��������}�E�X�̃|�W�V�����������Ƃ������Ƃ́A
        ///�}�E�X�|�W�V��������i0�j�Ƃ����Ƃ��̃^�[�Q�b�g�̃x�N�g���i�����Ƌ����j�����܂�B
        ///���Ȃ݂�GetMouseWorldPos()�ɂă}�E�X�ƃ^�[�Q�b�g��Z�l�͍��킹�Ă���̂�x,y�̋������������܂�B
        /// </summary>
        mOffset = target.transform.position - GetMouseWorldPos();
        this.useOffset = useOffset;
    }

    private Vector3 GetMouseWorldPos()
    {
        ///<summary>
        ///�}�E�X�̃X�N���[�����W���擾��
        ///z�ɂ̓J��������猩���^�[�Q�b�g��z�ɃJ������Ɍ����^�[�Q�b�g��z�l������B
        /// </summary>
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        /// <summary>
        /// z�l���J�������猩���^�[�Q�b�g��z�l�ɕύX�����}�E�X�̃X�N���[�����W��
        /// ���[���h���W�ɕϊ�
        /// /// </summary>
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    //�}�E�X�ɒǏ]������^�C�~���O�ŌĂ�
    public Vector3 GameObjectPosFromMouse()
    {
        if (useOffset) return GetMouseWorldPos() + mOffset;
        else return GetMouseWorldPos();
    }
}
