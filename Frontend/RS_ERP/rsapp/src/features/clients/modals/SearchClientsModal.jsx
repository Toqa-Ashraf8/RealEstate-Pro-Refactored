import React, { useEffect } from 'react'
import './SearchClientsModal.css'
import { useDispatch, useSelector } from 'react-redux';
import { variables } from '../../../assets/variables';
import { IoMdClose } from "react-icons/io";
import {setClientsForm,toggleSearchClientsModal } from '../../../assets/redux/clientSlice';
import { fetchClients, fetchNegotiationsByClient } from '../../../services/clientService';
const SearchClientsModal = () => {
  const {clientsList} = useSelector((state) => state.clients);
  const {isLoading}=useSelector((state)=>state.ui);
  const dispatch = useDispatch();

const handleSearch= async(i)=>{
    const selectedClient = clientsList[i];
    if (!selectedClient) return;  
    const clientid=selectedClient.ClientID;
    dispatch(setClientsForm(i));
   if (clientid) {
        await dispatch(fetchNegotiationsByClient(clientid));
    }
}
 useEffect(() => {
    const loadClients = async () => {
      try {
        await dispatch(fetchClients());
      } 
      catch (error) {
        console.error("فشل في جلب البيانات:", error);
      }
    };
    loadClients();
  }, [dispatch]);
  
  return (
    <div dir='rtl'>
      <div className="modals">
        <div className="modalcnt_s">
            <div className="hdr_s">
                <div className='hrdtitles_s'> 
                      <span 
                      className='close_search'
                      onClick={()=>dispatch(toggleSearchClientsModal(false))}
                      ><IoMdClose size={30} /></span>
                </div> 
                <h3 className='hds_title'>العملاء</h3>
        </div>
            <div className="bodys">
                <div className='tbl_ss'>
                    <table className='table table-striped tbl-srch'>
                    <thead>
                        <tr>
                            <th>الكود</th>
                            <th>العميل</th>
                            <th>رقم الموبايل</th>
                            <th>حالة العميل</th>
                            <th>ملاحظات</th>
                        </tr>
                    </thead>
                    <tbody>
                        {clientsList.length===0 ?<tr><td colSpan={7} className="empty-msg">لا توجد بيانات لعرضها</td></tr>:
                        clientsList.map((client,i)=>
                        <tr key={i} onClick={()=>handleSearch(i)}>
                        <td>{client.ClientID}</td>
                        <td>{client.ClientName}</td>
                        <td>{client.PhoneNumber}</td>
                        <td>{client.ClientStatus}</td>
                        <td>{client.Notes}</td>
                        </tr>
                        )}
                        </tbody>
                    </table>
                </div>
            </div>
            <div className="footer_s">
                <div style={{display:'flex',justifyContent:'flex-end'}}>
                    <button 
                    className='btn btn-danger btn_closes'
                    disabled={isLoading}
                    onClick={()=>dispatch(toggleSearchClientsModal(false))}
                    >إغلاق</button>
                </div>
            </div>
        </div>
      </div>
    </div>
  )
}

export default SearchClientsModal;
