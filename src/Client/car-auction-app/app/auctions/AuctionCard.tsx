import React from 'react'
import Image from 'next/image'
import CountdownTimer from './CountdownTimer'

type Props = {
    auction: any
}

export default function AuctionCard({auction} : Props)
{
    return(
        <a href="#">
            <div className='relative w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden'>
                <div>
                    <Image
                        src={auction.imageUrl}
                        alt='image'
                        fill
                        priority
                        className='object-cover'
                        sizes='(max-width:768px) 100vw, (max-width: 1200px) 50vw, 25vw'
                    />
                    <div className='absolute bottom-6 left-6'>
                        <CountdownTimer auctionEnd={auction.auctionEnd}/>
                    </div>
                </div>
            </div>

            <div className='flex justify-between items-center mt-4'>
                <h3 className='text-gray-700'>
                   <strong>{auction.make} {auction.model}</strong> 
                </h3>
                <p className='font-semibold text-sm'>
                   <strong>
                        {auction.year}
                   </strong> 
                </p>
            </div>
        </a>
    )  
}


