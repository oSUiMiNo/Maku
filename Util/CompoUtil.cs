using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;


namespace MyUtil
{
    public static class CompoUtil
    {
        //========================================
        // �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾
        // �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾
        //========================================
        public static Compo CheckAddCompo<Compo>(this GameObject GO) where Compo : Component
        {
            //Debug.Log($"�R���| {typeof(Compo).Name}");
            #region �Ăяo�����ʒm
            var caller = new System.Diagnostics.StackFrame(1, false);
            string callerMethodName = caller.GetMethod().Name;
            string callerClassName = caller.GetMethod().DeclaringType.Name;
            //Debug.Log("�N���X  " + callerClassName + " �́A     ���\�b�h  " + callerMethodName + "()  ����Ăяo����܂����B");
            #endregion
            //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
            Compo targetCompo = GO.GetComponent<Compo>();
            if (targetCompo == null)
            {
                targetCompo = GO.AddComponent<Compo>();
            }
            return targetCompo;
        }



        //========================================
        // �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾
        // �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B��L�� CheckComponent<T> �Ƃ͎኱�p�r���Ⴄ
        // CheckComponent<T> �̏ꍇ�A�^������ <����.GetType()> �݂����ɁA�N���X���擾���Ȃ���ԐړI�ɓn�����Ƃ���ƁA
        // <> �̕��������Z�q�Ɣ��f����Ă��܂��Ďg���Ȃ�
        // �����������ꍇ�ɂ�������g��
        // �������߂�l�� Component�^ �Ȃ̂ŁA�擾�����R���|�[�l���g���g�������ꍇ�͍X�ɕϊ����K�v
        // �� �����R���|���w��\
        //========================================
        public static List<Component> CheckAddCompos(this GameObject GO, params Type[] compos)
        {
            List<Component> returnCompos = new List<Component>();
            foreach (Type compo in compos)
            {
                //CheckAddCompo(GO, compo);
                //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
                Component targetCompo = GO.GetComponent(compo);
                if (targetCompo == null)
                {
                    targetCompo = GO.AddComponent(compo);
                }
                returnCompos.Add(targetCompo);
            }
            return returnCompos;
        }

        //public static Component CheckAddCompo(this GameObject GO, Type Compo)
        //{
        //    //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
        //    Component targetCompo = GO.GetComponent(Compo);
        //    if (targetCompo == null)
        //    {
        //        targetCompo = GO.AddComponent(Compo);
        //    }
        //    return targetCompo;
        //}

        //public void CheckAddMultiCompo(List<Type> compos, GameObject gObj)
        //{
        //    foreach (Type a in compos)
        //    {
        //        CheckAddCompo(a, gObj);
        //    }
        //}
    }
}
