'use client'
import { Pagination } from "flowbite-react"
import React, {useState} from "react"

type Props = {
    currentPage: number
    pageCount: number
    pageChanged: (page: number) => void
}

export default function AppPagination({currentPage, pageCount, pageChanged} : Props){


    return(
        <Pagination
         currentPage={currentPage}
         onPageChange={e => pageChanged(e)}
         totalPages={pageCount}
         layout="pagination"
         showIcons={true}
         className="text-blue-500 mb-5"
        />
    )
}


/*
    Inside AppPagination, when a new page number is selected, 
    the onPageChange event handler calls the pageChanged function with the new page number:
    onPageChange={e => pageChanged(e)}
    This pageChanged is actually the setPageNumber function from the Listings component, 
    so calling pageChanged updates the pageNumber state in the Listings component
*/