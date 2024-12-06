import logo from './logo.svg';
import './App.css';
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginForm from "./components/Login";
import CreateBoard from "./components/CreateBoard";

const App = () => {
  return (
    <Router>
      <div className="app-container">
        <Routes>
          <Route path="/" element={<LoginForm />} />
          <Route path="/create-board" element={<CreateBoard />} />
        </Routes>
      </div>
    </Router>
  );
};


export default App;
