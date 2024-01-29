using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftwall;
    [SerializeField]
    private GameObject _rightwall;
    [SerializeField]
    private GameObject _frontwall;
    [SerializeField]
    private GameObject _backwall;
    [SerializeField]
    private GameObject _unvisitedBlock;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        _leftwall.SetActive(false);
    }
    public void ClearRightWall()
    {
        _rightwall.SetActive(false);
    }
    public void ClearFrontWall()
    {
        _frontwall.SetActive(false);
    }
    public void ClearBackWall()
    {
        _backwall.SetActive(false);
    }

}