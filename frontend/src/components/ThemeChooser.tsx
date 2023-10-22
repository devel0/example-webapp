import { IconButton, Tooltip } from '@mui/material'
// import { FaMoon, FaSun } from 'react-icons/fa6'
import NightlightIcon from '@mui/icons-material/Nightlight';
import LightModeIcon from '@mui/icons-material/LightMode';
import { useAppDispatch, useAppSelector } from '../redux/hooks'
import { setTheme } from '../redux/globalSlice'
import { THEME_DARK, THEME_LIGHT } from '../constants'
import { GlobalState } from '../types/GlobalState'

export default function ThemeChooser() {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    return (

        <Tooltip title="Tema Light / Dark">
            <IconButton sx={{ color: 'inherit' }} onClick={() => {
                dispatch(setTheme(global.theme === 'dark' ? THEME_LIGHT : THEME_DARK))
            }}>
                {global.theme === THEME_DARK ? <NightlightIcon /> : <LightModeIcon />}
            </IconButton>
        </Tooltip>
    )
}