using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputNomeJogador : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField inputNomeJogador = null;
    [SerializeField] private Button botaoContinuar = null;

    public static string NomeDisplay { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName"; // Chave como se fosse de um dicionario

    void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) // Se essa chave nao existir
        {
            return;
        }

        string nomePadrao = PlayerPrefs.GetString(PlayerPrefsNameKey); 

        inputNomeJogador.text = nomePadrao; // Seta o nome la do input

        SetPlayerName(nomePadrao);
    }

    public void SetPlayerName(string nome)
    {
        botaoContinuar.interactable = !string.IsNullOrEmpty(nome);
    }

    public void SavePlayerName()
    {
        NomeDisplay = inputNomeJogador.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey,NomeDisplay);
    }
}
