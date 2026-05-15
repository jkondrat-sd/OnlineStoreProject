import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Account from './components/Account';
import CreateOrder from './components/CreateOrder';
import OrderList from './components/OrderList';
import OrderDetail from './components/OrderDetail';

function App() {
    return (
        <Router>
            <div className="app-container">
                <nav>
                    <ul>
                        <li><Link to="/">Счет</Link></li>
                        <li><Link to="/create-order">Создать заказ</Link></li>
                        <li><Link to="/orders">Мои заказы</Link></li>
                    </ul>
                </nav>

                <Routes>
                    <Route path="/" element={<Account />} />
                    <Route path="/create-order" element={<CreateOrder />} />
                    <Route path="/orders" element={<OrderList />} />
                    <Route path="/orders/:orderId" element={<OrderDetail />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;