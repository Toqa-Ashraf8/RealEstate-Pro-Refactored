import React, { useEffect } from 'react';
import './ClientsPage.css';
import { 
    Eye, 
    User, 
    Search, 
    Phone,
    Users,
    ChevronLeft
} from 'lucide-react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchAllClients, fetchClientFullDetails } from '../../services/clientsProfileService';
import { useNavigate } from 'react-router-dom';
import { searchClients } from '../../services/bookingService';

const ClientsPage = () => {
    const dispatch = useDispatch();
    const { clientData } = useSelector((state) => state.clientsProfile);
    const navigate = useNavigate();

    useEffect(() => {
        dispatch(fetchAllClients());
    }, [dispatch]);

    const setSelectedClient = async (id) => {
        try {
            await dispatch(fetchClientFullDetails(id)).unwrap();
            navigate('/clientdetails');
        } catch (error) {
            console.error("Failed to fetch details:", error);
        }
    }

    const searchTerm = (e) => {
        const searchValue = e.target.value;
        if (searchValue === "") {
            dispatch(fetchAllClients());
            return;
        }
        const searchData = {
            term: searchValue,
            fields: ["ClientName", "ClientID"]
        }
        dispatch(searchClients(searchData));
    }

    return (
        <div className="erp-page-container animate-fade">
            <header className="clients-header">
                <div className="title-area">
                    <div className="icon-box">
                        <Users size={24} />
                    </div>
                    <div className="text-box">
                        <h1>قاعدة بيانات العملاء</h1>
                        <p>إدارة بيانات التواصل والملفات الشخصية</p>
                    </div>
                </div>

                <div className="search-box-wrapper">
                    <Search size={18} className="search-icon-fixed" />
                    <input
                        type="text"
                        placeholder="ابحث باسم العميل أو الكود"
                        className="main-search-input"
                        onChange={(e) => searchTerm(e)}
                    />
                </div>
            </header>
            <div className="table-responsive-wrapper">
                <div className="compact-data-table">
                    <div className="data-table-header">
                        <div className="h-col">كود العميل</div>
                        <div className="h-col">الاسم الكامل</div>
                        <div className="h-col">رقم الجوال</div>
                        <div className="h-col center-text">الإجراءات</div>
                    </div>
                    <div className="data-table-body">
                        {clientData && clientData.length > 0 ? (
                            clientData.map((c, index) => (
                                <div className="data-table-row" key={c.ClientID || index}>
                                    <div className="d-cell code-text">
                                        {c.ClientID}
                                    </div>
                                    <div className="d-cell name-text">
                                        <User size={14} className="icon-dim" />
                                        <span>{c.ClientName}</span>
                                    </div>
                                    <div className="d-cell phone-text">
                                        <Phone size={14} className="icon-dim" />
                                        <span>{c.PhoneNumber}</span>
                                    </div>
                                    <div className="d-cell action-text">
                                        <button 
                                            className="row-action-btn"
                                            onClick={() => setSelectedClient(c.ClientID)}
                                        >
                                            <span>عرض الملف</span>
                                            <ChevronLeft size={16} />
                                        </button>
                                    </div>
                                </div>
                            ))
                        ) : (
                            <div className="empty-state">
                                <p>لم يتم العثور على أي بيانات حالياً</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ClientsPage;