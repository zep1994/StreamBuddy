'use client';
import { ApolloProvider } from '@apollo/client';
import client from '../lib/client'; // Path relative to 'app/' directory
import './globals.css';

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>
        <ApolloProvider client={client}>{children}</ApolloProvider>
      </body>
    </html>
  );
}