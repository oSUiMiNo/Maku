using MyUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 左クリックでなぞった領域に障害物を生成するスクリプト
public class InkDispenser : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	float Thickness = 0.3f;
	[SerializeField]
	float DistanceFromCamera => gameObject.Distance(mainCam.gameObject);
    [SerializeField]
	Material InkMT;

	Camera mainCam;
	GameObject Ink_Sphere;
    GameObject Ink_Cylinder;

	Vector3 prevPos;
	Vector3 nowPos;

	List<GameObject> writtenInks = new ();
	private Vector3 defaultPos = new (-1000, -1000, 1000);


	void Start()
	{
        mainCam = Camera.main;

		GameObject CreateInk(string name, PrimitiveType shape)
		{
            GameObject ink = GameObject.CreatePrimitive(shape);
            ink.transform.SetParent(transform);
            ink.transform.localPosition = Vector3.zero;
            ink.name = name;
            ink.SetActive(false);
            ink.GetComponent<Renderer>().material = InkMT;
			return ink;
        }
        Ink_Sphere = CreateInk("Ink_Sphere", PrimitiveType.Sphere);
		Ink_Cylinder = CreateInk("Ink_Cylinder", PrimitiveType.Cylinder);
	}


	void FixedUpdate()
	{
		// フレームごとに現在のマウス位置と前フレームのマウス位置を更新
		// terrain上にのみ生成
		if (Input.GetMouseButtonDown(0) && IsInArea())
		{
			prevPos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			prevPos = nowPos = defaultPos;
		}

		if (Input.GetMouseButton(0) && IsInArea())
		{
			nowPos = Input.mousePosition;
			CreateObstacle();
			prevPos = nowPos;
		}
	}


	void Update()
    {
        //if (Input.GetMouseButtonDown(0) && IsInArea())
        //{
        //    prevPos = Input.mousePosition;
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    prevPos = nowPos = defaultPos;
        //}

        if (Input.GetKeyDown(KeyCode.A)) Combine();
    }


	// nowPos,prevPosを結ぶようなカプセル型のオブジェクトを生成する
	// カプセル型のオブジェクトは2つの球と円柱で表現される
	void CreateObstacle()
	{
		var screenPos_Prev = new Vector3(prevPos.x, prevPos.y, DistanceFromCamera);
		var screenPos_Now = new Vector3(nowPos.x, nowPos.y, DistanceFromCamera);
		if (Vector3.Distance(prevPos, nowPos) == 0) return;
		if (prevPos == defaultPos) return;
		if (nowPos == defaultPos) return;

        Vector3 pos1 = mainCam.ScreenToWorldPoint(screenPos_Prev);
		Vector3 pos2 = mainCam.ScreenToWorldPoint(screenPos_Now);
		pos1.y = 0;
		pos2.y = 0;
		Vector3 pos3 = (pos1 + pos2) / 2;

		GameObject Sphere1 = Instantiate(Ink_Sphere, pos1, Quaternion.identity);
		GameObject Sphere2 = Instantiate(Ink_Sphere, pos2, Quaternion.identity);
		GameObject Cylinder = Instantiate(Ink_Cylinder, pos3, Quaternion.identity);
		
		Sphere1.SetActive(true);
        Sphere2.SetActive(true);
        Cylinder.SetActive(true);

        writtenInks.Add(Sphere1);
		writtenInks.Add(Sphere2);
		writtenInks.Add(Cylinder);

		Sphere1.transform.localScale = new Vector3(Thickness, Thickness, Thickness);
		Sphere2.transform.localScale = new Vector3(Thickness, Thickness, Thickness);
		Cylinder.transform.eulerAngles = new Vector3(90f, Vector3.SignedAngle(Vector3.forward, pos2 - pos1, Vector3.up), 0);
		Cylinder.transform.localScale = new Vector3(Thickness, Vector3.Distance(pos1, pos2) / 2, Thickness);
	}


	void Combine()
    {
        GameObject writingElementsParent = new GameObject($"Inks");
		//writingElementsParent.AddComponent<Test_MeshParticle>();
        writtenInks.ForEach(a => a.transform.SetParent(writingElementsParent.transform));
        writtenInks.Clear();
		writingElementsParent.CombineMesh(InkMT);
		//MeshUtil.CombineMesh(writingElementsParent, InkMT);
    }


    bool IsInArea()
	{
		// terrainのレイヤーのみ判定
		LayerMask mask = ~LayerMask.NameToLayer("Paper");
		var ray = mainCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
	}


	void OnTriggerEnter(Collider collider)
	{
		string debugText = $"amount = {collider.gameObject.layer}\neat num = {LayerMask.NameToLayer("ColonyPheromone")}\n";
		Debug.Log(debugText);
	}
}