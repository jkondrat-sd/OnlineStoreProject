import React, { useState, useEffect } from 'react';
import { createAccountAuto, deposit, getBalance } from '../services/api';

const Account = () => {
    const [userId, setUserId] = useState(null);
    const [balance, setBalance] = useState(null);

    const [initialAmount, setInitialAmount] = useState(0);
    const [initialCurrency, setInitialCurrency] = useState('USD');

    const [depositAmount, setDepositAmount] = useState(0);
    const [message, setMessage] = useState('');

    useEffect(() => {
        const storedUserId = localStorage.getItem('userId');
        if (storedUserId) {
            setUserId(storedUserId);
            fetchBalance(storedUserId);
        }
    }, []);

    const fetchBalance = async (id) => {
        try {
            const response = await getBalance(id);
            const { Amount, Currency } = response.data;
            setBalance({ amount: Amount, currency: Currency });
        } catch (error) {
            setBalance(null);
            setMessage('Счет не найден');
        }
    };

    const handleCreateAccount = async () => {
        try {
            const response = await createAccountAuto(initialAmount, initialCurrency);
            const { UserId, Balance, Currency } = response.data;
            localStorage.setItem('userId', UserId);
            setUserId(UserId);
            setBalance({ amount: Balance, currency: Currency });
            setMessage('Счет успешно создан');
        } catch (error) {
            setMessage('Ошибка создания счета: ' + error.message);
        }
    };

    const handleDeposit = async () => {
        try {
            await deposit(userId, depositAmount, balance.currency);
            setMessage('Счет успешно пополнен');
            await fetchBalance(userId);
        } catch (error) {
            setMessage('Ошибка пополнения счета: ' + error.message);
        }
    };
    
    const deleteAccount = async () => {
        setBalance(null);
        setMessage('Счет успешно удален');
        localStorage.setItem('userId', '');
    }

    return (
        <div className="account-container">
            <h2>Управление счетом</h2>
            {userId && <p>User ID: {userId}</p>}

            {balance ? (
                <div>
                    <h3>Баланс: {balance.amount} {balance.currency}</h3>
                    <div className="deposit-form">
                        <input
                            type="number"
                            min="0"
                            placeholder="Сумма для пополнения"
                            onChange={(e) => setDepositAmount(+e.target.value)}
                        />
                        <button onClick={handleDeposit}>Пополнить счет</button>

                        <button onClick={deleteAccount}>Удалить счет</button>
                    </div>
                </div>
            ) : (
                <div className="create-account-form">
                    <p>Счет не создан</p>
                    <input
                        type="number"
                        min="0"
                        onChange={(e) => setInitialAmount(+e.target.value)}
                        placeholder="Начальная сумма"
                    />
                    <select
                        value={initialCurrency}
                        onChange={(e) => setInitialCurrency(e.target.value)}
                    >
                        <option value="USD">USD</option>
                        <option value="EUR">EUR</option>
                        <option value="RUB">RUB</option>
                    </select>
                    <button onClick={handleCreateAccount}>Создать счет</button>
                </div>
            )}

            {message && <p className="message">{message}</p>}
        </div>
    );
};

export default Account;
