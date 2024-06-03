using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // �����, � ���� ���� ���������� EnemyAI
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;  // �������� ���� EnemyAI
    private float timer;  // ������, ���� ��������������� ��� ���������� ��������� 䳿 � ���� TakingTurn

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;  // ���������� ���� - ���������� ���� ������
    }

    private void Start()
    {
        //  ³��������� ���� � ��
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        // ���� ����� ������� �� ���, ����� � ������
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        // ����� � ��������� �� ��������� ����� EnemyAI
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                
                break;
            case State.TakingTurn:
                // ��������� ������� ��� ���������� ��������� ����
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    // ���� EnemyAI ����� ������� ��� ���, ������� �� ����� Busy, � ������ ������� ������� �� ���������� ����
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
               
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;  // ������������ ������� ��� ���������� ��������� 䳿 � ���� TakingTurn
        state = State.TakingTurn;  // ������� �� ����� TakingTurn
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        // ϳ� ��� ���� ���� ������� �� ����� TakingTurn, ���� ����� �� ��� ������
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;  // ������������ ������� ��� ���������� ��������� 䳿 � ���� TakingTurn
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        // ��������� �� EnemyAI ��� ��� ������� �������
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        // �������� ��� �������� �� ������ �������
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            // ��������, �� ���� ������ ������� ��������� ���� 䳿 �� ���� ��
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                // ����� �� ���� ��������� ��� �� ��
                continue;
            }

            // ����� ������ �������� 䳿 ��� EnemyAI
            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        // ��������� �������� 䳿, ���� ���� ����
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
