import React, { useState } from "react";

const CreateBoard = () => {
  const [boardName, setBoardName] = useState("");
  const [boardDescription, setBoardDescription] = useState("");
  const [boards, setBoards] = useState([]);

  const handleCreateBoard = () => {
    if (boardName.trim() && boardDescription.trim()) {
      setBoards([...boards, { name: boardName, description: boardDescription }]);
      setBoardName("");
      setBoardDescription("");
    } else {
      alert("Vul zowel een boardnaam als een beschrijving in");
    }

    console.log(localStorage.getItem('authToken'))
  };

  return (
    <div className="board-container">
      <h2>Maak een Board</h2>
      <input
        type="text"
        placeholder="Boardnaam"
        value={boardName}
        onChange={(e) => setBoardName(e.target.value)}
      />
      <textarea
        placeholder="Boardbeschrijving"
        value={boardDescription}
        onChange={(e) => setBoardDescription(e.target.value)}
        rows="4"
        style={{ width: "100%", marginBottom: "10px", padding: "10px" }}
      />
      <button onClick={handleCreateBoard}>Board Aanmaken</button>
      <ul>
        {boards.map((board, index) => (
          <li key={index}>
            <strong>{board.name}</strong>: {board.description}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default CreateBoard;
