import './App.css'
import packageJson from '../package.json'

function App() {
  const appVersion = packageJson.version

  return (
    <main className="login-container">
      <section className="login-card">
        <header className="login-header">
          <h1 className="login-title">Iniciar sesión</h1>
          <p className="login-subtitle">Accede a tu cuenta para continuar</p>
        </header>

        <form className="login-form">
          <div className="form-field">
            <label className="form-label" htmlFor="email">
              Correo electrónico
            </label>
            <input
              className="form-input"
              id="email"
              type="email"
              placeholder="tucorreo@empresa.com"
              autoComplete="email"
            />
          </div>

          <div className="form-field">
            <label className="form-label" htmlFor="password">
              Contraseña
            </label>
            <input
              className="form-input"
              id="password"
              type="password"
              placeholder="********"
              autoComplete="current-password"
            />
          </div>

          <button className="login-button" type="submit">
            Ingresar
          </button>

          <p className="login-version">Versión {appVersion}</p>
        </form>
      </section>
    </main>
  )
}

export default App
