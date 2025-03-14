﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScript.Steps;
using Debug = UnityEngine.Debug;

public class Enemy : MonoBehaviour
{


    public int RunAwaySpeed;
    public int RandomSpeed;
    public static bool EnemyCanReproduceHimself;
    private Vector2 randomDirection;

    private Vector3 randomSpeedVector;
    private Vector3 runAwaySpeedVector;
    // public Camera mainCam;// = new Camera();
    public GameObject prefab;
    public static int multiplicationPeriodConstMiliSecond=2000;
    public static int EnemyQuantity = 0;
    private static Object LockObject=new Object();
    private Vector2 randomSpeedVector2d
    {
        get { return new Vector2(randomSpeedVector.x, randomSpeedVector.y); }
        set { randomSpeedVector = new Vector3(value.x, value.y, 0); }
    }
    // Use this for initialization
    void Start()
    {
        lock (Enemy.LockObject)
        {
            EnemyQuantity++;
            Debug.Log(EnemyQuantity);
        }
        randomSpeedVector = new Vector2(Random.Range(100, -100), Random.Range(100, -100)).normalized * RandomSpeed / 10;
        GetComponent<Rigidbody2D>().AddForce(randomSpeedVector);
        InvokeRepeating("changeDirection", 5f, 5f);

        float multiplicationPeriod = (Random.Range(multiplicationPeriodConstMiliSecond, multiplicationPeriodConstMiliSecond*2)) / 1000f;
        InvokeRepeating("multiplicate", multiplicationPeriod, multiplicationPeriod);

        // InvokeRepeating("multiplicate", 1, 1);
    }
     
    // Update is called once per frame
    void Update()
    {
    }


    void changeDirection()
    {
        if (!_freeze)
        {
            randomDirection = new Vector2(Random.Range(100, -100), Random.Range(100, -100)).normalized;
            randomSpeedVector = new Vector3(randomDirection.x * RandomSpeed, randomDirection.y * RandomSpeed, 0);
            GetComponent<Rigidbody2D>().AddForce(randomSpeedVector);
        }

    }

    void multiplicate()
    {


     //   if (EnemyCanReproduceHimself && GameSetup.enemyQuantites < GameSetup.maxEnemyQuantity)
        if (EnemyCanReproduceHimself && Enemy.EnemyQuantity < GameSetup.maxEnemyQuantity)
        {
            Instantiate(prefab, this.gameObject.transform.position - 0.3f * (this.gameObject.transform.position).normalized, Quaternion.identity);
            //GameSetup.enemyQuantites++;
            changeDirection();
        }


    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //  GetComponent<Rigidbody2D>().velocity=new Vector2(0,0);
        //  if (col.gameObject.tag == "HorizontalWall")
        //  {
        //      //randomDirection=new Vector2(randomDirection.x, -randomDirection.y);
        //      // GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -GetComponent<Rigidbody2D>().velocity.y);
        //      randomSpeedVector = new Vector3(randomSpeedVector.x, -randomSpeedVector.y, 0);
        //      //    runAwaySpeedVector = new Vector3(runAwaySpeedVector.x, -runAwaySpeedVector.y, 0);
        //      Debug.Log("Enemy-HorizontalWall collision");
        //  }

        //  if (col.gameObject.tag == "VerticalWall")
        //  {
        //      //randomDirection=new Vector2(-randomDirection.x, randomDirection.y);
        //      // GetComponent<Rigidbody2D>().velocity = new Vector2(-GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);
        //      randomSpeedVector = new Vector3(-randomSpeedVector.x, randomSpeedVector.y, 0);
        //      //  runAwaySpeedVector = new Vector3(-runAwaySpeedVector.x, runAwaySpeedVector.y, 0);
        //      Debug.Log("Enemy-HorizontalWall collision");
        //  }


        if (col.gameObject.tag == "Wall")
        {
            //      Vector2 temp = (new Vector2(transform.position.x, transform.position.y)) - col.GetContact(0).point;
            //Vector2 n = col.GetContact(0).normal.normalized;

            //randomSpeedVector2d = randomSpeedVector2d - 2 * (Vector2.Dot(randomSpeedVector2d, n) * n);
            ////<Rigidbody2D>().transform.position = GetComponent<Rigidbody2D>().transform.position + (new Vector3(n.x, n.y,0))*0.1f;
            ////transform.position = transform.position +new Vector3(n.x, n.y, 0)* 2f;
            //Debug.Log("Enemy-Wall collision, normal:"+n + "compare to manual calculated normal"+temp);
            ////  Destroy(gameObject);
            changeDirection();
        }
    }

    private bool _freeze = false;
    public bool Freeze
    {
        get { return _freeze; }
        set
        {
            _freeze = value;
            if (_freeze)
            {
                if (GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    GetComponent<Rigidbody2D>().angularVelocity = 0;
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
            }
            else
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    public void Destroy()
    {
        tag = "DestroyedEnemy";
        GetComponent<Animator>().Play("EnemyDeath");
        gameObject.layer = 11;
        Freeze = true;
        InvokeRepeating("DestroyExecution", 0.4f, 0.4f);
        lock (Enemy.LockObject)
        {
            EnemyQuantity--;
            Debug.Log(EnemyQuantity);
        }
    }

    public void DestroyExecution()
    {

        Destroy(this.gameObject);
    }
}
