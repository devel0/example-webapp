import {
    Box, Button, Container,
    TextField
} from '@mui/material'
import { ExampleWebAppLogo } from './ExampleWebAppLogo'
import { useAppDispatch, useAppSelector } from '../redux/hooks'
import { loginAsyncThunk } from '../redux/auth'
import { useNavigate } from 'react-router'
import { GlobalState } from '../types/GlobalState'
import { APP_URL_Home } from '../constants'

export const LoginPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()

    const handleSubmit = async (event: any) => {
        event.preventDefault()
        const data = new FormData(event.currentTarget)

        let res = await dispatch(loginAsyncThunk({
            email: String(data.get("email")),
            password: String(data.get("password"))
        }))

        if (res.type === 'login/rejected') {
            // setLoginErrorMsg('login error')
        }
        else {
            // setLoginErrorMsg('')
            navigate(APP_URL_Home)
        }
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Box sx={{ alignSelf: 'center' }}>
                <Container component="main" maxWidth="xs" >

                    <ExampleWebAppLogo />

                    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label="Email Address"
                            name="email"
                            autoComplete="email"
                            autoFocus
                        />
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                        />

                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                        >
                            Sign In
                        </Button>
                    </Box >
                </Container>
            </Box>
        </Box>
    )

}