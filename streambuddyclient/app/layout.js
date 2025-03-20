export default function Layout({ children }) {
  return (
    <html lang="en">
      <body>
        <nav>
          <h1>StreamBuddy</h1>
        </nav>
        <main>{children}</main>
      </body>
    </html>
  );
}
