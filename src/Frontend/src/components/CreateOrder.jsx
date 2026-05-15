import React, { useState, useEffect } from 'react';
import { createOrder } from '../services/api';

const CreateOrder = () => {
    const [userId, setUserId] = useState('');
    const [items, setItems] = useState([
        { quantity: 1, price: 0, currency: 'USD' }
    ]);
    const [message, setMessage] = useState('');
    const [orderId, setOrderId] = useState(null);

    useEffect(() => {
        const storedUserId = localStorage.getItem('userId');
        if (storedUserId) setUserId(storedUserId);
    }, []);

    const handleItemChange = (index, field, value) => {
        const newItems = [...items];
        newItems[index][field] = value;
        setItems(newItems);
    };

    const addItem = () => {
        setItems([...items, { quantity: 1, price: 0, currency: 'USD' }]);
    };
    const removeItem = () => {
        if (items.length > 0) {
            setItems(items.slice(0, -1));
        }
    };

    const handleSubmit = async () => {
        if (!userId) {
            setMessage('UserId не задан.');
            return;
        }
        try {
            // Формируем тело запроса в соответствии с API
            const payload = {
                UserId: userId,
                Items: items.map(({ quantity, price, currency }) => ({
                    Quantity: quantity,
                    Price: price,
                    Currency: currency
                }))
            };
            const response = await createOrder(payload);
            const { OrderId, Status } = response.data;
            setOrderId(OrderId);
            setMessage(`Заказ #${OrderId} успешно создан! Статус: ${Status}`);
        } catch (error) {
            setMessage('Ошибка создания заказа: ' + (error.message || error));
        }
    };

    return (
        <div className="order-container">
            <h2>Создание заказа</h2>

            <div className="order-form">
                {items.map((item, index) => (
                    <div key={index} className="order-item">
                        <input
                            type="number"
                            min="1"
                            placeholder="Количество"
                            value={item.quantity}
                            onChange={(e) => handleItemChange(index, 'quantity', parseInt(e.target.value, 10))}
                        />
                        <input
                            type="number"
                            min="0"
                            placeholder="Цена"
                            value={item.price}
                            onChange={(e) => handleItemChange(index, 'price', parseFloat(e.target.value))}
                        />
                        <select
                            value={item.currency}
                            onChange={(e) => handleItemChange(index, 'currency', e.target.value)}
                        >
                            <option value="USD">USD</option>
                            <option value="EUR">EUR</option>
                            <option value="RUB">RUB</option>
                        </select>
                    </div>
                ))}

                <button onClick={addItem} className="add-item-btn">+ Добавить товар</button>
                <button onClick={removeItem} className="remove-item-btn">- Удалить товар</button>
                <button onClick={handleSubmit} className="submit-order-btn">Создать заказ</button>
            </div>

            {orderId && <p className="message">{message}</p>}
            {!orderId && message && <p className="error">{message}</p>}
        </div>
    );
};

export default CreateOrder;
