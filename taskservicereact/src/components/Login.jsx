import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (email && password) {
      try {
        const response = await fetch("http://localhost:9091/user/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
          body: JSON.stringify({ email, password }),
        });
  
        if (response.ok) {
          // Lees de respons als plain text
          const bearerToken = await response.text();
  
          // Verwijder 'Bearer ' van het begin
          const token = bearerToken.startsWith("Bearer ")
            ? bearerToken.slice(7) // Verwijder de eerste 7 tekens
            : bearerToken;
  
          console.log("Ontvangen token:", token);
  
          // Hier kun je het token opslaan, bijvoorbeeld in localStorage
          localStorage.setItem("authToken", token);
  
          // Navigeren naar de volgende pagina
          navigate("/create-board");
        } else {
          console.error("Fout bij inloggen:", response.status);
          alert("Inloggen mislukt. Controleer je gegevens.");
        }
      } catch (error) {
        console.error("Fout bij verbinden met de server:", error);
        alert("Kon niet verbinden met de server. Probeer later opnieuw.");
      }
    } else {
      alert("Vul alstublieft zowel e-mail als wachtwoord in.");
    }
  };

  return (
    <div className="login-container">
      <h2>Inloggen</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="email"
          placeholder="E-mailadres"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Wachtwoord"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button type="submit">Inloggen</button>
      </form>
    </div>
  );
};

export default Login;
