import { gql } from "@apollo/client";

export const SEARCH_MOVIES = gql`
  query SearchMovies($query: String!) {
    searchMovies(query: $query) {
      title
      overview
      posterPath
      releaseDate
      country
    }
  }
`;

export const GET_SHOW_AVAILABILITY = gql`
  query GetShowAvailability($type: String!, $id: String!) {
    getShowAvailability(type: $type, id: $id)
  }
`;
