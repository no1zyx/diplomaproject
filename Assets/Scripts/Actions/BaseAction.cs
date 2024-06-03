using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;


    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        // ��������� ������ �������� �� ��� �������� ��'����
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        // ��������� ������ ��, �� ������ ��� �������� ��'����
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        // ����������� ����� ����� �������, �� ������� ������ ��
        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            // ��������� ��'���� ������ 䳿 ��� ������� �������
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);

            // ��������� ������ 䳿 �� ������
            enemyAIActionList.Add(enemyAIAction);
        }

        // ���� � ���� � ���� ������� ������ ��
        if (enemyAIActionList.Count > 0)
        {
            // ���������� ������ ������� �� �� ���������� �������� (actionValue)
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);

            // ���������� �������� ������ 䳿 (����� � ������)
            return enemyAIActionList[0];
        }
        else
        {
            return null;
        }
    }
    // ��������� ����������� ��������� ������ 䳿 ��� ����� �������
    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

}