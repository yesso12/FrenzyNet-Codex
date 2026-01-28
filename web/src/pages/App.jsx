import React from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import DashboardPage from './DashboardPage';
import TermsPage from './TermsPage';
import AcceptableUsePage from './AcceptableUsePage';
import Layout from '../components/Layout';
import { getToken } from '../services/auth';

const PrivateRoute = ({ children }) => {
  if (!getToken()) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/terms" element={<TermsPage />} />
        <Route path="/acceptable-use" element={<AcceptableUsePage />} />
        <Route
          path="/dashboard"
          element={
            <PrivateRoute>
              <DashboardPage />
            </PrivateRoute>
          }
        />
      </Routes>
    </Layout>
  );
}
