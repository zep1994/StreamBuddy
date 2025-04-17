'use client';

import movies from './graphql/data/movies';
import './styles.css';

export default function Home() {
  return (
    <div className="page">
      <header className="header">
        <h1>StreamBuddy</h1>
        <p>Discover Top Movies on Streaming Platforms</p>
      </header>

      <section className="section">
        <h2>🔥 Trending Now</h2>
        <div className="movie-row">
          {movies.slice(0, 5).map((movie) => (
            <div key={movie.id} className="movie-card">
              <img src={movie.image} alt={movie.title} />
              <h3>{movie.title}</h3>
              <p>{movie.description}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="section">
        <h2>🎬 Popular Picks</h2>
        <div className="movie-row">
          {movies.slice(5, 10).map((movie) => (
            <div key={movie.id} className="movie-card">
              <img src={movie.image} alt={movie.title} />
              <h3>{movie.title}</h3>
              <p>{movie.description}</p>
            </div>
          ))}
        </div>
      </section>

      <footer className="footer">
        StreamBuddy © {new Date().getFullYear()}
      </footer>
    </div>
  );
}
