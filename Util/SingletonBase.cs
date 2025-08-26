using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using MyUtil;


#region シングルトン ( 通常クラス )
/// <summary>
/// 【使い方】
/// コンポーネント化しないスクリプトに継承させるとシングルトンになる。
/// 継承させる際、ジェネリックの型引数に派生クラス型を入れる。
/// 【詳しい使い方】
/// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa#f7d0608026a048eebb61c5a949c4f379
/// </summary>
/// <typeparam name="SingletonType"></typeparam>
public abstract class Singleton<SingletonType> :
    System.IDisposable
    where SingletonType : Singleton<SingletonType>, new()
{
    protected Singleton() { } // コンストラクタ（外部からの呼び出し禁止） 

    private static SingletonType ins;
    public static SingletonType Ins
    {
        get { return GetOrCreateInstance<SingletonType>(); }
    }
    public static bool IsCreated
    {
        //insは他のクラスからは参照できないため、他のクラスから「ins != null」派判定できない。よってこのプロパティを使う。
        get { return ins != null; }
    }

    protected static InheritSingletonType GetOrCreateInstance<InheritSingletonType>()
        where InheritSingletonType : class, SingletonType, new()
    {
        if (IsCreated)
        {
            // 基底クラスから呼ばれた後に継承先から呼ばれるとエラーになる。先に継承先から呼ぶ
            if (!typeof(InheritSingletonType).IsAssignableFrom(ins.GetType()))
            {
                Debug.LogErrorFormat("{1}が{0}を継承していません", typeof(InheritSingletonType), ins.GetType());
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









#region シングルトン ( MonoBehaviour )
/// <summary>
/// シーンのロード直後にシングルトンゲームオブジェクトを自動生成する
/// </summary>
public class SingletonCompoSetter
{
    /// <summary>
    /// シーンのロード前だと、シーン上のゲームオブジェクトを探すことが出来ないので
    /// RuntimeInitializeOnLoadMethod() の引数は RuntimeInitializeLoadType.AfterSceneLoad
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        CreateSingletonObjects();
    }

    static void CreateSingletonObjects()
    {
        /////<summary>
        ///// 「typeof(インターフェイス)」ではダメらしい。
        ///// また、「typeof(ジェネリッククラス)」だと型引数周りが上手いこと出来ない。
        ///// なので、SingletonCompo<SingletonType> を、空の抽象クラス NonGenericSingletonCompoBase で包んで
        ///// それを使って取得した。
        ///// </summary>
        //IEnumerable<Type> singletonClassesType;
        //singletonClassesType = System.Reflection.Assembly
        //    .GetAssembly(typeof(NonGenericSingletonCompoBase))
        //    .GetTypes()
        //    .Where(t =>
        //    {
        //        return
        //            t.IsSubclassOf(typeof(NonGenericSingletonCompoBase)) &&
        //            !t.IsAbstract &&    //コンポーネント化出来ない abstractクラスは除外。  
        //            !t.ContainsGenericParameters;    // ジェネリッククラスは下の、インスタンス生成の処理を通せないので除外。
        //        ///<summary>
        //        /// 【注意】
        //        /// 1 : 基底クラスでも、自身もコンポーネントになりうる場合は abstractクラスにしないのはまあ当然だが、
        //        /// コンポーネント化せず、ただの基底クラスとしてのみ使う場合は、ちゃんと抽象クラスにする。
        //        /// そうしないと ↑ ここではじけない。
        //        /// それと、意味を考えても、基底クラスとしてのみ使うということは抽象化専門のクラスなわけだから、
        //        /// abstract にした方が筋が通る。
        //        /// 
        //        /// 2 : シングルトンの基底クラスを作ろうとするとジェネリッククラスになるが、
        //        /// 「 object obj = Activator.CreateInstance(ジェネリッククラス) 」はできないので、
        //        /// はじく。
        //        /// ただし、そもそもシングルトンの基底クラスは普通 abstract にしており、はじかれるんだけど。
        //        /// </summary>
        //    });


        // 上記処理では他のアセンブリ内のスクリプトに適用されないので新調した
        IEnumerable<Type> singletonClassesType = AppDomain.CurrentDomain.GetAssemblies() // 現在のアプリケーションドメイン内の全アセンブリを取得
        .SelectMany(assembly =>
        {
            try
            {
                return assembly.GetTypes(); // アセンブリ内の全ての型を取得
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 一部の型がロードできなかった場合でも処理を継続
                return ex.Types.Where(t => t != null);
            }
        })
        .Where(t =>
        {
            return t != null &&
                   t.IsSubclassOf(typeof(NonGenericSingletonCompoBase)) && // NonGenericSingletonCompoBaseのサブクラスかどうか
                   !t.IsAbstract && // 抽象クラスではない
                   !t.ContainsGenericParameters; // ジェネリック型ではない
        });


        foreach (var a in singletonClassesType)
        {
            //DebugView.Log($"シングルトン {a.Name} を作成");
            ///<summary>
            /// 取得したType、a のクラスのインスタンスを object型で生成し、
            /// 基底クラスの NonGenericSingletonCompoBase型に変換し、
            /// 全派生クラス共通の抽象メソッドを呼ぶ。
            /// </summary>
            object obj = Activator.CreateInstance(a);
            NonGenericSingletonCompoBase n = (NonGenericSingletonCompoBase)obj;

            ///<summary>
            /// ただ基底クラスとしでだけ使いたい場合でも、
            /// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa
            /// ↑これの、「基底も継承先も同じインスタンスとして利用する使い方」だと基底クラスを abstract にできないので、
            /// はじかれずにシングルトンゲームオブシェクトが生成されてしまう。
            /// そこで、ただ基底クラスとしてのみ利用したい場合は「PureBaseClassフラグ」を true にすることで、
            /// CreateSingletonGameObject(); が呼ばれないようにしている。
            ///  SingletonCompo<> 内で false にオーバーライドしてあるため、純粋基底クラスにしたくない場合は実装しなくてよくて、
            /// 純粋基底クラスにしたい場合のみ true にオーバーライドすればよい。
            /// なお、純粋基底クラスである SingletonCompo<> 内で false になってしまっているが、
            /// ジェネリックでabstractなのでそもそもはじかれているからOK。
            /// </summary>
            if (n.PureBaseClass) continue;
            ///<summary>
            /// シングルトンコンポを継承してしまうと否が応でもゲームオブジェクトが生成されて中身が動いてしまおう。
            /// でも使いたくない時に丸ごとコメントアウトするのは面倒くさい。他クラスへの副作用とかもあるかもだし。
            /// なのでアクティブかどうかのフラグが false だったらはじく。
            /// SingletonCompo<> 内で false にオーバーライドしてあるため、アクティブで良い場合は実装しなくてよくて、
            /// 非アクティブにしたい場合のみ true にオーバーライドすればよい。
            /// </summary>
            if (!n.IsActive) continue;
            n.CreateSingletonGameObject();
        }
    }
}

public abstract class NonGenericSingletonCompoBase : SealableMonoBehaviour
{
    ///<summary>
    /// ただ基底クラスとしでだけ使いたい場合でも、
    /// https://www.notion.so/7d0966e1c52e48ec84e976af050a84aa
    /// ↑これの、「基底も継承先も同じインスタンスとして利用する使い方」だと基底クラスを abstract にできないので、
    /// はじかれずにシングルトンゲームオブシェクトが生成されてしまう。
    /// そこで、ただ基底クラスとしてのみ利用したい場合は「PureBaseClassフラグ」を true にすることで、
    /// SingletonCompoSetter で CreateSingletonGameObject(); が呼ばれないようにしている。
    /// SingletonCompo<> 内で false にオーバーライドしてあるため、純粋基底クラスにしたくない場合は実装しなくてよくて、
    /// 純粋基底クラスにしたい場合のみ true にオーバーライドすればよい。
    /// なお、純粋基底クラスである SingletonCompo<> 内で false になってしまっているが、
    /// ジェネリックでabstractなのでそもそもはじかれているからOK。
    /// </summary>
    public abstract bool PureBaseClass { get; protected set; }
    public abstract bool IsActive { get; protected set; }
    public abstract void CreateSingletonGameObject();
}

/// <summary>
/// 【使い方】
/// コンポーネント化するスクリプトに継承させるとシングルトンになる。
/// 継承させる際、ジェネリックの型引数に派生クラス型を入れる。
/// 自分でシングルトン用のゲームオブジェクトを作らなくても自動で生成されてコンポーネントをアタッチしてくれるが、
/// 自分でシングルトン用のゲームオブジェクトを作成した場合はそちらが採用されるので、必要に応じて手動で作っても良い。
/// 自分でゲームオブジェクトを作る場合は、
/// アタッチするシングルトンのコンポーネントと同じ名前にしないと削除されるので気を付けてね。
/// 同じ名前のゲームオブジェクトをであれば、コンポーネントをアタッチしていなくても自動でアタッチしてくれる。
/// 勿論必要に応じて手動でアタッチしても良い。
/// </summary>

///<summary>
/// 【一番のメリット】
/// 手動でシングルトンゲームオブシェクトを用意しないと使えない状態では、
/// 任意のシーンからプレイモードに入る際に、
/// そのシーンにも専用のシングルトンゲームオブシェクトを事前に準備しておかないとシングルトンが無い状態になってしまうが、
/// これを継承しておけばシングルトンが無いシーンでも自動生成されるので、
/// 事前に全シーンに同じシングルトンゲームオブシェクトを用意しておく必要が無くなる。
/// そのメリットを最大限享受するためには、手動でシングルトンゲームオブシェクトを拵えた方が都合の良い設計を極力控えた方が良い。
/// 例えばシングルトンゲームオブジェクトにあらかじめ何か子オブジェクトを配置するような設計は控え、代わりに、
/// 「new GameObject(名前)」で「動的に子を生成したり、「(GameObject)Resources.Load(名前)」で動的にロードするなど。
/// </summary>

///<summary>
/// 【外部からCompoを取得する際の注意】
/// Awake() が呼ばれるより前に Compo を呼んではいけない。
/// 例えば別のクラスでフィールドの宣言と同時にこのクラスの Compo をそこに代入するのはやばい。
/// 【理由】
/// ins に自身のインスタンスをしまう処理に this か GetComponent<>() が絶対に必要で、
/// this や GetComponent<>() は静的なメソッドでは実行できない。
/// ので GetInstance_N_GameObjectSingletonize<InheritSingletonType>() は static にできない。
/// それだと static である Compoプロパティに 関数の戻り値を直接渡せないため、
/// Compoプロパティが呼ばれるタイミングで GetInstance_N_GameObjectSingletonize<SingletonType>() を呼ぶことはできない。
/// 仕方がないので Awake() で呼んだタイミングで compo や Compo が設定される実装になった。
/// </summary>

///<summary>
/// 【フィールド初期化子の注意】
/// 例えばこれを継承したシングルトンコンポーネントに「public int A = new A()」というフィールド初期化子があったら、
/// プレイモードに入った際の一番最初と、コンポーネント化された際の2回、Aインスタンスが作られる。
/// </summary>

///<summary>
///【クラス名を変更する際の注意】
/// SingletonCompoManager の中で このクラスの名前「"SingletonCompo`1"」を使用しているため、
/// クラス名を変更する際はそちらも一緒に変更する。
/// ちなみにジェネリックの <T> の部分はクラス名に直すと「`1」らしい。 
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
        // WebGL用に修正
        thisGO.CheckAddCompo(GetType());          // 型はランタイムに確定しているので Type 版で確実に
        //thisGO.CheckAddCompo<SingletonType>();
    }

    protected SingletonCompo() { } // コンストラクタ（外部からの呼び出し禁止）

    private static SingletonType compo;
    public static SingletonType Compo
    {
        get { return compo; }
    }
    public static bool IsCreated
    {
        //insは他のクラスからは参照できないため、他のクラスから「ins != null」派判定できない。よってこのプロパティを使う。
        get { return compo != null; }
    }

    sealed protected override void Awake()
    {
        if (DestroyMistakenSingleton()) return;  //ここの return 大事。gameObject を Destroy しても、Awake() は最後まで実行されちゃうらしいので。
        GetInstance_N_SingletonizeGameObject<SingletonType>();
        Awake0();
        SubLateAwake();
    }
    protected virtual void Awake0() { }
    [Obsolete("Awake0() を使ってください。")]
    protected virtual void SubLateAwake() { }

    public bool DestroyMistakenSingleton()
    {
        if (gameObject.name != this.GetType().Name)
        {
            Debug.LogError(@$"スクリプトとゲームオブジェクト [{gameObject.name}], [{this.GetType()}] の
                名前が違うシングルトンゲームオブジェクトがあったから破壊しちゃったよ
                代わりに新しいの作っておいたけど、
                手動で設定した方を使いたい場合は名前を統一してね"
            );
            Destroy(gameObject);
            return true;
        }
        else if (IsCreated)
        {
            Debug.LogWarning("既にあるので自分は消えます。");
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

        // WebGL用に修正
        if (!thisGObj.TryGetComponent(GetType(), out var _))
        //if (!thisGObj.TryGetComponent(out InheritSingletonType a))
        {
            ///<summary>
            /// GetComponent<>() が コンポーネント自体を返すのに対し、
            /// TryGetComponent() は、コンポーネントがついているかどうかを返す。
            /// ここでいきなり CheckComponent() をしてしまうと、CheckComponent()関数内で
            /// 一時的にコンポーネントが取得されてしまう。
            /// そうすると何がやばいかというと、このクラスのシングルトンゲームオブシェクトを自動生成する際にも
            /// CheckComponent() しており、2回 CheckComponent() することになる。
            /// CheckComponent() で一時的に取得するだけとはいえ、一瞬だけインスタンスが2つになる。
            /// それでどんな弊害が起きるかというと、取得したコンポーネント内の初期化の部分が2回呼ばれてしまう。
            /// 例えばそのコンポーネント内に 「public int A = new A()」というフィールド初期化子があったら、
            /// 1度しか呼びたくなかったはずの Aのコンストラクタが 2回呼ばれてしまったりする。
            /// だから、コンポーネントを取得せずに、存在だけを確認できる TryGetComponent() で先に確認してからにする。
            /// ただし、Awake() が終わるまでの間に compo を設定しないと困る。というのも、
            /// CreateSingletonGameObject() で このコンポーネントを付加した瞬間に、
            /// CreateSingletonGameObject()のそれ以降の処理よりも先に Awake() が呼ばれるからだ。
            /// つまり CreateSingletonGameObject()のそれ以降の処理で compo を設定したのでは、
            /// Awake() よりも遅いいタイミングで compo が入ることになり、
            /// LateSubAwake() を実行する際に compoが無かったり、 
            /// 他のスクリプトのから Compo を呼んだ際にnullになったりする可能性があるからだ。
            /// よって、CreateSingletonGameObject() で付加済みの自分を破壊してもう一度付加することで上書きする必要がある。
            /// </summary>
            // WebGL用に修正
            var prev = GetComponent(GetType());
            if (prev) Destroy(prev);
            compo = (SingletonType)thisGObj.AddComponent(GetType());
            //Destroy(GetComponent<InheritSingletonType>());
            //compo = thisGObj.AddComponent<InheritSingletonType>();
        }
        else
        {
            // WebGL用に修正
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