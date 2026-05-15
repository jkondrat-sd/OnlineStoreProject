import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getOrderById } from '../services/api';

const OrderDetail = () => {
    const { orderId } = useParams();
    const [order, setOrder] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchOrder = async () => {
            try {
                const response = await getOrderById(orderId);
                setOrder(response.data);
            } catch (error) {
                console.error('Ошибка загрузки заказа:', error);
            } finally {
                setLoading(false);
            }
        };

        if (orderId) fetchOrder();
    }, [orderId]);

    if (loading) return <p>Загрузка...</p>;
    if (!order) return <p>Заказ не найден</p>;

    return (
        <div className="order-detail">
            <h2>Детали заказа #{order.Id}</h2>

            <div className="order-info">
                <p>
                    <strong>Статус:</strong>{' '}
                    <span className={`status-${order.Status.toLowerCase()}`}>{order.Status}</span>
                </p>
                <p>
                    <strong>Общая сумма:</strong> {order.TotalAmount} {order.Currency}
                </p>
                <p>
                    <strong>Дата создания:</strong>{' '}
                    {new Date(order.CreatedAt).toLocaleString()}
                </p>
            </div>

            <h3>Товары:</h3>
            <table>
                <thead>
                <tr>
                    <th>ID товара</th>
                    <th>Количество</th>
                    <th>Цена</th>
                    <th>Валюта</th>
                </tr>
                </thead>
                <tbody>
                {order.Items.map((item, index) => (
                    <tr key={index}>
                        <td>{item.ProductId}</td>
                        <td>{item.Quantity}</td>
                        <td>{item.Price}</td>
                        <td>{item.Currency}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default OrderDetail;