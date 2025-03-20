import { ApolloClient, InMemoryCache } from "@apollo/client";

const client = new ApolloClient({
  uri: "http://localhost:5279/api/streaming", 
  cache: new InMemoryCache(),
});

export default client;
