using UnityEngine;


namespace MyUtil
{
    public static class ScreenUtil
    {
        // GO�̐[�x��ς����Ƀ}�E�X�ɒǏ]�����邽�߂̃��[���hPos��Ԃ�
        public static Vector3 PosUnderMouse(this GameObject targ, bool UseOffset = false)
        {


            Vector3 targPos = targ.transform.position;
            Vector3 underMousePos = GetMousWorldPosOnTargetDepth(targ);

            if (UseOffset)
            {
                // �I�t�Z�b�g�ɂ���
                // �^�[�Q�b�g�̃|�W�V��������}�E�X�̃|�W�V�����������Ƃ������Ƃ�
                // �}�E�X�|�W�V��������i0�j�Ƃ����Ƃ��̃^�[�Q�b�g�̃x�N�g���i�����Ƌ����j�����܂�
                // underMousePos �ƃ^�[�Q�b�g�̐[�x�͍��킹�Ă���̂�(x,y)�������������܂�B
                Vector3 offset = targPos - underMousePos;
                return underMousePos + offset;
            }
            else
            {
                return underMousePos;
            }
        }


        // �X�N���[�����ʂł͂Ȃ��^�[�Q�b�g�Ɠ����[�x���ʏ�ɂ�����}�E�X�̍��W���v�Z
        static Vector3 GetMousWorldPosOnTargetDepth(GameObject targ)
        {
            // �J��������猩���^�[�Q�b�g��z�l���[�x
            float targDepth = Camera.main.WorldToScreenPoint(targ.transform.position).z;
            // �}�E�X�̃X�N���[�����W���擾��z�ɂ͐[�x������
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = targDepth;
            // ���[���h���W�ɕϊ����ĕԂ�
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }
    }
}