export default function MovieCard({ movie }) {
    return (
      <div className="movie-card">
        <h2>{movie.title}</h2>
        <p>{movie.overview}</p>
        {movie.posterPath && <img src={movie.posterPath} alt={movie.title} />}
        <p>Release Date: {movie.releaseDate}</p>
      </div>
    );
  }
  