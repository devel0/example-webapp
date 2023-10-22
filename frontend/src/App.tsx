import React, { useEffect } from 'react'
import './App.css'
import { history } from "./redux/store"
import { Route, Routes } from 'react-router'
import { HistoryRouter as Router } from "redux-first-history/rr6"
import Layout from './components/Layout'
import { MainPage } from './components/MainPage'
import { LoginPage } from './components/LoginPage'
import { ThemeProvider } from '@emotion/react'
import { useAppDispatch, useAppSelector } from './redux/hooks'
import { evalThemeChanged } from './styles/Theme'
import { ProtectedRoute } from './components/ProtectedRoute'
import { currentUserAsyncThunk } from './redux/auth'
import { GlobalState } from './types/GlobalState'
import { APP_URL_Admin_Users, APP_URL_Home, APP_URL_Login } from './constants'
import { UserManagement } from './components/admin/UserManagement'

function App() {
  const global = useAppSelector<GlobalState>((state) => state.global)
  const dispatch = useAppDispatch()
  const theme = React.useMemo(() => evalThemeChanged(global), [global.theme])

  // TODO: initial auto current user check
  useEffect(() => {
    console.log(`init current user`)
    dispatch(currentUserAsyncThunk())
  }, [])


  return (

    <ThemeProvider theme={theme}>
      <Router history={history}>
        <Layout>
          <Routes>
            <Route path={APP_URL_Home} element={
              <ProtectedRoute element={
                <MainPage />
              } />
            } />
            <Route path={APP_URL_Login} element={<LoginPage />} />
            <Route path={APP_URL_Admin_Users} element={<UserManagement />} />            
          </Routes>
        </Layout>
      </Router>
    </ThemeProvider>

  )
}

export default App
