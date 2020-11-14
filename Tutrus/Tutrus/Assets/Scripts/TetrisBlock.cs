﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour {
    public Vector3 rotationPoint;
    public float prevTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    public static Transform[, ] grid = new Transform[width, height]; // creating each tetromino colide

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            transform.position += new Vector3 (-1, 0, 0);
            if (!ValidMove ())
                transform.position -= new Vector3 (-1, 0, 0);
        } else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            transform.position += new Vector3 (1, 0, 0);
            if (!ValidMove ())
                transform.position -= new Vector3 (1, 0, 0);
        } else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            // rotate
            transform.RotateAround (transform.TransformPoint (rotationPoint), new Vector3 (0, 0, 1), 90);
            if (!ValidMove ())
                transform.RotateAround (transform.TransformPoint (rotationPoint), new Vector3 (0, 0, 1), -90);
        }

        // moving the tetromino down and also move the tetromino down faster if we press down key
        if (Time.time - prevTime > (Input.GetKeyDown (KeyCode.DownArrow) ? fallTime / 10 : fallTime)) {
            transform.position += new Vector3 (0, -1, 0);
            if (!ValidMove ()) {
                transform.position -= new Vector3 (0, -1, 0);
                AddToGrid ();
                CheckForLines ();

                this.enabled = false;
                FindObjectOfType<SpawnTetromino> ().NewTetromino (); // memanggil tetromino random jika sudah sampai bottom
            }
            prevTime = Time.time;
        }
        // penjelasan if diatas, jika waktu sekarang - waktu sebelum > waktu untuk turun, maka tetro turun 1, tp jika
        // user menekan down arrow, fall time / 10 ( turun lebih cepat )
    }

    void CheckForLines() {
        for (int i = height - 1; i >= 0; i--) // cek setiap baris dari atas ke bawah
        {
            if(HasLine(i)) // jika pada baris tertentu baris full
            {
                DeleteLine(i); // delete ( + points up )
                RowDown(i); // pindahkan semua tetromino ke bawah untuk mengisi baris kosong
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if(grid[j, i] == null)
                return false;
        }
        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    void AddToGrid () {
        foreach (Transform children in transform) {
            int roundedX = Mathf.RoundToInt (children.transform.position.x);
            int roundedY = Mathf.RoundToInt (children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    bool ValidMove () {
        foreach (Transform children in transform) {
            int roundedX = Mathf.RoundToInt (children.transform.position.x);
            int roundedY = Mathf.RoundToInt (children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) {
                return false;
            }
            if (grid[roundedX, roundedY] != null)
                return false;
        }
        return true;
    }
}