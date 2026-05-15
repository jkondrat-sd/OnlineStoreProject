import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getOrdersByUser } from '../services/api';

const OrderList = () => {
    const [userId, setUserId] = useState('');
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const storedUserId = localStorage.getItem('userId');
        if (storedUserId) {
            setUserId(storedUserId);
            fetchOrders(storedUserId);
        } else {
            setLoading(false);
        }
    }, []);

    const fetchOrders = async (id) => {
        try {
            const response = await getOrdersByUser(id);
            setOrders(response.data);
        } catch (error) {
            console.error('Ошибка загрузки заказов:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <p>Загрузка...</p>;

    return (
        <div className="order-list">
            <h2>Мои заказы</h2>

            {orders.length === 0 ? (
                <p>Заказов не найдено</p>
            ) : (
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>Сумма</th>
                        <th>Статус</th>
                        <th>Дата</th>
                    </tr>
                    </thead>
                    <tbody>
                    {orders.map(order => (
                        <tr key={order.Id}>
                            <td>
                                <Link to={`/orders/${order.Id}`}>{order.Id}</Link>
                            </td>
                            <td>{order.TotalAmount}</td>
                            <td className={`status-${order.Status.toLowerCase()}`}>
                                {order.Status}
                            </td>
                            <td>{new Date(order.CreatedAt).toLocaleString()}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default OrderList;