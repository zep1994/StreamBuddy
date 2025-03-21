import { ApolloClient, InMemoryCache, HttpLink } from '@apollo/client';

const client = new ApolloClient({
  link: new HttpLink({
    uri: 'http://localhost:5279/graphql', // Your .NET API GraphQL endpoint
  }),
  cache: new InMemoryCache(),
});

export default client; // Export as 'client', not 'appolloclient'