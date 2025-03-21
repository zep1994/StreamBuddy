'use client';
import { useQuery, gql } from '@apollo/client';

const GET_STREAMING_DATA = gql`
  query GetStreamingData {
    # Replace with your actual GraphQL query
    hello # Example query from your backend
  }
`;

export default function Home() {
  const { loading, error, data } = useQuery(GET_STREAMING_DATA);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error.message}</p>;

  return (
    <div>
      <h1>StreamBuddy</h1>
      <p>{data?.hello || 'No data yet'}</p>
    </div>
  );
}