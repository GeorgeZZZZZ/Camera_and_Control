using UnityEngine;
using System.Collections;

public class EnemyHealth: MonoBehaviour {
	public float maxHealth= 30;
	public float curHealth= 30;
	public float healthBarLength;
	public float healthBorder;
	
	public GameObject myhealthBar;
	public GameObject myhb;
	public int healthbarWidth;
	// Use this for initialization
	void Start () {
		//healthBarLength =Screen.width/2;
		healthbarWidth =50;
		myhb=(GameObject)Instantiate(myhealthBar,transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		AddjustCurrentHealth(0);
		

	}
	
	void OnGUI(){
		//GUI.Box(new Rect(0,0,Screen.width,Screen.height),"This is a title");
		//1. GUI.Box(new Rect(10,10,Screen.width/2/(maxHealth/curHealth),20), curHealth +"/"+ maxHealth);
		//2.
		//GUI.Box(new Rect(10,40,healthBarLength,20), curHealth +"/"+ maxHealth);
		
		
	}
	public void AddjustCurrentHealth(int adj)
	{
		curHealth += adj;
		if(curHealth<0)
		{
			curHealth =0;
		}
		if(curHealth>maxHealth)
		{
			curHealth=maxHealth;	
		}
		if(maxHealth<1)
		{
			maxHealth=1;
		}
		//healthBarLength = (Screen.width / 2)* (curHealth/(float)maxHealth);
		
		myhb.transform.position=Camera.main.WorldToViewportPoint(transform.position);
		float healthpercent =(curHealth/maxHealth)*50;
		healthbarWidth=(int)healthpercent;
		myhb.GetComponent<GUITexture>().pixelInset=new Rect(10,10,healthbarWidth,5);
	}
}
