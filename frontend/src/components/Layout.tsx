import {
    AppBar, Box, Button, CssBaseline, Divider, Drawer, IconButton, List,
    ListItem, ListItemButton, Toolbar, Tooltip, Typography, useTheme
} from '@mui/material'
import { PropsWithChildren, useState } from 'react'
import { useAppDispatch, useAppSelector } from '../redux/hooks'
import LangChooser from './LangChooser'
import ThemeChooser from './ThemeChooser'
import { GlobalState } from '../types/GlobalState'
import LogoutIcon from '@mui/icons-material/Logout'
import { logoutAsyncThunk } from '../redux/auth'
import { useNavigate } from 'react-router'
import { APP_HOME_TEXT, APP_URL_Home } from '../constants'
import MenuIcon from '@mui/icons-material/Menu'
import { ExampleWebAppIcon } from './ExampleWebAppIcon'

const drawerWidth = 240

const MainLayout = ({ children }: PropsWithChildren) => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const theme = useTheme()
    const navigate = useNavigate()
    const [mobileOpen, setMobileOpen] = useState(false)

    const handleDrawerToggle = () => {
        setMobileOpen((prevState) => !prevState)
    }    

    const drawer = (
        <Box onClick={handleDrawerToggle} sx={{ textAlign: 'center' }}>
            <Typography variant="h6" sx={{ my: 2 }}>
                {APP_HOME_TEXT}
            </Typography>
            <Divider />
            <List>

                <ListItem disablePadding>
                    <ListItemButton sx={{ textAlign: 'center' }}>                        
                        <LangChooser />
                    </ListItemButton>
                </ListItem>

                <ListItem disablePadding>
                    <ListItemButton sx={{ textAlign: 'center' }}>
                        <ThemeChooser />
                    </ListItemButton>
                </ListItem>

                <ListItem disablePadding>
                    <ListItemButton sx={{ textAlign: 'center' }}>
                        <Tooltip title="Logout">
                            <IconButton onClick={() => {
                                dispatch(logoutAsyncThunk())
                            }}>
                                <LogoutIcon />
                            </IconButton>
                        </Tooltip>
                    </ListItemButton>
                </ListItem>

            </List>
        </Box>
    )

    const container = window !== undefined ? () => window.document.body : undefined

    return (
        <Box sx={{ display: 'flex' }}>
            <CssBaseline />
            <AppBar component="nav">
                <Toolbar>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        edge="start"
                        onClick={handleDrawerToggle}
                        sx={{ mr: 2, display: { sm: 'none' } }}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography
                        variant="h6"
                        component="div"
                        sx={{ flexGrow: 1, display: { xs: 'none', sm: 'block' } }}
                    >
                        <Button onClick={() => navigate(APP_URL_Home)}>
                            <Box sx={{ display: 'inline-flex', alignItems: 'center' }}>
                                <Box sx={{ width: '2em' }}>
                                    <ExampleWebAppIcon />
                                </Box>
                                <Box>
                                    <Typography ml={1} className='toolbar-text'>
                                        {APP_HOME_TEXT}
                                    </Typography>
                                </Box>
                            </Box>
                        </Button>
                    </Typography>
                    <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
                        <Box sx={{ display: 'inline-flex', verticalAlign: 'middle' }}>
                            <LangChooser />
                        </Box>

                        <Box sx={{ display: 'inline-flex', verticalAlign: 'middle' }}>
                            <ThemeChooser />
                        </Box>

                        {global.currentUser ?
                            <Box sx={{ display: 'inline-flex', verticalAlign: 'middle' }}>
                                <Tooltip title="Logout">
                                    <IconButton onClick={() => {
                                        dispatch(logoutAsyncThunk())
                                    }}>
                                        <LogoutIcon />
                                    </IconButton>
                                </Tooltip>
                            </Box>
                            : <></>}
                    </Box>
                </Toolbar>
            </AppBar>
            <nav>
                <Drawer
                    container={container}
                    variant="temporary"
                    open={mobileOpen}
                    onClose={handleDrawerToggle}
                    ModalProps={{
                        keepMounted: true, // Better open performance on mobile.
                    }}
                    sx={{
                        display: { xs: 'block', sm: 'none' },
                        '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
                    }}
                >
                    {drawer}
                </Drawer>
            </nav>
            <Box component="main" sx={{ p: 3, width: '100%' }}>
                <Toolbar />
                {children}
            </Box>
        </Box >
    )
}
export default MainLayout