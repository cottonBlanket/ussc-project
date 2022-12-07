import React from 'react';
// import './App.css';
import './App.scss';

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import NotFoundPage from './pages/NotFoundPage';
import ProfilePage from './pages/ProfilePage';
import HomeLayout from './components/HomeLayout';
import ProfileLayout from './components/ProfileLayout';
import TaskPage from './pages/TaskPage';
import RequireAuth from './hoc/RequireAuth';
import DirectionsPage from './pages/DirectionsPage';
import ApplicationsPage from './pages/ApplicationsPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path='/' element={<HomeLayout />}>
          <Route index element={<HomePage />} />
          <Route path='*' element={<NotFoundPage />} />
        </Route>
        <Route
          path='/profile'
          element={
            <RequireAuth>
              <ProfileLayout />
            </RequireAuth>
          }
        >
          <Route index element={<ProfilePage />} />
        </Route>
        <Route
          path='/tasks'
          element={
            <RequireAuth>
              <ProfileLayout />
            </RequireAuth>
          }
        >
          <Route index element={<TaskPage />} />
        </Route>
        <Route
          path='/directions'
          element={
            <RequireAuth>
              <ProfileLayout />
            </RequireAuth>
          }
        >
          <Route index element={<DirectionsPage />} />
        </Route>
        <Route
          path='/applications'
          element={
            <RequireAuth>
              <ProfileLayout />
            </RequireAuth>
          }
        ></Route>
        <Route index element={<ApplicationsPage />} />
      </Routes>
    </Router>
  );
}

export default App;
