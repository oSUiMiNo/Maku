using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class MyUI : SealableMonoBehaviourMyExtention
{
    public Subject<Unit> On_Enter = new Subject<Unit>();
    public Subject<Unit> On_Exit = new Subject<Unit>();
    public Subject<Unit> On_Down = new Subject<Unit>();
    public Subject<Unit> On_Over = new Subject<Unit>();
    public Subject<Unit> On_Drag = new Subject<Unit>();
    public Subject<Unit> On_Click = new Subject<Unit>();
    public Subject<Unit> On_ClickMargin = new Subject<Unit>();

    public IObservable<bool> ClickedObservable => Clicked;
    public ReactiveProperty<bool> Clicked = new ReactiveProperty<bool>();

    protected sealed override async void Awake()
    {
        //presenter = UIPresenter.Childlen[gameObject];
        InputEventHandler.OnDown_MouseLeft += () => CheckClickMargin();
        SetLayer("UI");
        Awake0(); Awake1();
        //InputEventHandler.OnDown_MouseLeft += () => DebugView.Log($"どこかしらクリックした");
        //On_ClickMargin.Subscribe(_ => DebugView.Log($"余白をクリックした"));
    }
    protected virtual void Awake0() { }
    protected virtual void Awake1() { }

    public void SetLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    private void OnMouseEnter()
    {
        On_Enter.OnNext(Unit.Default);
    }

    private void OnMouseExit()
    {
        On_Exit.OnNext(Unit.Default);
    }

    private void OnMouseDown()
    {
        On_Down.OnNext(Unit.Default);
        CheckClick();
        if (Clicked.Value) On_Click.OnNext(Unit.Default);
    }

    private void OnMouseOver()
    {
        On_Over.OnNext(Unit.Default);
    }

    private void OnMouseDrag()
    {
        On_Drag.OnNext(Unit.Default);
    }

    async void CheckClick()
    {
        Clicked.Value = true;
        await UniTask.DelayFrame(1);
        Clicked.Value = false;
    }

    async void CheckClickMargin()
    {
        if (Clicked.Value) return;
        On_ClickMargin.OnNext(Unit.Default);
    }
}

