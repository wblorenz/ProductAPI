import aspireLogo from '/Aspire.png';
import './App.css';
import ProductsTable from './ProductsTable';

function App() {
    return (
        <div className="app-container">
            <header className="app-header">
                <a
                    href="https://aspire.dev"
                    target="_blank"
                    rel="noopener noreferrer"
                    aria-label="Visit Aspire website (opens in new tab)"
                    className="logo-link"
                >
                    <img src={aspireLogo} className="logo" alt="Aspire logo" />
                </a>
                <h1 className="app-title">Aspire Starter</h1>
                <p className="app-subtitle">Modern distributed application development</p>
            </header>

            <main className="main-content">
                <ProductsTable />
            </main>
        </div>
    );
}

export default App;
