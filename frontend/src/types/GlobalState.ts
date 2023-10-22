import { PaletteMode } from '@mui/material'
import { LANG_TYPES } from '../constants'
import { CurrentUserNfo } from './CurrentUserNfo'
import { UserListItemResponseDto } from '../api';

export interface GlobalState {
    theme: PaletteMode;
    lang: LANG_TYPES,

    // this will serialized in the local storage for a quick page refresh
    currentUser: CurrentUserNfo | undefined,
    usersList: UserListItemResponseDto[] | undefined,

}