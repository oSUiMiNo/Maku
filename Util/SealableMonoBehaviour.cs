using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//！！！！！！！！！！！！！！！！！！！！！！！！！！！
// SealableMonoBehaviour に移動済み
//！！！！！！！！！！！！！！！！！！！！！！！！！！！
/// <summary>
/// 【使い方】
/// 1: なにかの基底クラスにしたいクラスにこれを継承させておく。
/// 
/// 2: projectのAssetsフォルダ直下に、「csc.rsp」というファイルを追加。更にこれの中身のテキストを、
/// 「-warnaserror+:0114」とだけ書いておく。
/// これにより、派生クラスで Start() などを実装してしまった際に、エラーにしてくれるので、
/// 誤って基底クラスでも派生クラスでも別の処理を書いたStart()を実装っしてしまって、
/// 基底クラスの Start()などに書いた必要な処理が上書きされてしまうことを防げる。
/// 
/// 3: 派生先でも Start()相当の関数を使いたいのであれば、
/// 基底クラスで SubStart() みたいな仮想メソッドを作り、基底クラスの Start()の中で呼んでおき、
/// 派生クラスでオーバーライドする。
/// </summary>
public class SealableMonoBehaviour : MonoBehaviour //MonoBehaviourMyExtention
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
}

