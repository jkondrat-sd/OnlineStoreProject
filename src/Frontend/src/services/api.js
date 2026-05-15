import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:8080';

export const createAccountAuto = async (balance = 0, currency = "USD") => {
    return axios.post(`${API_BASE_URL}/api/accounts/create-account`, {
        balance,
        currency
    });
};

export const deposit = async (userId, amount, currency = 'USD') => {
    return axios.post(`${API_BASE_URL}/api/accounts/${userId}/deposit`, {
        amount,
        currency
    });
};

export const getBalance = async (userId) => {
    return axios.get(`${API_BASE_URL}/api/accounts/${userId}/balance`);
};

export const createOrder = async (payload) => {
    return axios.post(`${API_BASE_URL}/api/orders/create-order`, payload);
};

export const getOrdersByUser = async (userId) => {
    return axios.get(`${API_BASE_URL}/api/orders/get-orders/${userId}`);
};

export const getOrderById = async (orderId) => {
    return axios.get(`${API_BASE_URL}/api/orders/get-order/${orderId}`);
};