using UnityEngine;
using UnityEngine.VFX;


/// <summary>
/// 【読んでね】
/// このスクリプトにはVFXGraphのいくつかのプロパティを設定する処理が書いてあるが、
/// いづれもメッシュから動的に AttributeMap を生成するのに必要なプロパティである。
/// その目的に対して最低限の機能だけを書いたものなので、
/// ここでパーティクルの数の設定など、関係ないプロパティはいじらない。
/// あくまでここでは動的に AttributeMap を設定する責任だけをうけおい、
/// 他の設定は他のスクリプトなりインスペクターなりVFXGraphから直接なりで行う。
/// </summary>

/// <summary>
/// 【使い方】
/// これをアタッチしたゲームオブシェクトのインスペクターから子オブジェクトに、
/// 実際のVFXGraphをアタッチしたオブシェクト(つまりエフェクト本体)を1つだけ設置する。
/// それが自動的に effect変数 に入って処理が進む。
/// それ以外の子オブジェクトは作らず、エフェクト本体を1つのみ。
/// エディタ上では実質、これをアタッチしたオブシェクトがエフェクトそのもののように扱う。
/// アタッチするオブシェクトのネーミングは「子オブシェクトの名前+Unit」とか良いと思う。
/// </summary>
public class VFXUnit_DynamicAttributeMap : VFXUnitComponent
{
    public static class PropName
    {
        public const string Map_Position = "PositionMap";
        public const string Map_UV = "UVMap";
        public const string Map_Normal = "NormalMap";
        public const string Tex_ModelMain = "ModelMainTex";
        public const string VertexCount = "ParticleCount";
    }
    
    public MapSet mapSet;
    public Texture modelMainTex;

    protected override void Init()
    {
        SetProperties();
    }

    public override void SetProperties()
    {
        vfx.SetTexture(PropName.Map_UV, mapSet.uv);
        vfx.SetTexture(PropName.Map_Position, mapSet.position);
        vfx.SetTexture(PropName.Map_Normal, mapSet.normal);
        vfx.SetTexture(PropName.Tex_ModelMain, modelMainTex);
        vfx.SetInt(PropName.VertexCount, mapSet.vtxCount);
    }
}
 


