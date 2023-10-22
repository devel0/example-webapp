import { useAppDispatch, useAppSelector } from '../redux/hooks'
import { GlobalState } from '../types/GlobalState'
import { Box, Button } from '@mui/material'
import { useNavigate } from 'react-router'
import { APP_URL_Admin_Users } from '../constants'

export const MainPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()

    return (
        <Box margin={1}>
            <Button onClick={() => navigate(APP_URL_Admin_Users)}>User management</Button>            
        </Box>
    )

}