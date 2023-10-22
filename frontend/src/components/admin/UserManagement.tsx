import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../redux/hooks'
import { listUsersAsyncThunk } from '../../redux/auth'
import { GlobalState } from '../../types/GlobalState'
import { Box, Link, Typography } from '@mui/material'
import { useNavigate } from 'react-router'
import { DataGrid, GridColDef } from '@mui/x-data-grid'
import { UserListItemResponseDto } from '../../api'

export const UserManagement = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        dispatch(listUsersAsyncThunk())
    }, [])    

    const columns: GridColDef[] = [        
        {
            field: 'userName',
            headerName: 'UserName',
            width: 130,
            renderCell: (params) => {
                const row = params.row as UserListItemResponseDto
                return (
                    <Link href="/">
                        {JSON.stringify(row.userName)}
                    </Link>
                )
            }
        },
    ]

    return (
        <Box>
            <Typography variant='h5'>User management</Typography>            

            <DataGrid
                rows={global.usersList ?? []}
                getRowId={(x) => x.userName}
                columns={columns}
                initialState={{
                    pagination: {
                        paginationModel: { page: 0, pageSize: 5 },
                    },
                }}
                pageSizeOptions={[5, 10]}
                checkboxSelection
            />
        </Box>
    )

}