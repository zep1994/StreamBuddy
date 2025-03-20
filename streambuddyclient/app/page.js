"use client";

import { useState } from "react";
import { useLazyQuery } from "@apollo/client";
import client from "./graphql/client"; // ✅ Correct import
import { SEARCH_MOVIES } from "./graphql/queries"; // ✅ Ensure correct path

export default function Home() {
  const [query, setQuery] = useState("");
  const [searchMovies, { data, loading, error }] = useLazyQuery(SEARCH_MOVIES, {
    client,
  });

  const handleSearch = () => {
    if (query.length < 2) {
      alert("Search query must be at least 2 characters long.");
      return;
    }
    searchMovies({ variables: { query } });
  };

  return (
    <div className="container">
      <h1>StreamBuddy Movie Search</h1>
      <input
        type="text"
        placeholder="Search for a movie..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
      />
      <button onClick={handleSearch} disabled={loading}>
        {loading ? "Searching..." : "Search"}
      </button>

      {error && <p>Error: {error.message}</p>}

      {data && (
        <div className="results">
          {data.searchMovies.map((movie) => (
            <div key={movie.id} className="movie">
              <h2>{movie.title}</h2>
              <p>{movie.overview}</p>
              {movie.posterPath && <img src={movie.posterPath} alt={movie.title} width="150" />}
              <p>Release Date: {movie.releaseDate}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
