// https://stackoverflow.com/a/47754325

import { JSXElementProp } from '../types/JSXElementProp'
import { useAppDispatch, useAppSelector } from '../redux/hooks'
import { Navigate, useLocation } from 'react-router'
import { GlobalState } from '../types/GlobalState'
import { APP_URL_Login } from '../constants'

export const ProtectedRoute = (props: JSXElementProp) => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()    
    const location = useLocation()
    
    if (global.currentUser)
        return props.element

    // console.log(`navigate remember location: ${JSON.stringify(location)}`)

    return <Navigate to={{ pathname: APP_URL_Login }} state={{ from: location }} />
}