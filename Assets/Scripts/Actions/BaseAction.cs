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
        // Створення списку можливих дій для ворожого об'єкта
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        // Отримання списку дій, які можливі для ворожого об'єкта
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        // Проходження через кожну позицію, де можлива ворожа дія
        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            // Отримання об'єкта ворожої дії для поточної позиції
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);

            // Додавання ворожої дії до списку
            enemyAIActionList.Add(enemyAIAction);
        }

        // Якщо є хоча б одна можлива ворожа дія
        if (enemyAIActionList.Count > 0)
        {
            // Сортування списку ворожих дій за зменшенням значення (actionValue)
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);

            // Повернення найкращої ворожої дії (першої в списку)
            return enemyAIActionList[0];
        }
        else
        {
            return null;
        }
    }
    // Реалізація конкретного отримання ворожої дії для певної позиції
    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

}