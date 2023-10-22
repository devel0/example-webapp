import { MenuItem, Select } from '@mui/material'
import { LANG_TYPES } from '../constants'
import { useAppDispatch, useAppSelector } from '../redux/hooks'
import { setLang } from '../redux/globalSlice'
import { GlobalState } from '../types/GlobalState'

function LangChooser() {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    return (
        <div>
            <Select
                labelId="demo-simple-select-label"
                id="demo-simple-select"
                sx={{
                    boxShadow: 'none',
                    '.MuiOutlinedInput-notchedOutline': { border: 'none' }
                }}
                value={global.lang}
                label="Language"
                onChange={(v) => {
                    const newLang = v.target.value as LANG_TYPES

                    dispatch(setLang(newLang))
                }}
            >
                <MenuItem value={"it"}>🇮🇹 IT</MenuItem>
                <MenuItem value={"en"}>🇬🇧 EN</MenuItem>
                <MenuItem value={"de"}>🇩🇪 DE</MenuItem>
            </Select>
        </div>
    )
}

export default LangChooser