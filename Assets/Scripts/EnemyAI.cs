using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Стани, в яких може перебувати EnemyAI
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;  // Поточний стан EnemyAI
    private float timer;  // Таймер, який використовується для визначення тривалості дії в стані TakingTurn

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;  // Початковий стан - очікування ходу ворога
    }

    private void Start()
    {
        //  Відстеження ходу в грі
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        // Якщо зараз гравець має хід, вихід з методу
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        // Логіка в залежності від поточного стану EnemyAI
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                
                break;
            case State.TakingTurn:
                // Зменшення таймера для визначення тривалості ходу
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    // Якщо EnemyAI вдало виконав свій хід, перехід до стану Busy, в іншому випадку перехід до наступного ходу
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
        timer = 0.5f;  // Встановлення таймера для визначення тривалості дії в стані TakingTurn
        state = State.TakingTurn;  // Перехід до стану TakingTurn
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        // Під час зміни ходу перехід до стану TakingTurn, якщо зараз не хід гравця
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;  // Встановлення таймера для визначення тривалості дії в стані TakingTurn
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        // Виконання дій EnemyAI для всіх ворожих одиниць
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

        // Перегляд всіх можливих дій ворожої одиниці
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            // Перевірка, чи може ворожа одиниця витратити очки дії на дану дію
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                // Ворог не може дозволити собі цю дію
                continue;
            }

            // Логіка вибору найкращої дії для EnemyAI
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

        // Виконання найкращої дії, якщо така існує
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
