const API_BASE_URL = "http://localhost:5279/api/streaming";

export async function searchMovies(query, country = "us", showType = "movie") {
    try {
        const response = await fetch(`${API_BASE_URL}/shows/search/title?query=${query}&country=${country}&showType=${showType}`);
        if (!response.ok) {
            throw new Error("Failed to fetch movies");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching movies:", error);
        return [];
    }
}

export async function getTopShows(country = "us", services = "netflix") {
    try {
        const response = await fetch(`${API_BASE_URL}/shows/top?country=${country}&services=${services}`);
        if (!response.ok) {
            throw new Error("Failed to fetch top shows");
        }
        return await response.json();
    } catch (error) {
        console.error("Error fetching top shows:", error);
        return [];
    }
}
