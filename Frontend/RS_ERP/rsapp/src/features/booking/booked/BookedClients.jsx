import React, { useEffect, useRef } from 'react';
import { 
    Users, 
    User, 
    Eye, 
    Printer, 
    Search, 
    Calendar, 
    Landmark, 
    MoreVertical, 
    ArrowUpDown,
    SquarePen,
    Trash2 
} from 'lucide-react';
import './BookedClients.css';
import { useDispatch, useSelector } from 'react-redux';
import { 
    deleteBookingRow,   
    setReservationStatus
} from '../../../assets/redux/bookingSlice';
import { useNavigate } from 'react-router-dom';
import { useReactToPrint } from 'react-to-print';
import BookingsReport from '../../../assets/reports/BookingsReport';

import { 
    deleteBookingData,
    fetchAllReservedClients, 
    fetchReservedClientById, 
    searchBookingClients
} from '../../../services/bookingService';
import { toast } from 'react-toastify';

const BookedClients = () => {
const {reservedClients}=useSelector((state)=>state.booking);
    const dispatch=useDispatch();
    const navigate=useNavigate();
   
const handlePrint = useReactToPrint({
        contentRef: () => componentRef,
        documentTitle: 'تقرير_حجز_عميل',
});
useEffect(()=>{ 
    dispatch(fetchAllReservedClients());
},[])
const editReservedClients=async(id,index)=>{
    const selectedClient=reservedClients[index];
    localStorage.setItem('activeBookingClient', JSON.stringify(selectedClient));
    await dispatch(fetchReservedClientById(id));
     dispatch(setReservationStatus(1));
    await navigate('/complete_booking?clientId='+id);
}
const printReport = async (index, id) => {
    try {
        await dispatch(fetchReservedClientById(id)).unwrap();
        dispatch(setReservationStatus(1)); 
        handlePrint();  
        navigate('/bookingreport');
        
    } catch (error) {
        console.error("Failed to fetch client details", error);
    }
}

const deleteBooking=async(index)=>{
  const selectedBookingClient=reservedClients[index]
 const reservedBookingId=reservedClients[index].BookingID; 
    try {
         const result= await dispatch(deleteBookingData(selectedBookingClient)).unwrap(); 
            toast.success("تم حذف الحجز وتحديث حالة الوحدة!", {
                theme: "colored",
                position: "top-left",
            });  
            dispatch(deleteBookingRow(reservedBookingId));
        }
        catch (error) {
            toast.error("حدث خطأ في الاتصال بالخادم!", {
            theme: "colored",
            position: "top-left",
        }); 
 }   
} 
const searchClients=(e)=>{
    const searchValue = e.target.value;
    if (searchValue === "") {
         dispatch(fetchAllReservedClients()); 
        return;
    }
    const searchData={
        term:searchValue,
        fields:["ClientName","unitName","ProjectName"]
    }
    dispatch(searchBookingClients(searchData));
}

    return (
        <div className="booked_list_wrapper">      
          <div className="booked_list_header">
                <div className="booked_list_title_area">
                    <h2><Users size={24} /> سجل الحجوزات المكتملة</h2>
                    <p>إدارة واستعراض{reservedClients.length}عقد مبرم</p>
                </div>
                <div className="booked_list_search_bar">
                    <Search size={18} />
                    <input 
                    type="text" 
                    placeholder="بحث عن عميل أو وحدة"
                    onChange={(e)=>searchClients(e)}
                     />
                </div>
            </div>

            <div className="table_container_premium">
                <table className="booked_table">
                    <thead>
                        <tr>
                            <th>العميل <ArrowUpDown size={14} /></th>
                            <th>المشروع</th>
                            <th>الوحدة</th>
                            <th className="text_center">الإجراءات</th>
                        </tr>
                    </thead>
                    <tbody>
                       {reservedClients.length>0 && 
                        reservedClients.map((client,index)=>
                            <tr key={index}>
                    <td>
                        <div className="user_cell">
                        <div className="user_avatar_mini"><User size={14} /></div>
                            <span>{client.ClientName}</span>
                        </div>
                        </td>
                    <td>
                        <div className="project_cell">
                        <p>{client.ProjectName}</p>
                        </div>
                    </td>  
                      <td>
                        <div className="project_cell">
                        <p>{client.unitName}</p>
                        </div>
                    </td>       
                        <td>
                        <div className="table_actions">
                        <button 
                            className="action_icon view" 
                            title="تعديل" 
                            onClick={()=>editReservedClients(client.BookingID,index)}>
                            <SquarePen  size={18} color='blue' />
                            </button>
                        <button 
                            className="action_icon view" 
                            title="حذف"
                            onClick={()=>deleteBooking(index)}
                            >
                            <Trash2   size={18} color='red' />
                        </button>
                            <button 
                            className="action_icon view" 
                            title="طباعة"
                            onClick={()=>printReport(index,client.BookingID)}
                            >
                            <Printer  size={18} color='teal' />
                            </button>
                        </div>
                    </td>
                    </tr>
                )}
             </tbody>
             </table>
            </div>  
        </div>
    );
};

export default BookedClients;