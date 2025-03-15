import { useEffect, useState } from "react";
import { searchMovies, getTopShows } from "./services/streamingService";
import styles from "./page.module.css";

export default function Home() {
    const [topNetflixShows, setTopNetflixShows] = useState([]);
    const [searchQuery, setSearchQuery] = useState("");
    const [searchResults, setSearchResults] = useState([]);

    useEffect(() => {
        async function fetchNetflixShows() {
            const shows = await getTopShows("us", "netflix");
            setTopNetflixShows(shows);
        }
        fetchNetflixShows();
    }, []);

    const handleSearch = async () => {
        if (!searchQuery.trim()) return;
        const results = await searchMovies(searchQuery);
        setSearchResults(results);
    };

    return (
        <div className={styles.container}>
            <h1 className={styles.title}>StreamBuddy Dashboard</h1>

            {/* Search Bar */}
            <div className={styles.searchBox}>
                <input
                    type="text"
                    placeholder="Search for a movie or show..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                />
                <button onClick={handleSearch}>Search</button>
            </div>

            {/* Search Results */}
            <section>
                <h2>Search Results</h2>
                <div className={styles.moviesGrid}>
                    {searchResults.length > 0 ? (
                        searchResults.map((movie) => (
                            <div key={movie.id} className={styles.movieCard}>
                                <img
                                    src="/placeholder.jpg" // Placeholder until actual images are available
                                    alt={movie.title}
                                    className={styles.movieImage}
                                />
                                <h3>{movie.title} ({movie.releaseYear})</h3>
                            </div>
                        ))
                    ) : (
                        <p>No movies found.</p>
                    )}
                </div>
            </section>

            {/* Top Netflix Shows */}
            <section>
                <h2>Top Netflix Shows</h2>
                <div className={styles.moviesGrid}>
                    {topNetflixShows.length > 0 ? (
                        topNetflixShows.map((show) => (
                            <div key={show.id} className={styles.movieCard}>
                                <img
                                    src="/placeholder.jpg" // Placeholder
                                    alt={show.title}
                                    className={styles.movieImage}
                                />
                                <h3>{show.title} ({show.releaseYear})</h3>
                            </div>
                        ))
                    ) : (
                        <p>No top shows available.</p>
                    )}
                </div>
            </section>
        </div>
    );
}
