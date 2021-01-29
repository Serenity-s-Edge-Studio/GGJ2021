using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComicBookManager : MonoBehaviour
{
    public UnityEvent OnStartComic;
    public UnityEvent OnEndComic;

    public ComicBookSO[] SceneComic;
    public ComicBookSO[] ClueComic;
}
