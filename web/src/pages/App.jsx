import React from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import DashboardPage from './DashboardPage';
import TermsPage from './TermsPage';
import AcceptableUsePage from './AcceptableUsePage';
import LandingPage from './LandingPage';
import AccountPage from './AccountPage';
import AdminPage from './AdminPage';
import OwnerPage from './OwnerPage';
import Layout from '../components/Layout';
import { getProfile, getToken } from '../services/auth';

const PrivateRoute = ({ children }) => {
  if (!getToken()) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

const RoleRoute = ({ roles, children }) => {
  const profile = getProfile();
  if (!profile) {
    return <Navigate to="/login" replace />;
  }
  if (!roles.includes(profile.role)) {
    return <Navigate to="/dashboard" replace />;
  }
  return children;
};

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<LandingPage />} />
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
        <Route
          path="/account"
          element={
            <PrivateRoute>
              <AccountPage />
            </PrivateRoute>
          }
        />
        <Route
          path="/admin"
          element={
            <RoleRoute roles={["admin", "owner"]}>
              <AdminPage />
            </RoleRoute>
          }
        />
        <Route
          path="/owner"
          element={
            <RoleRoute roles={["owner"]}>
              <OwnerPage />
            </RoleRoute>
          }
        />
      </Routes>
    </Layout>
  );
}
