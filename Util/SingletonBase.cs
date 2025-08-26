using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using MyUtil;


#region �V���O���g�� ( �ʏ�N���X )
/// <summary>
/// �y�g�����z
/// �R���|�[�l���g�����Ȃ��X�N���v�g�Ɍp��������ƃV���O���g���ɂȂ�B
/// �p��������ہA�W�F�l���b�N�̌^�����ɔh���N���X�^������B
/// �y�ڂ����g�����z
/// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa#f7d0608026a048eebb61c5a949c4f379
/// </summary>
/// <typeparam name="SingletonType"></typeparam>
public abstract class Singleton<SingletonType> :
    System.IDisposable
    where SingletonType : Singleton<SingletonType>, new()
{
    protected Singleton() { } // �R���X�g���N�^�i�O������̌Ăяo���֎~�j 

    private static SingletonType ins;
    public static SingletonType Ins
    {
        get { return GetOrCreateInstance<SingletonType>(); }
    }
    public static bool IsCreated
    {
        //ins�͑��̃N���X����͎Q�Ƃł��Ȃ����߁A���̃N���X����uins != null�v�h����ł��Ȃ��B����Ă��̃v���p�e�B���g���B
        get { return ins != null; }
    }

    protected static InheritSingletonType GetOrCreateInstance<InheritSingletonType>()
        where InheritSingletonType : class, SingletonType, new()
    {
        if (IsCreated)
        {
            // ���N���X����Ă΂ꂽ��Ɍp���悩��Ă΂��ƃG���[�ɂȂ�B��Ɍp���悩��Ă�
            if (!typeof(InheritSingletonType).IsAssignableFrom(ins.GetType()))
            {
                Debug.LogErrorFormat("{1}��{0}���p�����Ă��܂���", typeof(InheritSingletonType), ins.GetType());
            }
        }
        else
        {
            ins = new InheritSingletonType();
        }
        return ins as InheritSingletonType;
    }

    public virtual void Dispose()
    {
        ins = default;
    }
}
#endregion









#region �V���O���g�� ( MonoBehaviour )
/// <summary>
/// �V�[���̃��[�h����ɃV���O���g���Q�[���I�u�W�F�N�g��������������
/// </summary>
public class SingletonCompoSetter
{
    /// <summary>
    /// �V�[���̃��[�h�O���ƁA�V�[����̃Q�[���I�u�W�F�N�g��T�����Ƃ��o���Ȃ��̂�
    /// RuntimeInitializeOnLoadMethod() �̈����� RuntimeInitializeLoadType.AfterSceneLoad
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        CreateSingletonObjects();
    }

    static void CreateSingletonObjects()
    {
        /////<summary>
        ///// �utypeof(�C���^�[�t�F�C�X)�v�ł̓_���炵���B
        ///// �܂��A�utypeof(�W�F�l���b�N�N���X)�v���ƌ^�������肪��肢���Əo���Ȃ��B
        ///// �Ȃ̂ŁASingletonCompo<SingletonType> ���A��̒��ۃN���X NonGenericSingletonCompoBase �ŕ���
        ///// ������g���Ď擾�����B
        ///// </summary>
        //IEnumerable<Type> singletonClassesType;
        //singletonClassesType = System.Reflection.Assembly
        //    .GetAssembly(typeof(NonGenericSingletonCompoBase))
        //    .GetTypes()
        //    .Where(t =>
        //    {
        //        return
        //            t.IsSubclassOf(typeof(NonGenericSingletonCompoBase)) &&
        //            !t.IsAbstract &&    //�R���|�[�l���g���o���Ȃ� abstract�N���X�͏��O�B  
        //            !t.ContainsGenericParameters;    // �W�F�l���b�N�N���X�͉��́A�C���X�^���X�����̏�����ʂ��Ȃ��̂ŏ��O�B
        //        ///<summary>
        //        /// �y���Ӂz
        //        /// 1 : ���N���X�ł��A���g���R���|�[�l���g�ɂȂ肤��ꍇ�� abstract�N���X�ɂ��Ȃ��̂͂܂����R�����A
        //        /// �R���|�[�l���g�������A�����̊��N���X�Ƃ��Ă̂ݎg���ꍇ�́A�����ƒ��ۃN���X�ɂ���B
        //        /// �������Ȃ��� �� �����ł͂����Ȃ��B
        //        /// ����ƁA�Ӗ����l���Ă��A���N���X�Ƃ��Ă̂ݎg���Ƃ������Ƃ͒��ۉ����̃N���X�Ȃ킯������A
        //        /// abstract �ɂ��������؂��ʂ�B
        //        /// 
        //        /// 2 : �V���O���g���̊��N���X����낤�Ƃ���ƃW�F�l���b�N�N���X�ɂȂ邪�A
        //        /// �u object obj = Activator.CreateInstance(�W�F�l���b�N�N���X) �v�͂ł��Ȃ��̂ŁA
        //        /// �͂����B
        //        /// �������A���������V���O���g���̊��N���X�͕��� abstract �ɂ��Ă���A�͂������񂾂��ǁB
        //        /// </summary>
        //    });


        // ��L�����ł͑��̃A�Z���u�����̃X�N���v�g�ɓK�p����Ȃ��̂ŐV������
        IEnumerable<Type> singletonClassesType = AppDomain.CurrentDomain.GetAssemblies() // ���݂̃A�v���P�[�V�����h���C�����̑S�A�Z���u�����擾
        .SelectMany(assembly =>
        {
            try
            {
                return assembly.GetTypes(); // �A�Z���u�����̑S�Ă̌^���擾
            }
            catch (ReflectionTypeLoadException ex)
            {
                // �ꕔ�̌^�����[�h�ł��Ȃ������ꍇ�ł��������p��
                return ex.Types.Where(t => t != null);
            }
        })
        .Where(t =>
        {
            return t != null &&
                   t.IsSubclassOf(typeof(NonGenericSingletonCompoBase)) && // NonGenericSingletonCompoBase�̃T�u�N���X���ǂ���
                   !t.IsAbstract && // ���ۃN���X�ł͂Ȃ�
                   !t.ContainsGenericParameters; // �W�F�l���b�N�^�ł͂Ȃ�
        });


        foreach (var a in singletonClassesType)
        {
            //DebugView.Log($"�V���O���g�� {a.Name} ���쐬");
            ///<summary>
            /// �擾����Type�Aa �̃N���X�̃C���X�^���X�� object�^�Ő������A
            /// ���N���X�� NonGenericSingletonCompoBase�^�ɕϊ����A
            /// �S�h���N���X���ʂ̒��ۃ��\�b�h���ĂԁB
            /// </summary>
            object obj = Activator.CreateInstance(a);
            NonGenericSingletonCompoBase n = (NonGenericSingletonCompoBase)obj;

            ///<summary>
            /// �������N���X�Ƃ��ł����g�������ꍇ�ł��A
            /// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa
            /// ������́A�u�����p����������C���X�^���X�Ƃ��ė��p����g�����v���Ɗ��N���X�� abstract �ɂł��Ȃ��̂ŁA
            /// �͂����ꂸ�ɃV���O���g���Q�[���I�u�V�F�N�g����������Ă��܂��B
            /// �����ŁA�������N���X�Ƃ��Ă̂ݗ��p�������ꍇ�́uPureBaseClass�t���O�v�� true �ɂ��邱�ƂŁA
            /// CreateSingletonGameObject(); ���Ă΂�Ȃ��悤�ɂ��Ă���B
            ///  SingletonCompo<> ���� false �ɃI�[�o�[���C�h���Ă��邽�߁A�������N���X�ɂ������Ȃ��ꍇ�͎������Ȃ��Ă悭�āA
            /// �������N���X�ɂ������ꍇ�̂� true �ɃI�[�o�[���C�h����΂悢�B
            /// �Ȃ��A�������N���X�ł��� SingletonCompo<> ���� false �ɂȂ��Ă��܂��Ă��邪�A
            /// �W�F�l���b�N��abstract�Ȃ̂ł��������͂�����Ă��邩��OK�B
            /// </summary>
            if (n.PureBaseClass) continue;
            ///<summary>
            /// �V���O���g���R���|���p�����Ă��܂��Ɣۂ����ł��Q�[���I�u�W�F�N�g����������Ē��g�������Ă��܂����B
            /// �ł��g�������Ȃ����Ɋۂ��ƃR�����g�A�E�g����͖̂ʓ|�������B���N���X�ւ̕���p�Ƃ������邩�������B
            /// �Ȃ̂ŃA�N�e�B�u���ǂ����̃t���O�� false ��������͂����B
            /// SingletonCompo<> ���� false �ɃI�[�o�[���C�h���Ă��邽�߁A�A�N�e�B�u�ŗǂ��ꍇ�͎������Ȃ��Ă悭�āA
            /// ��A�N�e�B�u�ɂ������ꍇ�̂� true �ɃI�[�o�[���C�h����΂悢�B
            /// </summary>
            if (!n.IsActive) continue;
            n.CreateSingletonGameObject();
        }
    }
}

public abstract class NonGenericSingletonCompoBase : SealableMonoBehaviour
{
    ///<summary>
    /// �������N���X�Ƃ��ł����g�������ꍇ�ł��A
    /// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa
    /// ������́A�u�����p����������C���X�^���X�Ƃ��ė��p����g�����v���Ɗ��N���X�� abstract �ɂł��Ȃ��̂ŁA
    /// �͂����ꂸ�ɃV���O���g���Q�[���I�u�V�F�N�g����������Ă��܂��B
    /// �����ŁA�������N���X�Ƃ��Ă̂ݗ��p�������ꍇ�́uPureBaseClass�t���O�v�� true �ɂ��邱�ƂŁA
    /// SingletonCompoSetter �� CreateSingletonGameObject(); ���Ă΂�Ȃ��悤�ɂ��Ă���B
    /// SingletonCompo<> ���� false �ɃI�[�o�[���C�h���Ă��邽�߁A�������N���X�ɂ������Ȃ��ꍇ�͎������Ȃ��Ă悭�āA
    /// �������N���X�ɂ������ꍇ�̂� true �ɃI�[�o�[���C�h����΂悢�B
    /// �Ȃ��A�������N���X�ł��� SingletonCompo<> ���� false �ɂȂ��Ă��܂��Ă��邪�A
    /// �W�F�l���b�N��abstract�Ȃ̂ł��������͂�����Ă��邩��OK�B
    /// </summary>
    public abstract bool PureBaseClass { get; protected set; }
    public abstract bool IsActive { get; protected set; }
    public abstract void CreateSingletonGameObject();
}

/// <summary>
/// �y�g�����z
/// �R���|�[�l���g������X�N���v�g�Ɍp��������ƃV���O���g���ɂȂ�B
/// �p��������ہA�W�F�l���b�N�̌^�����ɔh���N���X�^������B
/// �����ŃV���O���g���p�̃Q�[���I�u�W�F�N�g�����Ȃ��Ă������Ő�������ăR���|�[�l���g���A�^�b�`���Ă���邪�A
/// �����ŃV���O���g���p�̃Q�[���I�u�W�F�N�g���쐬�����ꍇ�͂����炪�̗p�����̂ŁA�K�v�ɉ����Ď蓮�ō���Ă��ǂ��B
/// �����ŃQ�[���I�u�W�F�N�g�����ꍇ�́A
/// �A�^�b�`����V���O���g���̃R���|�[�l���g�Ɠ������O�ɂ��Ȃ��ƍ폜�����̂ŋC��t���ĂˁB
/// �������O�̃Q�[���I�u�W�F�N�g���ł���΁A�R���|�[�l���g���A�^�b�`���Ă��Ȃ��Ă������ŃA�^�b�`���Ă����B
/// �ܘ_�K�v�ɉ����Ď蓮�ŃA�^�b�`���Ă��ǂ��B
/// </summary>

///<summary>
/// �y��Ԃ̃����b�g�z
/// �蓮�ŃV���O���g���Q�[���I�u�V�F�N�g��p�ӂ��Ȃ��Ǝg���Ȃ���Ԃł́A
/// �C�ӂ̃V�[������v���C���[�h�ɓ���ۂɁA
/// ���̃V�[���ɂ���p�̃V���O���g���Q�[���I�u�V�F�N�g�����O�ɏ������Ă����Ȃ��ƃV���O���g����������ԂɂȂ��Ă��܂����A
/// ������p�����Ă����΃V���O���g���������V�[���ł��������������̂ŁA
/// ���O�ɑS�V�[���ɓ����V���O���g���Q�[���I�u�V�F�N�g��p�ӂ��Ă����K�v�������Ȃ�B
/// ���̃����b�g���ő�����󂷂邽�߂ɂ́A�蓮�ŃV���O���g���Q�[���I�u�V�F�N�g��n���������s���̗ǂ��݌v���ɗ͍T���������ǂ��B
/// �Ⴆ�΃V���O���g���Q�[���I�u�W�F�N�g�ɂ��炩���߉����q�I�u�W�F�N�g��z�u����悤�Ȑ݌v�͍T���A����ɁA
/// �unew GameObject(���O)�v�Łu���I�Ɏq�𐶐�������A�u(GameObject)Resources.Load(���O)�v�œ��I�Ƀ��[�h����ȂǁB
/// </summary>

///<summary>
/// �y�O������Compo���擾����ۂ̒��Ӂz
/// Awake() ���Ă΂����O�� Compo ���Ă�ł͂����Ȃ��B
/// �Ⴆ�Εʂ̃N���X�Ńt�B�[���h�̐錾�Ɠ����ɂ��̃N���X�� Compo �������ɑ������̂͂�΂��B
/// �y���R�z
/// ins �Ɏ��g�̃C���X�^���X�����܂������� this �� GetComponent<>() ����΂ɕK�v�ŁA
/// this �� GetComponent<>() �͐ÓI�ȃ��\�b�h�ł͎��s�ł��Ȃ��B
/// �̂� GetInstance_N_GameObjectSingletonize<InheritSingletonType>() �� static �ɂł��Ȃ��B
/// ���ꂾ�� static �ł��� Compo�v���p�e�B�� �֐��̖߂�l�𒼐ړn���Ȃ����߁A
/// Compo�v���p�e�B���Ă΂��^�C�~���O�� GetInstance_N_GameObjectSingletonize<SingletonType>() ���ĂԂ��Ƃ͂ł��Ȃ��B
/// �d�����Ȃ��̂� Awake() �ŌĂ񂾃^�C�~���O�� compo �� Compo ���ݒ肳�������ɂȂ����B
/// </summary>

///<summary>
/// �y�t�B�[���h�������q�̒��Ӂz
/// �Ⴆ�΂�����p�������V���O���g���R���|�[�l���g�Ɂupublic int A = new A()�v�Ƃ����t�B�[���h�������q����������A
/// �v���C���[�h�ɓ������ۂ̈�ԍŏ��ƁA�R���|�[�l���g�����ꂽ�ۂ�2��AA�C���X�^���X�������B
/// </summary>

///<summary>
///�y�N���X����ύX����ۂ̒��Ӂz
/// SingletonCompoManager �̒��� ���̃N���X�̖��O�u"SingletonCompo`1"�v���g�p���Ă��邽�߁A
/// �N���X����ύX����ۂ͂�������ꏏ�ɕύX����B
/// ���Ȃ݂ɃW�F�l���b�N�� <T> �̕����̓N���X���ɒ����Ɓu`1�v�炵���B 
/// </summary>
public abstract class SingletonCompo<SingletonType> : NonGenericSingletonCompoBase
    where SingletonType : SingletonCompo<SingletonType>, new()
{
    public override bool PureBaseClass { get; protected set; } = false;
    public override bool IsActive { get; protected set; } = true;
    public sealed override void CreateSingletonGameObject()
    {
        string thisName = GetType().Name;
        GameObject thisGO = GameObject.Find(thisName);
        if (thisGO == null)
        {
            thisGO = new GameObject(thisName);
        }
        //CheckAddComponent<SingletonType>(thisObj);
        // WebGL�p�ɏC��
        thisGO.CheckAddCompo(GetType());          // �^�̓����^�C���Ɋm�肵�Ă���̂� Type �łŊm����
        //thisGO.CheckAddCompo<SingletonType>();
    }

    protected SingletonCompo() { } // �R���X�g���N�^�i�O������̌Ăяo���֎~�j

    private static SingletonType compo;
    public static SingletonType Compo
    {
        get { return compo; }
    }
    public static bool IsCreated
    {
        //ins�͑��̃N���X����͎Q�Ƃł��Ȃ����߁A���̃N���X����uins != null�v�h����ł��Ȃ��B����Ă��̃v���p�e�B���g���B
        get { return compo != null; }
    }

    sealed protected override void Awake()
    {
        if (DestroyMistakenSingleton()) return;  //������ return �厖�BgameObject �� Destroy ���Ă��AAwake() �͍Ō�܂Ŏ��s���ꂿ�Ⴄ�炵���̂ŁB
        GetInstance_N_SingletonizeGameObject<SingletonType>();
        Awake0();
        SubLateAwake();
    }
    protected virtual void Awake0() { }
    [Obsolete("Awake0() ���g���Ă��������B")]
    protected virtual void SubLateAwake() { }

    public bool DestroyMistakenSingleton()
    {
        if (gameObject.name != this.GetType().Name)
        {
            Debug.LogError(@$"�X�N���v�g�ƃQ�[���I�u�W�F�N�g [{gameObject.name}], [{this.GetType()}] ��
                ���O���Ⴄ�V���O���g���Q�[���I�u�W�F�N�g������������j�󂵂��������
                ����ɐV�����̍���Ă��������ǁA
                �蓮�Őݒ肵�������g�������ꍇ�͖��O�𓝈ꂵ�Ă�"
            );
            Destroy(gameObject);
            return true;
        }
        else if (IsCreated)
        {
            Debug.LogWarning("���ɂ���̂Ŏ����͏����܂��B");
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    InheritSingletonType GetInstance_N_SingletonizeGameObject<InheritSingletonType>()
        where InheritSingletonType : class, SingletonType
    {
        if (IsCreated) return compo as InheritSingletonType;
        string thisGName = this.GetType().Name;
        GameObject thisGObj = GameObject.Find(thisGName);
        if (thisGObj == null)
        {
            thisGObj = new GameObject(thisGName);
        }

        // WebGL�p�ɏC��
        if (!thisGObj.TryGetComponent(GetType(), out var _))
        //if (!thisGObj.TryGetComponent(out InheritSingletonType a))
        {
            ///<summary>
            /// GetComponent<>() �� �R���|�[�l���g���̂�Ԃ��̂ɑ΂��A
            /// TryGetComponent() �́A�R���|�[�l���g�����Ă��邩�ǂ�����Ԃ��B
            /// �����ł����Ȃ� CheckComponent() �����Ă��܂��ƁACheckComponent()�֐�����
            /// �ꎞ�I�ɃR���|�[�l���g���擾����Ă��܂��B
            /// ��������Ɖ�����΂����Ƃ����ƁA���̃N���X�̃V���O���g���Q�[���I�u�V�F�N�g��������������ۂɂ�
            /// CheckComponent() ���Ă���A2�� CheckComponent() ���邱�ƂɂȂ�B
            /// CheckComponent() �ňꎞ�I�Ɏ擾���邾���Ƃ͂����A��u�����C���X�^���X��2�ɂȂ�B
            /// ����łǂ�ȕ��Q���N���邩�Ƃ����ƁA�擾�����R���|�[�l���g���̏������̕�����2��Ă΂�Ă��܂��B
            /// �Ⴆ�΂��̃R���|�[�l���g���� �upublic int A = new A()�v�Ƃ����t�B�[���h�������q����������A
            /// 1�x�����Ăт����Ȃ������͂��� A�̃R���X�g���N�^�� 2��Ă΂�Ă��܂����肷��B
            /// ������A�R���|�[�l���g���擾�����ɁA���݂������m�F�ł��� TryGetComponent() �Ő�Ɋm�F���Ă���ɂ���B
            /// �������AAwake() ���I���܂ł̊Ԃ� compo ��ݒ肵�Ȃ��ƍ���B�Ƃ����̂��A
            /// CreateSingletonGameObject() �� ���̃R���|�[�l���g��t�������u�ԂɁA
            /// CreateSingletonGameObject()�̂���ȍ~�̏���������� Awake() ���Ă΂�邩�炾�B
            /// �܂� CreateSingletonGameObject()�̂���ȍ~�̏����� compo ��ݒ肵���̂ł́A
            /// Awake() �����x�����^�C�~���O�� compo �����邱�ƂɂȂ�A
            /// LateSubAwake() �����s����ۂ� compo������������A 
            /// ���̃X�N���v�g�̂��� Compo ���Ă񂾍ۂ�null�ɂȂ����肷��\�������邩�炾�B
            /// ����āACreateSingletonGameObject() �ŕt���ς݂̎�����j�󂵂Ă�����x�t�����邱�Ƃŏ㏑������K�v������B
            /// </summary>
            // WebGL�p�ɏC��
            var prev = GetComponent(GetType());
            if (prev) Destroy(prev);
            compo = (SingletonType)thisGObj.AddComponent(GetType());
            //Destroy(GetComponent<InheritSingletonType>());
            //compo = thisGObj.AddComponent<InheritSingletonType>();
        }
        else
        {
            // WebGL�p�ɏC��
            compo = (SingletonType)thisGObj.GetComponent(GetType());
            //compo = thisGObj.GetComponent<InheritSingletonType>();
        }
        DontDestroyOnLoad(thisGObj);
        return compo as InheritSingletonType;
    }

    private void OnDestroy()
    {
        compo = null;
    }
}
#endregion