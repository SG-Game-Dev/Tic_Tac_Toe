using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe
{
    public class BoardView
    {
        readonly Transform root;

        public BoardView(GameObject buttons)
        {
            root = buttons.transform;
        }

        public void ClearAll()
        {
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }

        // हर button पर click listener लगाओ
        public void BindClicks(Action<TextMeshProUGUI, GameObject> onClick)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var button = root.GetChild(i);
                var buttonText = button.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => onClick(buttonText, button.gameObject));
                }
            }
        }

        public static TextMeshProUGUI GetLabel(GameObject cell)
        {
            return cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public void SetMark(GameObject cell, string mark) => GetLabel(cell).text = mark;
        public void ClearMark(GameObject cell) => GetLabel(cell).text = "";

        public void LockCell(GameObject cell)
        {
            cell.GetComponent<Button>().interactable = false;
        }

        public List<GameObject> GetEmptyCells()
        {
            List<GameObject> empty = new List<GameObject>();
            for (int i = 0; i < root.childCount; i++)
            {
                var btn = root.GetChild(i).gameObject;
                if (GetLabel(btn).text == "") empty.Add(btn);
            }
            return empty;
        }
    }
}
