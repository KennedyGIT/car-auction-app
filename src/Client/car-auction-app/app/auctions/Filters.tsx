import { Button } from "flowbite-react";
import React from "react";
import { useParamsStore } from "../hooks/useParamsStore";
import { AiOutlineClockCircle, AiOutlineSortAscending } from "react-icons/ai";
import { BsFillStopCircleFill, BsStopwatchFill } from "react-icons/bs";
import { GiFinishLine, GiFlame } from "react-icons/gi";



const pageSizeButtons = [4, 8, 12];

const OrderButtons = [
    {
        label: 'Alphabetical',
        icon: AiOutlineSortAscending,
        value: 'make'
    },
    {
        label: 'End Date',
        icon: AiOutlineClockCircle,
        value: 'endingSoon'
    },
    {
        label: 'Recently Added',
        icon: BsFillStopCircleFill,
        value: 'new'
    },
]

const FilterButtons = [
    {
        label: 'Active Auctions',
        icon: GiFlame,
        value: 'live'
    },
    {
        label: 'Ends in < 6 hours',
        icon: GiFinishLine,
        value: 'endingSoon'
    },
    {
        label: 'Completed',
        icon: BsStopwatchFill,
        value: 'finished'
    },
]

export default function Filters()
{
    const pageSize = useParamsStore(state => state.pageSize);
    const setParams = useParamsStore(state => state.setParams);
    const orderBy = useParamsStore(state => state.orderBy)
    const filterBy = useParamsStore(state => state.filterBy)

    return(
        <div className="flex justify-between items-center mb-4">
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Filter By</span>
                <Button.Group>
                   {FilterButtons.map(({label, icon: Icon, value}) => (
                    <Button
                     key={value}
                     onClick={() => setParams({filterBy: value})}
                     color={`${filterBy === value ? 'blue' : 'gray'}`}
                    >
                        <Icon className="mr-3 h-4 w-4"/>
                        {label}
                    </Button>
                   ))}
                </Button.Group>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Order By</span>
                <Button.Group>
                   {OrderButtons.map(({label, icon: Icon, value}) => (
                    <Button
                     key={value}
                     onClick={() => setParams({orderBy: value})}
                     color={`${orderBy === value ? 'blue' : 'gray'}`}
                    >
                        <Icon className="mr-3 h-4 w-4"/>
                        {label}
                    </Button>
                   ))}
                </Button.Group>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Page Size</span>
                <Button.Group>
                    {pageSizeButtons.map((value, i) => (
                        <Button key={i}
                            onClick={() => setParams({pageSize : value})}
                            color={`${pageSize === value ? 'blue' : 'gray'}`}
                        >
                            {value}
                        </Button>
                    ))}
                </Button.Group>
            </div>
        </div>
    )
}